using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    [SerializeField]
    private Material material;
    [SerializeField]
    private Vector3 scale;

    public int touchCount { get; set; }

    public int pokeCount { get; set; }

    public int laserCount { get; set; }

    void Awake()
    {

        material = gameObject.GetComponent<Renderer>().material;
        scale = gameObject.transform.localScale;
        touchCount = 0;
        laserCount = 0;
        pokeCount = 0;
    }

    public Vector3 GetScale()
    {

        return scale;
    }

    public Material GetMaterial()
    {

        return material;
    }


    // Reset the node back to its original colour
    public void Reset()
    {

        gameObject.GetComponent<Renderer>().material = GetMaterial();
    }
}
