using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkinEdge : MonoBehaviour
{
    [SerializeField]
    private Transform[] bones;

    [SerializeField]
    private Transform n1, n2;

    [SerializeField]
    private Transform armature;

    [SerializeField]
    private Vector3 offsetEuler;

    [SerializeField]
    private Vector3 boneOffsetEuler;

    [SerializeField]
    private Transform attractToObj;

    [SerializeField]
    private float Fa = 0;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private bool updateValidate = false;


    // Update is called once per frame
    void Update()
    {
        if (n1 != null && n2 != null)
        {
            // Position assignment
            bones[0].transform.position = n1.transform.position;

            bones[1].transform.position = n2.transform.position;

            Vector3 delta0 = n1.position - n2.position;
            Vector3 delta1 = n2.position - n1.position;

            Quaternion q0 = Quaternion.LookRotation(delta0);
            Quaternion q1 = Quaternion.LookRotation(delta1);

            Vector3 euler1 = q0.eulerAngles;


            bones[0].transform.eulerAngles = euler1 + offset;
            bones[1].transform.eulerAngles = euler1 + offset;

        }
    }

    public void positionAssignment()
    {
        if (n1 != null && n2 != null)
        {
            // Position assignment
            bones[0].transform.position = n1.transform.position;

            bones[1].transform.position = n2.transform.position;

            Vector3 delta0 = n1.position - n2.position;
            Vector3 delta1 = n2.position - n1.position;

            Quaternion q0 = Quaternion.LookRotation(delta0);
            Quaternion q1 = Quaternion.LookRotation(delta1);

            Vector3 euler1 = q0.eulerAngles;


            bones[0].transform.eulerAngles = euler1 + offset;
            bones[1].transform.eulerAngles = euler1 + offset;

        }
    }

    void OnValidate()
    {
        Bone[] Bbones = GetComponentsInChildren<Bone>();
        bones = new Transform[Bbones.Length];
        for(int i = 0; i < Bbones.Length; i++)
        {
            bones[i] = Bbones[i].transform;
        }
        positionAssignment();
        transform.name = n1.transform.name + " TO :" + n2.transform.name;
    }
}
