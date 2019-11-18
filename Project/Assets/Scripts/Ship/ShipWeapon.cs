using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	27/10/2019
 *	Last:	08/11/2019
 *	Vers:	1.1 - Adjusted Player No. to be an Int.
 */

public class ShipWeapon : MonoBehaviour
{
	//Player number is passed from the Ship object. This is used for getting the correct player's input.
	private int myPlayer_Index;

	//Player color is also passed from the ship. This is used to color the energy meter sprite.
	private Color myPlayer_Color;

	//The player number is used to set the correct name of the input we're looking for.
	private string myInput;

	//The weapon charges energy when receiving input.
	private float myEnergy_Current;

	//This is the amount that the weapon charges each frame of input.
	private float myEnergy_Charge;

	//This is the maximum that the weapon can be charged.
	private float myEnergy_Max;

	//The weapon's energy meter is an instance of this prefab.
	private GameObject myEnergyMeter_Prefab;

	//The weapon's energy meter instance.
	private GameObject myEnergyMeter_Instance;

	//The weapon's energy meter sprite is used to set color to the player's color.
	private SpriteRenderer myEnergyMeter_Renderer;

	//The animator is used to control what sprite animations are playing when charging.
	private Animator myEnergyMeter_Animator;

	//The sound of the weapon charging comes from instances of this prefab.
	private GameObject myChargeSound_Prefab;

	//We control how often this sound object is created using a cooldown value.
	private float myChargeSound_Cooldown;

	//The pitch of the charge sound is adjusted as we receive input.
	private float myChargeSound_Pitch;

	//The bullet fired is an instance of this prefab.
	private GameObject myBulletPrefab;

	//The charged bullet fired is an instance of this prefab.
	private GameObject myChargedBulletPrefab;

	//The sound of the weapon firing comes from an instance of this prefab.
	private GameObject myShootSound_Prefab;

    //The muzzle transform is where the bullet is placed when instantiated.
    private Transform myMuzzle;

	//The energy meter sprite is shifted up the Y axis by this amount.
	private float myYShift;

	private void Awake()
	{
		myEnergy_Current = 0f;

		myInput = "Weapon0";
	}

	private void OnDisable()
	{
		myEnergy_Current = 0f;
		Destroy(myEnergyMeter_Instance);
	}

	public void Setup(Player aPlayer, Ship aShip, GameObject anEnergyMeterPrefab, GameObject aChargeSoundPrefab, GameObject aBulletPrefab, GameObject aChargedBulletPrefab, GameObject aShootSoundPrefab, GameObject aMuzzlePoint)
	{
		myPlayer_Index = aPlayer.Index;

		myEnergy_Max = aShip.MaxEnergy;
		myEnergy_Charge = aShip.EnergyChargeAmount;

		myYShift = aShip.EnergyYShift;

		myPlayer_Color = aPlayer.Palette.Colors[0];
		myPlayer_Color.a = 0.75f;

		myInput = "Weapon" + myPlayer_Index.ToString();

		myEnergyMeter_Prefab = anEnergyMeterPrefab;
		myChargeSound_Prefab = aChargeSoundPrefab;
		myBulletPrefab = aBulletPrefab;
		myChargedBulletPrefab = aChargedBulletPrefab;
		myShootSound_Prefab = aShootSoundPrefab;

        myMuzzle = aMuzzlePoint.transform;
	}

	private void FireBullet()
	{
		GameObject bulletClone;
		//Instantiate the bullet prefab.
		if (myEnergy_Current > (myEnergy_Max * 0.5f))
		{
			bulletClone = Instantiate(myChargedBulletPrefab, myMuzzle.position, myMuzzle.rotation, transform.parent) as GameObject;
			bulletClone.GetComponent<BulletCharged>().Setup(GetComponent<ShipManager>(), myPlayer_Color);
		}
		else
		{
			bulletClone = Instantiate(myBulletPrefab, myMuzzle.position, myMuzzle.rotation, transform.parent) as GameObject;
			bulletClone.GetComponent<Bullet>().Setup(GetComponent<ShipManager>(), myPlayer_Color);
		}
		
        bulletClone.GetComponent<Rigidbody2D>().velocity = myMuzzle.up * (myEnergy_Max + gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);

		//Instantiate the Shoot sound.
		GameObject thisShotSound = Instantiate(myShootSound_Prefab, transform);
		//Access the Audio Controller and play the sound.
		thisShotSound.GetComponent<AudioComponent>().PlayMe(0.9f, 1.0f);
	}

	private void CreateMeter()
	{
		if (myEnergyMeter_Instance == null)
		{
			GameObject energyMeter = Instantiate(myEnergyMeter_Prefab, transform);
			myEnergyMeter_Instance = energyMeter;

			myEnergyMeter_Renderer = myEnergyMeter_Instance.GetComponent<SpriteRenderer>();
			myEnergyMeter_Renderer.color = myPlayer_Color;

			myEnergyMeter_Animator = myEnergyMeter_Instance.GetComponent<Animator>();

			myEnergyMeter_Instance.transform.localPosition = new Vector2(0f, 0f + myYShift);
		}
	}

	private void Update()
	{
		if (!GameManager.isPaused)
		{
			//When the weapon button is first pressed, instantiate the energy meter prefab.
			if (Input.GetButtonDown(myInput))
				CreateMeter();

			//As the weapon button is held down, we increase an energy value.
			if (Input.GetButton(myInput))
			{
				//Increase our ship's energy by the charge amount over time, clamped to Max Energy.
				myEnergy_Current += (myEnergy_Charge * Time.deltaTime);
				myEnergy_Current = Mathf.Min(myEnergy_Current, myEnergy_Max);

				//Only play a charging sound if the cooldown is at 0 or lower.
				if (myChargeSound_Cooldown <= 0)
				{
					//Increase the pitch of the charge sound if the ship is generating energy.
					if (myEnergy_Current >= 0f)
						myChargeSound_Pitch += 0.15f;

					//Instantiate the Charge sound.
					GameObject thisChargeSound = Instantiate(myChargeSound_Prefab, transform);
					//Access the Audio Controller and play the sound.
					thisChargeSound.GetComponent<AudioComponent>().PlayMe(0.66f, -Mathf.Clamp(myChargeSound_Pitch, 0.33f, 1.33f));
					//Set the sound cooldown.
					myChargeSound_Cooldown = 0.15f;
				}
				//Decrease the cooldown over time.
				else
					myChargeSound_Cooldown -= 1f * Time.deltaTime;
			}

			//If the weapon button is released, we fire a Bullet and pass some information.
			if (Input.GetButtonUp(myInput))
			{
				FireBullet();

				//Reset the charge sound pitch.
				myChargeSound_Pitch = 0.33f;

				//Destroy the energy meter instance.
				Destroy(myEnergyMeter_Instance);

				myEnergy_Current = 0f;
			}
		}
	}
}
