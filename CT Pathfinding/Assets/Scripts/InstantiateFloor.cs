using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateFloor : MonoBehaviour {

	public Sprite doorSprite;

	public List<Tile> tileList = new List<Tile> ();
	public Tile startTile;
	public Tile targetTile;

	public int rows, columns;					//Aantal rijen en kolommen die de vloer moet hebben

	void Start () {

		//Sla de z-positie van de camera op
		float camPosZ = Camera.main.transform.position.z;

		//Centreer de camera in verhouding tot de vloer
		Camera.main.transform.position = new Vector3 (rows/2, columns/2, camPosZ);															

		CreateFloor ();
		CreateStartEnd ();
	}

	private void CreateFloor(){
		for (int y = 0; y < columns; y++) {
			for (int x = 0; x < rows; x++) {
				//Instantiate de tegel en plaats het op de goede coordinaten
				GameObject tile = Instantiate(Resources.Load ("floor-tile"), new Vector3(x, y, 0), Quaternion.identity) as GameObject;

				//Maak het object waar dit script op zit de parent van de tegel
				tile.transform.parent = this.transform;

				//Neem het tile-component van de tegel
				Tile currTile = tile.GetComponent <Tile> ();

				//Voeg de tegel toe aan de lijst
				tileList.Add (currTile);
			}
		}
	}

	private void CreateStartEnd(){

		int startPosX = Random.Range (0, rows);	//Genereer een random x-positie voor de startnode
		int endPosX = Random.Range (0, rows);	//Genereer een random x-positie voor de eindnode
		int endPosY = rows;						//Plaats de targetnode één rij boven de laatste rij

		//Instantiate de tegel en plaats het op de goede coordinaten
		GameObject start = Instantiate(Resources.Load ("floor-tile"), new Vector3(startPosX, -1, 0), Quaternion.identity) as GameObject;
		GameObject target = Instantiate(Resources.Load ("floor-tile"), new Vector3(endPosX, endPosY, 0), Quaternion.identity) as GameObject;

		//Maak het object waar dit script op zit de parent van de tegels
		start.transform.parent = this.transform;
		target.transform.parent = this.transform;

		//Neem het tile-component van de tegels
		Tile startTile = start.GetComponent <Tile> ();
		Tile targetTile = target.GetComponent <Tile> ();

		//Voeg de tegels toe aan de lijst
		tileList.Add (startTile);
		tileList.Add (targetTile);

		//Verander de sprite van de start en eind tile
		start.GetComponent <SpriteRenderer> ().sprite = doorSprite;
		target.GetComponent <SpriteRenderer> ().sprite = doorSprite;
	}
}
