using UnityEngine;
using System.Collections;

public class ResetPos : MonoBehaviour {

	public GameObject player;
	public GameObject toDestroy;
	public Transform soul;

	public int times = 0;
	private float currentOffset = 0f;

	void Start() {
		UpdateOffset();
	}

	void UpdateOffset() {
		currentOffset = soul.transform.position.x;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject == player) {
			if (times == 2) {
				Destroy(this.gameObject);
				Destroy(toDestroy);
			}
			times += 1;

			other.transform.position = new Vector3(0f,0.5f,-20f);
			UpdateOffset();

			// Make soul/camera farther apart
			soul.transform.position = new Vector3(currentOffset+5f,0.5f,-20f);
		}
			}

	//void OnTriggerStay (Collider other) {
	//}
	//void OnTriggerExit (Collider other) {
	//}
}
