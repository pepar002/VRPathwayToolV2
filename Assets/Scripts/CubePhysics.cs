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

    //private bool active = false;

    private float moveMultiplier = 10.0f;

    // stores the transform information before current update frame
    private Vector3 prevPos;
    private Quaternion prevRotation;
    private Vector3 prevScale;


    // stores origin scale for cube
    private Vector3 originScale;

    public bool Rotate { get => rotate; set { rotate = value; ResetRotation(); } }

    public bool ChangeGraphScale { get => scale; set { scale = value;  ResetScale(); } }

    public bool Movement { get => move; set { move = value; ResetPosition(); } }

    private void Start()
    {
        // store initial scale
        originScale = cube.transform.localScale;
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
            
            if (prevRotation != cube.transform.rotation)
            {
                graph.transform.rotation = cube.transform.rotation;
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


    // reset scale to origin size
    private void ResetScale()
    {
        Debug.Log("Scale : " + originScale);
        cube.transform.localScale = originScale;
        prevScale = originScale;
    }

    private void ResetRotation()
    {
        prevRotation = cube.transform.rotation;
    }

    private void ResetPosition()
    {
        prevPos = cube.transform.position;
    }

    private void GetPreviousTransform()
    {
        prevPos = cube.transform.position;
        prevRotation = cube.transform.rotation;
        prevScale = cube.transform.localScale;
    }
}
