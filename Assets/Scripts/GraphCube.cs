using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphCube : MonoBehaviour
{
    public Transform palmLocation;
    public GameObject headCamera;
    public GameObject cube;

    public void DisplayCube()
    {
        cube.transform.position = palmLocation.position;
        Vector3 v = cube.transform.position - headCamera.transform.position;
        Quaternion q = Quaternion.LookRotation(v);
        cube.transform.rotation = q;
        cube.SetActive(true);
        cube.GetComponent<GraphMovement>().Active = true;
    }

    public void HideCube() {
        if (cube.GetComponent<GraphMovement>().Active)
        {
            cube.SetActive(false);
            cube.GetComponent<GraphMovement>().Active = false;
        }

    }
}

