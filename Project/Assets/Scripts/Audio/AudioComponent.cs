using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	28/09/2019
 * Last:	17/10/2019
 * Name:	Audio Controller
 * Vers:	1.1 - Self destruct time is now tied to clip length.
 */

public class AudioComponent : MonoBehaviour
{
	private AudioSource mySound;
	private IEnumerator myCoroutine;

	/**
	 * Called by the parent, this function sets the desired parameters for the audio before playing.
	 */
	public void PlayMe(float aVolume, float aPitch)
	{
		mySound = gameObject.GetComponent<AudioSource>();
		myCoroutine = DeleteMe();

		mySound.volume = aVolume;
		mySound.pitch = aPitch;

		//Having a negative pitch means we should play from the end of the clip, as it's in reverse.
		if (aPitch <= 0f)
		{
			mySound.timeSamples = mySound.clip.samples - 1;
		}

		//Play the audio with the new parameters.
		mySound.Play();

		//Start the DeleteMe coroutine for cleanup.
		StartCoroutine(myCoroutine);
	}

	IEnumerator DeleteMe()
	{
		//Wait until the clip is played in full before self destroying
		yield return new WaitForSeconds(mySound.clip.length);
		Destroy(this.gameObject);
	}
}
