using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathwayNode : MonoBehaviour
{
    [SerializeField]
    public CompoundView compoundView;

    private float offset = 0.1f;

    [SerializeField]
    private bool show = false;

    private bool viewPopup = false;

    private Vector3 defScale;

    private bool scaleDown = false;

    private bool initAnim = false;

    private void Awake()
    {
        compoundView.hideAll();
        defScale = transform.localScale;
    }

    private void Update()
    {
        if (show)
        {
            showCompoundView();
            show = false;
        }
        scaleDown = true;
        if (scaleDown)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, defScale, 5 * Time.deltaTime);
        }
        if (initAnim)
        {
            if (Vector3.Distance(transform.localScale, defScale * 1.1f) > 0.01f)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, defScale * 1.1f, 5 * Time.deltaTime);
            }
            else
            {
                initAnim = false;
                scaleDown = true;
            }
        }
    }

    public void showCompoundView()
    {
        if (viewPopup == false)
        {
            compoundView.transform.position = transform.position + Vector3.up * offset;
            compoundView.showAll();
            initAnim = true;
            viewPopup = true;
        }

    }

    public void OnCollisionStay(Collision other)
    {
        scaleDown = false;
        if (compoundView != null && scaleDown == false)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, defScale * 1.1f, 5 * Time.deltaTime);
        }
    }

    public void OnCollisionExit(Collision other)
    {
        scaleDown = true;
    }
}
