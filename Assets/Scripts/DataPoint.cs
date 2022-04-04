using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoint : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private string data;

    public void constructor(string id, string data)
    {
        this.id = id;
        this.data = data;
    }
   
    public void highlight()
    {

    }
    
    public string ID()
    {
        return id;
    }
}
