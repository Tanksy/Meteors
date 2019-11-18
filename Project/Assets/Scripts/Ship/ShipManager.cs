using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *	Auth:	Jake Anderson
 *	Date:	10/10/2019
 *	Last:	08/11/2019
 *	Name:	Ship Manager
 *	Vers:	1.3 - Improvement to scoring, Adjusted Player No. to be an Int.
 */

[System.Serializable]
public class ShipManager : MonoBehaviour
{
	[Header("Ship Data")]
	[Tooltip("The Ship's data.")]
	[SerializeField] private Ship myShip;
	public bool Status { get { return myShip.Status; } }
    public float CollisionThreshold { get { return myShip.CollisionThreshold; } }
	public List<DeathRecord> DeathRecords { get { return myShip.Deaths; } }

	[Header("Energy Data")]
	[Tooltip("GameObject used to display this ship's energy.")]
	[SerializeField] private GameObject myEnergyMeter_Prefab;

	[Header("Boost Data")]
	[Tooltip("GameObject used to display this ship's boost.")]
	[SerializeField] private GameObject myBoostMeter_Prefab;
	[Tooltip("The trail emitted when boosting is an instance of this prefab.")]
	[SerializeField] private GameObject myBoostTrail_Prefab;

	[Header("Weapon Data")]
	[Tooltip("The Bullet fired by this ship.")]
	[SerializeField] private GameObject myBullet_Prefab;
	[Tooltip("The Charged Bullet fired by this ship.")]
	[SerializeField] private GameObject myBullet_ChargedPrefab;
    [Tooltip("The object used to position the bullet when fired.")]
    [SerializeField] private GameObject myMuzzleObject;

	[Header("Visual Data")]
	private RectTransform myCanvas;
	//Used to hide the sprite when dead.
	private SpriteRenderer mySprite;
	public SpriteRenderer Sprite { get { return mySprite; } }
    [Tooltip("The ship's detail sprite layer.")]
    [SerializeField] private SpriteRenderer myDetailSprite;

	[Header("Audio Data")]
	[Tooltip("Audio object for the Death sound.")]
	[SerializeField] private GameObject myDeathSound;
	[Tooltip("Audio object for the Shoot sound.")]
	[SerializeField] private GameObject myShootSound_Prefab;
	[Tooltip("Audio object for the Charge sound.")]
	[SerializeField] private GameObject myChargeSound_Prefab;
	[Tooltip("Audio object for the Boost sound.")]
	[SerializeField] private GameObject myBoostSound_Prefab;

	[Header("Effect Data")]
	[Tooltip("Effect object for the ship's death.")]
	[SerializeField] private GameObject myDeathEffect;
	[Tooltip("Effect object for the ship's teleport in.")]
	[SerializeField] private GameObject myTeleportEffect;

	//Component Data
	//Used for collision handling.
	private PolygonCollider2D myCollider;
	//Used for physics interactions.
	private Rigidbody2D myRigidBody;

	//The player number is set by the Game Manager.
	private Player myPlayer;
	public int PlayerIndex { get { return myPlayer.Index; } }
	//The player color is also set by the Game Manager.
	private Palette myPlayer_Palette;
	public Palette Palette { get { return myPlayer_Palette; } set { myPlayer_Palette = value; } }
	public Color PrimaryColor { get { return myPlayer_Palette.Colors[0]; } }
	public Color SecondaryColor { get { return myPlayer_Palette.Colors[1]; } }
	public byte PlayerScore { get { return myPlayer.Score; } }

	//Used to handle the respawning time.
	private IEnumerator myRespawnCoroutine;

	//This is the ship instance that our ShipManager is attached to.
	private GameObject myInstance;
	public GameObject Instance { get { return myInstance; } set { myInstance = value; } }
	//This handles ship movement and inputs, as well as engine particle emission.
	private ShipMovement myMovement;
	//This handles ship weapons, firing & energy.
	private ShipWeapon myWeapon;
	//This handles the ship booster and engine trails.
	private ShipBooster myBooster;

    //This is used to check if a player is boosting when they collide.
    public bool Boosting { get { return myBooster.Boosting; } }

	//The ship will spawn here at the start of a new match.
	private Vector2[] mySpawnpoints;

    //The ScoreManager is passed to us by the GameManager. We access the ScoreManager and tell it to increase our displayed score whenever we score a point.
    private ScoreManager myScoreManager;

	private void Awake()
	{
		myCollider = GetComponent<PolygonCollider2D>();
		myRigidBody = GetComponent<Rigidbody2D>();

		mySprite = GetComponent<SpriteRenderer>();
	}

