using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Palette
{
	[Header("Colors")]
	[SerializeField] private Color[] myColors;
	public Color[] Colors { get { return myColors; } set { myColors = value; } }
}
