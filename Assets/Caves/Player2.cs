using UnityEngine;
using System.Collections;

public class Player2 : MonoBehaviour {

	Rigidbody rb;
	Vector3 velocity;

	// Use this for initialization
	void Start() {
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update() {
		velocity = new Vector3(Input.GetAxisRaw("HorizontalArrows"), 0, Input.GetAxisRaw("VerticalArrows")).normalized * 10;
	}

	void FixedUpdate() { // rb.position = starting pos.
		rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
	}
}
