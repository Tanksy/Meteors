using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	German Cruz / Adapted by Jake Anderson
 *	Date:	22/12/2018
 *	Last:	09/11/2019
 *	Name:	Animated Sprite Mask
 *	Vers:	1.2 - Checks SpriteRender != null.
 *	Orig:	https://gitlab.com/snippets/1785639
 */

[System.Serializable]
public class AnimatedSpriteMask : MonoBehaviour
{
	[Header("Properties")]
	[Tooltip("The mask to animate.")]
	[SerializeField] private SpriteMask mySpriteMask;
	public SpriteMask Mask { get { return mySpriteMask; } set { mySpriteMask = value; } }
	[Tooltip("The GameObject holding the animation.")]
	[SerializeField] private SpriteRenderer myTargetRenderer;
	public SpriteRenderer Sprite { get { return myTargetRenderer; } set { myTargetRenderer = value; } }

	private void LateUpdate()
	{
		if (myTargetRenderer != null)
			if (mySpriteMask.sprite != myTargetRenderer.sprite)
				mySpriteMask.sprite = myTargetRenderer.sprite;
	}
}
