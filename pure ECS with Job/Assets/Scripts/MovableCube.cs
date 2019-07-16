using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableCube : MonoBehaviour {


    public GameObject movableCube;
    private Camera cam;
    Vector3 point;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        movableCube.GetComponent<Rigidbody>().useGravity = false;
    }
	
	// Update is called once per frame
	void Update () {
        MovingCube();

    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
    private void MovingCube()
    {

        movableCube.transform.localPosition = GetWorldPositionOnPlane(Input.mousePosition, 0);
        //movableCube.transform.localScale = new Vector3(1, 1, 1);
    }

}
