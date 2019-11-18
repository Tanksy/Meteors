using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *  Auth:   Jake Anderson
 *  Date:   28/10/2019
 *  Last:   08/11/2019
 *  Name:   Score Manager
 *  Vers:   2.0 - Instead of trying to track player deaths, ships themselves contact the Manager upon scoring.
 */

public class ScoreManager : MonoBehaviour
{
	//The player colors are passed from the Game Manager in an array, we get the right player and assign the right color to it.
	private Color[] myColors;

	//The message canvas is used to hold the TextControllers.
	private GameObject myMessageCanvas;

	//Each player's Score is displayed and updated using a TextController, manipulating a Text component on the Message Canvas.
	private TextController[] myTexts;

	//Each text holder is an instance of this prefab.
	private GameObject myTextHolder;

    //The score goal is passed from the Game Manager. This is used to determine a winner at the end of a match.
    private byte myScoreGoal;

    //The GameManager will tell the ScoreManager that the game is in progress. This is used to prevent Update from doing anything when it needn't.
    private bool isInProgress;
    public bool MatchInProgress { get { return isInProgress; } set { isInProgress = value; } }

	public void CreateScores()
	{
		myTexts = new TextController[2];
		for (int i = 0; i < myTexts.Length; i++)
		{
			GameObject thisText = Instantiate(myTextHolder, myMessageCanvas.transform);
			myTexts[i] = thisText.GetComponent<TextController>();
			myTexts[i].Instance = thisText;

			Vector2 position = new Vector2();
			if (i == 0)
				position = new Vector2(-460, 284);
			else
				position = new Vector2(460, 284);
			
			myTexts[i].Setup(myColors[i], position);
		}
	}

	public void Setup(Color[] aColorArray, GameObject aMessageCanvas, GameObject aTextHolderPrefab, byte aScoreGoal)
	{
		myColors = aColorArray;
		myMessageCanvas = aMessageCanvas;
		myTextHolder = aTextHolderPrefab;
        myScoreGoal = aScoreGoal;
	}

	public void RemoveTexts()
	{
		for (byte i = 0; i < myTexts.Length; i++)
			Destroy(myTexts[i].Instance);
	}

	public void FadeOutScores()
	{
		for (byte i = 0; i < myTexts.Length; i++)
			myTexts[i].FadingOut = true;
	}

	//When a ship scores it calls this method. We update that player's score text to show their new score and change its scale and color.
	public void UpdateScore(int aPlayer)
	{
        myTexts[aPlayer].Score();
	}

    public bool PlayerMetGoal(Player[] aPlayerArray)
    {
        bool ret = false;

        foreach(Player player in aPlayerArray)
            if (player.Score >= myScoreGoal)
                ret = true;

        return ret;
    }
}
