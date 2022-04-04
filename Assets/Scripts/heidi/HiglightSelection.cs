using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiglightSelection : MonoBehaviour
{

    public Material selectionMaterial;

    private void OnTriggerEnter(Collider other)
    {

        if (!other.gameObject.CompareTag("Grabbable"))
        {

            return;
        }
        else
        {

            var grabObject = other.gameObject.GetComponent<Grabbable>();

            //other.gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            other.GetComponent<Renderer>().material = selectionMaterial;

            if (grabObject.touchCount == 0)
            {

                //other.gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                //other.GetComponent<Renderer>().material = selectionMaterial;
            }

            grabObject.touchCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (!other.gameObject.CompareTag("Grabbable"))
        {

            return;
        }
        else
        {

            var grabObject = other.gameObject.GetComponent<Grabbable>();
            grabObject.touchCount--;

            other.GetComponent<Renderer>().material = grabObject.GetMaterial();
            //other.gameObject.transform.localScale = grabObject.GetScale();

            //if (grabObject.touchCount != 0)
            //{


            //}
            //else
            //{

            //    other.GetComponent<Renderer>().material = grabObject.GetMaterial();
            //    other.gameObject.transform.localScale = grabObject.GetScale();
            //}
        }

    }
}
