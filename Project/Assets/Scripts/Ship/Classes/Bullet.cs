using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	19/09/2019
 * Last:	23/11/2019
 * Name:	Bullet
 * Vers:	2.1.1 - Moved border check to border manager.
 */

[System.Serializable]
public class Bullet : MonoBehaviour
{
	//The player number is passed from the ShipWeapon script. This is used by the bullet in collision checks and tagging.
	private ShipManager myPlayer;
	public ShipManager Owner { get { return myPlayer; } }

	//Used to set the color/opacity of the bullet sprite.
	private SpriteRenderer mySpriteRenderer;
	private float myLifetime;

	private void Awake()
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		myLifetime = 3f;
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

	private void Update()
	{
		if (!GameManager.isPaused)
			//Opacity is reduced every tic by a multiplicative amount.
			if (myLifetime > 0f)
				myLifetime -= 1f * Time.deltaTime;
			else
				Destroy(gameObject);
	}
}
