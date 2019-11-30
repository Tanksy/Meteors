using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *	Auth:	Jake Anderson
 *	Date:	17/10/2019
 *	Last:	08/11/2019
 *	Name:	Star Generator
 *	Vers:	1.2 - Corrected boundaries for Sprite Stars to generate within the map.
 */

public class StarGenerator : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Enable to generate Stars as sprite objects.")]
	[SerializeField] private bool isUsingSpriteStars;
	[Tooltip("Enable to generate a noise map texture as a backdrop sprite.")]
	[SerializeField] private bool isUsingNoiseMaps;

	[Header("Sprite Stars")]
	[Tooltip("How many Stars should we generate?")]
	[SerializeField] private int myStarCount;
	[Tooltip("This array holds all selectable Star sprites.")]
	[SerializeField] private Sprite[] myStars;
	//This array holds colors passed from the Backdrop Controller.
	private Color[] myStarColors;
	public Color[] Colors { set { myStarColors = value; } }
	[Tooltip("Minimum scale for Star objects.")]
	[SerializeField] private float myMinScale;
	[Tooltip("Maximum scale for Star objects.")]
	[SerializeField] private float myMaxScale;
	[Tooltip("This list holds all generated stars.")]
	[SerializeField] private List<GameObject> myStarObjects;

	[Header("NoiseMap Stars")]
	[Tooltip("The 'Pixels per Unit' of the generated star field")]
	[SerializeField] private int myNoisePPU;
	[Tooltip("The noise cutoff value is a small adjustment to simple generation that controls which pixels of the texture are white or not.")]
	[Range(0.001f, 0.01f)]
	[SerializeField] private float myNoiseCutoff;

	[Header("Generated Texture")]
	[Tooltip("Our Texture2D Generated for the background noise.")]
	[SerializeField] private Texture2D myStarTexture;
	private Sprite myStarTextureSprite;
	private SpriteRenderer mySpriteRenderer;
	private GameObject[,] myStarPanels;
	public int StarPanelCounter;
	public int TotalPanels;

	private BorderManager myBorders;

	private void Awake()
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();

		GameObject border = GameObject.Find("Border");
		myBorders = border.GetComponent<BorderManager>();

		myStarObjects = new List<GameObject>();
		myStarPanels = new GameObject[(int)myBorders.Width / 2,(int)myBorders.Height / 2];

		StarPanelCounter = 0;
		TotalPanels = myStarPanels.GetLength(0) * myStarPanels.GetLength(1);
	}

	private void Start()
	{
		Vector2 newPos = new Vector2(-(myBorders.Width / 2), -(myBorders.Height / 2));
		transform.position = newPos;
	}

	public bool FadeOut()
	{
		bool ret = false;

		//Lower the alpha of the sprite renderer.
		Color newColor = mySpriteRenderer.color;
		newColor.a -= 0.1f;
		mySpriteRenderer.color = newColor;

		foreach (GameObject spriteStar in myStarObjects)
		{
			SpriteRenderer sprite = spriteStar.GetComponent<SpriteRenderer>();
			Color nColor = sprite.color;
			nColor.a -= 0.1f;
			sprite.color = nColor;
		}
		foreach (GameObject starPanel in myStarPanels)
		{
			SpriteRenderer sprite = starPanel.GetComponent<SpriteRenderer>();
			Color nColor = sprite.color;
			nColor.a -= 0.1f;
			sprite.color = nColor;
		}

		if (newColor.a <= 0f)
		{
			foreach (GameObject spriteStar in myStarObjects)
				Destroy(spriteStar);
			foreach (GameObject starPanel in myStarPanels)
				Destroy(starPanel);
			ret = true;
		}

		return ret;
	}

	private void FillTexture()
	{
		for (int y = 0; y < myNoisePPU * 2; y++)
		{
			for (int x = 0; x < myNoisePPU * 2; x++)
			{
				Color newColor = Color.white * Random.value;
				if (newColor.r < (0.99f + myNoiseCutoff))
				{
					newColor.r = 0;
					newColor.g = 0;
					newColor.b = 0;
				}
				myStarTexture.SetPixel(x, y, newColor);
			}
		}
		myStarTexture.Apply();
	}

	/**
	 * Generates a batch of Star objects with sprites set from the star sprites array.
	 */
	public void GenerateSpriteStars()
	{
		for (int i = 0; i < myStarCount; i++)
		{
			GameObject thisStar = new GameObject();
			thisStar.name = "Star Sprite " + i.ToString();
			SpriteRenderer renderer = thisStar.AddComponent<SpriteRenderer>();
			renderer.sprite = myStars[(int)Random.Range(0, myStars.Length)];
			renderer.sortingOrder = 5;

			Color newColor;
			float random = Random.Range(0f, 1f);
			if (random >= 0.5f)
			{
				newColor = myStarColors[(int)Random.Range(0, myStarColors.Length)];
				newColor.a = 1.0f;
			}
			else
			{
				newColor.r = 1.0f;
				newColor.g = 1.0f;
				newColor.b = 1.0f;
				newColor.a = 1.0f;
			}
			renderer.color = newColor;

			float scalar = Random.Range(myMinScale, myMaxScale);
			thisStar.transform.localScale = new Vector2(scalar, scalar);

			Vector2 newPos = new Vector2(Random.Range(0, myBorders.Width), Random.Range(0, myBorders.Height));
			thisStar.transform.position = newPos;
			thisStar.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0, 360)));
			//Parent the Star object to the Star Controller
			thisStar.transform.parent = transform;
			myStarObjects.Add(thisStar);
		}
	}

	public void GenerateNoiseMap()
	{
		//Generate a background square of noise.
		myStarTexture = new Texture2D(myNoisePPU * 2, myNoisePPU * 2, TextureFormat.RGB24, false);
		myStarTexture.name = "Procedural Stars";
		myStarTexture.wrapMode = TextureWrapMode.Clamp;
		myStarTexture.filterMode = FilterMode.Point;

		Rect rect = Rect.MinMaxRect(0, 0, myNoisePPU * 2, myNoisePPU * 2);
		Vector2 pivot = new Vector2(0, 0);

		myStarTextureSprite = Sprite.Create(myStarTexture, rect, pivot, myNoisePPU);
		FillTexture();
	}

	public void GenerateStarPanels()
	{
		for (int x = 0; x < myStarPanels.GetLength(0); x++)
			for (int y = 0; y < myStarPanels.GetLength(1); y++)
			{
				GameObject panel = new GameObject();
				panel.name = "Star Panel (" + x.ToString() + "," + y.ToString() + ")";
				SpriteRenderer renderer = panel.AddComponent<SpriteRenderer>();
				renderer.sprite = myStarTextureSprite;
				renderer.sortingOrder = 0;

				Vector2 newPos = new Vector2(transform.position.x + (x * 2), transform.position.y + (y * 2));

				panel.transform.position = newPos;
				panel.transform.parent = this.transform;

				StarPanelCounter++;
				myStarPanels[x, y] = panel;
			}

	}

	public void Generate()
	{
		if (isUsingSpriteStars)
			GenerateSpriteStars();
		if (isUsingNoiseMaps)
		{
			GenerateNoiseMap();
			GenerateStarPanels();
		}
	}
}
