using UnityEngine;
using System.Collections;

public class randomplace : MonoBehaviour {
    public GameObject[] stuffToPlace;
    public GameObject wall;
    public GameObject grass;
    public GameObject stone;
    private int[,] grid = new int[6, 6];
    Vector3 targetCoords;
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
            print(placeable);
        }

        while (numofwall > 0)
        {
            float targetCoordsX = center.x + Random.Range(-moveAreaX * scale, moveAreaX * scale);
            float targetCoordsZ = center.z + Random.Range(-moveAreaZ * scale, moveAreaZ * scale);
            targetCoords = new Vector3(targetCoordsX, 1.0f, targetCoordsZ);
            Object clone;
            float changedlength = Random.Range(30f, 70f);
            wall.transform.localScale += new Vector3(-0.5f, 0, changedlength);
            if (Random.Range(0f,359f)>180f)
            {
                clone = Instantiate(wall, targetCoords, Quaternion.Euler(0, 90, 0));
            }
            else
            {
                clone = Instantiate(wall, targetCoords, Quaternion.Euler(0, 0, 0));
            }
            wall.transform.localScale += new Vector3(0.5f, 0, -changedlength);
            --numofwall;
        }
    }

        // Update is called once per frame
        void Update () {
	
	}
}
