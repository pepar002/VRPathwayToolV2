using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VRige
{
    // Author: Adam Drogemuller
    public class EdgeCreator : MonoBehaviour
    {

        public int edgeCount = 0;
        private ArrayList edges = new ArrayList();
        private ArrayList bezierEdges = new ArrayList();
        private ArrayList cylEdges = new ArrayList();
        public Material lineMat;
        public GameObject EdgePrefab;
        private bool generateCylEdges = true;
        public GameObject edgeParent;
        public float xScale;
        public float yScale;
        private bool edgesEnabled = true;

        private UnityAction<float> nodeMoveListener;
        private int cooldown = 0;

        public ArrayList CylEdges { get => cylEdges; }

        private void Awake()
        {
            nodeMoveListener = new UnityAction<float>(UpdateCylEdges);
        }

        private void OnEnable()
        {
            EventManager.StartListening("nodeMove", nodeMoveListener);
        }
        private void OnDisable()
        {
            EventManager.StopListening("nodeMove", nodeMoveListener);
        }

        //add edges for each node
        public void addEdges(Edge e)
        {
            bool hasEdge = false;
            foreach (Edge edge in edges)
            {
                if (edge.hashCode() == e.hashCode())
                {
                    hasEdge = true;
                }
            }
            if (hasEdge == false)
            {
                edges.Add(e);
                edgeCount++;
                if (generateCylEdges)
                {
                    GameObject c = Instantiate(EdgePrefab, edgeParent.transform);
                    CylEdges.Add(c);
                }
                
            }
            // potentially a problem
            edges.Add(e);
            edgeCount++;
            if (generateCylEdges)
            {
                GameObject c = Instantiate(EdgePrefab, edgeParent.transform);
                CylEdges.Add(c);
            }
        }
        //deletes the edges. primarily used when spawning a different pathway
        public void deleteEdges()
        {
            foreach(GameObject c in CylEdges)
            {
                Destroy(c);

            }
            cylEdges = new ArrayList();
            edges = new ArrayList();
        }

        //hides the edges in the pathway, mainly used when hiding the whole pathway
        public void hideEdges(bool hide)
        {
            if (hide && edgesEnabled)
            {
                foreach (GameObject edge in cylEdges)
                {
                    edge.SetActive(false);
                }
                edgesEnabled = false;
            }
            else if(!hide && !edgesEnabled)
            {
                foreach (GameObject edge in cylEdges)
                {
                    edge.SetActive(true);
                }
                edgesEnabled = true;

            }
        }
        //
        public void scaleEdges(float amount)
        {
            foreach (GameObject edge in cylEdges)
            {
                if ((edge.transform.localScale.x + amount) > 0) edge.transform.localScale = new Vector3(edge.transform.localScale.x + amount, 1, edge.transform.localScale.z + amount);
            }
        }
        void Update()
        {
            //DrawConnectingLines();
            //UpdateCylEdges(0);
            cooldown = 0;
        }

        //update the edge connections when moved or grabbed
        public void UpdateCylEdges(float f)
        {
            if(cooldown < 1)
            {
                cooldown++;
                for (int i = 0; i < CylEdges.Count; i++)
                {
                    //Debug.LogError("Aligning edge");
                    GameObject edge = (GameObject)CylEdges[i];
                    Edge e = (Edge)edges[i];
                    Vector3 initialScale = edge.transform.localScale;

                    float distance = Vector3.Distance(e.getA(), e.getB());
                    edge.transform.localScale = new Vector3(initialScale.x, distance, initialScale.z);

                    //Vector3 middle = (e.getA() + e.getB()) / 2f;
                    edge.transform.position = e.getA();

                    Vector3 rotation = (e.getB() - e.getA());
                    edge.transform.up = rotation;
                }
            }
        }
        //not used - was gl lines as edges
        void DrawConnectingLines()
        {
            //GL.PushMatrix();
            GL.Begin(GL.LINES);
            lineMat.SetPass(0);
            int k = 0;

            foreach (Edge e in edges)
            {
                Color c = e.getColor();
                GL.Color(new Color(c.r, c.g, c.b, 0.8f));
                GL.Vertex3(e.getA().x, e.getA().y, e.getA().z);
                GL.Vertex3(e.getB().x, e.getB().y, e.getB().z);
            }
            GL.End();
            //GL.PopMatrix();
        }

        public bool connected(Edge A, Edge B)
        {
            if (A.getB() == B.getB())
            {
                return true;
            }
            if (A.getA() == B.getA())
            {
                return true;
            }
            if (A.getB() == B.getA())
            {
                return true;
            }
            if (A.getA() == B.getB())
            {
                return true;
            }
            return false;
        }
    }
}
