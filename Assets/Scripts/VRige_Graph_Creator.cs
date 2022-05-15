using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

// Author: Adam Drogemuller
namespace VRige
{
    public class VRige_Graph_Creator : MonoBehaviour
    {

        [Header("Input Dataset to Create Graph")]
        private string graphDataset;
        public TextAsset xmlDataset;
        public TextAsset key;
        public EdgeCreator edgeCreator;

        // 3D coordinates
        public Vector3[,,] gridPositions3D;
        public bool[,,] gridPositionTaken3D;
        public int GraphSize = 0;
        private ArrayList nodes;
        public GameObject defaultNode;
        public GameObject origin;
        public GameObject label;
        [Header("Colour of Nodes")]
        public Color orthologColor;
        public Color compoundColor;
        public bool colorGraph = false;
        [Header("Scale of graph")]
        [Tooltip("Low - nodes are further, High - nodes are closer")]
        public float graphScale = 1;
        [Header("Scale of nodes")]
        public float scale = 1;
        public float gridGap = 0.1f;
        [Header("Spread of Graph")]
        [Range(0, 50)]
        public float area = 50f;
        public float t = 1.0f;
        [Range(0, 100)]
        public float smoothTime = 1f;
        private float v2d = 1;

        private bool flag = false;
        private List<DataNode> dataNodes;

        // Use this for initialization
        void Start()
        {
            //edgeCreator = transform.GetComponent<EdgeCreator>();
            GenerateGraph(xmlDataset, key);
            VRigeEventManager.PressPalmPyruvate += GenerateGraph;
            VRigeEventManager.PressPalmGlycolysis += GenerateGraph;

            VRigeEventManager.SliderMoveX += MoveGraphX;
        }
        // Update is called once per frame
        void Update()
        {
            // if t > threshold, apply force-directed layout
            if (t > 0.95)
            {
                flag = false;
                float k = Mathf.Sqrt(area);
                //Action<float> Fr = (float z) => {
                //    return k * k / z;
                //};

                // calculate repulsive forces
                foreach (VirtualNode v in nodes)
                {
                    v.disp = Vector3.zero;

                    foreach (VirtualNode u in nodes)
                    {
                        if (v != u)
                        {
                            Vector3 delta = v.transform.position - u.transform.position;

                            float Fr = k * k / delta.magnitude * 0.1f;
                            v.disp = (v.disp + (delta.normalized) * Fr) / graphScale;
                        }
                    }
                }

                // calculate attractive forces
                foreach (VirtualNode v in nodes)
                {
                    foreach (VirtualNode urb in v.neighbors)
                    {
                        var u = urb;
                        var delta = v.transform.position - u.transform.position;

                        float Fa = delta.magnitude * delta.magnitude / k;
                        v.disp = (v.disp - delta.normalized * Fa) / graphScale;

                    }
                }

                foreach (VirtualNode n in nodes)
                {
                    Vector3 movement = Vector3.Lerp(n.transform.position, n.transform.position + (n.disp.normalized) * Mathf.Min(n.disp.magnitude, t), smoothTime * Time.deltaTime);
                    n.transform.position = new Vector3((movement.x * v2d), movement.y, movement.z);
                }

                t = t * 0.999f;

            }
            else
            {
                if (flag == false)
                {
                    gameObject.transform.position = origin.transform.position;
                    generateLabels();
                    hideGraph(false);
                    RealignGraph();
                    flag = true;
                }
            }

        }

        private void RealignGraph()
        {

            foreach (VirtualNode node in nodes)
            {
                Vector3 position = node.transform.position;
                Vector3 newPos = new Vector3(position.x, position.y, position.z + 5);


                //Vector3 half = new Vector3(opposite.x + opposite.x / 2, opposite.y + opposite.y / 2, opposite.z + opposite.z / 2);
                node.transform.position = newPos;
            }
        }
        public void GenerateGraph(TextAsset xml, TextAsset key)
        {
            t = 1.0f;
            xmlDataset = xml;
            this.key = key;
            ExtractDataset();
            WriteDataset();
            edgeCreator = FindObjectOfType<EdgeCreator>();
            UndirectedGraph();
        }
        public void WriteDataset()
        {
            string dataset = "";
            foreach (DataNode node in dataNodes)
            {
                string rList = "";
                if (node.SubReactions.Count > 0)
                {
                    foreach (int sub in node.SubReactions)
                    {
                        dataset += $"{getDataNodeFromID(sub).DisplayName} {node.DisplayName}\n";
                    }
                    
                }
                if (node.ProdReactions.Count > 0)
                {
                    foreach (int prod in node.ProdReactions)
                    {
                        rList += $"{getDataNodeFromID(prod).DisplayName} ";
                    }
                    dataset += $"{node.DisplayName} {rList}\n";
                }
            }
            graphDataset = dataset;
        }

