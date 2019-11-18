using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	17/10/2019
 *	Last:	13/11/2019
 *	Name:	Delayed Destroy
 *	Vers:	2.0 - Begins the coroutine to destroy self any time Delay is set, or if it's already set.
 */

public class DelayedDestroy : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Time it takes to be destroyed.")]
	[SerializeField] private float myTime;
	public float Delay { set { myTimer = new WaitForSeconds(value); myTime = value; StartCoroutine(DestroyMe()); } }
	private WaitForSeconds myTimer;

	private void Start()
	{
		if (myTime != 0)
		{
			myTimer = new WaitForSeconds(myTime);
			StartCoroutine(DestroyMe());
		}
	}

	private IEnumerator DestroyMe()
	{
		yield return myTimer;
		Destroy(this.gameObject);
	}
}
