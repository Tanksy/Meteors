using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	20/09/2019
 * Last:	30/10/2019
 * Name:	Asteroid
 * Vers:	1.2.3 - Match ending fade out and destroy
 */

public class Asteroid : MonoBehaviour
{
	[Header("Asset Data")]
	[Tooltip("Audio object for the Death sound.")]
	[SerializeField] private GameObject myDeathSound;
	[Tooltip("The prefab to use for debris.")]
	[SerializeField] private GameObject myDebris;

	[Header("Settings")]
	[Tooltip("1 for Small, 2 for Medium, 3 for Large")]
	[SerializeField] private byte myCategory;
	private Color myColor;
    [Tooltip("How much force is applied to splinter asteroids.")]
    [SerializeField] private float mySplinterForce;

	private int myPlayerTag;
	public int PlayerTag { get { return myPlayerTag; } set { myPlayerTag = value; } }

    private ShipManager myTagger;
    public ShipManager Tagger { get { return myTagger; } set { myTagger = value; } }

	//Used to get prefabs for splinter asteroids.
	private AsteroidManager myParentController;

	//Used to calculate myMass and control physics interactions.
	private Rigidbody2D myRigidbody;

	//Used to set, hide and show the sprite.
	private SpriteRenderer mySpriteRenderer;
	public SpriteRenderer Sprite { get { return mySpriteRenderer; } set { mySpriteRenderer = value; } }

	//Used to know if the asteroid is a splinter from a Large Asteroid.
	private bool isSplinter;

	//Used to know if the asteroid is respawning or not.
	private bool isSpawning;

	//Used to know if the match is ending or not.
	private bool isEnding;

	//Used for the respawn coroutine.
	private IEnumerator myCoroutine;

	//Used to remember our initial spawn point.
	private Vector3 mySpawnPoint;

	//Getters
	public byte GetCategory() { return myCategory; }

	//Setters
	public void SetSplinter(bool aBool) { isSplinter = aBool; }
	public void SetSpawning(bool aBool) { isSpawning = aBool; }

