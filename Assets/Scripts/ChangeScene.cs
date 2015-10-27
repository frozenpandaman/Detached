using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

	public GameObject player;
	public Transform soul;

	public string ToLevel;

	void Start() {

	}

	void OnTriggerEnter (Collider other) {
		Application.LoadLevel (ToLevel);
	}
}
