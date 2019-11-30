using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	26/10/2019
 *	Last:	08/11/2019
 *	Vers:	1.1 - Adjusted Player No. to be an Int.
 */

public class ShipMovement : MonoBehaviour
{
	//Force is calculated by multiplying a direction vector with this value.
	private float myForceMultiplier;

	//This holds our player's input. This is updated every frame in Update().
	private Vector2 myPlayerInputVector;

	//This angle is calculated from our input vector and used to rotate the ship.
	private float myPlayerInputAngle;

	//This holds our player's Input names once this script wakes.
	private string myXInput;
	private string myYInput;

	//The ship's Rigidbody is used for movement.
	private Rigidbody2D myRigidbody;

	//The ship's particle system emits when the ship is moving.
	private ParticleSystem myEngineParticles;

	private void Awake()
	{
		myRigidbody = GetComponent<Rigidbody2D>();
		myEngineParticles = GetComponentInChildren<ParticleSystem>();

		myXInput = "JoyX0";
		myYInput = "JoyY0";
	}

	private void OnEnable()
	{
		//Make sure the rigidbody isn't moving.
		myRigidbody.velocity = Vector2.zero;

		//Re-enable rigidbody simulation.
		myRigidbody.simulated = true;

		//Reset the inputs.
		myPlayerInputVector = Vector2.zero;
		myPlayerInputAngle = 0f;
	}

	private void OnDisable()
	{
		//Make sure the rigidbody isn't moving.
		myRigidbody.velocity = Vector2.zero;

		//When the ship is no longer an active object in the scene, disable the movement by switching the rigidbody to kinematic.
		myRigidbody.simulated = false;

		//Prevent the engine from emitting particles.
		var emission = myEngineParticles.emission;
		emission.enabled = false;
	}

	public void Setup(int aNumber, float aForceMultiplier, Color aPlayerColor)
	{
        //We get the Player's index from the ShipManager to set up the correct input for this ship.
		myXInput = "JoyX" + aNumber.ToString();
		myYInput = "JoyY" + aNumber.ToString();

		myForceMultiplier = aForceMultiplier;

		myEngineParticles.startColor = aPlayerColor;
	}

	private void Move()
	{
		//Apply forces in the direction from our input, multiplied by our ship's Force.
		myRigidbody.AddForce(myPlayerInputVector * myForceMultiplier);
	}

	private void Turn()
	{
		//Set the transform's rotation to a new vector based on our angle.
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, myPlayerInputAngle));
	}

	private void Update()
	{
		//Update our player input vector and calculate a new rotation angle in the process.
		myPlayerInputVector = new Vector2(Input.GetAxis(myXInput), Input.GetAxis(myYInput));
		//To leave our ship pointing in the direction it was last moving, we only calculate the angle when our input isn't zero.
		if (myPlayerInputVector != Vector2.zero)
			myPlayerInputAngle = Mathf.Atan2(-myPlayerInputVector.x, myPlayerInputVector.y) * Mathf.Rad2Deg;
		
		//Check our input vector to handle engine particle emission.
		if (myPlayerInputVector != Vector2.zero)
		{
			var emission = myEngineParticles.emission;
			emission.enabled = true;
		}
		else
		{
			var emission = myEngineParticles.emission;
			emission.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		Move();
		Turn();
	}
}