        private void ExtractDataset()
        {
            if (xmlDataset != null)
            {
                dataNodes = new List<DataNode>();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlDataset.text);
                XmlNodeList nodeList = doc.SelectNodes("/pathway/entry");
                foreach(XmlNode node in nodeList)
                {
                    if(node.Attributes["type"].Value != "map")
                    {
                        int id = int.Parse(node.Attributes["id"].Value);
                        string type = node.Attributes["type"].Value;
                        String[] l = node.Attributes["name"].Value.Split(':',' ');
                        string name = l[1];
                        DataNode dNode = new DataNode(id, name, type);
                        dataNodes.Add(dNode);
                    }

                }
                String keyData = key.text;
                keyData = keyData.Replace("  	", ";");
                String[] lines = keyData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string s in lines)
                {
                    string[] keyItems = s.Split(';');
                    //Debug.LogError(keyItems[0]);
                    foreach(DataNode node in dataNodes)
                    {
                        if(node.Name == keyItems[0])
                        {
                            if (keyItems[1].Contains("EC:")){
                                int start = keyItems[1].IndexOf("EC:") + 3;
                                int end = keyItems[1].Length-1;
                                node.DisplayName = keyItems[1].Replace(' ', '_').Substring(start, end-start);
                            }
                            else
                            {
                                node.DisplayName = keyItems[1].Replace(' ', '_');
                            }
                            
                        }
                    }

                }

