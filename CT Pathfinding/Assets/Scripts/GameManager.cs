using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	private InstantiateFloor floor;

	public GameObject optionsPanel;
	public Text rowText, columnText;
	public Slider rowSlider, columnSlider;

	void Start () {
		floor = FindObjectOfType<InstantiateFloor> ();

		rowText.text = rowSlider.value.ToString ();
		columnText.text = columnSlider.value.ToString ();

		floor.rows = Mathf.RoundToInt (rowSlider.value);
		floor.columns = Mathf.RoundToInt (columnSlider.value);
	}

	public void SliderValueChange(){
		rowText.text = rowSlider.value.ToString ();
		columnText.text = columnSlider.value.ToString ();
	}

	public void StartGame(){
		floor.rows = Mathf.RoundToInt(rowSlider.value);
		floor.columns = Mathf.RoundToInt(columnSlider.value);

		optionsPanel.SetActive (false);
		StartCoroutine (floor.createFloor);

	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			optionsPanel.SetActive (true);
		}
	}
}
