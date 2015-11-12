using UnityEngine;
using System.Collections;

public class MouseCamera : MonoBehaviour {

	public float minX = -360.0f;
	public float maxX = 360.0f;

	public float minY = -45.0f;
	public float maxY = 45.0f;

	public float sensX = 200.0f;
	public float sensY = 200.0f;

	float rotationY = 0.0f;
	float rotationX = 0.0f;

	void Update () {

		rotationX += Input.GetAxis ("Mouse X") * sensX * Time.deltaTime;
		rotationY = -45.0f;
		rotationY = Mathf.Clamp (rotationY, minY, maxY);
		transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
	}
}
