using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WriteFloorImage : MonoBehaviour {

	private InstantiateFloor floor;
	private Shader shader;

	public int textureWidth, textureHeight;

	public Color color1, color2;

	void Start(){
		floor = FindObjectOfType<InstantiateFloor> ();
		shader = Shader.Find ("Unlit/Texture");
	}

	public void WriteImage()
	{
		Texture2D texture = new Texture2D(floor.rows, floor.columns);
		GetComponent<Renderer> ().material.mainTexture = texture;
		GetComponent<Renderer> ().material.shader = shader;
		print (texture.GetPixels ());


		foreach (Tile t in floor.tileList) {

			int xPos = Mathf.RoundToInt (t.pos.x);
			int yPos = Mathf.RoundToInt (t.pos.y);

			if (t.walkable) {
				texture.SetPixel (xPos, yPos, color1);
			} else {
				texture.SetPixel (xPos, yPos, color2);
			}
		}

		transform.position = new Vector2 (textureWidth / 2, textureHeight / 2);
		transform.localScale = new Vector2 (textureWidth, textureHeight);

		texture.anisoLevel = 0;
		texture.filterMode = FilterMode.Point;

		texture.Apply();
	}
}
