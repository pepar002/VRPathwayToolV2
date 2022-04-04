using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRige;

public class AutoRotate : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotate;

    [SerializeField]
    private VRige_Graph_Creator vrige;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (vrige.t < 0.95)
        {
            transform.Rotate(rotate * Time.deltaTime);
        }
    }
}
