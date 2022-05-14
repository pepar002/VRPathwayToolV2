using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BillboardBehaviour : MonoBehaviour {

    private GameObject hmdCamera;

    void Start()
    {

        if (!hmdCamera)
        {
            hmdCamera = GameObject.Find("CenterEyeAnchor");
        }
    }

    void Update () {
        
        Vector3 v = transform.position - hmdCamera.transform.position;
        Quaternion q = Quaternion.LookRotation(v);
        transform.rotation = q;
	}
}
