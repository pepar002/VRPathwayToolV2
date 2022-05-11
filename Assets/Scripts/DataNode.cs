using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataNode
{
    private int id;
    private string name;
    private string displayName;
    private EnumType type;
    private List<int> relations;
    private List<int> subReactions;
    private List<int> prodReactions;
    public enum EnumType { ORTHOLOG, COMPOUND, NONE };

    public DataNode(int id, string name, string type)
    {
        this.id = id;
        this.name = name;

        if(type == "ortholog")
        {
            this.type = EnumType.ORTHOLOG;
        }
        else if(type == "compound")
        {
            this.type = EnumType.COMPOUND;
        }
        else
        {
            this.type = EnumType.NONE;
        }

        relations = new List<int>();
        SubReactions = new List<int>();
        ProdReactions = new List<int>();
    }

    public int Id { get => id; }
    public string Name { get => name;}
    public EnumType Type { get => type; }
    public string DisplayName { get => displayName; set => displayName = value; }
    public List<int> Relations { get => relations; set => relations = value; }
    public List<int> SubReactions { get => subReactions; set => subReactions = value; }
    public List<int> ProdReactions { get => prodReactions; set => prodReactions = value; }
}
