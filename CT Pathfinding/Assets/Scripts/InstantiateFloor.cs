using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class InstantiateFloor : MonoBehaviour {

	private WriteFloorImage wfi;

	public IEnumerator createFloor;

	private int currIteration;

	private Transform rob, friet, frikandel;

	private Tile startTile, targetTile;

	public bool floorCreated = false;

	public Tile[] tileList;
	public List<Tile> targetList = new List<Tile> ();
	public List<Tile> calculatedPath;

	public bool allPathsFound;
	public List<bool> pathFoundList = new List<bool> ();

	public int rows, columns;					//Aantal rijen en kolommen die de vloer moet hebben
	public int tileCount;						//Totaal aantal tegels (rows * columns)

	[Header ("UI")]
	public Slider loadingBar;
	public Text loadingTxt;
	public Text timerTxt;

	void Start () {

		wfi = FindObjectOfType<WriteFloorImage> ();

		createFloor = CreateFloor ();

		rob = GameObject.Find ("rob").gameObject.transform;
		friet = GameObject.Find ("friet").gameObject.transform;
		frikandel = GameObject.Find ("frikandel").gameObject.transform;
		StartCoroutine (createFloor);
	}

	void Update(){
		//Randomise the field again
		if (Input.GetKeyDown (KeyCode.Space)) {
			timerTxt.text = ""; 
			StartCoroutine (GenerateNewFloor ()); 
		}

		if (!floorCreated) {
			timerTxt.text = CurrentTime ();
		}
	}

	private string CurrentTime(){
		return Time.unscaledTime.ToString ();
	}

	private IEnumerator GenerateNewFloor(){
		//Set all pathfound bools to false
		pathFoundList.Clear ();
		currIteration = 0;
		allPathsFound = false;

		CreateStartAndTarget ();

		foreach (Tile t in tileList) {
			t.RandomizeIfWalkable ();
		}

		for (int i = 0; i < targetList.Count - 1; i++) {
			pathFoundList.Add (false);
			FindPath (targetList[i], targetList[i + 1]);
			yield return null;
		}
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

	private void InitLoadingBar(){
		tileCount = rows * columns;
		loadingBar.maxValue = tileCount;
	}

	private IEnumerator CreateFloor(){

		InitLoadingBar ();

		for (int y = 0; y < rows; y++) {
			for (int x = 0; x < columns; x++) {
				//Instantiate de tegel en plaats het op de goede coordinaten
				GameObject tile = Instantiate(Resources.Load ("floor-tile"), new Vector3(x, y, 0), Quaternion.identity, this.transform) as GameObject;

				int currTileIndex = x * y;

				//Neem het tile-component van de tegel
				Tile currTile = tile.GetComponent <Tile> ();

				currTile.pos = new Vector2 (x, y);

				loadingBar.value = currTileIndex;
				loadingTxt.text = currTileIndex.ToString ();
			}

			yield return null;
		}

		StartCoroutine (FindTiles ());
		loadingBar.gameObject.SetActive (false);
	}

	private IEnumerator FindTiles(){
		while (tileList.Length == 0) {
			tileList = FindObjectsOfType <Tile> ();
			yield return null;
		}

		StartCoroutine (SetTileIndex ());
	}

	private IEnumerator SetTileIndex(){

		for (int i = 0; i < tileList.Length; i++) {
			tileList [i].index = i;

		}
		yield return null;
		floorCreated = true;

		AddNeighbourTiles ();
	}

	private void AddNeighbourTiles(){

		foreach (Tile t in tileList) {
			//Northtile
			if (t.index + rows < tileList.Length) {
				t.neighbourTiles.Add (tileList [t.index + rows]);
			}

			//Easttile
			if (t.index + 1 < tileList.Length && tileList [t.index + 1].pos.y == t.pos.y) {
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

		CenterCamera ();
		print (CurrentTime ());
	}

	private void CreateStartAndTarget(){

		targetList.Clear ();
		calculatedPath.Clear ();
		friet.SetParent (null);
		
		if (targetTile != null) {
			targetTile.SetWalkable ();
		}

		//The start-tile is the first tile in the tilelist
		startTile = tileList [0];
		targetList.Add (startTile);

		//Add inbetween target
		int randomInbetween = Random.Range (1, tileList.Length - 1);
		targetList.Add (tileList [randomInbetween]);

		//The target-tile is the last tile in the tilelist
		int randomTarget = Random.Range ((rows * columns) - rows , tileList.Length - 1);
		targetTile = tileList [randomTarget];
		targetList.Add (targetTile);

		rob.position = startTile.transform.position;
		friet.position = targetList[1].transform.position;
		frikandel.position = targetTile.transform.position;
	}

	private void FindPath(Tile start, Tile end){

		List<Tile> openSet = new List<Tile> ();
		HashSet<Tile> closedSet = new HashSet<Tile> ();

		start.walkable = true;
		end.walkable = true;

		openSet.Add (start);

		while (openSet.Count > 0) {
			Tile currTile = openSet [0];

			for (int i = 1; i < openSet.Count; i++) {
				if (openSet[i].fCost < currTile.fCost || openSet[i].fCost == currTile.fCost && openSet[i].hCost < currTile.hCost){
					currTile = openSet [i];
				}
			}

			openSet.Remove (currTile);
			closedSet.Add (currTile);

			if (currTile == end) {
				pathFoundList [currIteration] = true;
				currIteration++;

				for (int i = 0; i < pathFoundList.Count; i++) {
					if (pathFoundList [i] == false) {
						allPathsFound = false;
						i = 0;
					} else {
						allPathsFound = true;
					}
				}

				if (allPathsFound) {
					RetracePath (start, end);
				}
				return;
			}
				
			foreach (Tile neighbour in currTile.neighbourTiles) {
				if (!neighbour.walkable || closedSet.Contains (neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currTile.gCost + GetDistance (currTile, neighbour);

				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance (neighbour, end);
					neighbour.parent = currTile;
					//neighbour.sr.color = Color.yellow;
						
					if (!openSet.Contains (neighbour)) {
						openSet.Add (neighbour);
					}
				}
			}
			//wfi.WriteImage ();
		}

		//Als er geen pad gevonden kan worden, genereer een nieuw doolhof
		if (openSet.Count == 0 && !allPathsFound) {
			foreach (Tile t in tileList) {
				t.RandomizeIfWalkable ();
			}
			FindPath (start, end);
		}
	}

	void RetracePath(Tile start, Tile end){
		List<Tile> path = new List<Tile>();
		Tile currTile = end;

		path.Clear ();

		while (currTile != start) {
			path.Add (currTile);
			currTile = currTile.parent;
		}

		path.Reverse ();

		for (int i = 0; i < path.Count; i++) {
			calculatedPath.Add (path [i]);
		}
			
		if (tileList != null) {
			StartCoroutine (ColorPath ());
		}
	}

	private IEnumerator ColorPath(){

		float waitTime = 2.0F / calculatedPath.Count;

		targetList [0].sr.color = Color.red;
		targetList [targetList.Count - 1].sr.color = Color.green;

		for (int i = 1; i < targetList.Count - 1; i++) {
			targetList [i].sr.color = Color.blue;
		}

		for (int i = 0; i < calculatedPath.Count; i++) {
			calculatedPath [i].sr.color = Color.red;
			rob.position = calculatedPath [i].transform.position;
			yield return new WaitForSeconds(waitTime);

			if (rob.position == friet.transform.position) {
				friet.SetParent (rob);
			}

			//Als het doolhof is gereset, lopen Rob en het pad niet meer gelijk.
			//Break dan uit de for-loop
			if (rob.position != calculatedPath [i].transform.position){
				break;
			}
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
