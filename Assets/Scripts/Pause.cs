using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	bool paused = false;
	public GUIStyle textStyle = new GUIStyle();
	public GUIStyle textStyle2 = new GUIStyle();

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) { // On Esc press
			paused = gamePaused();
		}
	}

	void OnGUI()
	{
		if (paused) {

			GUI.Box(new Rect (0, 0, Screen.width, Screen.height), "PAUSED", textStyle);

			//if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 130, 100, 25), "Unpause")) {
			//	 paused = gamePaused();
			//}
			if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 95, 100, 25), "Restart level")) {
				 Application.LoadLevel(Application.loadedLevel);
			}
			if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 60, 100, 25), "Menu")) {
				 Application.LoadLevel("MainMenu");
			}
			if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 25, 100, 25), "Quit")) {
				 Application.Quit();
			}

			GUI.Box(new Rect (0, 0, Screen.width, Screen.height), "Press Esc to unpause.", textStyle2);
		}

	}

	bool gamePaused()
	{
		if (Time.timeScale == 1f) {
			Time.timeScale = 0f;
			return true;
		} else {
			Time.timeScale = 1f;
			return false;
		}
	}
}