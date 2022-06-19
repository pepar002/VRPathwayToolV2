using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugging : MonoBehaviour
{

    public OVRHand hand;

    public SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject sphere = getSphereInHand(hand.gameObject, "Sphere");

        Debug.Log("Compare sphere transforms: " + comparePositions(sphereCollider.gameObject, sphere));
    }

    GameObject getSphereInHand(GameObject handObj, string childObjName) {
        var transforms = handObj.GetComponentsInChildren<Transform>();

        foreach (Transform child in transforms) {
            if (child.name == childObjName) {
                return child.gameObject;
            }
        }

        return null;
    }

    bool comparePositions(GameObject obj1, GameObject obj2) {
        return obj1.transform.position == obj2.transform.position;
    }

}
