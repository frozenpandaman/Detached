using UnityEngine;
using System.Collections;

public class ResetPos : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		other.transform.position = new Vector3(0f,0.5f,-20f);
		// Make soul/camera farther apart
		// Re-generate terrain (PCG)
	}

	//void OnTriggerStay (Collider other) {
	//}
	//void OnTriggerExit (Collider other) {
	//}
}
