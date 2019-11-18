using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Auth:	Jake Anderson
 * Date:	20/09/2019
 * Last:	23/10/2019
 * Name:	Asteroid Controller
 * Vers:	1.3.2 - Public access to Spawn method
 */

public class AsteroidManager : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Width of the field in Units.")]
	[SerializeField] private float myFieldWidth;
	public float FieldWidth { get { return myFieldWidth; } }
	[Tooltip("Height of the field in Units.")]
	[SerializeField] private float myFieldHeight;
	public float FieldHeight { get { return myFieldHeight; } }
	[Tooltip("The number of 'Small' asteroids to spawn.")]
	[SerializeField] private int myNumS;
	[Tooltip("The number of 'Med' asteroids to spawn.")]
	[SerializeField] private int myNumM;
	[Tooltip("the number of 'Large' asteroids to spawn.")]
	[SerializeField] private int myNumL;
	[Tooltip("Enable to spawn a random count of asteroids, using the spawn numbers as ballpark target values.")]
	[SerializeField] private bool useRandomCounts;

	private Color myColor;
	public Color Color { get { return myColor; } }

    [Header("Asteroids")]
    [Tooltip("Holds a list of Asteroid categories which in turn hold their Asteroids.")]
    [SerializeField] private AsteroidCategory[] myCategories;
    public AsteroidCategory[] Categories { get { return myCategories; } }

	//All asteroids spawned are held within this list.
	private List<GameObject> myAsteroids;

	public void SetSpawnpoint(int aIndex, Vector2 aPosition)
	{
		GameObject prefab = myCategories[aIndex].Asteroids[(int)Random.Range(0, myCategories[aIndex].Asteroids.Length)];
		GameObject asteroid = Instantiate(prefab, this.transform);
		myAsteroids.Add(asteroid);
		asteroid.GetComponent<Asteroid>().Setup(myColor, aPosition);
	}

	private void Awake()
	{
		myAsteroids = new List<GameObject>();
	}

	public void Setup(Color aColor)
	{
		myColor = aColor;
		Spawn();
	}

	public void FadeAndDestroy()
	{
		Asteroid[] asteroids = GetComponentsInChildren<Asteroid>();
		foreach(Asteroid asteroid in asteroids)
			asteroid.FadeAndDestroy();
	}

	/**
	 * We go through the array, checking each Asteroid's category.
	 *	Then, we use the SetSpawnpoint function to set desired spawnpoints,
	 *	then triggering the Asteroid's spawn function.
	 */
	private void Spawn()
	{
		//If we're using ballpark figures, offset the count by either 1 or 2, randomly picked.
		if (useRandomCounts)
		{
			//Create new values from our given values.
			int newSCount = (int)Mathf.Clamp(Random.Range(myNumS - Random.Range(1f, 2f), myNumS + Random.Range(1f, 2f)), 1f, myNumS + 4f);
			int newMCount = (int)Mathf.Clamp(Random.Range(myNumM - Random.Range(1, 2), myNumM + Random.Range(1, 2)), 1f, myNumM + 4f);
			int newLCount = (int)Mathf.Clamp(Random.Range(myNumL - Random.Range(1, 2), myNumL + Random.Range(1, 2)), 1f, myNumL + 2f);
			//Set our count values to the new ones.
			myNumS = newSCount;
			myNumM = newMCount;
			myNumL = newLCount;
		}
		//For each category, spawn asteroids to the number specified.
		foreach (AsteroidCategory category in myCategories)
		{
			switch (category.Category)
			{
				//Small Asteroids are spawned anywhere within the field.
				case 0:
					for (int j = 0; j < myNumS; j++)
						SetSpawnpoint(category.Category, new Vector2(Random.Range(-myFieldWidth / 2, myFieldWidth / 2), Random.Range(-myFieldHeight / 2, myFieldHeight / 2)));
					break;

				//Medium Asteroids are spawned closer to the center.
				case 1:
					for (int j = 0; j < myNumM; j++)
						SetSpawnpoint(category.Category, new Vector2(Random.Range((-myFieldWidth / 2) / 2, (myFieldWidth / 2) / 2), Random.Range((-myFieldHeight / 2) / 2, (myFieldHeight / 2) / 2)));
					break;

				//Large Asteroids are spawned the most central.
				case 2:
					for (int j = 0; j < myNumL; j++)
						SetSpawnpoint(category.Category, new Vector2(Random.Range((-myFieldWidth / 2) / 3, (myFieldWidth / 2) / 3), Random.Range((-myFieldHeight / 2) / 3, (myFieldHeight / 2) / 3)));
					break;
			}
		}
	}
}
