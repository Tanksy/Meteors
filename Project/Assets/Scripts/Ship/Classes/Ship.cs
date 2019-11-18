using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	11/10/2019
 *	Last:	24/10/2019
 *	Name:	Ship
 *	Vers:	1.1 - Boost
 */

[System.Serializable]
public class Ship
{
	[Header("Status")]
	[Tooltip("This ship's living status.")]
	[SerializeField] private bool isAlive;
	public bool Status { get { return isAlive; } set { isAlive = value; } }
	[Tooltip("This ship's recorded deaths.")]
	[SerializeField] private List<DeathRecord> myDeaths;
	public List<DeathRecord> Deaths { get { return myDeaths; } set { myDeaths = value; } }
	[Tooltip("This ship's current kill streak.")]
	[SerializeField] private byte myKillStreak;
	//Setting the kill streak also checks against the highest streak and sets that if myKillStreak is higher.
	public byte Streak { get { return myKillStreak; } set { myKillStreak = value; if (myKillStreak > myHighestStreak) myHighestStreak = value; } }
	[Tooltip("This ship's highest kill streak.")]
	[SerializeField] private byte myHighestStreak;
	public byte HighestStreak { get { return myHighestStreak; } }
	[Tooltip("This ship's current lifetime.")]
	[SerializeField] private float myLifetime;
	//Setting the lifetime also checks against the highest lifetime and sets that if myLifetime is higher. 
	public float Lifetime { get { return myLifetime; } set { myLifetime = value; if (myLifetime > myHighestLifetime) myHighestLifetime = value; } }
	[Tooltip("This ship's highest lifetime.")]
	[SerializeField] private float myHighestLifetime;
	public float HighestLifetime { get { return myHighestLifetime; } }

	[Header("Movement Data")]
	[Tooltip("Force applied per FixedUpdate when holding the Move axis.")]
	[SerializeField] private float myForce;
	public float Force { get { return myForce; } set { myForce = value; } }
	[Tooltip("Speed at which the ship rotates when holding the Turn axis.")]
	[SerializeField] private float myHandling;
	public float Handling { get { return myHandling; } set { myHandling = value; } }

	[Header("Energy Data")]
	[Tooltip("Maximum energy storage.")]
	[SerializeField] private float myMaxEnergy;
	public float MaxEnergy { get { return myMaxEnergy; } set { myMaxEnergy = value; } }
	private float myEnergy;
	public float Energy { get { return myEnergy; } set { myEnergy = Mathf.Clamp(value, 0f, myMaxEnergy); } }
	[Tooltip("How much Energy is charged per frame of input received.")]
	[SerializeField] private float myEnergyChargeAmount;
	public float EnergyChargeAmount { get { return myEnergyChargeAmount; } }
	[Tooltip("How much the energy charging sprite is shifted up the Y axis.")]
	[SerializeField] private float myEnergyYShift;
	public float EnergyYShift { get { return myEnergyYShift; } }

	[Header("Boost Data")]
	[Tooltip("The maximum amount of Boost this ship can charge.")]
	[SerializeField] private float myMaxBoost;
	public float MaxBoost { get { return myMaxBoost; } }
	[Tooltip("How much Boost is charged per frame of input received.")]
	[SerializeField] private float myBoostChargeAmount;
	public float BoostChargeAmount { get { return myBoostChargeAmount; } set { myBoostChargeAmount = value; } }
	[Tooltip("The minimum amount of force applied when boosting.")]
	[SerializeField] private float myBoostForce;
	public float BoostForce { get { return myBoostForce; } }
	[Tooltip("How much the booster charging sprite is shifted down the Y axis.")]
	[SerializeField] private float myBoostYShift;
	public float BoostYShift { get { return myBoostYShift; } }

	[Header("Other Properties")]
	[Tooltip("How big of a difference in velocity this ship must have with colliding objects to be killed.")]
	[SerializeField] private float myCollisionThreshold;
	public float CollisionThreshold { get { return myCollisionThreshold; } set { myCollisionThreshold = value; } }

	//Default Constructor
	public Ship()
	{
		myForce = 3f;
		myHandling = 3f;

		myMaxEnergy = 3f;
		myEnergy = 0f;
		myEnergyChargeAmount = 2f;

		myMaxBoost = 3f;
		myBoostChargeAmount = 2f;
		myBoostForce = 5f;

		myDeaths = new List<DeathRecord>();
		myKillStreak = 0;
		myHighestStreak = 0;
		myLifetime = 0f;
		isAlive = true;
	}

	//Manual Constructor
	public Ship(float aForce, float aHandling, float aMaxEnergy, float aEnergyChargeSpeed, float aMaxBoost, float aBoostChargeSpeed, float aBoostForce)
	{
		myForce = aForce;
		myHandling = aHandling;

		myMaxEnergy = aMaxEnergy;
		myEnergy = 0f;
		myEnergyChargeAmount = aEnergyChargeSpeed;

		myMaxBoost = aMaxBoost;
		myBoostChargeAmount = aBoostChargeSpeed;
		myBoostForce = aBoostForce;

		myDeaths = new List<DeathRecord>();
		myKillStreak = 0;
		myHighestStreak = 0;
		myLifetime = 0f;
		isAlive = true;
	}

	public void ResetStreaks()
	{
		myKillStreak = 0;
		myHighestStreak = 0;
		myLifetime = 0f;
	}
}
