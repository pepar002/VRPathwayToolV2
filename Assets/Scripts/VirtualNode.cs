using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRige
{
    public class VirtualNode : MonoBehaviour
    {

        public string ID = "-999";
        private int dataID = 0;
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

        public int DataID { get => dataID; set => dataID = value; }
        public Color DefaultColor { get => defaultColor; set => defaultColor = value; }


        // initialization
        private void Awake()
        {
            neighbors = new ArrayList();
            mesh = GetComponent<MeshRenderer>();
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

        public void selectNode()
        {
            if (!activeNode)
            {
                activeNode = true;
                if(!axis)
                {
                    axis = ScatterPlotSceneManager.Instance.SpawnGraph(transform, transform.position);
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
/*                    axis.SetActive(true);
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