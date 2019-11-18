using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoController : MonoBehaviour
{
	[Tooltip("The prefab that holds the Meteors logo.")]
	[SerializeField] private GameObject myLogoPrefab;

	private IEnumerator DisplayLogoAfterDelay()
	{
		yield return new WaitForSeconds(0.333f * 1.5f);

		UIFader fader = gameObject.AddComponent(typeof(UIFader)) as UIFader;
		GameObject logo = Instantiate(myLogoPrefab, this.transform);
		logo.transform.Translate(new Vector2(0f, -2f));
		SpriteRenderer sr = logo.GetComponent<SpriteRenderer>();
		sr.sortingOrder = 1;
	}

	private void Start()
	{
		StartCoroutine("DisplayLogoAfterDelay");
	}
}
