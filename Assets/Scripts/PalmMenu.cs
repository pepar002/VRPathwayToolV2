using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform palmLocation;
    public GameObject headCamera;
    public GameObject menu;

    public void ShowMenu()
    {
        transform.position = palmLocation.position;
        Vector3 v = transform.position - headCamera.transform.position;
        Quaternion q = Quaternion.LookRotation(v);
        transform.rotation = q;
        menu.SetActive(true);
    }
}
