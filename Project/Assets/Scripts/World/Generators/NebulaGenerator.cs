using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	18/10/2019
 *	Last:	18/10/2019
 *	Name:	Nebula Controller
 *	Vers:	1.0 - Initial version.
 */

public class NebulaGenerator : MonoBehaviour
{
	[Header("Nebulae")]
	[Tooltip("This array holds all selectable Nebulae.")]
	[SerializeField] private Nebula[] myNebulae;
	//This array holds colors passed from the Backdrop Controller.
	private Color[] myNebulaColors;
	public Color[] Colors { set { myNebulaColors = value; } }

	[Header("Settings")]
	[Tooltip("Enable to have the nebulae position be randomly offset from the center point.")]
	[SerializeField] private bool useRandomOffset;
	[Tooltip("Enable to have the nebulae rotation be random within a 180 degree angle.")]
	[SerializeField] private bool useRandomRotation;
	[Tooltip("Enable to have the nebulae scale be slightly adjusted to a random value.")]
	[SerializeField] private bool useRandomScale;
	[Tooltip("The smallest the scale can be.")]
	[Range(0.5f, 1.0f)]
	[SerializeField] private float myScaleMin;
	[Tooltip("The largest the scale can be.")]
	[Range(1.0f, 1.5f)]
	[SerializeField] private float myScaleMax;

	//Holds all generated nebula objects.
	private List<GameObject> myNebulaeObjects;

	public int NebulaLayerCounter;
	public int TotalNebulaLayers;

	private IEnumerator myCoroutine;

	private void Awake()
	{
		TotalNebulaLayers = 3;
	}

	public bool FadeOut()
	{
		bool ret = false;

		Color newColor = Color.white;

		foreach (GameObject nebula in myNebulaeObjects)
		{
			SpriteRenderer[] layers = nebula.GetComponentsInChildren<SpriteRenderer>();
			foreach(SpriteRenderer sprite in layers)
			{
				newColor = sprite.color;
				newColor.a -= 0.1f;
				sprite.color = newColor;

				if (newColor.a <= 0f)
					Destroy(sprite.gameObject);
			}

			if (newColor.a <= 0f)
			{
				Destroy(nebula);
				ret = true;
			}
		}

		return ret;
	}

	private IEnumerator GenerateLayers(int aPickedNebula, GameObject aNebulaContainer)
	{
		//Each nebula is broken down into sequential layers (background to foreground) to allow for some cool color palette effects.
		//For our number of sprites in the array, create a GameObject with a Sprite Renderer attached and sprite for that index element.
		for (int i = 0; i < myNebulae[aPickedNebula].Layers.Length; i++)
		{
			GameObject thisLayer = new GameObject();
			thisLayer.name = "Layer" + i + 1;

			SpriteRenderer thisRenderer = thisLayer.AddComponent<SpriteRenderer>();
			thisRenderer.sprite = myNebulae[aPickedNebula].Layers[i];
			thisRenderer.sortingOrder = 2 + i;

			//Adjust the color of the layer using our chosen palette.
			thisRenderer.color = myNebulaColors[i];

			thisLayer.transform.parent = aNebulaContainer.transform;
			NebulaLayerCounter++;
			yield return new WaitForSeconds(0.1f);
		}
	}

	public void Generate()
	{
		//Generate the parent object that will hold all the layers.
		GameObject nebula = new GameObject();
		nebula.name = "Nebula";

		myNebulaeObjects = new List<GameObject>();

		//Pick a nebula to use.
		int myPickedNebula = (int)Random.Range(0, myNebulae.Length);

		myCoroutine = GenerateLayers(myPickedNebula, nebula);
		StartCoroutine(myCoroutine);

		//Random adjustments to the enitre nebula.
		if (useRandomOffset)
			nebula.transform.Translate(new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-1.5f, 1.5f)));
		if (useRandomRotation)
			nebula.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0, 180)));
		if (useRandomScale)
		{
			float random = Random.Range(myScaleMin, myScaleMax);
			nebula.transform.localScale = new Vector3(random, random, 1f);
		}

		nebula.transform.parent = transform;
		myNebulaeObjects.Add(nebula);
	}
}
