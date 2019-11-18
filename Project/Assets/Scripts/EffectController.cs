using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	19/10/2019
 *	Last:	19/10/2019
 *	Name:	Effect Controller
 *	Vers:	1.0 - Initial Version.
 */

public class EffectController : MonoBehaviour
{
	//private Animator myAnimator;
	private SpriteRenderer mySpriteRenderer;
	public Color Color { get { return mySpriteRenderer.color; } set { mySpriteRenderer.color = value; } }

	[Header("Settings")]
	[Tooltip("How long the game object will live for.")]
	[SerializeField] private float myLifetime;

	private IEnumerator myCoroutine;

	private IEnumerator WaitAndDie(float aTime)
	{
		yield return new WaitForSeconds(aTime);
		Destroy(this.gameObject);
	}

	private void Awake()
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		//Separate ourselves from the parent.
		transform.parent = null;

		//myAnimator = GetComponent<Animator>();
		myCoroutine = WaitAndDie(myLifetime);
		StartCoroutine(myCoroutine);
	}
}
