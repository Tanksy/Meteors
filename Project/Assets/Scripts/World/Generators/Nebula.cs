using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	18/10/2019
 *	Last:	18/10/2019
 *	Name:	Nebula
 *	Vers:	1.0 - Initial version.
 */

[System.Serializable]
public class Nebula
{ 
	[Header("Layers")]
	[Tooltip("The sprites to use as layers (Background to foreground ordering)")]
	[SerializeField] private Sprite[] myLayers;
	public Sprite[] Layers { get { return myLayers; } set { myLayers = value; } }
}
