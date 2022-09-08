using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphMovement : MonoBehaviour
{
    public GameObject graph;
    public GameObject cube;
    public bool rotate;

    // set if graph can be rotated or move along x, y and z plane
    //public bool SetRotate { get => rotate; set => rotate = value; }
    void Start()
    {
        rotate = true;
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
                UpdateContsraints();
                graph.transform.rotation = cube.transform.rotation;
            }
            else {
                UpdateContsraints();
                graph.transform.position = cube.transform.TransformPoint(2, 2, 1);
            }
        }
    }

    public void UpdateContsraints() {
        if (rotate)            
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        else 
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

    }


}
