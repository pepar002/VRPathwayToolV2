using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class ControllerGrabber : MonoBehaviour
{
    //public Transform controller;
    List<GameObject> objects = new List<GameObject>();
    List<GameObject> graphs = new List<GameObject>();

    public void OnTriggerEnter(Collider other) //picking up objects with rigidbodies
    {

        if (other.gameObject.CompareTag("Grabbable"))
        {

            objects.Add(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Graph"))
        {

            graphs.Add(other.gameObject);
        }
        else {

            return;
        }

        //if (!other.gameObject.CompareTag("Grabbable") )
        //{

        //    return;
        //}

        //objects.Add(other.gameObject);
    }

    public void OnTriggerExit(Collider other) // releasing those objects with rigidbodies
    {

        if (other.gameObject.CompareTag("Grabbable"))
        {

            objects.Remove(other.gameObject);
        }
        else
        {

            graphs.Remove(other.gameObject);
        }

        //objects.Remove(other.gameObject);
    }


    void Update() // refreshing program confirms trigger pressure and determines whether holding or releasing object

    {

        float lTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        float rTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        bool aButton = OVRInput.Get(OVRInput.Button.One); // pressed a button

        if (lTrigger > 0.2f || rTrigger > 0.2f)
        {

            foreach (GameObject go in objects)
            {

                GrabObject(go);
            }
            
        }
        else
        {
            foreach (GameObject go in objects)
            {

                ReleaseObject(go);
            }

        }

        if (aButton == true)
        {
            Debug.Log("Button A is pressed.");
            // possibly showing the labels of the graph
            foreach (GameObject go in graphs)
            {
                ShowLables(go);
                //bool isGraph = CheckEnterGraphArea(go);

                //Debug.Log("is Graph: " + isGraph);

                //if (isGraph) {

                //    ShowLables(go);
                //}
             
            }
        }
    }


    private void GrabObject(GameObject other) //create parentchild relationship between object and hand so object follows hand
    {

        Rigidbody rb = other.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        other.transform.position = transform.position;
        other.transform.rotation = transform.rotation;
    }


    private void ReleaseObject(GameObject other) //removing parentchild relationship so you drop the object
    {

        Rigidbody rb = other.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;
    }

    private bool CheckEnterGraphArea(GameObject other)
    {
        if (!other.gameObject.CompareTag("Graph"))
        {

            return false;
        }

        return true;
    }

    private void ShowLables(GameObject graph) {

        var pathwayController = graph.GetComponent<PathwayController>();
        pathwayController.ToggleNodesLabel();

        if (pathwayController)
        {

            Debug.Log("True");
        }
        else {

            Debug.Log("False");
        }
    }
}
