using UnityEngine;
using System.Collections;

public class ResetPos : MonoBehaviour {

	public GameObject player;
	public Transform soul;

	private float currentOffset = 0f;

	void Start() {
		UpdateOffset();
	}

	void UpdateOffset() {
		currentOffset = soul.transform.position.x;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject == player) {
			other.transform.position = new Vector3(0f,0.5f,-20f);
			UpdateOffset();
		}
		soul.transform.position = new Vector3(currentOffset+5f,0.5f,-20f);
		// Make soul/camera farther apart
		// Re-generate terrain (PCG)
	}

	//void OnTriggerStay (Collider other) {
	//}
	//void OnTriggerExit (Collider other) {
	//}
}
