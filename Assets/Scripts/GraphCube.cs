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
        cube.SetActive(false);
        cube.transform.position = palmLocation.position;
        cube.SetActive(true);
    }

}

