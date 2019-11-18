using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**	Auth:	Jake Anderson
 *	Date:	11/11/2019
 *	Last:	11/11/2019
 *	Vers:	1.0 - Initial Version.
 */

public class MusicComponent : MonoBehaviour
{
	[Header("Audio Sources")]
	[Tooltip("The Audio Source that plays outside of battles.")]
	[SerializeField] private AudioSource myMusic_MenuLoop;
	[Tooltip("The Audio Source that plays inside of battles.")]
	[SerializeField] private AudioSource myMusic_BattleLoop;
	//This holds both audio sources in an array.
	private AudioSource[] myMusic;

	[Header("Values")]
	[Tooltip("How long the audio takes to cross-fade between sources.")]
	[SerializeField] private float myCrossfadeTime;
	[Tooltip("The maximum the volume can go.")]
	[Range(0.0f, 1.0f)]
	[SerializeField] private float myMaxVolume;
	public float Volume { get { return myMaxVolume; } set { myMaxVolume = Mathf.Clamp(value, 0.0f, 1.0f); } }
	public bool Mute { get { return myMusic[myPlayer].mute; } set { foreach (AudioSource a in myMusic) a.mute = value; } }
	[Tooltip("The number of volume changes made in a second.")]
	[SerializeField] private int myChangesPerSecond;

	private int myPlayer;

	private IEnumerator[] myFaders;

	private void Awake()
	{
		myPlayer = 0;
		myFaders = new IEnumerator[2];
		myMusic = new AudioSource[2];
	}

	private void Start()
	{
		myMusic[0] = myMusic_MenuLoop;
		myMusic[1] = myMusic_BattleLoop;
	}

	public void Pause()
	{
		Play(myMusic_MenuLoop.clip);
	}

	public void Resume()
	{
		Play(myMusic_BattleLoop.clip);
	}

	private void Play(AudioClip aClip)
	{
		//We don't play the clip that we're already playing.
		if (aClip == myMusic[myPlayer].clip)
			return;

		//Kill all existing fader coroutines to cancel them.
		foreach (IEnumerator i in myFaders)
			if (i != null)
				StopCoroutine(i);
		
		//Fade out the active clip.
		if (myMusic[myPlayer].volume > 0)
		{
			myFaders[0] = FadeAudio(myMusic[myPlayer], myCrossfadeTime, 0.0f, () => { myFaders[0] = null; });
			StartCoroutine(myFaders[0]);
		}

		//Fade in the new clip.
		int nextPlayer = (myPlayer + 1) % myMusic.Length;
		myMusic[nextPlayer].clip = aClip;
		myFaders[1] = FadeAudio(myMusic[nextPlayer], myCrossfadeTime, 1.0f, () => { myFaders[1] = null; });
		StartCoroutine(myFaders[1]);

		//Register the new player.
		myPlayer = nextPlayer;
	}

	private IEnumerator FadeAudio(AudioSource anAudio, float aDuration, float aTargetVolume, System.Action aCallback)
	{
		//Calculate how many steps are required to fade the audio.
		int steps = (int)(myChangesPerSecond * aDuration);
		float stepSize = (aTargetVolume - anAudio.volume) / steps;

		//Perform the fade.
		for (int i = 0; i < steps; i++)
		{
			anAudio.volume += stepSize;
			if (anAudio.Equals(myMusic_BattleLoop))
				anAudio.pitch += stepSize;
			yield return new WaitForSecondsRealtime(aDuration / steps);
		}

		//Ensure the volume we end with is the volume we want.
		anAudio.volume = aTargetVolume;
		if (anAudio.Equals(myMusic_BattleLoop))
			anAudio.pitch = aTargetVolume;

		//Callback
		if (aCallback != null)
			aCallback();
	}
}
