using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PalmUI : MonoBehaviour
{
    private Color originalColour;
    private MeshRenderer mesh;
    private NodeGrabber collidedHand;
    public static bool collided = false;

    

    // Start is called before the first frame update
    void Start()
    {
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
                collidedHand = null;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "HandHighlight_L")
        {
            if (!collided)
            {
                if(name == "PalmButtonUp")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    EventManager.OnPressPalmUpButton();
                }
                else if(name == "PalmButtonDown") 
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    EventManager.OnPressPalmDownButton();
                }
                else if(name == "PalmButtonScaleUp")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    EventManager.OnPressPalmScaleUpButton();
                }
                else if(name == "PalmButtonScaleDown")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    EventManager.OnPressPalmScaleDownButton();
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "HandHighlight_L")
        {
            if (collided)
            {
                collided = false;
                mesh.material.color = Color.red;
                //pressed.Invoke();
            }
        }
    }
}
