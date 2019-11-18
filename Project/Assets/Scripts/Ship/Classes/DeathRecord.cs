using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	25/10/2019
 *	Last:	25/10/2019
 *	Name:	Death Record
 *	Vers:	1.0
 */

[System.Serializable]
public class DeathRecord
{
	[Header("Information")]
	[Tooltip("Why did this ship die?")]
	[SerializeField] private string myReason;
	public string Reason { get { return myReason; } set { myReason = value; } }
	[Tooltip("Who was behind this ships demise?")]
	[SerializeField] private int myKiller;
	public int Killer { get { return myKiller; } set { myKiller = value; } }

	public DeathRecord()
	{
		myReason = "No Reason Given.";
		myKiller = -1;
	}

	public DeathRecord(string aReason, int aKillerPlayerNumber)
	{
		myReason = aReason;
		myKiller = aKillerPlayerNumber;
	}
}
