﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using UnityEngine.UI;

// Author: Adam Drogemuller
namespace VRige
{
    public class VRige_Graph_Creator : MonoBehaviour
    {

        [Header("Input Dataset to Create Graph")]
        private string graphDataset;
        public XmlDocument xmlDataset;
        public TextAsset key;
        public EdgeCreator edgeCreator;
        public Transform edgeParent;
        public NodeMenu nodeMenu;
        public Camera hmdCamera;

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
        public float graphScale;
        [Header("Scale of nodes")]
        public float scale;
        public float gridGap;
        [Header("Spread of Graph")]
        [Range(0, 50)]
        public float area;
        public float t;
        [Range(0, 100)]
        public float smoothTime;
        private float v2d = 1;

        private float testTime = 0f;
        private bool flag = true;
        private List<DataNode> dataNodes;

        private bool nodesEnabled = true;
        private bool isGenerated = false;

        public enum PathwayType { PYRUVATE, CITRATE }

        // Use this for initialization
        void Start()
        {
            DataExtrator.Instance.LoadPyruvatePathway();
            //generate the graph from the datasets
           // GenerateGraph(DataExtrator.Instance.PathwayXmls["ko00620"]);
            //experimental--palm ui events
            VRigeEventManager.PressPalmPyruvate += GenerateGraph;
            VRigeEventManager.PressPalmGlycolysis += GenerateGraph;

            VRigeEventManager.SliderMoveX += MoveGraphX;
        }
        // Update is called once per frame
        void Update()
        {
            // if t > threshold, apply force-directed layout
            if (isGenerated && t > 0.95)
            {
                Debug.Log("Loading... seconds:" + testTime);
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
                testTime = testTime + Time.deltaTime;
                t = t * 0.999f;

            }
            else
            {
                if (flag == false)
                {
                    //after generated reposition, generate labels
                    gameObject.transform.position = origin.transform.position;
                    generateUI();
                    RealignGraph();
                    Debug.LogAssertion("Loaded Successfully in " + testTime + " seconds");
                    flag = true;
                }
            }

        }

        private void resetGraph()
        {
            flag = true;
            nodesEnabled = false;
            isGenerated = false;
            GraphSize = 0;
            t = 1f;
            testTime = 0;
            Debug.LogError(graphScale + ", " + scale + ", " + gridGap + ", " + area + ", " + t + ", " + smoothTime);
        }

        public void hideNodes(bool hide)
        {
            if (hide && nodesEnabled)
            {
                foreach(VirtualNode node in nodes)
                {
                    node.gameObject.SetActive(false);
                }
                edgeCreator.hideEdges(true);
                nodesEnabled = false;
                Debug.Log("Hiding Nodes");
            }
            else if(!hide && !nodesEnabled)
            {
                foreach (VirtualNode node in nodes)
                {
                    node.gameObject.SetActive(true);
                }
                edgeCreator.hideEdges(false);
                nodesEnabled = true;
                Debug.Log("Showing Nodes");

            }
        }

        //realign position and rescale for correct vr size
        private void RealignGraph()
        {

            foreach (VirtualNode node in nodes)
            {
                Vector3 position = node.transform.position;
                Vector3 newPos = new Vector3(position.x, position.y, position.z + 5);
                node.transform.position = newPos;
            }
            this.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }

        //main control method that will generate the graph based in the selected xml dataset
        public void GenerateGraph(XmlDocument xml)
        {
            resetGraph();
            Debug.Log("Generating new graph");
            xmlDataset = xml;
            //this.key = key;
            
            ExtractDataset();
            WriteDataset();
            edgeCreator = FindObjectOfType<EdgeCreator>();
            UndirectedGraph();
            AssignScatterplots();
            SendNodeApiRequests();
            isGenerated = true;
            Debug.Log("Generated new graph, now loading graph " + t);
        }

        public void GenerateGraph(string pathwayID)
        {
            // Stop all running co routines in this script before loading another pathway
            StopAllCoroutines();
            // Load a pathway according to given id
            GenerateGraph(DataExtrator.Instance.PathwayXmls[pathwayID]);
        }


