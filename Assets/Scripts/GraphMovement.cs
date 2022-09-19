using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphMovement : MonoBehaviour
{
    public GameObject graph;
    public GameObject cube;
    private bool rotate = false;
    private bool scale = false;
    private bool active = false;

    private Vector3 cubeScale;
    private Vector3 beforePos;
    public bool Rotate { get => rotate; set { rotate = value;  } }

    public bool Active { get => active; set { Debug.Log("Cube is now active"); active = value; GetCurrentCubePosition(); } }
    public bool ChangeGraphScale { get => scale; set { scale = value;  GetCurrentCubeScale(); } }

 
    void Update()
    {
        UpdateGraphState();
    }

    // Various settings to allow the graph to mimic the cube's movement, rotation and scale
    public void UpdateGraphState() {
        if (active)
        {
            // rotate the object
            if (rotate)
            {
                graph.transform.rotation = cube.transform.rotation;
            }
            else
            {
                // get the displacement of the cube
                var cubeDisplacement = cube.transform.position - beforePos;

                beforePos = cube.transform.position;

                // Move the graph with respect to the cube displacement
                graph.GetComponent<VRige.VRige_Graph_Creator>().MoveGraphX(graph.transform.position.x + cubeDisplacement.x * 10.0f);
                graph.GetComponent<VRige.VRige_Graph_Creator>().MoveGraphY(graph.transform.position.y + cubeDisplacement.y * 10.0f);
                graph.GetComponent<VRige.VRige_Graph_Creator>().MoveGraphZ(graph.transform.position.z + cubeDisplacement.z * 10.0f);

            }

            // TO DO add movement settings (allowing multiple settings to exists at once

            // Scale the graph with respect to the scale change on the cube
            if (scale) {
                var changeInScale = cube.transform.localScale.x - cubeScale.x;
                cubeScale = cube.transform.localScale;
                graph.GetComponent<VRige.VRige_Graph_Creator>().ScaleGraph(changeInScale);
            }
        }
    }


    public void ScaleGraph() { 
        //
    }

    // stores the current position of the cube
    private void GetCurrentCubePosition() {
        beforePos = cube.transform.position;
    }

    // store the current scale of the cube
    private void GetCurrentCubeScale()
    {
        cubeScale = cube.transform.localScale;
    }
}
