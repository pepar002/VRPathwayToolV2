using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollider : MonoBehaviour
{

    List<GameObject> graphs = new List<GameObject>();

    public void OnTriggerEnter(Collider other) //picking up objects with rigidbodies
    {
        Debug.Log("Trigger");
        var parentObject = other.transform.parent.gameObject;

        if (!parentObject.CompareTag("Graph"))
        {

            Debug.Log("Nope.");
            return;
        }
     
        graphs.Add(parentObject);
       
    }

    public void OnTriggerExit(Collider other) // releasing those objects with rigidbodies
    {
        var parentObject = other.transform.parent.gameObject;

        if (!parentObject.CompareTag("Graph"))
        {

            return;
        }

        graphs.Remove(parentObject);
    }


    void Update()
    {
        bool aButton = OVRInput.Get(OVRInput.Button.One); // pressed a button

        if (aButton == true)
        {
            Debug.Log("Button A is pressed.");
            // possibly showing the labels of the graph
            foreach (GameObject go in graphs)
            {
                ShowLables(go);
            }
        }
    }


    private void ShowLables(GameObject graph)
    {

        var pathwayController = graph.GetComponent<PathwayController>();
        pathwayController.ToggleNodesLabel();

        if (pathwayController)
        {

            Debug.Log("True");
        }
        else
        {

            Debug.Log("False");
        }
    }
}
