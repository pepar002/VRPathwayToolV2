using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class DataNode
{

    /// <summary>
    /// stores the type of data node from the kegg database to be used with graph generation
    /// </summary>
    private int id;
    private string entry;
    private string displayName;
    private EnumType type;
    private List<int> relations;
    private List<int> subReactions;
    private List<int> prodReactions;

    private List<string> names;
    private string formula;  // only for compounds
    private string symbol;  // only for enzymes/ortholog

    public enum EnumType { ORTHOLOG, COMPOUND, NONE };

    public DataNode(int id, string entry, string type)
    {
        this.id = id;
        this.entry = entry;

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
        names = new List<string>();
    }

    public int Id { get => id; }
    public string Entry { get => entry;}
    public EnumType Type { get => type; }
    public string DisplayName { get => displayName; set => displayName = value; }
    public List<int> Relations { get => relations; set => relations = value; }
    public List<int> SubReactions { get => subReactions; set => subReactions = value; }
    public List<int> ProdReactions { get => prodReactions; set => prodReactions = value; }

    public List<string> Names { get => names; }

    public string Formula { get => formula; }

    public void GetInfo(string response) {
        string[] lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        List<string> categories = new List<string> { "ENTRY", "NAME", "FORMULA", "COMMENT", "PATHWAY" };


        bool name = false;

        foreach (string line in lines) {
            string[] words = line.Trim(new Char[] {' ',';'}).Split(" ");
            if (words[0] == "NAME") {
                name = true;
                names.Add(words[1].Trim());
                continue;
            } else if (categories.Contains(words[0]))
                name = false;

            if (words[0] == "FORMULA")
                formula = words[1];


            if (name)
                names.Add(line.Trim(new Char[] { ' ', ';' }));
        }

        if (entry == "C15973")
            Debug.Log(names.Count);
    }

    public void GetImage(Texture2D imageTexture) {
        Debug.Log(String.Format("got image texture for {0}", entry));
    }
}
