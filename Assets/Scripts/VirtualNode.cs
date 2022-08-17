using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRige
{
    public class VirtualNode : MonoBehaviour
    {

        public string ID = "-999";
        private int dataID = 0;
        public int spID = -1;
        public bool activeNode = false;

        public Vector3 disp = Vector3.zero;
        private Rigidbody a;
        public ArrayList neighbors;

        private float Length = 0.01f;
        private float EdgeLength;
        private float SpringK = 10.2f;
        private float forceApply = 100f;
        private float timer = 0;
        public float f_a = 0;
        public float neighborCount = 0;
        private bool circleNode = false;
        private bool look = false;
        private bool found = false;

        public int addDeg = 0;
        private bool grabbed = false;
        private bool rotate = false;
        public bool rotateToCenter = false;
        private Color defaultColor;

        private MeshRenderer mesh;
        public MeshRenderer outline;

        private bool axis = false;
        private List<GameObject> scatterplots;
        public NodeMenu nodeMenu;
        public Transform spLocation;
        public GameObject spParent;
        public GameObject headCamera;

        public bool containsData;

        public int DataID { get => dataID; set => dataID = value; }
        public Color DefaultColor { get => defaultColor; set => defaultColor = value; }


        // initialization
        private void Awake()
        {
            neighbors = new ArrayList();
            mesh = GetComponent<MeshRenderer>();
            spID = -1;

            GameObject menu = (GameObject)Resources.Load("NodeMenu");
            //nodeMenu = menu.GetComponent<NodeMenu>();
        }

        private void Start()
        {
            if (!headCamera)
            {
                headCamera = GameObject.Find("CenterEyeAnchor");
            }

            if (spID > 0)
            {
                containsData = true;
                outline.gameObject.SetActive(true);
            }
            else
            {
                containsData = false;
            }
        }

        // set neighbours
        public void setNodes(VirtualNode a, VirtualNode b)
        {
            if (neighbors.Contains(b) == false)
            {
                neighbors.Add(b);
                neighborCount++;
                if (b.GetComponent<VirtualNode>().neighbors.Contains(b) == false)
                {
                    b.GetComponent<VirtualNode>().neighbors.Add(a);
                    b.GetComponent<VirtualNode>().neighborCount++;
                }
            }
        }

        private void Update()
        {
            // forces nodes to move if NaN disp is calculated
            if (disp.x != disp.x)
            {
                float randX = Random.Range(0, 0.1f);
                float randY = Random.Range(0, 0.1f);
                float randZ = Random.Range(0, 0.1f);
                transform.position += new Vector3(randX, randY, randZ);
            }

            if (axis)
            {
/*                GameObject.Find("axis " + gameObject.name).transform.parent = spParent.transform;
                GameObject.Find("axis CONDITION-" + gameObject.name).transform.parent = spParent.transform;*/
            }

        }
        //when a node is selected spawn the specific graph if there is one
        public void selectNode()
        {
            if (!activeNode)
            {
                activeNode = true;
                if(!axis)
                {
                    if (containsData)
                    {
                        nodeMenu.transform.position = transform.position;

                        Vector3 v = nodeMenu.transform.position - headCamera.transform.position;
                        Quaternion q = Quaternion.LookRotation(v);
                        nodeMenu.transform.rotation = q;

                        foreach (Transform g in nodeMenu.GetComponentsInChildren<Transform>())
                        {
                            if (g.name == "spLocation")
                            {
                                spLocation = g;
                            }
                        }


                        spParent = new GameObject(gameObject.name + " sp");

                        

                        spParent.transform.position = spLocation.position;
                        axis = ScatterPlotSceneManager.Instance.SpawnGraph(spLocation, spLocation.position, spID, this.name);


                        GameObject.Find("axis " + gameObject.name).transform.parent = spParent.transform;
                        GameObject.Find("axis CONDITION-" + gameObject.name).transform.parent = spParent.transform;

                        Vector3 spV = spParent.transform.position - headCamera.transform.position;
                        Quaternion q2 = Quaternion.LookRotation(spV);
                        spParent.transform.rotation = q2;

                        nodeMenu.gameObject.SetActive(true);
                    }
                    
                    //axis.transform.localScale = new Vector3(axis.transform.localScale.x * 2, axis.transform.localScale.y * 2, axis.transform.localScale.z * 2);
                    //visualizations = new List<GameObject>();
/*                    foreach (Visualization v in axis.GetComponent<SAxis>().correspondingVisualizations())
                    {
                        visualizations.Add(v.gameObject);
                        v.transform.localScale = new Vector3(v.transform.localScale.x * 2, v.transform.localScale.y * 2, v.transform.localScale.z * 2);
                    }*/
                }
                else
                {
                    if (containsData)
                    {
                        nodeMenu.transform.position = transform.position;
                        Vector3 v = nodeMenu.transform.position - headCamera.transform.position;
                        Quaternion q = Quaternion.LookRotation(v);
                        nodeMenu.transform.rotation = q;

                        try
                        {
                            foreach (GameObject sp in scatterplots)
                            {
                                sp.SetActive(true);
                            }
                        }
                        catch
                        {}

                        nodeMenu.gameObject.SetActive(true);
                        spParent.SetActive(true);
                        /*axis.SetActive(true);
                        foreach (GameObject v in visualizations)
                        {
                            v.SetActive(true);
                        }*/
                    }
                }
                mesh.material.color = Color.green;
            }
            else
            {
/*                foreach (GameObject v in visualizations)
                {
                    v.SetActive(false);
                }*/

                activeNode = false;
                //axis.SetActive(false);
                mesh.material.color = defaultColor;
                /*GameObject.Find("axis " + gameObject.name).transform.parent = spParent.transform;
                GameObject.Find("axis CONDITION-" + gameObject.name).transform.parent = spParent.transform;*/
                try
                {
                    GameObject.Find("axis " + gameObject.name).transform.position = spLocation.transform.position;
                    GameObject.Find("axis CONDITION-" + gameObject.name).transform.position = spLocation.transform.position;
                }
                catch { }
                

                try
                {
                    scatterplots = new List<GameObject>();
                    scatterplots.Add(GameObject.Find("axis " + gameObject.name));
                    scatterplots.Add(GameObject.Find("axis CONDITION-" + gameObject.name));
                    scatterplots.Add(GameObject.Find("axis " + gameObject.name + " axis CONDITION-" + gameObject.name + " visualisation"));
                    scatterplots.Add(GameObject.Find("axis CONDITION visualisation"));
                    scatterplots.Add(GameObject.Find("axis " + gameObject.name + " visualisation"));

                    foreach(GameObject sp in scatterplots)
                    {
                        sp.SetActive(false);
                    }

                   /* GameObject.Find("axis " + gameObject.name).SetActive(false);
                    GameObject.Find("axis CONDITION visualisation").SetActive(false);
                    GameObject.Find("axis " + gameObject.name + " visualisation").SetActive(false);
                    GameObject.Find("axis CONDITION-" + gameObject.name).SetActive(false);
                    GameObject.Find("axis " + gameObject.name + " axis CONDITION-" + gameObject.name + " visualisation").SetActive(false);*/
                }
                catch {
                }
                try
                {
                    scatterplots.Add(GameObject.Find("axis CONDITION-" + gameObject.name + " visualisation"));
                    scatterplots.Add(GameObject.Find("axis " + gameObject.name + " visualisation"));

                    foreach (GameObject sp in scatterplots)
                    {
                        sp.SetActive(false);
                    }
                }
                catch { }
                //spParent.SetActive(false);
                nodeMenu.gameObject.SetActive(false);
            }
            
        }


        // return ID
        public string getID()
        {
            return ID;
        }

        // set ID
        public void setID(string ID)
        {
            this.ID = ID;
        }

        // set if circle
        public void setCircle(bool circle)
        {
            this.circleNode = true;
        }

        // check if circle
        public bool checkIfCircle()
        {
            if (neighborCount > 3)
            {
                return true;
            }
            return circleNode;
        }
    }
}