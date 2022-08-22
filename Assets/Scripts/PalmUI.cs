using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.IO;

public class PalmUI : MonoBehaviour
{
    private Color originalColour;
    private MeshRenderer mesh;
    private NodeGrabber collidedHand;
    public static bool collided = false;
    private bool activeGraph = false;



   /// <summary>
   /// Whole class not used anymore, was used for testing purposes in the old version of this
   /// 
   /// essentially allowed for a ui to pop up when you held your palm up. pressing buttons moved graph and spawned new pathways
   /// </summary>
   /// 
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
                    VRigeEventManager.OnPressPalmUpButton();
                }
                else if(name == "PalmButtonDown") 
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    VRigeEventManager.OnPressPalmDownButton();
                }
                else if(name == "PalmButtonScaleUp")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    VRigeEventManager.OnPressPalmScaleUpButton();
                }
                else if(name == "PalmButtonScaleDown")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    VRigeEventManager.OnPressPalmScaleDownButton();
                }
                else if(name == "PalmButtonControls")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    transform.parent.GetChild(0).gameObject.SetActive(false);
                    transform.parent.GetChild(1).gameObject.SetActive(false);
                    transform.parent.GetChild(2).gameObject.SetActive(false);
                    transform.parent.GetChild(3).gameObject.SetActive(false);
                    transform.parent.GetChild(4).gameObject.SetActive(false);

                    transform.parent.GetChild(5).gameObject.SetActive(true);
                    transform.parent.GetChild(6).gameObject.SetActive(true);
                    transform.parent.GetChild(7).gameObject.SetActive(true);

                }
                else if (name == "PalmButtonList")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    transform.parent.GetChild(0).gameObject.SetActive(true);
                    transform.parent.GetChild(1).gameObject.SetActive(true);
                    transform.parent.GetChild(2).gameObject.SetActive(true);
                    transform.parent.GetChild(3).gameObject.SetActive(true);
                    transform.parent.GetChild(4).gameObject.SetActive(true);

                    transform.parent.GetChild(5).gameObject.SetActive(false);
                    transform.parent.GetChild(6).gameObject.SetActive(false);
                    transform.parent.GetChild(7).gameObject.SetActive(false);

                }
                else if (name == "PalmButtonPyruvate")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    VRigeEventManager.OnPressPalmPyruvate(DataExtrator.Instance.PathwayXmls["ko00620"], Resources.Load<TextAsset>("Datasets/PyruvateKey").text);
                }
                else if (name == "PalmButtonGlycolysis")
                {
                    collided = true;
                    NodeGrabber hand = other.GetComponentInParent<NodeGrabber>();
                    mesh.material.color = Color.blue;
                    VRigeEventManager.OnPressPalmGlycolysis(DataExtrator.Instance.PathwayXmls["00010"], Resources.Load<TextAsset>("Datasets/GlycolysisKey").text);
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
                mesh.material.color = originalColour;
            }
        }
    }
}
