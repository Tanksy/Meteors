using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	23/11/2019
 *	Last:	23/11/2019
 *	Vers:	1.0 - Border detects objects when they leave the trigger area and send them to the opposite side of the border to create a loop.
**/

public class BorderManager : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("The width of the border.")]
	[SerializeField] private float myWidth;
	public float Width { get { return myWidth; } }
	[Tooltip("The height of the border.")]
	[SerializeField] private float myHeight;
	public float Height { get { return myHeight; } }

	private BoxCollider2D myCollider;

	private void Awake()
	{
		myCollider = GetComponent<BoxCollider2D>();
	}

	//Set the size of the map.
	private void Start()
	{
		myCollider.size = new Vector2(myWidth, myHeight);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		collision.transform.Translate(new Vector2(collision.transform.position.x * -2f, collision.transform.position.y * -2f), Space.World);
	}
}
