using UnityEngine;
using System.Collections;

public class ChangeMenu : MonoBehaviour {

    public void ChangeOnClick(string ToMenu) {
        Application.LoadLevel(ToMenu);
    }
}
