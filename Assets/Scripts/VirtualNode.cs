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

        private bool axis = false;
        private List<GameObject> visualizations;

        private MeshRenderer mesh;
        public MeshRenderer outline;

        public NodeMenu nodeMenu;
        public Transform spLocation;
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
            /*foreach(GameObject g in menu.GetComponentsInChildren<GameObject>())
            {
                if(g.name == "ScatterPlot")
                {
                    spLocation = g.transform;
                }
            }*/
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
                        axis = ScatterPlotSceneManager.Instance.SpawnGraph(transform, transform.position, spID);
                        nodeMenu.transform.position = transform.position;
                        Vector3 v = nodeMenu.transform.position - headCamera.transform.position;
                        Quaternion q = Quaternion.LookRotation(v);
                        nodeMenu.transform.rotation = q;

                        nodeMenu.gameObject.SetActive(true);
                        //NodeMenu menu = Instantiate(nodeMenu);
                        //menu.transform.position = transform.position;
                        //menu.Name.text = name;
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
                    nodeMenu.transform.position = transform.position;
                    Vector3 v = nodeMenu.transform.position - headCamera.transform.position;
                    Quaternion q = Quaternion.LookRotation(v);
                    nodeMenu.transform.rotation = q;
                    nodeMenu.gameObject.SetActive(true);


                    /*axis.SetActive(true);
                    foreach (GameObject v in visualizations)
                    {
                        v.SetActive(true);
                    }*/
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