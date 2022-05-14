using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionDebug : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider collision)
    {
        GameObject hand = collision.gameObject.GetComponentInParent<OVRHand>().gameObject;
        GestureDetector gesture = hand.GetComponentInChildren<GestureDetector>();
        if (collision.name == "IndexTip" && gesture.isPointing()) { 

        }
    }
}
