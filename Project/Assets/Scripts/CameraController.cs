using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	18/09/2019
 * Last:	23/10/2019
 * Name:	Camera Controller
 * Vers:	1.1 - Player following
 */

public class CameraController : MonoBehaviour
{
	[Tooltip("The size added to the camera's size to prevent ships from moving off-screen.")]
	[SerializeField] private float myScreenEdgeBuffer;
	[Tooltip("The minimum size of the camera.")]
	[SerializeField] private float myMinSize;
	[Tooltip("The maximum size of the camera.")]
	[SerializeField] private float myMaxSize;
	[Tooltip("The rough amount of time the camera takes to dampen its movement.")]
	[SerializeField] private float myDampTime;

	private float myZoomSpeed;

	private Vector3 myMoveVelocity;
	private Vector3 myDesiredPos;

	private Transform[] myPlayers;
	public Transform[] Players { get { return myPlayers; } set { myPlayers = value; } }

	//The child camera of this rig.
	private Camera myCamera;

	void Awake()
	{
		myCamera = GetComponentInChildren<Camera>();
		myPlayers = new Transform[2];
	}

	private void FindAveragePosition()
	{
		Vector3 averagePos = new Vector3();
		int numTargets = 0;

		for (int i = 0; i < myPlayers.Length; i++)
		{
			//Check that the target is active.
			if (!myPlayers[i].gameObject.GetComponent<ShipManager>().Status)
				continue;
			
			averagePos += myPlayers[i].position;
			numTargets++;
		}

		//If we have more than 0 targets, then we average the position across the number of targets.
		if (numTargets > 0)
			averagePos /= numTargets;

		//Override z just in-case.
		averagePos.z = transform.position.z;

		//Set our desired position.
		myDesiredPos = averagePos;
	}

	private void Move()
	{
		//Find the average position.
		FindAveragePosition();

		//Move rig to that desired position.
		transform.position = Vector3.SmoothDamp(transform.position, myDesiredPos, ref myMoveVelocity, myDampTime);
	}

	private float FindDesiredSize()
	{
		float ret = 0f;

		Vector3 desiredLocalPos = transform.InverseTransformPoint(myDesiredPos);

		for (int i = 0; i < myPlayers.Length; i++)
		{
			if (!myPlayers[i].gameObject.GetComponent<ShipManager>().Status)
				continue;

			Vector3 targetLocalPos = transform.InverseTransformPoint(myPlayers[i].position);
			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

			//Find the maximum between our current value and the desired position's absolute values.
			ret = Mathf.Max(ret, Mathf.Abs(desiredPosToTarget.y));
			ret = Mathf.Max(ret, Mathf.Abs(desiredPosToTarget.x) / myCamera.aspect);
		}

		ret += myScreenEdgeBuffer;

		//Check we're not below the minimum size.
		ret = Mathf.Max(ret, myMinSize);
		//Check we're not above the maximum size.
		ret = Mathf.Min(ret, myMaxSize);

		return ret;
	}

	private void Zoom()
	{
		float desiredSize = FindDesiredSize();

		myCamera.orthographicSize = Mathf.SmoothDamp(myCamera.orthographicSize, desiredSize, ref myZoomSpeed, myDampTime);
	}

	public void SetStartPositionAndSize()
	{
		FindAveragePosition();
		transform.position = myDesiredPos;
		myCamera.orthographicSize = FindDesiredSize();
	}

	private void FixedUpdate()
	{
		if (myPlayers[0] != null)
		{
			Move();
			Zoom();
		}
	}
}
