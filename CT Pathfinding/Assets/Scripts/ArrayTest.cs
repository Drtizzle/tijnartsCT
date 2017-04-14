using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArrayTest : MonoBehaviour {

	private bool floorInitialised;

	public int columns, rows;

	public Tile[] tileList;

	[Header("UI")]
	public Slider loadingBar;
	public Text loadingText;
	public Text timerText;

	void Start(){

		loadingBar.maxValue = columns * rows;

		StartCoroutine (CheckTime ());
		StartCoroutine (CreateFloor ());
	}

	private IEnumerator CreateFloor(){
		for (int y = 0; y < columns; y++) {
			for (int x = 0; x < rows; x++) {
				//Instantiate de tegel en plaats het op de goede coordinaten
				GameObject tile = Instantiate(Resources.Load ("floor-tile"), new Vector3(x, y, 0), Quaternion.identity, this.transform) as GameObject;

				int currTileIndex = x * y;
			
				//Neem het tile-component van de tegel
				Tile currTile = tile.GetComponent <Tile> ();

				currTile.pos = new Vector2 (x, y);

				loadingBar.value = currTileIndex;
				loadingText.text = currTileIndex.ToString ();
			}

			yield return null;
		}

		StartCoroutine (FindTiles ());
		CenterCamera ();
	}

	private IEnumerator FindTiles(){
		while (tileList.Length == 0) {
			tileList = FindObjectsOfType <Tile> ();
			print ("oi");
			yield return null;
		}

		StartCoroutine (SetTileIndex ());
	}

	private IEnumerator SetTileIndex(){
		
		for (int i = 0; i < tileList.Length; i++) {
			tileList [i].index = i;

		}
		yield return null;
		floorInitialised = true;
	}

	private IEnumerator CheckTime(){
		while(!floorInitialised){
			timerText.text = CurrentTime ();
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

	private string CurrentTime(){
		return Time.unscaledTime.ToString ();
	}
}
