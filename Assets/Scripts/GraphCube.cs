using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphCube : MonoBehaviour
{
    public Transform palmLocation;
    public GameObject headCamera;
    public GameObject cube;
/*    private void OnEnable()
    {
        cube.transform.position = palmLocation.position;
        //Vector3 v = cube.transform.position - headCamera.transform.position;
        //Quaternion q = Quaternion.LookRotation(v);
        //cube.transform.rotation = q;
    }*/

    public void DisplayCube()
    {
        cube.SetActive(false);
        cube.transform.position = palmLocation.position;
        //Vector3 v = cube.transform.position - headCamera.transform.position;
        //Quaternion q = Quaternion.LookRotation(v);
        //cube.transform.rotation = q;
        cube.SetActive(true);
    }

}

