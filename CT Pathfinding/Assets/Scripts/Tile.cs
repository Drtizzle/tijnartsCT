using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	private InstantiateFloor floor;

	public Vector3 pos;										//Position of this tile

	public List<Tile> neighbourTiles = new List<Tile> ();

	public bool walkable = true;

	public int gCost;										//Distance from start to this tile
	public int hCost;										//Distance from target to this tile

	public Tile parent;

	void Start(){

		int randomWalkable = Random.Range (0, 100);

		if (randomWalkable < 70) {
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

			/*	If a tile in the list has the same x position + 1, add to East Tile
				If a tile in the list has the same x position - 1, add to West Tile
				If a tile in the list has the same y position + 1, add to North Tile
				If a tile in the list has the same y position - 1, add to South Tile
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
			//Add Diagonal Neighbours
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

