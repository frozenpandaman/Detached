using UnityEngine;
using System.Collections;

public class MovingPickup : MonoBehaviour {

	float fMagnitude = 0.75f;
    Vector3 v3Axis = new Vector3(0, 0.1f, 0);

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		transform.position += v3Axis * Mathf.Sin (Time.time) * fMagnitude;
	}
}
