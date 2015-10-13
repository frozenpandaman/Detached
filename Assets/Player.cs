 using UnityEngine;
 using System.Collections;

 public class playerController : MonoBehaviour {

     public float sensitivityX = 15F;
     public float runSpeed = 10,
               sprintSpeed = 20;
     private Transform cameraTransform;


     // Use this for initialization
     void Start () {
         cameraTransform = Camera.main.transform;
     }

     // Update is called once per frame
     void Update () {
         if (Input.GetKey("w")) {
         	print("hi");
            if (Input.GetKey (KeyCode.LeftShift)) {
              transform.position += cameraTransform .forward * (Time.deltaTime * sprintSpeed);
              Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 80, 2*Time.deltaTime);
            }
            else {
              transform.position += cameraTransform .forward * (Time.deltaTime * runSpeed);
              Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, 2*Time.deltaTime);
            }
         }
         if (Input.GetKey("s")) {
            transform.position += Vector3.back * (Time.deltaTime * runSpeed);
         }
         if (Input.GetKey("a")) {
            transform.position += Vector3.left * (Time.deltaTime * runSpeed);
         }
         if (Input.GetKey("d")) {
            transform.position += Vector3.right * (Time.deltaTime * runSpeed);
         }
     }
 }