                XmlNodeList nodeRelationList = doc.SelectNodes("/pathway/relation");
                foreach (XmlNode node in nodeRelationList)
                {
                    int id1 = int.Parse(node.Attributes["entry1"].Value);
                    int id2 = int.Parse(node.Attributes["entry2"].Value);
                    foreach(DataNode dNode in dataNodes)
                    {
                        if(dNode.Id == id1)
                        {
                            foreach(DataNode dNode2 in dataNodes)
                            {
                                if(dNode2.Id == id2)
                                {
                                    dNode.Relations.Add(dNode2.Id);
                                }
                            }
                        }
                    }
                }
                XmlNodeList nodeReactionList = doc.SelectNodes("/pathway/reaction");
                foreach (XmlNode node in nodeReactionList)
                {
                    int parent = int.Parse(node.Attributes["id"].Value);
                    DataNode target = getDataNodeFromID(parent);
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if(node.Attributes["type"].Value == "reversible")
                        {
                            target.ProdReactions.Add(int.Parse(child.Attributes["id"].Value));
                        }
                        else
                        {
                            if (child.Name == "substrate")
                            {
                                target.SubReactions.Add(int.Parse(child.Attributes["id"].Value));
                            }
                            else if (child.Name == "product")
                            {
                                target.ProdReactions.Add(int.Parse(child.Attributes["id"].Value));
                            }
                        }
                    }
                }
                //string list = "";
               /* foreach (DataNode node in dataNodes)
                {
                    string rList = "";
                    foreach (int relation in node.Relations)
                    {
                        rList = rList + $"          >{getDataNodeFromID(relation).DisplayName}\n";
                    }
                    foreach (int sub in node.SubReactions)
                    {
                        rList = rList + $"          -{getDataNodeFromID(sub).DisplayName}\n";
                    }
                    foreach (int prod in node.ProdReactions)
                    {
                        rList = rList + $"          +{getDataNodeFromID(prod).DisplayName}\n";
                    }
                    Debug.Log($"({node.Id}) { node.Name}: { node.DisplayName}\n" + rList);
                }*/
            }
        }

        private DataNode getDataNodeFromID(int id)
        {
            foreach(DataNode node in dataNodes)
            {
                if(node.Id == id){
                    return node;
                }
                
            }
            return null;
        }
        public void MoveGraphX(float f)
        {
            gameObject.transform.position = new Vector3(f, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        public void MoveGraphY(float f)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, f, gameObject.transform.position.z);
        }
        public void MoveGraphZ(float f)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, f);
        }
        public void ScaleGraph(float f)
        {
            gameObject.transform.localScale = new Vector3(f, f, f);
            //gameObject.transform.position = origin.transform.position;
        }

        private void generateLabels()
        {
            foreach(VirtualNode node in nodes)
            {
                GameObject l = Instantiate(label, node.transform);
                l.GetComponentInChildren<TextMesh>().text = node.name;
            }
        }

        // 3D Population
        private void populateGridPositions3D(int size)
        {
            Vector3 start = new Vector3(0, 0, 0);
            gridPositions3D = new Vector3[size, size, size];
            gridPositionTaken3D = new bool[size, size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        gridPositions3D[i, j, k] = new Vector3(0, 0, 0) + new Vector3(gridGap * k, gridGap * i, -gridGap * j);
                        gridPositionTaken3D[i, j, k] = false;
                    }
                }
            }
        }

        private void hideGraph(bool hide)
        {
            if (hide)
            {
                /*foreach (VirtualNode n in nodes)
                {
                    n.GetComponent<Renderer>().enabled = false;
                }
                foreach(GameObject edge in edgeCreator.CylEdges)
                {
                    edge.GetComponent<Renderer>().enabled = false;
                }*/
            }
            else
            {
                /*foreach (VirtualNode n in nodes)
                {
                    n.GetComponent<Renderer>().enabled = true;
                }
                foreach (GameObject edge in edgeCreator.CylEdges)
                {
                    edge.GetComponent<Renderer>().enabled = true;
                }*/
            }
            
        }

        // creates undirected graph
        private void UndirectedGraph()
        {
            if (graphDataset != null)
            {
                if(nodes != null)
                {
                    foreach(VirtualNode n in nodes)
                    {
                        Destroy(n.gameObject);
                    }
                    edgeCreator.deleteEdges();
                }
                nodes = new ArrayList();
                //String allData = graphDataset.text;
                //string allData = File.ReadAllText(Application.dataPath + "/Datasets/" + graphDataset.name + ".txt");
                //allData.Replace("\r", "\n");
                String[] lines = graphDataset.Split("\n"[0]);
                populateGridPositions3D(lines.Length * 2);
                // read through each line
                foreach (String s in lines)
                {
                    Color col = compoundColor;
                    if (colorGraph == true)
                    {
                        col = UnityEngine.Random.ColorHSV(0.1f, 0.8f, 0.7f, 1f, 0.5f, 1f);
                    }

                    // split line on empty character
                    String[] subLines = s.Split(new char[0]);

                    // line node
                    VirtualNode lineNode = null;

                    // check if first node on line has been created
                    if (checkIfCreated(subLines[0]) == false && subLines[0] != String.Empty)
                    {
                        int x = UnityEngine.Random.Range(0, lines.Length * 2);
                        int y = UnityEngine.Random.Range(0, lines.Length * 2);
                        int z = UnityEngine.Random.Range(0, lines.Length * 2);
                        while (gridPositionTaken3D[x, y, z] == true)
                        {
                            x = UnityEngine.Random.Range(0, lines.Length * 2);
                            y = UnityEngine.Random.Range(0, lines.Length * 2);
                            z = UnityEngine.Random.Range(0, lines.Length * 2);
                        }
                        // initialize pos
                        Vector3 pos = Vector3.zero;
                        pos = gridPositions3D[x, y, z];
                        gridPositionTaken3D[x, y, z] = true;

                        GameObject n = Instantiate(defaultNode, pos, transform.rotation);
                        n.name = subLines[0];

                        VirtualNode getNode = n.GetComponent<VirtualNode>();
                        getNode.setID(subLines[0]);
                        foreach (DataNode dNode in dataNodes)
                        {
                            if(getNode.getID() == dNode.DisplayName)
                            {
                                getNode.DataID = dNode.Id;
                                if(dNode.Type == DataNode.EnumType.ORTHOLOG)
                                {
                                    n.GetComponent<MeshRenderer>().material.color = orthologColor;
                                    getNode.DefaultColor = orthologColor;
                                }
                                else if(dNode.Type == DataNode.EnumType.COMPOUND)
                                {
                                    n.GetComponent<MeshRenderer>().material.color = compoundColor;
                                    getNode.DefaultColor = compoundColor;
                                }
                            }
                        }

                        lineNode = getNode;
                        lineNode.setCircle(true);
                        lineNode.transform.parent = this.transform;
                        nodes.Add(lineNode);
                        n.transform.localScale = new Vector3(scale, scale, scale);

                    }
                    else
                    {
                        lineNode = grabNode(subLines[0]);
                    }

                    int i = 0;
                    // go through and create all adjacencies 
                    foreach (String sub_s in subLines)
                    {
                        if (checkIfCreated(sub_s) == false && sub_s != string.Empty && i != 0)
                        {
                            int x = UnityEngine.Random.Range(0, lines.Length);
                            int y = UnityEngine.Random.Range(0, lines.Length);
                            int z = UnityEngine.Random.Range(0, lines.Length);
                            while (gridPositionTaken3D[x, y, z] == true)
                            {
                                x = UnityEngine.Random.Range(0, lines.Length);
                                y = UnityEngine.Random.Range(0, lines.Length);
                                z = UnityEngine.Random.Range(0, lines.Length);
                            }
                            // initialize pos
                            Vector3 pos = Vector3.zero;
                            pos = gridPositions3D[x, y, z];

                            GameObject n = Instantiate(defaultNode, pos, transform.rotation);
                            n.GetComponent<MeshRenderer>().material.color = col;
                            n.name = sub_s;
                            //n.GetComponent<MeshRenderer>().enabled = false;
                            VirtualNode getNode = n.GetComponent<VirtualNode>();
                            getNode.setID(sub_s);
                            foreach (DataNode dNode in dataNodes)
                            {
                                if (getNode.getID() == dNode.DisplayName)
                                {
                                    getNode.DataID = dNode.Id;
                                    if (dNode.Type == DataNode.EnumType.ORTHOLOG)
                                    {
                                        n.GetComponent<MeshRenderer>().material.color = orthologColor;
                                        getNode.DefaultColor = orthologColor;
                                    }
                                    else if (dNode.Type == DataNode.EnumType.COMPOUND)
                                    {
                                        n.GetComponent<MeshRenderer>().material.color = compoundColor;
                                        getNode.DefaultColor = compoundColor;
                                    }
                                }
                            }
                            getNode.setNodes(getNode, lineNode);
                            lineNode.setNodes(lineNode, getNode);
                            getNode.transform.parent = this.transform;
                            Edge e = new Edge(lineNode.transform, n.transform, false);
                            edgeCreator.addEdges(e);
                            nodes.Add(getNode);

                            n.transform.localScale = new Vector3(scale, scale, scale);

                        }
                        else
                        {
                            if (checkIfCreated(sub_s) == true && i != 0 && subLines[0] != String.Empty)
                            {
                                VirtualNode n = grabNode(sub_s);
                                n.setNodes(n, lineNode);
                                lineNode.setNodes(lineNode, n);
                                Edge e = new Edge(lineNode.transform, n.transform, false);
                                edgeCreator.addEdges(e);
                              //  print("added edge");
                                n.transform.localScale = new Vector3(scale, scale, scale);

                            }
                        }
                        i++;
                    }
                }

                GraphSize = nodes.Count;
                hideGraph(true);
            }
        }

        // check if the node has already been created
        private bool checkIfCreated(String ID)
        {
            if (nodes.Count > 0)
            {
                foreach (VirtualNode n in nodes)
                {
                    if (n.getID().Equals(ID))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // return node
        private VirtualNode grabNode(String ID)
        {
            if (nodes.Count > 0)
            {
                foreach (VirtualNode n in nodes)
                {
                    if (n.getID().Equals(ID))
                    {
                        return n;
                    }
                }
            }
            return null;
        }

    }
}