using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	29/09/2019
 * Last:	29/09/2019
 * Name:	Debris
 * Vers:	1.0
 * Hist:
 *	1.0 - Initial version.
 */

public class Debris : MonoBehaviour
{
	//Used when randomly flipping the X/Y of the sprite.
	private SpriteRenderer mySprite;

	//Used to direct the debris.
	private Vector2 myMovementVector;

	//Uded to spin the sprite.
	private float myNewRotation;


	private void Start()
	{
		mySprite = gameObject.GetComponent<SpriteRenderer>();

		//Start with a random rotation.
		float randomRotation = Random.Range(0f, 180f);
		this.transform.Rotate(new Vector3(0f, 0f, randomRotation), Space.World);

		//Simple 50% chance to flip the sprite for either axis.
		float roll = Random.value;
		if (roll > 0.5f)
			mySprite.flipX = true;
		roll = Random.value;
		if (roll > 0.5f)
			mySprite.flipY = true;

		//Get a random movement vector that has at least some speed.
		myMovementVector = new Vector2();
		while (myMovementVector.x == 0f || myMovementVector.y == 0f)
		{
			myMovementVector = new Vector2((int)Random.Range(-5f, 5f), (int)Random.Range(-5f, 5f));
		}

		transform.parent = null;
	}

	//Debris are pretty simple objects that just rotate, drift and shrink over their life.
	private void Update()
	{
		myNewRotation += Time.deltaTime * 50f;

		transform.localRotation = Quaternion.Euler(0, 0, myNewRotation);
		transform.localScale -= new Vector3(0.6f, 0.6f, 0f) * Time.deltaTime;

		if (transform.localScale.x <= 0.0f)
		{
			//We're small enough to be removed.
			Destroy(this.gameObject);
		}

		transform.Translate(myMovementVector * Time.deltaTime);
		myMovementVector -= myMovementVector * 0.1f;

	}
}
