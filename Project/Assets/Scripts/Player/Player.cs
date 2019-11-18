using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	18/09/2019
 * Last:	08/11/2019
 * Name:	Player
 * Vers:	2.3 - Adjusted Player No. to be an Int.
 */

[System.Serializable]
public class Player
{
	private int myIndex;
	public int Index { get { return myIndex; } set { myIndex = value; } }

	[Header("Player Data")]
	[Tooltip("The Player's trail and bullet color.")]
	[SerializeField] private Palette myPalette;
	public Palette Palette { get { return myPalette; } }
    [Tooltip("The Player's current score.")]
	[SerializeField] private byte myScore;
	public byte Score { get { return myScore; } set { myScore = value; } }

	[Tooltip("This Player's medals.")]
	[SerializeField] private List<Medal> myMedals;
	public List<Medal> Medals { get { return myMedals; } set { myMedals = value; } }

	//When a player wins a match, the winner is announced in a message, with their player number in their color. This getter is used to put the number and color together as a HTML message.
	public string ColoredName { get { return "<color=#" + ColorUtility.ToHtmlStringRGB(myPalette.Colors[0]) + ">PLAYER " + (myIndex+1).ToString() + "</color>"; } }
}