	public void Setup(Player aPlayer, Vector2[] aSpawnpointArray, ScoreManager aScoreManager)
	{
		myPlayer = aPlayer;
		myPlayer_Palette = aPlayer.Palette;

		mySprite.color = SecondaryColor;
		myDetailSprite.color = PrimaryColor;

		myMovement = gameObject.AddComponent<ShipMovement>();
		myMovement.Setup(aPlayer.Index, myShip.Force, PrimaryColor);
		myWeapon = gameObject.AddComponent<ShipWeapon>();
		myWeapon.Setup(aPlayer, myShip, myEnergyMeter_Prefab, myChargeSound_Prefab, myBullet_Prefab, myBullet_ChargedPrefab, myShootSound_Prefab, myMuzzleObject);
		myBooster = gameObject.AddComponent<ShipBooster>();
		myBooster.Setup(aPlayer.Index, PrimaryColor, myShip, myBoostSound_Prefab, myBoostMeter_Prefab, myBoostTrail_Prefab);

		mySpawnpoints = aSpawnpointArray;
        myScoreManager = aScoreManager;

		gameObject.layer = 9 + aPlayer.Index;
	}

	public void EnableControl()
	{
		myMovement.enabled = true;
		myWeapon.enabled = true;
		myBooster.enabled = true;
	}

	public void DisableControl()
	{
		myMovement.enabled = false;
		myWeapon.enabled = false;
		myBooster.enabled = false;
	}

	private void WarpOut()
	{
		TeleportEffect(false);
	}

	private IEnumerator Respawn(Transform aKillerTransform)
	{
		yield return new WaitForSeconds(3f);

		//If our killer has a transform.
		if (aKillerTransform)
		{
			//We want to spawn away from our opponent and prevent them from spawn camping us,
			//	so check their x position and spawn on the opposite half of the map from them.
			if (aKillerTransform.position.x <= 0f)
			{
				//West side.
				Spawn(mySpawnpoints[1], new Vector3(0f, 0f, 180f));
			}
			else
			{
				//East side.
				Spawn(mySpawnpoints[0], new Vector3(0f, 0f, 0f));
			}
		}
		else
			Spawn(mySpawnpoints[Random.Range(0, mySpawnpoints.Length)], new Vector3(0f, 0f, 0f));
	}

	public void Spawn(Vector2 aPosition, Vector2 aRotation)
	{
		//Turn the sprites visible.
		Color newColor = mySprite.color;
		newColor.a = 1f;
		mySprite.color = newColor;

		Color newDetailColor = myDetailSprite.color;
		newDetailColor.a = 1f;
		myDetailSprite.color = newDetailColor;

		myShip.ResetStreaks();

		//Reactivate the Movement script.
		myMovement.enabled = true;
		myWeapon.enabled = true;
		myBooster.enabled = true;

		//Move the object.
		transform.SetPositionAndRotation(aPosition, Quaternion.Euler(aRotation));

		//We're alive!
		myShip.Status = true;
	}

	private void AddDeathRecord(GameObject aKiller)
	{
		if (aKiller.GetComponent<Asteroid>())
		{
			Asteroid roid = aKiller.GetComponent<Asteroid>();
			if (roid.PlayerTag != 0)
			{
				myShip.Deaths.Add(new DeathRecord("Smashed", roid.PlayerTag));
			}
			else
			{
				myShip.Deaths.Add(new DeathRecord("Crashed", -1));
			}
		}
		else if (aKiller.GetComponent<BulletCharged>())
		{
			ShipManager killer = aKiller.GetComponent<BulletCharged>().Owner;
			myShip.Deaths.Add(new DeathRecord("Shot", killer.PlayerIndex));
		}
		else if (aKiller.GetComponent<ShipBooster>().Boosting)
		{
			myShip.Deaths.Add(new DeathRecord("Rammed", aKiller.GetComponent<ShipManager>().PlayerIndex));
		}
		else if (aKiller.GetComponent<ShipManager>())
		{
			//Add this death to the ship's death record.
			myShip.Deaths.Add(new DeathRecord("Crashed", -1));
		}
	}

