using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	17/10/2019
 *	Last:	23/10/2019
 *	Name:	Backdrop Controller
 *	Vers:	1.1 - Moved generation from Start to GenerateNewBackdrop method
 */

public class BackdropManager : MonoBehaviour
{
	[Header("Prefabs")]
	[Tooltip("Nebula Manager prefab.")]
	[SerializeField] private GameObject myNebulaManagerPrefab;
	private GameObject myNebulaManager;
	private NebulaGenerator myNebulaGenerator;
	[Tooltip("Star Manager prefab.")]
	[SerializeField] private GameObject myStarManagerPrefab;
	private GameObject myStarManager;
	private StarGenerator myStarGenerator;

	//The palette is passed from the world manager and is used to color the nebula layers and sprite stars.
	private Palette myPalette;

	//When we're ending the game, this is set to true. While true, all child sprite renderers lose opacity, eventually being destroyed at 0 opacity.
	private bool isEnding;

	public void Setup(Palette aColorPalette)
	{
		myPalette = aColorPalette;
		GenerateNewBackdrop();
	}

	public void FadeAndDestroy()
	{
		isEnding = true;
	}

	public void GenerateNewBackdrop()
	{
		myNebulaManager = Instantiate(myNebulaManagerPrefab, this.transform);
		myStarManager = Instantiate(myStarManagerPrefab, this.transform);

		myNebulaGenerator = myNebulaManager.GetComponent<NebulaGenerator>();
		myStarGenerator = myStarManager.GetComponent<StarGenerator>();
		
		//Create a new color array and fill it with the first three colors from the palette.
		Color[] nebulaColors = new Color[3];
		for (int i = 0; i < 3; i++)
		{
			nebulaColors[i] = myPalette.Colors[i];
		}
		//Create a new color array and fill it with the first two colors from the palette.
		Color[] starColors = new Color[2];
		for (int i = 0; i < 2; i++)
		{
			starColors[i] = myPalette.Colors[i];
		}

		//Pass the color array on to the Nebula Controller.
		myNebulaGenerator.Colors = nebulaColors;
		//Pass the color array on to the Star Controller.
		myStarGenerator.Colors = starColors;

		myNebulaGenerator.Generate();
		myStarGenerator.Generate();
	}

	private void Awake()
	{
		isEnding = false;
	}

	private void Update()
	{
		if (isEnding)
		{
			foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
			{
				//If the alpha is > 0, we lower it over time.
				if (sprite.color.a > 0f)
				{
					Color newColor = sprite.color;
					newColor.a -= 0.66f * Time.deltaTime;
					sprite.color = newColor;
				}
				//If the alpha is <= 0f, we destroy the object.
				else
					Destroy(sprite.gameObject);
			}
		}
	}
}
