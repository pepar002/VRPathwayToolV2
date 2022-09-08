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
        transform.position = palmLocation.position;
        Vector3 v = transform.position - headCamera.transform.position;
        Quaternion q = Quaternion.LookRotation(v);
        transform.rotation = q;
        cube.SetActive(true);
    }
}

