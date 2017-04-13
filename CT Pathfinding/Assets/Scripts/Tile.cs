using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public SpriteRenderer sr;

	public int index;
	public Vector2 pos;

	public List<Tile> neighbourTiles = new List<Tile> ();

	public bool walkable;

	public int gCost;										//Afstand van startpunt tot deze tegel
	public int hCost;										//Afstand naar eindpunt van deze tegel

	public Tile parent;

	public void RandomizeIfWalkable(){
		int randomWalkable = Random.Range (0, 100);

		if (randomWalkable < 60) {
			SetWalkable ();
		} else {
			SetUnwalkable ();
		}
	}

	public void SetWalkable(){
		walkable = true;
		sr.color = Color.white;
		//sr.enabled = false;
	}

	public void SetUnwalkable(){
		walkable = false;
		sr.color = Color.black;
		//sr.enabled = true;
	}

	public int fCost {
		get { 
			return gCost + hCost;
		}
	}
}