	private void Awake()
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		myRigidbody = GetComponent<Rigidbody2D>();
	}

	public void Setup(Color aColor, Vector2 aPosition)
	{
		myColor = aColor;
		mySpriteRenderer.color = myColor;

		Spawn(aPosition);
	}

	//Constructor
	private void Start()
	{
		myParentController = transform.parent.GetComponent<AsteroidManager>();
		myCoroutine = DestroyAndRespawn();
		mySpawnPoint = transform.position;
		myPlayerTag = -1;
	}

	public void FadeAndDestroy()
	{
		isEnding = true;
	}

	/**
	 * Called when we die.
	 * This handles the Asteroid's death, and triggers respawn if not marked as a splinter asteroid.
	 * Respawning is handled in Update.
	 */
	IEnumerator DestroyAndRespawn()
	{
		//Hide the sprite behind the backdrop.
		mySpriteRenderer.sortingOrder = -1;
		//Stop physical interactions.
		myRigidbody.simulated = false;

		//Instantiate the Death audio.
		GameObject thisDeathSound = Instantiate(myDeathSound, this.transform);
		//Access the Audio Controller and play the sound, passing a random value for pitch.
		thisDeathSound.GetComponent<AudioComponent>().PlayMe(0.8f, Random.Range(0.6f, 1.1f));
		//Disconnect the death sound from the Asteroid parent, so the sound won't follow the asteroid.
		thisDeathSound.transform.parent = null;

		//Spawn some Debris objects.
		for (int count = 0; count < (3 * myCategory); count++)
			Instantiate(myDebris, transform);

		//If we're a splinter, we just get destroyed.
		if (isSplinter)
		{
			yield return new WaitForSeconds(3);
			Destroy(gameObject);
		}
		//Otherwise, we can respawn.
		else
		{
			//Create a var to hold our new Asteroid.
			GameObject newRock;

			//Check the category to determine the respawn time.
			switch (myCategory)
			{
				//Only Large Asteroids have a longer respawn time. Huge Asteroids can't die, so don't respawn.
				default:
					yield return new WaitForSeconds(3);
					//Instantiate a new Asteroid.
					newRock = Instantiate(myParentController.Categories[0].Asteroids[(int)Random.Range(0, myParentController.Categories[0].Asteroids.Length)], this.transform.parent);
					break;

				case 2:
					yield return new WaitForSeconds(9);
					//Instantiate a new Asteroid.
					newRock = Instantiate(myParentController.Categories[1].Asteroids[(int)Random.Range(0, myParentController.Categories[1].Asteroids.Length)], this.transform.parent);
					break;
			}

			//Play the death sound in reverse, to indicate it's spawned.
			GameObject spawnSound = Instantiate(myDeathSound, newRock.transform);
			spawnSound.GetComponent<AudioComponent>().PlayMe(0.8f, -1.0f);

			//Run the Spawn function of the new asteroid.
			newRock.GetComponent<Asteroid>().Spawn(mySpawnPoint);

			//Destroy the old Asteroid.
			Destroy(gameObject);
		}
	}

	private void Spawn(Vector2 aPosition)
	{
		//Set the position and a random rotation
		transform.position = aPosition;
		transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0, 360)));
		//Start at 0 scale.
		transform.localScale = new Vector3(0f, 0f, 0f);

		//We want to avoid the asteroid moving with 0 force in both directions.
		//	This loop will re-roll if both x and y are 0.
		float x = 0.0f;
		float y = 0.0f;
		while (x == 0.0f && y == 0.0f)
		{
			x = Mathf.Round(Random.Range(-1f, 1f));
			y = Mathf.Round(Random.Range(-1f, 1f));
		}
		gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(x,y) * (20 * myCategory));

		//Set isSpawning to true to allow Update to handle the spawn effect.
		isSpawning = true;
	}

    private void OnCollisionEnter2D(Collision2D aCollision)
    {
		//If we collided with a charged bullet.
		if (aCollision.gameObject.GetComponent<BulletCharged>())
		{
			SpriteRenderer bulletSprite = aCollision.gameObject.GetComponent<SpriteRenderer>();
			float bulletAlpha = bulletSprite.color.a;
			if (bulletAlpha > 0.33f)
			{
				switch (myCategory)
				{
					//Small
					case 1:
						StartCoroutine(myCoroutine);
						break;
					//Medium
					case 2:
						for (int i = 0; i < 2; i++)
						{
							//Get the Small Asteroid prefab from the parent controller.
							GameObject prefab = myParentController.Categories[0].Asteroids[(int)Random.Range(0, myParentController.Categories[0].Asteroids.Length)];
							//Instantiate at our position, set the parent transform to ours.
							GameObject splinter = Instantiate(prefab, transform.parent);
							//Move the asteroids a little bit.
							splinter.transform.position = transform.position + new Vector3(Random.Range(-0.4f, 0.4f), 0f, 0f);
							//Set the asteroid as a splinter.
							splinter.GetComponent<Asteroid>().SetSplinter(true);

							//Apply a random rotation.
							float randomRotation = Random.Range(0f, 360f);
							splinter.transform.Rotate(new Vector3(0f, 0f, randomRotation), Space.World);
							//Apply force to the splinter asteroid.
							Rigidbody2D splintRB = splinter.GetComponent<Rigidbody2D>();
							Vector2 direction = aCollision.contacts[0].point - (Vector2)transform.position;
							direction = -direction.normalized;
							splintRB.AddForceAtPosition(direction * mySplinterForce, aCollision.contacts[0].point);
							Asteroid splinterController = splinter.GetComponent<Asteroid>();

							//Get the bullet owner.
							BulletCharged bullet = aCollision.gameObject.GetComponent<BulletCharged>();
							//Tag the asteroids with the new owner.
							splinterController.PlayerTag = bullet.Owner.PlayerIndex;
							splinterController.Tagger = bullet.Owner;
						}

						//Destroy the asteroid.
						StartCoroutine(myCoroutine);
						break;
					//Large
					case 3:
						break;
				}
			}
		}
		if (aCollision.gameObject.GetComponent<Bullet>())
            myTagger = aCollision.gameObject.GetComponent<Bullet>().Owner;
    }

    //Objects leaving the map border will be moved to the opposite of their position.
    private void OnTriggerExit2D(Collider2D aCollider)
	{
		if (aCollider.tag == "Border")
		{
			transform.Translate(new Vector2(transform.position.x * -2f, transform.position.y * -2f), Space.World);
		}
	}

	private void Update()
	{
		if (isSpawning)
		{
			//As we're "spawning", scale and rotate the object.
			this.transform.localScale += new Vector3(1.0f,1.0f,1.0f) * Time.deltaTime;
			this.transform.Rotate(new Vector3(0, 0, 75) * Time.deltaTime);

			if (this.transform.localScale.x >= 1.0f)
			{
				//Reactivate myRigidBody.
				myRigidbody.simulated = true;
				//Stop the Coroutine.
				StopAllCoroutines();

				isSpawning = false;
			}
		}
		if (isEnding)
		{
			if (mySpriteRenderer.color.a > 0f)
			{
				Color newColor = mySpriteRenderer.color;
				newColor.a -= 0.66f * Time.deltaTime;
				mySpriteRenderer.color = newColor;
			}
			else
				Destroy(gameObject);
		}
	}
}
