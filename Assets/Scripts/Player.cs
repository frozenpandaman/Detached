using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

	Rigidbody rb;
	Vector3 velocity;
	public int pickups = 0;
	public Text text;

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
	}

	void FixedUpdate()
	{ // rb.position = starting pos.
		rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Pickup")) {
			other.gameObject.SetActive(false);
			++pickups;
			text.text = pickups.ToString();
		}
	}
}