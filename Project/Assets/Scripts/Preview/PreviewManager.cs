using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	13/11/2019
 *	Last:	13/11/2019
 *	Vers:	1.0 - Inital Version.
 **/

public class PreviewManager : MonoBehaviour
{
	//The Player this preview is for.
	private Player myPlayer;
	public Player Player { get { return myPlayer; } set { myPlayer = value; } }

	[Header("Animations")]
	[Tooltip("The animation that surrounds the preview ship is an instance of this prefab.")]
	[SerializeField] private GameObject myShipAreaPrefab;
	private GameObject myShipAreaInstance;
	public GameObject ShipArea { get { return myShipAreaInstance; } set { myShipAreaInstance = value; } }
	[Tooltip("The animation that surrounds the player's badges is an instance of this prefab.")]
	[SerializeField] private GameObject myMedalsAreaPrefab;
	private GameObject myMedalsAreaInstance;
	[Tooltip("The animation that surrounds the winner's trophy is an instance of this prefab.")]
	[SerializeField] private GameObject myTrophyAreaPrefab;
	private GameObject myTrophyAreaInstance;

	[Header("Medals")]
	[Tooltip("The medals awarded to the player are instances of this prefab.")]
	[SerializeField] private GameObject myMedalPrefab;
	[Tooltip("The trophy awarded to the winner is an instance of this prefab.")]
	[SerializeField] private GameObject myTrophyPrefab;

	[Header("Ship Previews")]
	[Tooltip("The ship preview is an instance of this prefab.")]
	[SerializeField] private GameObject myPreviewPrefab;
	private GameObject myPreviewInstance;
	private ShipPreviewManager myShipPreviewManager;
	public ShipPreviewManager ShipPreview { get { return myShipPreviewManager; } set { myShipPreviewManager = value; } }
	[Tooltip("This array holds each available ship type.")]
	[SerializeField] private GameObject[] myPreviews;

	private bool isAcceptingInput;
	public bool CaptureInput { get { return isAcceptingInput; } set { isAcceptingInput = value; } }
	private float myJoyCooldown;

	private float myShipIndex;
	private float myColorIndex;

	private bool isReady = false;
	public bool Ready { get { return isReady; } set { isReady = value; } }

	private IEnumerator ShowTrophy()
	{
		yield return new WaitForSeconds(0.5f);

		GameObject thisTrophy = Instantiate(myTrophyPrefab, this.transform);
		isAcceptingInput = true;
	}

	private IEnumerator ShowTrophyArea()
	{
		yield return new WaitForSeconds(1f);
		myTrophyAreaInstance = Instantiate(myTrophyAreaPrefab, this.transform);

		StartCoroutine(ShowTrophy());
	}

	private IEnumerator ShowMedals()
	{
		yield return new WaitForSeconds(0.5f);

		bool winner = false;

		foreach (Medal med in myPlayer.Medals)
		{
			GameObject thisMedal = Instantiate(myMedalPrefab, this.transform);

			//If the medal is for winning, we set Winner to true and check it outside of the loop, then show the trophy.
			if (med.Name == "Winner")
				winner = true;
		}

		if (winner)
			StartCoroutine(ShowTrophyArea());
		else
			isAcceptingInput = true;
	}

	private IEnumerator ShowMedalsArea()
	{
		yield return new WaitForSeconds(1f);
		myMedalsAreaInstance = Instantiate(myMedalsAreaPrefab, this.transform);

		StartCoroutine(ShowMedals());
	}

	private IEnumerator ShowPreview()
	{
		yield return new WaitForSeconds(1f);
		myPreviewInstance = Instantiate(myPreviewPrefab, this.transform) as GameObject;
		myShipPreviewManager = myPreviewInstance.GetComponent<ShipPreviewManager>();
		myShipPreviewManager.Setup(myPlayer.Palette.Colors[1], myPlayer.Palette.Colors[0]);

		if (myPlayer.Medals.Count > 0)
			StartCoroutine(ShowMedalsArea());
		else
			isAcceptingInput = true;
	}

	public void Clear()
	{
		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
			Destroy(sr.gameObject);
		isAcceptingInput = false;
	}

	public void Show()
	{
		myShipAreaInstance = Instantiate(myShipAreaPrefab, this.transform);
		StartCoroutine(ShowPreview());
	}

	public void Setup(Player aPlayer)
	{
		myPlayer = aPlayer;
		myShipIndex = 0;
		myColorIndex = myPlayer.Index;
		isReady = false;
	}

	private void Update()
	{
		if (isAcceptingInput)
			if (Input.GetButton("Weapon" + myPlayer.Index))
			{
				if (myShipAreaInstance != null)
					myShipAreaInstance.GetComponent<SpriteRenderer>().color = myPlayer.Palette.Colors[0];
				isReady = true;
			}
			else
			{
				if (myShipAreaInstance != null)
					myShipAreaInstance.GetComponent<SpriteRenderer>().color = Color.white;
				isReady = false;
			}
	}
}
