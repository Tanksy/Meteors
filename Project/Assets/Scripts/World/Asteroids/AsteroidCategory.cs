using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Auth:   Jake Anderson
 *  Date:   18/10/2019
 *  Last:   18/10/2019
 *  Name:   Asteroid Category
 *  Vers:   1.0 - Initial Version.
 */

[System.Serializable]
public class AsteroidCategory
{
    [Tooltip("This category")]
    [SerializeField] private byte myCategory;
    public byte Category { get { return myCategory; } }
    [Tooltip("The Asteroids in this category")]
    [SerializeField] private GameObject[] myAsteroids;
    public GameObject[] Asteroids { get { return myAsteroids; } }
}
