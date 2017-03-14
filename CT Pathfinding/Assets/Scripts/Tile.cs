using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public SpriteRenderer sr;

	public int index;
	public Vector2 pos;

	public List<Tile> neighbourTiles = new List<Tile> ();

	public bool walkable = true;

	public int gCost;										//Afstand van startpunt tot deze tegel
	public int hCost;										//Afstand naar eindpunt van deze tegel

	public Tile parent;

	public void RandomizeWalkable(){
		int randomWalkable = Random.Range (0, 100);

		if (randomWalkable < 60) {
			walkable = true;
			sr.color = Color.white;
		} else {
			walkable = false;
			sr.color = Color.black;
		}
	}

	public int fCost {
		get { 
			return gCost + hCost;
		}
	}
}

