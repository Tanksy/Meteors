using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	27/10/2019
 *	Last:	23/11/2019
 *	Vers:	1.2 - Checks for existence of meter when charging. No longer passes data to Trail Controller.
 */

public class ShipBooster : MonoBehaviour
{
	//Player number is passed from the Ship Manager. This is used for getting the correct player's input.
	private int myPlayer_Number;

	//Player color is also passed from the ship. This is used to color the trail renderer.
	private Color myPlayer_Color;
	public Color Color { get { return myPlayer_Color; } }

	//The trail object is an instance of this prefab.
	private GameObject myTrail_Prefab;
	public GameObject TrailPrefab { get { return myTrail_Prefab; } }

	//The Trail Controller is used to decouple the existing trail object from us, and create a new one.
	private TrailController myTrail_Controller;

	//The rigidbody is used to move the ship when boosting.
	private Rigidbody2D myRigidbody;

	//The Player number is used to make a string for getting the correct player's input.
	private string myInput;

	//When receicing player input, Boost is charged up to a maximum.
	private float myBoost_Current;

	//The maximum boost is used to limit the time a player can spend boosting.
	private float myBoost_Max;

	//The boost is charged by this value multiplied by time.
	private float myBoost_Charge;

	//The extra force applied by the boost comes from the Ship object.
	private float myBoost_Force;

	//When the ship is boosting we lock player input.
	private bool isBoosting;
	public bool Boosting { get { return isBoosting; } }

	//The sound played when charging the boost is an instance of this prefab.
	private GameObject myBoostSound_Prefab;

	//As the input is received and boost is charged, the sound's pitch is increased.
	private float myBoostSound_Pitch;

	//The sound plays when it's not on cooldown to allow a time gap in between sound objects being spawned.
	private float myBoostSound_Cooldown;

	//The UI object that holds the boost image is an instance of this prefab.
	private GameObject myBoostMeter_Prefab;

	//The instance of the Boost UI object used to tell the player how much boost is charged.
	private GameObject myBoostMeter_Instance;

	//The Sprite Renderer is used to alter the opacity of the boost meter sprite.
	private SpriteRenderer myBoostMeter_Renderer;

	//The animator is used to control the sprite animation when charging.
	private Animator myBoostMeter_Animator;

	//The boost meter is shifted down the Y axis of the ship to be closer to the engine of the sprite.
	private float myYShift;

	private void Awake()
	{
		myTrail_Controller = gameObject.GetComponent<TrailController>();
		myRigidbody = GetComponent<Rigidbody2D>();

		myInput = "Boost0";
	}

	public void Setup(int aPlayerNumber, Color aPlayerColor, Ship aShip, GameObject aSoundPrefab, GameObject aMeterPrefab, GameObject aTrailPrefab)
	{
		myPlayer_Number = aPlayerNumber;
		myPlayer_Color = aPlayerColor;

		myInput = "Boost" + myPlayer_Number.ToString();

		myBoost_Charge = aShip.BoostChargeAmount;
		myBoost_Force = aShip.BoostForce;
		myBoost_Max = aShip.MaxBoost;

		myYShift = aShip.BoostYShift;

		myBoostSound_Prefab = aSoundPrefab;
		myBoostMeter_Prefab = aMeterPrefab;

		myTrail_Prefab = aTrailPrefab;
		myTrail_Controller.Setup();
	}

	private void OnDisable()
	{
		myBoostSound_Pitch = 0.33f;
		myBoost_Current = 0f;
		isBoosting = false;

		if (myBoostMeter_Instance != null)
			Destroy(myBoostMeter_Instance);
	}

	private void CreateMeter()
	{
		if (myBoostMeter_Instance == null)
		{
			GameObject boostMeter = Instantiate(myBoostMeter_Prefab, transform);
			myBoostMeter_Instance = boostMeter;

			myBoostMeter_Renderer = myBoostMeter_Instance.GetComponent<SpriteRenderer>();
			myBoostMeter_Renderer.color = myPlayer_Color;

			myBoostMeter_Animator = myBoostMeter_Instance.GetComponent<Animator>();

			myBoostSound_Pitch = 0.33f;

			myBoostMeter_Instance.transform.localPosition = new Vector2(0f, 0f - myYShift);
		}
	}

	private void Update()
	{
		//Switch the isBoosting bool to false and keep the TrailController disabled whilst we have no boost charged.
		if (isBoosting && myBoost_Current <= 0f)
		{
			isBoosting = false;
			myTrail_Controller.Cut();
		}

		//If we're not already boosting, we accept input.
		if (!isBoosting)
		{
			if (Input.GetButtonDown(myInput))
				CreateMeter();

			//If the input button is pressed, the ship charges boost.
			if (Input.GetButton(myInput) && myBoostMeter_Instance != null)
			{
				//Charge up the boost by the Charge amount over time, stopping it from going over the maximum.
				myBoost_Current += myBoost_Charge * Time.deltaTime;
				myBoost_Current = Mathf.Min(myBoost_Current, myBoost_Max);

				//Update the animator's track of our ship's energy.
				myBoostMeter_Animator.SetFloat("Boost", myBoost_Current);

				//If we're out of the sound cooldown period, instantiate a new sound object and gradually increase pitch.
				if (myBoostSound_Cooldown <= 0)
				{
					if (myBoost_Current >= 0)
						myBoostSound_Pitch += 0.33f;

					GameObject thisBoostSound = Instantiate(myBoostSound_Prefab, this.transform);
					thisBoostSound.GetComponent<AudioComponent>().PlayMe(0.7f, -Mathf.Clamp(myBoostSound_Pitch, 0.33f, 1.66f));

					myBoostSound_Cooldown = 0.15f;
				}
				else
					myBoostSound_Cooldown -= 1f * Time.deltaTime;
			}

			if (Input.GetButtonUp(myInput))
			{
				if (myBoost_Current > 0f)
				{
					isBoosting = true;
					myTrail_Controller.Emit();
				}

				Destroy(myBoostMeter_Instance);
			}
		}
	}

	private void FixedUpdate()
	{
		//If the ship is boosting, then we apply force to the rigid body following the ship's direction.
		if (isBoosting)
		{
			//Add our boost force to the rigidbody, modified by how much boost is still charged.
			myRigidbody.AddForce(transform.up * (myBoost_Force * myBoost_Current));

			//As force is applied, we lose boost. The boost we lose equates to our maximum boost over a second, to make the boost drain quickly.
			myBoost_Current -= (myBoost_Max * Time.deltaTime);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (isBoosting)
			myTrail_Controller.Snip();
	}
}