using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public ArrayList CylEdges { get => cylEdges; }

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
                    GameObject c = Instantiate(EdgePrefab);
                    CylEdges.Add(c);
                }
                
            }
            // potentially a problem
            edges.Add(e);
            edgeCount++;
            if (generateCylEdges)
            {
                GameObject c = Instantiate(EdgePrefab);
                CylEdges.Add(c);
            }
        }

        public void deleteEdges()
        {
            foreach(GameObject c in CylEdges)
            {
                Destroy(c);

            }
            cylEdges = new ArrayList();
            edges = new ArrayList();
        }

        void OnPostRender()
        {
            //DrawConnectingLines();
            UpdateCylEdges();
        }

        void UpdateCylEdges()
        {
            /*foreach(GameObject g in cylEdges)
            {
                cylEdges.Remove(g);
                Destroy(g);
                
            }*/
            for (int i = 0; i < CylEdges.Count; i++)
            {
                //Debug.LogError("Aligning edge");
                GameObject edge = (GameObject) CylEdges[i];
                Edge e = (Edge) edges[i];
                Vector3 initialScale = edge.transform.localScale;

                float distance = Vector3.Distance(e.getA(), e.getB());
                edge.transform.localScale = new Vector3(initialScale.x, distance, initialScale.z);

                //Vector3 middle = (e.getA() + e.getB()) / 2f;
                edge.transform.position = e.getA();

                Vector3 rotation = (e.getB() - e.getA());
                edge.transform.up = rotation;

                
            }
        }

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
