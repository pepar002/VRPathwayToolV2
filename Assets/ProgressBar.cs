using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VRige
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private VRige_Graph_Creator vrige;

        private float t;

        private float percent = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (percent < 1)
            {
                t = vrige.t;
                float max = 1 - 0.95f;
                float current = 1 - t;

                percent = (current / max);
                transform.localScale = new Vector3(percent, transform.localScale.y, transform.localScale.z);
            } else
            {
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
