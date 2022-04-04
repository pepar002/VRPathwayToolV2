using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Adam Drogemuller
namespace VRige
{
    public class VRige_Graph_Creator : MonoBehaviour
    {

        [Header("Input Dataset to Create Graph")]
        public TextAsset graphDataset;
        public EdgeCreator edgeCreator;

        // 3D coordinates
        public Vector3[,,] gridPositions3D;
        public bool[,,] gridPositionTaken3D;
        public int GraphSize = 0;
        private ArrayList nodes;
        public GameObject defaultNode;
        [Header("Colour of Nodes")]
        public Color defaultColor;
        public bool colorGraph = false;
        [Header("Scale of graph")]
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

        // Use this for initialization
        void Start()
        {
            nodes = new ArrayList();
            edgeCreator = FindObjectOfType<EdgeCreator>();
            UndirectedGraph();
        }

        // Update is called once per frame
        void Update()
        {

            // if t > threshold, apply force-directed layout
            if (t > 0.95)
            {
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
                String allData = graphDataset.text;
                allData.Replace("\r", "\n");
                String[] lines = allData.Split("\n"[0]);
                populateGridPositions3D(lines.Length * 2);
                // read through each line
                foreach (String s in lines)
                {
                    Color col = defaultColor;
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
                        n.GetComponent<MeshRenderer>().material.color = col;
                        n.name = subLines[0];
                        //n.GetComponent<MeshRenderer>().enabled = false;
                        VirtualNode getNode = n.GetComponent<VirtualNode>();
                        getNode.setID(subLines[0]);
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
                            getNode.setNodes(getNode, lineNode);
                            lineNode.setNodes(lineNode, getNode);
                            getNode.transform.parent = this.transform;
                            Edge e = new Edge(lineNode.transform, n.transform, false);
                            edgeCreator.addEdges(e);
                            nodes.Add(getNode);
                            print("added edge");

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

    }
}