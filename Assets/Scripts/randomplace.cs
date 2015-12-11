using UnityEngine;
using System.Collections;

public class randomplace : MonoBehaviour {
    public GameObject[] stuffToPlace;
    public GameObject wall;
    public GameObject grass;
    public GameObject stone;
    //private int[,] grid = new int[6, 6];
    Vector3 targetCoords;
    Vector3 trueCoords;
    public int maxNumberOfObjectsToPlace = 30;
    // Use this for initialization
    void Start () {
        float scale = 1f;
        float moveAreaX = gameObject.GetComponent<Renderer>().bounds.size.x / 2;
        float moveAreaZ = gameObject.GetComponent<Renderer>().bounds.size.z / 2;

        Vector3 center = gameObject.GetComponent<Renderer>().bounds.center;
        int numofgrass = 100;
        int numofstone = 50;
        int numofwall = 15;
        while (numofgrass > 0)
        {
            float targetCoordsX = center.x + Random.Range(-moveAreaX * scale, moveAreaX * scale);
            float targetCoordsZ = center.z + Random.Range(-moveAreaZ * scale, moveAreaZ * scale);
            targetCoords = new Vector3(targetCoordsX, -1.0f, targetCoordsZ);
            Instantiate(grass, targetCoords, Quaternion.Euler(0, Random.Range(0f, 359f), 0));
            --numofgrass;
        }
        while (numofstone > 0)
        {
            float targetCoordsX = center.x + Random.Range(-moveAreaX * scale, moveAreaX * scale);
            float targetCoordsZ = center.z + Random.Range(-moveAreaZ * scale, moveAreaZ * scale);
            targetCoords = new Vector3(targetCoordsX, 0.0f, targetCoordsZ);
            Instantiate(stone, targetCoords, Quaternion.Euler(0, Random.Range(0f, 359f), 0));
            --numofstone;
        }

        foreach (GameObject placeable in stuffToPlace)
        {
            float targetCoordsX = center.x + Random.Range(-moveAreaX * scale, moveAreaX * scale);
            float targetCoordsZ = center.z + Random.Range(-moveAreaZ * scale, moveAreaZ * scale);
            targetCoords = new Vector3(targetCoordsX,0.2f, targetCoordsZ);
            placeable.transform.position = targetCoords;
            //print(placeable);
        }

        while (numofwall > 0)
        {
            float targetCoordsX = center.x + Random.Range(-moveAreaX * scale, moveAreaX * scale);
            float targetCoordsZ = center.z + Random.Range(-moveAreaZ * scale, moveAreaZ * scale);
            targetCoords = new Vector3(targetCoordsX, 1.0f, targetCoordsZ);
            if (Random.Range(0f,359f)>180f)
            {
                int numofwalllength = Random.Range(5,15);
                for (int i =0; i<numofwalllength; ++i)
                {
                    trueCoords = new Vector3(targetCoordsX + i * 10, 1.0f, targetCoordsZ);
                    Instantiate(wall, trueCoords, Quaternion.Euler(0, 90, 0));
                }
            }
            else
            {
                int numofwalllength = Random.Range(5, 15);
                for (int i = 0; i < numofwalllength; ++i)
                {
                    trueCoords = new Vector3(targetCoordsX, 1.0f, targetCoordsZ+i*10);
                    Instantiate(wall, trueCoords, Quaternion.Euler(0, 0, 0));
                }
            }
            --numofwall;
        }
    }

        // Update is called once per frame
        void Update () {
	
	}
}
