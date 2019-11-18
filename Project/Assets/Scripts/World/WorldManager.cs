﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/**
 * Auth:	Jake Anderson
 * Date:	18/09/2019
 * Last:	25/10/2019
 * Name:	World Manager
 * Vers:	2.0 - Clean separation of world gen from game management.
 */

public class WorldManager : MonoBehaviour
{
	[Header("Colour Palettes")]
	[Tooltip("This array holds palettes to be randomly picked for the backdrop.")]
	[SerializeField] private Palette[] myPalettes;
	//Chosen Colors.
	private int myChosenPalette;
	public Palette Palette { get { return myPalettes[myChosenPalette]; } }

	[Header("Prefab Data")]
	[Tooltip("The Backdrop Manager prefab.")]
	[SerializeField] private GameObject myBackdropPrefab;
	private GameObject myBackdropManager;
	[Tooltip("The Asteroid Manager prefab.")]
	[SerializeField] private GameObject myAsteroidManagerPrefab;
	private GameObject myAsteroidManager;
	//The world container is an object generated by the World Manager to hold the generated world objects.
	private GameObject myWorldContainer;

	private void Awake()
	{
		//Randomly select a palette by setting myChosenPalette. The getter will use myChosenPalette whenever a script asks for Palette.
		myChosenPalette = (int)Random.Range(0, myPalettes.Length);
	}

	public void EndMatch()
	{
		BackdropManager backdrop = myBackdropManager.GetComponent<BackdropManager>();
		backdrop.FadeAndDestroy();

		AsteroidManager asteroids = myAsteroidManager.GetComponent<AsteroidManager>();
		asteroids.FadeAndDestroy();
	}

	public void GenerateWorld()
	{
		GameObject backdropController = Instantiate(myBackdropPrefab, this.transform);
		myBackdropManager = backdropController;
		myBackdropManager.name = "Backdrop Manager";

		BackdropManager backdrop = myBackdropManager.GetComponent<BackdropManager>();
		backdrop.Setup(Palette);
	}

	public void GenerateAsteroids()
	{
		GameObject asteroidManager = Instantiate(myAsteroidManagerPrefab, this.transform);
		myAsteroidManager = asteroidManager;
		myAsteroidManager.name = "Asteroid Manager";

		AsteroidManager asteroids = GetComponentInChildren<AsteroidManager>();
		asteroids.Setup(Palette.Colors[3]);
	}
}