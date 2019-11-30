using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *	Auth:	Jake Anderson
 *	Date:	09/11/2019
 *	Last:	09/11/2019
 *	Name:	UI Manager
 *	Vers:	1.0 - Initial Version.
 */

public class UIManager : MonoBehaviour
{
	//When the menu begins to show, this sprite prefab is instantiated.
	private GameObject myMenu_BackgroundEffectPrefab;
	private GameObject myMenu_BackgroundEffect;
	//Following the menu effect, the logo prefab is also instantiated.
	private GameObject myLogo_Prefab;
	private GameObject myLogo_Instance;

	//UI elements such as the menu sprites are children of the message canvas.
    private Transform myMessageCanvas;

	private bool isPauseable = false;
	private bool isWaiting = false;

	//The Music Component is told when we're paused.
	private MusicComponent myMusicComponent;

	//The player's array
	private Player[] myPlayers;
	private PreviewManager[] myPreviews;

	private int myBlinks;
	private GameObject myReadyAudioPrefab;

	public void Setup(GameObject aMenuEffectPrefab, GameObject aLogoPrefab, Player[] aPlayerArray, GameObject aReadyAudioPrefab)
	{
		myMenu_BackgroundEffectPrefab = aMenuEffectPrefab;
		myLogo_Prefab = aLogoPrefab;

        myMessageCanvas = GameObject.Find("MessageCanvas").transform;


        myMusicComponent = GetComponent<MusicComponent>();

		myPlayers = aPlayerArray;

		myPreviews = GetComponentsInChildren<PreviewManager>();

		myReadyAudioPrefab = aReadyAudioPrefab;
	}

	private IEnumerator Blink()
	{
		while (myBlinks > 0)
		{
			for (int i = 0; i < myPreviews.Length; i++)
				if (myPreviews[i].ShipArea != null)
					if (myPreviews[i].ShipArea.GetComponent<SpriteRenderer>().color == Color.white)
						myPreviews[i].ShipArea.GetComponent<SpriteRenderer>().color = myPlayers[i].Palette.Colors[0];
					else
						myPreviews[i].ShipArea.GetComponent<SpriteRenderer>().color = Color.white;

			myBlinks -= 1;
			yield return new WaitForSeconds(0.1f);
		}

		StartMatch();
	}

	private void StartMatch()
	{
		myMenu_BackgroundEffect = Instantiate(myMenu_BackgroundEffectPrefab, myMessageCanvas) as GameObject;

		for (int i = 0; i < myPreviews.Length; i++)
			myPreviews[i].Clear();
		
		isPauseable = true;
		myMusicComponent.Resume();

		myMenu_BackgroundEffect.AddComponent<UIFader>();
		gameObject.GetComponent<GameManager>().StartMatch();
	}

	private IEnumerator ShowPreviews()
	{
		yield return new WaitForSeconds(1.33f);
		for (int i = 0; i < myPreviews.Length; i++)
		{
			myPreviews[i].Setup(myPlayers[i]);
			myPreviews[i].Show();
			myPreviews[i].Ready = false;
			myPreviews[i].CaptureInput = true;
			isWaiting = true;
		}
	}

	private void FadeLogo()
	{
		if (myMenu_BackgroundEffect != null)
			myMenu_BackgroundEffect.AddComponent<UIFader>();
		myLogo_Instance.AddComponent<UIFader>();

		StartCoroutine(ShowPreviews());
	}

	private IEnumerator ShowLogo()
	{
		myLogo_Instance = Instantiate(myLogo_Prefab, myMessageCanvas) as GameObject;

		yield return new WaitForSeconds(1.66f);

		FadeLogo();
	}

	public void StartEndingSequence()
	{
		myMenu_BackgroundEffect = Instantiate(myMenu_BackgroundEffectPrefab, myMessageCanvas) as GameObject;
		myMusicComponent.Pause();

		StartCoroutine(ShowLogo());
	}

	public void StartOpeningSequence()
	{
		StartCoroutine(ShowLogo());
	}

	private void Update()
	{
		if (isPauseable)
			if (Input.GetButtonDown("Pause1") || Input.GetButtonDown("Pause2"))
			{
				if (GameManager.isPaused)
				{
					gameObject.GetComponent<GameManager>().Resume();
					myMusicComponent.Resume();
				}
				else
				{
					gameObject.GetComponent<GameManager>().Pause();
					myMusicComponent.Pause();
				}
			}

		if (isWaiting)
			if (myPreviews[0].Ready && myPreviews[1].Ready)
			{
				for (int i = 0; i < myPreviews.Length; i++)
					myPreviews[i].CaptureInput = false;

				GameObject thisSound = Instantiate(myReadyAudioPrefab, this.transform);
				thisSound.GetComponent<AudioComponent>().PlayMe(1.0f, 1.0f);

				myBlinks = 12;
				StartCoroutine(Blink());

				isWaiting = false;
			}
	}
}
