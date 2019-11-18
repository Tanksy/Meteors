using UnityEngine;
using System.Collections;

/**
 *	Auth:	Jake Anderson.
 *	Date:	17/10/2019
 *	Last:	08/11/2019
 *	Name:	Trail Controller 
 *	Vers:	2.1 - Trying to get the Trail to stop emitting when a ship dies/respawns.
 */
 
public class TrailController : MonoBehaviour
{
	//The trail object is an instance of this prefab.
	private GameObject myTrail_Prefab;

	//The trail is an instance of a prefab object, so we can "Disconnect" it and emit a new trail when a ship crosses the map border while boosting.
	private GameObject myTrail_Instance;
	public GameObject Instance { get { return myTrail_Instance; } }

	//The TrailRenderer of the instance is used to control emission and color of the trail.
	private TrailRenderer myTrail_Renderer;

	//This color comes from the ship and is used to set the trail renderer color.
	private Color myTrail_Color;

	//The destroyer script attached to the Trail instance is used to destroy the game object after a delay.
	private DelayedDestroy myTrail_Destroyer;

    public void Setup(Color aColor, GameObject aTrailPrefab)
	{
		myTrail_Prefab = aTrailPrefab;
		myTrail_Color = aColor;
		myTrail_Instance = Instantiate(myTrail_Prefab, transform) as GameObject;
		myTrail_Renderer = myTrail_Instance.GetComponent<TrailRenderer>();
		myTrail_Destroyer = myTrail_Instance.GetComponent<DelayedDestroy>();
		myTrail_Renderer.startColor = myTrail_Color;
	}

	public void StopEmit()
	{
		if (myTrail_Instance != null)
			myTrail_Renderer.emitting = false;
	}

	public void Emit()
	{
		if (myTrail_Instance != null)
			myTrail_Renderer.emitting = true;
	}

	//Nullify our parent to cut the connection.
	public void Cut()
	{
		if (myTrail_Instance != null)
		{
			myTrail_Instance.transform.parent = null;

			float time = myTrail_Renderer.time;
			myTrail_Destroyer.Delay = time;
		}
	}

	//Cuts then recreates the trail.
	public void Snip()
	{
		Cut();
		Setup(myTrail_Color, myTrail_Prefab);
		Emit();
	}
}