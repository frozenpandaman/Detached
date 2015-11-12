using UnityEngine;
using System.Collections;

public class randomplace : MonoBehaviour {
    public GameObject[] stuffToPlace;
    private int[,] grid = new int[6, 6];
    Vector3 targetCoords;
    public int maxNumberOfObjectsToPlace = 30;
    // Use this for initialization
    void Start () {
        float scale = 1f;
        float moveAreaX = gameObject.GetComponent<Renderer>().bounds.size.x / 2;
        float moveAreaZ = gameObject.GetComponent<Renderer>().bounds.size.z / 2;
        Vector3 center = gameObject.GetComponent<Renderer>().bounds.center;
        foreach (GameObject placeable in stuffToPlace)
        {
            float targetCoordsX = center.x + Random.Range(-moveAreaX * scale, moveAreaX * scale);
            float targetCoordsZ = center.z + Random.Range(-moveAreaZ * scale, moveAreaZ * scale);
            targetCoords = new Vector3(targetCoordsX, 2.1f, targetCoordsZ);
            placeable.transform.position = targetCoords;
            print(placeable);
        }
    }

        // Update is called once per frame
        void Update () {
	
	}
}