	public void Die(GameObject aKiller)
	{
		//Deactivate the Movement script.
		myMovement.enabled = false;
		myWeapon.enabled = false;
		myBooster.enabled = false;

		//Turn the ship sprites invisible.
		Color newColor = mySprite.color;
		newColor.a = 0f;
		mySprite.color = newColor;

		Color newDetailColor = myDetailSprite.color;
		newDetailColor.a = 0f;
		myDetailSprite.color = newDetailColor;

		//Instantiate the Death Effect.
		GameObject thisDeathEffect = Instantiate(myDeathEffect, this.transform);
		EffectController controller = thisDeathEffect.GetComponent<EffectController>();
		controller.Color = PrimaryColor;
		//Instantiate the Death sound.
		GameObject thisDeathSound = Instantiate(myDeathSound, this.transform);
		//Access the Audio Controller and play the sound.
		thisDeathSound.GetComponent<AudioComponent>().PlayMe(0.9f, Random.Range(0.8f, 1.0f));
		//Separate the sound object from the parent.
		thisDeathSound.transform.parent = null;

        if (aKiller.GetComponent<ShipManager>())
            Debug.Log("Crashed into Player " + aKiller.GetComponent<ShipManager>().PlayerIndex);
        if (aKiller.GetComponent<Asteroid>())
            Debug.Log("Crashed into Roid.");

		myRespawnCoroutine = Respawn(aKiller.transform);
		StartCoroutine(myRespawnCoroutine);
	}

	private IEnumerator CleanupTeleport()
	{
		yield return new WaitForSeconds(0.667f);
		Destroy(GetComponent<SpriteMask>());
		Destroy(GetComponent<AnimatedSpriteMask>());

		mySprite.maskInteraction = SpriteMaskInteraction.None;
		myDetailSprite.maskInteraction = SpriteMaskInteraction.None;
	}

	//Applies a sprite mask effect.
	public void TeleportEffect(bool isTeleportingIn)
	{
        if (!GetComponent<SpriteMask>())
        {
			if (GetComponent<SpriteRenderer>())
			{
				SpriteMask spriteMask = gameObject.AddComponent(typeof(SpriteMask)) as SpriteMask;
				AnimatedSpriteMask maskAnimator = gameObject.AddComponent(typeof(AnimatedSpriteMask)) as AnimatedSpriteMask;
				maskAnimator.Mask = spriteMask;

				//Create a new TeleportEffect object.
				GameObject animationHolder = Instantiate(myTeleportEffect, transform);
				//Move the object off-screen.
				animationHolder.transform.Translate(new Vector2(100, 100));

				maskAnimator.Sprite = animationHolder.GetComponent<SpriteRenderer>();

				if (isTeleportingIn == true)
				{
					mySprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
					myDetailSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
					StartCoroutine("CleanupTeleport");
				}
				else
				{
					mySprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
					myDetailSprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
				}
			}
        }
	}

	//Called by the world at the end of a round.
	public void EndGame()
	{
		TeleportEffect(false);

		if (myShip.HighestStreak > 1)
		{
			Medal streakMedal = new Medal("Kill Streak", 1, "Achieved a " + myShip.HighestStreak.ToString() + " killing spree!");
			myPlayer.Medals.Add(streakMedal);
		}

		if (myShip.HighestLifetime > 60f)
		{
			Medal lifetimeMedal = new Medal("Lifetime", 1, "Lived for " + ((int)myShip.HighestLifetime).ToString() + " seconds!");
			myPlayer.Medals.Add(lifetimeMedal);
		}
	}

	private void Update()
	{
		//Count up our Lifetime.
		myShip.Lifetime += 1f * Time.deltaTime;
	}

	public void Score()
	{
		//Increase my score.
		myPlayer.Score++;
		//Increase my streak.
		myShip.Streak++;
        //Tell the ScoreManager to increase our score.
        myScoreManager.UpdateScore(myPlayer.Index);
	}

	private void OnCollisionEnter2D(Collision2D aCollision)
	{
		if (myShip.Status)
		{
			GameObject collidedWith = aCollision.gameObject;
            
            //If we've collided with a Bullet.
            if (collidedWith.GetComponent<BulletCharged>())
            {
                BulletCharged bullet = collidedWith.GetComponent<BulletCharged>();
                float alpha = bullet.Alpha;
                if (alpha > 0.33f)
                {
                    //Log the death.
                    AddDeathRecord(collidedWith);
                    Die(collidedWith);
                    bullet.Owner.Score();
                }
            }
            else
            {
                //If the object I am colliding with is moving faster than me and greater than my collision theshold, and it's not a simple bullet.
                if (aCollision.relativeVelocity.magnitude > myShip.CollisionThreshold && !collidedWith.GetComponent<Bullet>())
                {
                    //Log the death.
                    AddDeathRecord(collidedWith);
                    Die(collidedWith);
                }
            }
		}
	}

	//Objects leaving the map border will be moved to the opposite of their position.
	private void OnTriggerExit2D(Collider2D aCollider)
	{
		if (aCollider.tag == "Border")
		{
            Debug.Log("Passed the Border");
			gameObject.transform.Translate(new Vector2(transform.position.x * -2f, transform.position.y * -2f), Space.World);
		}
	}
}
