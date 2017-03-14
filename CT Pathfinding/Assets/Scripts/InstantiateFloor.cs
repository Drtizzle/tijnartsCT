using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateFloor : MonoBehaviour {

	private Transform rob;
	private Transform frikandel;

	public bool floorCreated = false;
	public bool pathFound = false;
	
	public List<Tile> tileList = new List<Tile> ();

	public Tile startTile;
	public Tile targetTile;

	public int rows, columns;					//Aantal rijen en kolommen die de vloer moet hebben

	void Awake () {
		rob = GameObject.Find ("rob").gameObject.transform;
		frikandel = GameObject.Find ("frikandel").gameObject.transform;

		CenterCamera ();
		CreateFloor ();
		AddNeighbourTiles ();
	}

	void Update(){
		//Randomise the field again
		if (Input.GetKeyDown (KeyCode.Space)) {
			StopCoroutine(ColorPath ());
			GenerateNewFloor ();
		}
	}

	private void GenerateNewFloor(){
		pathFound = false;
		CreateStartEnd ();

		foreach (Tile t in tileList) {
			t.RandomizeWalkable ();
		}

		FindPath ();
	}

	private void CenterCamera(){
		//Sla de z-positie van de camera op
		float camPosZ = Camera.main.transform.position.z;

		//Centreer de camera in verhouding tot de vloer
		Camera.main.transform.position = new Vector3 (rows/2, columns/2, camPosZ);

		//Pas de orthografische grootte aan, zodat de vloer in het beeld past
		//Pak het grootste getal van de rijen of kolommen om de camera op aan te passen
		if (rows > columns) {
			Camera.main.orthographicSize = (rows / 2) + 1;
		} else {
			Camera.main.orthographicSize = (columns / 2) + 1;
		}
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

				currTile.index = tileList.IndexOf (currTile);
				currTile.pos = new Vector2 (x, y);
			}
		}

		floorCreated = true;
	}

	private void AddNeighbourTiles(){

		foreach (Tile t in tileList) {
			//Northtile
			if (t.index + rows < tileList.Count) {
				t.neighbourTiles.Add (tileList [t.index + rows]);
			}

			//Easttile
			if (t.index + 1 < tileList.Count && tileList [t.index + 1].pos.y == t.pos.y) {
				t.neighbourTiles.Add (tileList [t.index + 1]); 
			}

			//SouthTile
			if (t.index - rows > 0) {
				t.neighbourTiles.Add (tileList [t.index - rows]); 
			}
		
			//WestTile
			if (t.index - 1 > 0 && tileList [t.index - 1].pos.y == t.pos.y) {
				t.neighbourTiles.Add (tileList [t.index - 1]); 
			}
		}
	}

	private void CreateStartEnd(){
		int randomTarget = Random.Range (1, tileList.Count - 1);

		if (targetTile != null) {
			targetTile.sr.color = Color.white;
		}

		startTile = tileList [0];
		targetTile = tileList [randomTarget];

		rob.position = startTile.transform.position;
		frikandel.position = targetTile.transform.position;
	}

	private void FindPath(){
		List<Tile> openSet = new List<Tile> ();
		HashSet<Tile> closedSet = new HashSet<Tile> ();

		startTile.walkable = true;
		targetTile.walkable = true;

		openSet.Add (startTile);

		while (openSet.Count > 0) {
			Tile currTile = openSet [0];

			for (int i = 1; i < openSet.Count; i++) {
				if (openSet[i].fCost < currTile.fCost || openSet[i].fCost == currTile.fCost && openSet[i].hCost < currTile.hCost){
					currTile = openSet [i];
				}
			}

			openSet.Remove (currTile);
			closedSet.Add (currTile);

			if (currTile == targetTile) {
				RetracePath (startTile, targetTile);
				pathFound = true;
				return;
			}
				
			foreach (Tile neighbour in currTile.neighbourTiles) {
				if (!neighbour.walkable || closedSet.Contains (neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currTile.gCost + GetDistance (currTile, neighbour);

				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance (neighbour, targetTile);
					neighbour.parent = currTile;
					neighbour.sr.color = Color.yellow;
						
					if (!openSet.Contains (neighbour)) {
						openSet.Add (neighbour);
					}
				}
			}
		}

		if (openSet.Count == 0 && !pathFound) {
			foreach (Tile t in tileList) {
				t.RandomizeWalkable ();
			}
			FindPath ();
		}
	}
		
	public List<Tile> calculatedPath;

	void RetracePath(Tile start, Tile end){
		List<Tile> path = new List<Tile>();
		Tile currTile = end;

		path.Clear ();

		while (currTile != start) {
			path.Add (currTile);
			currTile = currTile.parent;
		}

		path.Reverse ();

		calculatedPath = path;

		if (tileList != null) {
			StartCoroutine (ColorPath ());
		}
	}

	IEnumerator ColorPath(){

		float waitTime = 2.0F / calculatedPath.Count;

		startTile.sr.color = Color.red;
		targetTile.sr.color = Color.green;

		frikandel.position = targetTile.transform.position;

		for (int i = 0; i < calculatedPath.Count; i++) {
			calculatedPath [i].sr.color = Color.red;
			rob.position = calculatedPath [i].transform.position;
			yield return new WaitForSeconds(waitTime);
		}
	}

	int GetDistance (Tile tileA, Tile tileB){

		//Rond alle posities af naar INTs
		int xPosA = Mathf.RoundToInt (tileA.transform.position.x);
		int yPosA = Mathf.RoundToInt (tileA.transform.position.y);
		int xPosB = Mathf.RoundToInt (tileB.transform.position.x);
		int yPosB = Mathf.RoundToInt (tileB.transform.position.y);

		int dstX = Mathf.Abs (xPosA - xPosB);
		int dstY = Mathf.Abs (yPosA - yPosB);

		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		} else {
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
}
