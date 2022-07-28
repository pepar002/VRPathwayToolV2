using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardBehaviourY : MonoBehaviour
{

    private GameObject hmdCamera;

    void Start()
    {

        if (!hmdCamera)
        {
            hmdCamera = GameObject.Find("CenterEyeAnchor");
        }
    }

    void Update()
    {

        Vector3 v = transform.position - hmdCamera.transform.position;
        Quaternion q = Quaternion.LookRotation(v);
        transform.rotation = q;
        /*transform.LookAt(transform.position + hmdCamera.transform.rotation * Vector3.forward,
              hmdCamera.transform.rotation * Vector3.up);*/
        Vector3 e = transform.eulerAngles;
        e.x = 0;
        e.z = 0;
        transform.eulerAngles = e;
    }
}
