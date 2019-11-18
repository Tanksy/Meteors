using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Medal
{
	[Header("Properties")]
	[Tooltip("The name of the Medal.")]
	[SerializeField] private string myName;
	public string Name { get { return myName; } set { myName = value; } }
	[Tooltip("The rank of the Medal.")]
	[SerializeField] private byte myRank;
	public byte Rank { get { return myRank; } set { myRank = value; } }
	[Tooltip("The descriptor of the Medal.")]
	[SerializeField] private string myDescriptor;
	public string Descriptor { get { return myDescriptor; } set { myDescriptor = value; } }

	public Medal()
	{
		myName = "No Name.";
		myRank = 0;
		myDescriptor = "No Descriptor.";
	}

	public Medal(string aName, byte aRank, string aDescriptor)
	{
		myName = aName;
		myRank = aRank;
		myDescriptor = aDescriptor;
	}
}
