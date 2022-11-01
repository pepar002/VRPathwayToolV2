using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cube object was changed to Dodecahedron
public class CubePhysics : MonoBehaviour
{
    public GameObject graph;
    public GameObject cube;
    private bool rotate = false;
    private bool scale = false;
    private bool move = true;  // starts with the movement setting ON

    private float moveMultiplier = 10.0f;

    // stores the transform information before current update frame
    private Vector3 prevPos;
    private Quaternion prevRotation;
    private Vector3 prevScale;
    private float minScale = 1.5f;
    private float maxScale = 6.0f;

    public bool Rotate { get => rotate; set { rotate = value; GetPreviousTransform(); } }

    public bool ChangeGraphScale { get => scale; set { scale = value; GetPreviousTransform(); } }

    public bool Movement { get => move; set { move = value; GetPreviousTransform(); } }

    private void Start()
    {
        // store rotation and scale once cube is spawned
        prevRotation = cube.transform.rotation;
        prevScale = cube.transform.localScale;
    }

    void Update()
    {
        UpdateGraphState();
    }
    private void OnEnable()
    {

        GetPreviousTransform();
    }


    // Various settings to allow the graph to mimic the cube's movement, rotation and scale
    public void UpdateGraphState() {

        // rotate the graph
        if (rotate)
        {
            // get rotation offset between graph and cube
            if (prevRotation != cube.transform.rotation) {
                var angelOffset = prevRotation.eulerAngles - graph.transform.eulerAngles;

                // maintain the rotation offset
                var newRotation = cube.transform.eulerAngles - angelOffset;

                // update graph and previous-cube rotations
                graph.transform.eulerAngles = newRotation;
            }
                
            prevRotation = cube.transform.rotation;
        }
            
        // move the graph
        if (move)
        {
            // get the displacement of the cube
            var cubeDisplacement = cube.transform.position - prevPos;

            prevPos = cube.transform.position;           

            // Move the graph with respect to the cube displacement
            graph.GetComponent<VRige.VRige_Graph_Creator>().MoveGraphX(graph.transform.position.x + cubeDisplacement.x * moveMultiplier);
            graph.GetComponent<VRige.VRige_Graph_Creator>().MoveGraphY(graph.transform.position.y + cubeDisplacement.y * moveMultiplier);
            graph.GetComponent<VRige.VRige_Graph_Creator>().MoveGraphZ(graph.transform.position.z + cubeDisplacement.z * moveMultiplier);

        }

        // Scale the graph with respect to the scale change on the cube
        if (scale) {
            // only one dimension is used since the change is the same for all dimensions            
            var changeInScale = cube.transform.localScale.x - prevScale.x;
            prevScale = cube.transform.localScale;
            if (changeInScale != 0)
            {
                graph.GetComponent<VRige.VRige_Graph_Creator>().ScaleGraph(changeInScale);
            }
            
        }
        
    }

    // Updates all the previous variables to hold current cube transform when cube is enabled or buttons are enabled/disabled
    private void GetPreviousTransform()
    {

        prevPos = cube.transform.position;
        // reset cube to its prev scale and rotation if scale and rotation is enabled
        if (scale) cube.transform.localScale = prevScale;
        if (rotate) cube.transform.rotation = prevRotation;
    }
}
