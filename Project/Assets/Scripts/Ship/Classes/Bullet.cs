using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	19/09/2019
 * Last:	08/11/2019
 * Name:	Bullet
 * Vers:	2.1 - Adjusted Player No. to be an Int.
 */

[System.Serializable]
public class Bullet : MonoBehaviour
{
	//The player number is passed from the ShipWeapon script. This is used by the bullet in collision checks and tagging.
	private ShipManager myPlayer;
	public ShipManager Owner { get { return myPlayer; } }

	//Used to set the color/opacity of the bullet sprite.
	private SpriteRenderer mySpriteRenderer;
	public float Alpha { get { return mySpriteRenderer.color.a; } set { Color newColor = mySpriteRenderer.color; newColor.a = value; mySpriteRenderer.color = newColor; } }

	private void Awake()
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Setup(ShipManager aShipManager, Color aPlayerColor)
	{
		myPlayer = aShipManager;
		mySpriteRenderer.color = aPlayerColor;

		gameObject.layer = 9 + aShipManager.PlayerIndex;
	}

	private void OnCollisionEnter2D(Collision2D aCollision)
	{
		//If we collided with an Asteroid or a Ship
		if (aCollision.gameObject.GetComponent<Asteroid>() || aCollision.gameObject.GetComponent<ShipManager>())
		{
			Vector2 direction = aCollision.contacts[0].point - (Vector2)transform.position;
			direction = -direction;
			aCollision.rigidbody.AddForce(direction * 300f);

			Destroy(this.gameObject);
		}
		else
			Destroy(this.gameObject);
	}

    private void OnTriggerExit2D(Collider2D aCollider)
	{
		if (aCollider.tag == "Border")
		{
            //Objects leaving the map border will be moved to the opposite of their position.
            transform.Translate(new Vector2(transform.position.x * -2f, transform.position.y * -2f), Space.World);
		}
	}

	private void Update()
	{
		if (!GameManager.isPaused)
			//Opacity is reduced every tic by a multiplicative amount.
			if (mySpriteRenderer.color.a > 0f)
			{
				Color newColor = mySpriteRenderer.color;
				newColor.a -= 0.5f * Time.deltaTime;
				
				//Set the new color.
				GetComponent<SpriteRenderer>().color = newColor;
			}
			else
			{
				//At under 10% opacity, destroy the bullet.
				Destroy(gameObject);
			}
	}
}
