using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataNode
{
    private int id;
    private string name;
    private List<DataNode> relations;
   public DataNode(int id, string name)
    {
        this.id = id;
        this.name = name;
        relations = new List<DataNode>();
    }

    public List<DataNode> Relations { get => relations; set => relations = value; }
    public int Id { get => id; }
    public string Name { get => name;}
}
