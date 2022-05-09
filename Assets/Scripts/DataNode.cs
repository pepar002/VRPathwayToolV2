using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataNode
{
    private int id;
    private string name;
    private string displayName;
    private List<int> relations;
    private List<int> subReactions;
    private List<int> prodReactions;
    public DataNode(int id, string name)
    {
        this.id = id;
        this.name = name;
        relations = new List<int>();
        SubReactions = new List<int>();
        ProdReactions = new List<int>();
    }

    public List<int> Relations { get => relations; set => relations = value; }
    public int Id { get => id; }
    public string Name { get => name;}
    public string DisplayName { get => displayName; set => displayName = value; }
    public List<int> SubReactions { get => subReactions; set => subReactions = value; }
    public List<int> ProdReactions { get => prodReactions; set => prodReactions = value; }
}
