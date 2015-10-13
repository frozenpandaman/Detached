using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public Vector3 offset;
	//float speed = 75.0f;

	// Use this for initialization
	void Start() {
		offset = transform.position - player.transform.position;
	}

	void Update() {
		//Vector3 v3 = new Vector3(0.0f, Input.GetAxis("Horizontal"), 0.0f);
		//transform.Rotate(v3 * speed * Time.deltaTime);
	}

	// LateUpdate is called once per frame, but
	// also guaranteed to run after all items in Update have been run.
	void LateUpdate() {
		transform.position = player.transform.position + offset;
	}
}
