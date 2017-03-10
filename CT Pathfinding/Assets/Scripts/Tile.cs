using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	private InstantiateFloor floor;

	public Vector3 pos;										//Positie van deze tegel

	public List<Tile> neighbourTiles = new List<Tile> ();

	public bool walkable = true;

	public int gCost;										//Afstand van startpunt tot deze tegel
	public int hCost;										//Afstand naar eindpunt van deze tegel

	public Tile parent;

	void Start(){

		int randomWalkable = Random.Range (0, 100);

		if (randomWalkable < 75) {
			walkable = true;
		} else {
			walkable = false;
			GetComponent <SpriteRenderer>().color = Color.black;
		}
			
		AddAdjacentTiles ();
	}

	private void AddAdjacentTiles(){
		floor = FindObjectOfType<InstantiateFloor> ();

		foreach (Tile t in floor.tileList) {

			/*	Als een tegel een x positie + 1 van deze tegel heeft,
				als een tegel een x positie - 1 van deze tegel heeft,
				als een tegel een y positie + 1 van deze tegel heeft,
				als een tegel een y positie - 1 van deze tegel heeft,
				voeg toe aan de buur-tegel lijst
			*/

			if (t.pos.x == pos.x + 1 && t.pos.y == pos.y) {
				neighbourTiles.Add (t);
			}
			if (t.pos.x == pos.x - 1 && t.pos.y == pos.y) {
				neighbourTiles.Add (t);
			}
			if (t.pos.x == pos.x && t.pos.y == pos.y + 1) {
				neighbourTiles.Add (t);
			}
			if (t.pos.x == pos.x && t.pos.y == pos.y - 1) {
				neighbourTiles.Add (t);
			}

			/*
			//Voeg diagonale buurtegels toe
			if (t.pos.x == pos.x - 1 && t.pos.y == pos.y + 1) {
				neighbourTiles.Add (t);
			}
			if (t.pos.x == pos.x + 1 && t.pos.y == pos.y + 1) {
				neighbourTiles.Add (t);
			}
			if (t.pos.x == pos.x -1  && t.pos.y == pos.y - 1) {
				neighbourTiles.Add (t);
			}
			if (t.pos.x == pos.x + 1 && t.pos.y == pos.y - 1) {
				neighbourTiles.Add (t);
			}
			*/
		}
	}

	public int fCost {
		get { 
			return gCost + hCost;
		}
	}
}