        //write the file string that generates the correct node generations based on node data
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

        //assign correct scatterplot to correct node
        public void AssignScatterplots()
        {
            foreach(DataNode node in dataNodes)
            {
                int spID = ScatterPlotSceneManager.Instance.assignNodeId(node.Entry);
                Debug.LogError("spID: " + spID);
                if (getVirtualNode(node.Id) != null)
                {
                    getVirtualNode(node.Id).spID = spID;
                    getVirtualNode(node.Id).name = node.Entry;
                }
            }
        }
        //get specific node from id
        public VirtualNode getVirtualNode(int dataNodeid)
        {
            foreach (VirtualNode node in nodes)
            {
                if(node.DataID == dataNodeid)
                {
                    return node;
                }
            }
            return null;
        }

        //this creates data nodes based on the specific xml data inputted.
        //As the xml data created by Kegg is generated in a certain way,
        //this extracts that data into individual node data objects
        private void ExtractDataset()
        {
            if (xmlDataset != null)
            {
                dataNodes = new List<DataNode>();
                // uses the pathway xml loaded from data extractor class
                XmlDocument doc = xmlDataset;
                //doc.LoadXml(xmlDataset.text);

                XmlNodeList nodeList = doc.SelectNodes("/pathway/entry");
                //Debug.Log("In Graph Creator: " + nodeList.Count);
                foreach (XmlNode node in nodeList)
                {
                    if(node.Attributes["type"].Value != "map")
                    {
                        int id = int.Parse(node.Attributes["id"].Value);
                        string type = node.Attributes["type"].Value;
                        String[] l = node.Attributes["name"].Value.Split(':',' ');
                        string entryId = "";
                        if (l.Length > 1)
                        {
                            entryId = l[1]; // TO DO not all attributes have the same name format
                        }
                        else {
                            entryId = l[0]; // Some entry name for some pathways are "undefined"
                        }
                        DataNode dNode = new DataNode(id, entryId, type);
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
                        if(node.Entry == keyItems[0])
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
            }
        }
        //get data node from specific id
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
        // used by ui to scale and translate graph
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
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + f, gameObject.transform.localScale.y + f, gameObject.transform.localScale.z + f);
            //gameObject.transform.position = origin.transform.position;
        }

        //generates the labels for each node
        private void generateUI()
        {
            foreach(VirtualNode node in nodes)
            {
                GameObject l = Instantiate(label, node.transform);
                l.GetComponentInChildren<TMPro.TMP_Text>().text = getDataNodeFromID(node.DataID).DisplayName;
                //l.GetComponentInChildren<Canvas>().worldCamera = hmdCamera;
                NodeMenu menu = Instantiate(nodeMenu);
                menu.transform.position = node.transform.position;
                node.nodeMenu = menu;
                menu.Name.text = node.name;
                menu.gameObject.SetActive(false);
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


        // TESTING send api requests to get node information
        // need to send out a few requests at a time (or else forbidden)
        private void SendNodeApiRequests()
        {
            StartCoroutine(StartMultipleRoutines());
           
        }

        // starts multiple coroutines to send different api requests in sequence (sync)
        private IEnumerator StartMultipleRoutines() {
            //Execute coroutines for getting the node data synchronously while async to the application execution
            foreach (DataNode n in dataNodes) {
                // send http request to get data node information
                VirtualNode vNode = getVirtualNode(n.Id);
                if (vNode != null) {

                    string url = DataExtrator.URI + "/get/" + n.Entry;
                    yield return StartCoroutine(DataExtrator.Instance.SendRequest(url, vNode.GetInfo));

                    // send http request to get compound image (images from API are in GIF format - not supported by unitywebrequest)
                    /*                if (n.Type == DataNode.EnumType.COMPOUND)
                                    {
                                        string imageUrl = DataExtrator.URI + "/get/" + n.Entry + "/image";
                                        yield return StartCoroutine(DataExtrator.Instance.SendImageRequest(imageUrl, vNode.GetImage));
                                    }*/
                }
                vNode = null;
            }

        }

    }
}