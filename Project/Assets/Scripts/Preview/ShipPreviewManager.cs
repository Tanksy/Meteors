using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	13/11/2019
 *	Last:	13/11/2019
 *	Vers:	1.0 - Initial Version.
 **/

public class ShipPreviewManager : MonoBehaviour
{
	[Header("Transforms")]
	[Tooltip("This transform holds the Body SpriteRenderer.")]
	[SerializeField] private RectTransform mySpriteBody;
	//The SpriteRenderer of the Body instance.
	private SpriteRenderer mySpriteBodyRenderer;
	public Color BodyColor { get { return mySpriteBodyRenderer.color; } set { mySpriteBodyRenderer.color = value; } }
	public Sprite BodySprite { get { return mySpriteBodyRenderer.sprite; } set { mySpriteBodyRenderer.sprite = value; } }

	[Tooltip("This transform holds the Detail SpriteRenderer.")]
	[SerializeField] private RectTransform mySpriteDetail;
	//The SpriteRenderer of the Detail instance.
	private SpriteRenderer mySpriteDetailRenderer;
	public Color DetailColor { get { return mySpriteDetailRenderer.color; } set { mySpriteDetailRenderer.color = value; } }
	public Sprite DetailSprite { get { return mySpriteDetailRenderer.sprite; } set { mySpriteDetailRenderer.sprite = value; } }

	private void Awake()
	{
		mySpriteBodyRenderer = mySpriteBody.GetComponent<SpriteRenderer>();
		mySpriteDetailRenderer = mySpriteDetail.GetComponent<SpriteRenderer>();
	}

	public void Setup(Color aBodyColor, Color aDetailColor)
	{
		mySpriteBodyRenderer.color = aBodyColor;
		mySpriteDetailRenderer.color = aDetailColor;
	}
}