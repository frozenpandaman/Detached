using UnityEngine;
using System.Collections;

public class LoadMenu : MonoBehaviour {

	IEnumerator Start() {
		 yield return new WaitForSeconds(5f);
		 Application.LoadLevel("MainMenu");
	 }
}
