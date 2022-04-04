using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class Locomotion : MonoBehaviour
{

    //public GameObject camera;

    public const float WALK_SPEED = 2.5f;
    public const float SPRINT_SPEED = 6f;


    public GameObject player;
    private float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        //Vector3 movement = camera.transform.TransformDirection(input.x, 0f, input.y);
        //movement.y = 0;
        //movement = movement.magnitude == 0 ? Vector3.zero : (movement / movement.magnitude);
        //movement *= Time.deltaTime * (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) ? SPRINT_SPEED : WALK_SPEED);
        //this.transform.Translate(movement);

        //var joystickAxis = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.LTouch);
        //float fixedY = player.position.y;

        //player.position += (transform.right * joystickAxis.x + transform.forward * joystickAxis.y) * Time.deltaTime * speed;
        //player.position = new Vector3(player.position.x, fixedY, player.position.z);

        Vector2 input = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.LTouch);
        Vector3 movement = player.transform.TransformDirection(input.x, 0f, input.y);
        movement.y = 0;
        movement = movement.magnitude == 0 ? Vector3.zero : (movement / movement.magnitude);
        movement *= Time.deltaTime * (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) ? SPRINT_SPEED : WALK_SPEED);
        player.transform.Translate(movement);
    }
}
