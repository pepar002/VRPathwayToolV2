using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NodeGrabbable : OVRGrabbable
{
    private Color originalColour;
    private MeshRenderer mesh;
    private NodeGrabber collidedHand;
    private bool activeNode = false;
    private bool activeReset = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        mesh = GetComponent<MeshRenderer>();

        if (mesh.material.color != null)
        {
            originalColour = mesh.material.color;
        };
    }

    private void Update()
    {
        if (collidedHand)
        {
            if (collidedHand.grabbedObject != null && !collidedHand.grabbedObject.Equals(this))
            {
                ResetColour();
                collidedHand = null;
            }
        }
    }


    private void OnTriggerStay(Collider collision)
    {
        // Comparison done with name as tags are not detected on the hands for unknown reasons
        if (collision.name == "HandHighlight_L" || collision.name == "HandHighlight_R")
        {
            NodeGrabber hand = collision.gameObject.GetComponentInParent<NodeGrabber>();
            if (!hand.grabbedObject && mesh.material.color != Color.green)
            {
                // photonView.RPC("ChangeGrabbableColour", RpcTarget.AllBuffered, "green", 0f, 0f, 0f);
                if (hand.gesture.isMiddlePinching())
                {
                    ChangeGrabbableColour("green", 0f, 0f, 0f);
                    collidedHand = hand;
                }
                else if (hand.gesture.isIndexPinching() && activeReset == false){
                    if (!activeNode)
                    {
                        ChangeGrabbableColour("yellow", 0f, 0f, 0f);
                        activeReset = true;
                        activeNode = true;
                        //ScatterPlotSceneManager.Instance.SpawnGraph(transform.position);
                    }
                    else
                    {
                        ChangeGrabbableColour("blue", 0f, 0f, 0f);
                        activeNode = false;
                        activeReset = true;
                    }
                    collidedHand = hand;

                }
                
            }
        }
        /*else if (collision.name == "IndexTip")
        {
            NodeGrabber hand = collision.gameObject.GetComponentInParent<NodeGrabber>();
            if (hand.gesture.isPointing() && !hand.gesture.isIndexPinching())
            {
                ChangeGrabbableColour("red", 0f, 0f, 0f);
                collidedHand = hand;
            }
        }*/

        else if (collision.name == "Laser_L" || collision.name == "Laser_R")
        {
            // photonView.RPC("ChangeGrabbableColour", RpcTarget.AllBuffered, "yellow", 0f, 0f, 0f);
            ChangeGrabbableColour("yellow", 0f, 0f, 0f);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.name == "IndexTip" || collision.name == "HandHighlight_L" || collision.name == "HandHighlight_R" || collision.name == "Laser_L" || collision.name == "Laser_R")
        {
            print("testing active");
            ResetColour();
            activeReset = false;
            collidedHand = null;
        }
    }

    public void ResetColour()
    {
        // photonView.RPC("ChangeGrabbableColour", RpcTarget.AllBuffered, "null", originalColour.r, originalColour.g, originalColour.b);
        if (!activeNode)
        {
            ChangeGrabbableColour("null", originalColour.r, originalColour.g, originalColour.b);
        }
        else
        {
            ChangeGrabbableColour("yellow", 0f, 0f, 0f);
        }
        
    }

    public void ChangeGrabbableColour(string colour, float r, float g, float b)
    {
        mesh = GetComponent<MeshRenderer>();
        float alpha = mesh.material.color.a;
        if (colour.Equals("blue"))
        {
            mesh.material.color = Color.blue;
        }
        else if (colour.Equals("yellow"))
        {
            mesh.material.color = Color.yellow;
        }
        else if (colour.Equals("green"))
        {
            mesh.material.color = Color.green;
        }
        else if (colour.Equals("red"))
        {
            mesh.material.color = Color.red;
        }
        else if (colour.Equals("null"))
        {
            mesh.material.color = new Color(r, g, b);
        }

        // Change transparency of nodes
        Color col = mesh.material.color;
        //col.a = alpha; 
        mesh.material.color = col;
    }
}
