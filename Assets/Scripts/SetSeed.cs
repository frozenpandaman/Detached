using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetSeed : MonoBehaviour {

	public string seed;
	public Text seedText;

	// initialization
	void Start() {
		SetRandomSeed(); // or - random, based on current time?
		seedText = GameObject.Find("Seed InputField").GetComponent<Text>();
	}

	void SetRandomSeed() {
		seed = Time.time.ToString() + new System.Random().Next(0, 470);
	}

	public void UpdateSeed(string newSeed) {
		if (newSeed == "") {
			SetRandomSeed();
		} else {
			seed = newSeed;
		}
	}
}
