using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WriteFloorImage : MonoBehaviour {

	private InstantiateFloor floor;
	private Shader shader;

	public int textureWidth, textureHeight;
	public Color color1, color2;
	public Texture2D texture;

	void Start(){
		floor = FindObjectOfType<InstantiateFloor> ();
		shader = Shader.Find ("Unlit/Texture");
	}

	public void WriteImage() {
		texture = new Texture2D(floor.rows, floor.columns);
		GetComponent<Renderer> ().material.mainTexture = texture;
		GetComponent<Renderer> ().material.shader = shader;

		textureWidth = floor.rows;
		textureHeight = floor.columns;

		transform.position = new Vector2 (textureWidth / 2, textureHeight / 2);
		transform.localScale = new Vector2 (textureWidth, textureHeight);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;


		foreach (Tile t in floor.tileList) {

			int xPos = Mathf.RoundToInt (t.pos.x);
			int yPos = Mathf.RoundToInt (t.pos.y);

			if (t.walkable) {
				texture.SetPixel (xPos, yPos, color1);
			} else {
				texture.SetPixel (xPos, yPos, color2);
			}
		}


		texture.Apply ();

		floor.floorCreated = true;
	}

	public IEnumerator WriteRows(Texture2D texture){
		for (int i = 0; i < floor.rows; i ++) {

			Tile t = floor.tileList [i];

			for (int x = 0; x < floor.columns; x ++) {
				
				int xPos = Mathf.RoundToInt (t.pos.x);
				int yPos = Mathf.RoundToInt (t.pos.y);

				if (t.walkable) {
					texture.SetPixel (xPos, yPos, color1);
				} else {
					texture.SetPixel (xPos, yPos, color2);
				}
			}
			yield return null;
		}

		texture.Apply();


	}
}
