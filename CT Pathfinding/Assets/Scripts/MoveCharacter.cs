using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour {

	private InstantiateFloor f;

	void Start () {
		f = FindObjectOfType<InstantiateFloor> ();

		transform.position = f.startTile.pos;
	}
}
