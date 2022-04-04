using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysInfo : MonoBehaviour
{
    [SerializeField]
    private string Dataset = "";
    [SerializeField]
    private string EdgeRendering = "";
    [SerializeField]
    private int GraphSize = 0;
    [SerializeField]
    private int EdgeSize = 0;
    [SerializeField]
    private int Triangles = 0;
    [SerializeField]
    private int MaxDegree = 0;
    [SerializeField]
    private Vector3 eulerAngUpdate;

    public void construct(string dataset, string edgeRendering, int graphSize, int edgeSize, int triangles, int maxDegree)
    {
        this.Dataset = dataset;
        this.EdgeRendering = edgeRendering;
        this.GraphSize = graphSize;
        this.EdgeSize = edgeSize;
        this.Triangles = triangles;
        this.MaxDegree = maxDegree;
    }

    public string dataset {
        get { return Dataset; }
        set { Dataset = value; }
    }

    public string edgerendering {
        get { return EdgeRendering; }
        set { EdgeRendering = value; }
    }

    public int graphsize {
        get { return GraphSize; }
        set { GraphSize = value; }
    }

    public int edgesize {
        get { return EdgeSize; }
        set { EdgeSize = value; }
    }

    public int triangles {
        get { return Triangles; }
        set { Triangles = value; }
    }

    public int maxdegree {
        get { return MaxDegree; }
        set { MaxDegree = value; }
    }

    void OnValidate()
    {
        //  transform.localEulerAngles = eulerAngUpdate;


    }

}
