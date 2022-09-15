using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphMovement : MonoBehaviour
{
    public GameObject graph;
    public GameObject cube;
    private bool rotate;

    public bool Rotate { get => rotate; set => rotate = value; }

    // set if graph can be rotated or move along x, y and z plane
    //public bool SetRotate { get => rotate; set => rotate = value; }
    void Start()
    {
        rotate = false;
    }

 
    void Update()
    {
        UpdateGraphState();
    }

    public void UpdateGraphState() {
        if (this.gameObject.activeSelf)
        {
            if (rotate)
            {
                graph.transform.rotation = cube.transform.rotation;
            }
            else {
                graph.transform.position = cube.transform.TransformPoint(2, 2, 1);
            }
        }
    }

    /*    public void UpdateContsraints() {
            if (rotate)            
                cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            else 
                cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        }*/

    public void ScaleGraph() { 
        //
    }


}
