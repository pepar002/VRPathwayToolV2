using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogUI : MonoBehaviour
{
    public GameObject headcamera;
    private float speed = 10.0f;
    private Quaternion angelOffset;

    // Start is called before the first frame update
    void Start()
    {
        // Stores the origin rotation of the centeranchorcamera
        angelOffset = headcamera.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(headcamera.transform.position, transform.position);


        if (distance > 1.0f)
        {
            transform.position = new Vector3(headcamera.transform.position.x + 0.5f, headcamera.transform.position.y, headcamera.transform.position.z);

            // We used RotateAround to make sure that the dialogue box is directly infront of the player/camera.
            transform.RotateAround(headcamera.transform.position, new Vector3(0, 1, 0), headcamera.transform.rotation.eulerAngles.y - angelOffset.eulerAngles.y);

            // Make object face the player
            Vector3 v = transform.position - headcamera.transform.position;
            Quaternion q = Quaternion.LookRotation(v);
            transform.rotation = q;
        }

    }
}
