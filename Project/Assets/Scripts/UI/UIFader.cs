using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFader : MonoBehaviour
{
	private void LateUpdate()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		Color newColor = sprite.color;

		float newAlpha = newColor.a;
		newAlpha -= 1f * Time.deltaTime;
		newColor.a = Mathf.Clamp(newAlpha, 0f, 1.0f);
		sprite.color = newColor;

		if (newColor.a <= 0f)
			Destroy(this.gameObject);
	}
}
