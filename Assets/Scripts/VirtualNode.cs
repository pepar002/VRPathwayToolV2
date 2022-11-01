using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction.Grab;
using UnityEngine.Events;

namespace VRige
{
    public class VirtualNode : MonoBehaviour
    {

        public string ID = "-999";
        private int dataID = 0;
        public int spID = -1;
        public bool activeNode = false;
        public string ttest = "-";

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
        private Vector3 storedPosition;

        public int addDeg = 0;
        private bool grabbed = false;
        private bool rotate = false;
        public bool rotateToCenter = false;
        private Color defaultColor;

        private MeshRenderer mesh;
        public MeshRenderer outline;

        private bool axis = false;
        private List<SAxis> spAxis;

        private Button pinNode;
        private Button unPinNode;
        private bool pinned;

        public NodeMenu nodeMenu;
        public Transform spLocation;
        public GameObject spParent;
        public GameObject headCamera;


        public bool containsData;

        private bool menuUpdated = false;  // TO DO testing

        private List<string> names;
        private string formula;  // only for compounds
        private string symbol;  // only for enzymes/ortholog



        public int DataID { get => dataID; set => dataID = value; }
        public Color DefaultColor { get => defaultColor; set => defaultColor = value; }
        public List<string> Names { get => names; }

        public string Formula { get => formula; }

        // initialization
        private void Awake()
        {
            neighbors = new ArrayList();
            names = new List<string>();
            mesh = GetComponent<MeshRenderer>();
            spID = -1;
            storedPosition = transform.localPosition;

            GameObject menu = (GameObject)Resources.Load("NodeMenu");
            //nodeMenu = menu.GetComponent<NodeMenu>();
        }

        //make node face user and initalization
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

        // set neighbours for the node for use with edges
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
            if(transform.position != storedPosition)
            {
                EventManager.TriggerEvent("nodeMove", 0f);
                storedPosition = transform.position;
            }

        }
        //when a node is selected spawn the specific graph if there is one and enable the menu if it has data
        public void selectNode()
        {
            if (containsData)
            {
                if (!activeNode)
                {
                    activeNode = true;
                
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
                    foreach(Button b in nodeMenu.GetComponentsInChildren<Button>())
                    {
                        if(b.name == "PinNode")
                        {
                            pinNode = b;
                        }
                        if (b.name == "UnPinNode")
                        {
                            unPinNode = b;
                        }
                    }

                    nodeMenu.spID = spID;

                    /*if (!pinned) pinNode.onClick.AddListener(OnPinClick);
                    if(pinned) unPinNode.onClick.AddListener(OnPinClick);*/

                    spParent = new GameObject(gameObject.name + " sp");
                    spParent.transform.position = spLocation.position;
                    spAxis = ScatterPlotSceneManager.Instance.SpawnGraph(spLocation, spLocation.position, spID, this.name);

                    foreach(SAxis axis in spAxis)
                    {
                        axis.transform.parent = spParent.transform;
                    }

                    spParent.transform.rotation = nodeMenu.transform.rotation;
                    spParent.transform.rotation = Quaternion.Euler(nodeMenu.transform.eulerAngles.x, nodeMenu.transform.eulerAngles.y + 180, nodeMenu.transform.eulerAngles.z);

                    nodeMenu.gameObject.SetActive(true);

                    mesh.material.color = Color.green;
                }
                else
                {
                    deselectNode();
                }
            }
        }
        //closes the node menu and removes scatterplot instance
        public void deselectNode()
        {
            activeNode = false;
            mesh.material.color = defaultColor;

            foreach (SAxis axis in spAxis)
            {
                Destroy(axis.gameObject);

            }
            Destroy(spParent);
            nodeMenu.gameObject.SetActive(false);
        }

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

        // get information pertaining this node (names and symbol/formula)
        public void GetInfo(string response)
        {
            string[] lines = response.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            List<string> categories = new List<string> { "ENTRY", "NAME", "FORMULA", "COMMENT", "PATHWAY" };


            bool foundName = false;

            foreach (string line in lines)
            {
                string[] words = line.Trim(new char[] { ' ', ';' }).Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (words[0] == "NAME")
                {
                    foundName = true;
                    names.Add(string.Join(" ", words[1..]));
                    continue;
                }
                else if (categories.Contains(words[0]))
                    foundName = false;

                if (words[0] == "FORMULA" || words[0] == "SYMBOL")
                    formula = words[1].Trim(new[] { ' ', ',', ';', ':'});


                if (foundName)
                    names.Add(line.Trim(new char[] { ' ', ';' }));
            }


            // TO DO testing
            if (!menuUpdated && nodeMenu != null)
            {
                UpdateNodeMenu();
                menuUpdated = true;
            }
        }

        // UPDATE node menu with formula and descriptions
        public void UpdateNodeMenu() {
            if (formula != null && !string.IsNullOrEmpty(formula))
            {
                nodeMenu.Formula.text += ": " + formula;
            }

            if (names.Count > 0)
            {
                string description = string.Join("\n\t", names);

                nodeMenu.Description.text += "\n\t" + description;
            }
            nodeMenu.Ttest.text += ttest;
        }

    }
}