using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

[SelectionBase]
public class CompoundView : MonoBehaviour
{

    [SerializeField]
    private Visualisation vis;

    [SerializeField]
    private string y_axis = "";

    [SerializeField]
    private Transform[] components;

    [SerializeField]
    private bool updated = false;

    private void Update()
    {
        if(y_axis != "" && updated == false)
        {
            vis.yDimension = y_axis;
            vis.updateProperties();
            updated = true;
        }
    }

    public void OnEnable()
    {
        updated = false;
        if (y_axis != "" && updated == false)
        {
            vis.yDimension = y_axis;
            vis.updateProperties();
            updated = true;
        }
    }

    public void showAll()
    {
        foreach(Transform t in components)
        {
            t.gameObject.SetActive(true);
        }
    }

    public void hideAll()
    {
        foreach (Transform t in components)
        {
            t.gameObject.SetActive(false);
        }
    }
}
