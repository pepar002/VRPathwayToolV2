using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRige
{
    public class VirtualNode : MonoBehaviour
    {

        public string ID = "-999";
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

        // initialization
        private void Awake()
        {
            neighbors = new ArrayList();
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