using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class NormaliserHandle : MonoBehaviour, SGrabbable {

    [SerializeField]
    UnityEvent OnEntered;

    [SerializeField]
    UnityEvent OnExited;
    
    SAxis parentAxis;

    Vector3 initialScale = Vector3.one;
    Vector3 rescaled = Vector3.one;

    float initX = 0f;

    // Use this for initialization
    void Start () {
        parentAxis = GetComponentInParent<SAxis>();
        initialScale = transform.localScale;
        rescaled = initialScale;
        rescaled.x *= 2f;
        rescaled.z *= 2f;

        initX = transform.position.x;

    }
	
	// Update is called once per frame
	void Update () { }

    public void OnEnter(WandController controller)
    {
        OnEntered.Invoke();
        
        transform.localScale = rescaled;
    }

    public void OnExit(WandController controller)
    {
        OnExited.Invoke();
        transform.localScale = initialScale;

    }

    public bool OnGrab(WandController controller)
    {
        return true;
    }

    public void OnRelease(WandController controller)
    {
    }

    /*
     * Linear interpolation between the controller (collider) and the axis widget
     * whenever its dragging
     */
    public void OnDrag(WandController controller)
    {
        float offset = parentAxis.CalculateLinearMapping(controller.GetSphere().transform);
        Vector3 newP = Vector3.Lerp(parentAxis.MinPosition, parentAxis.MaxPosition, offset);
        transform.position = newP;
        parentAxis.isDirty = true;

    }

    public int GetPriority()
    {
        return 15;
    }

    public void ProximityEnter()
    {
        transform.DOLocalMoveX(-4, 0.35f).SetEase(Ease.OutBack);
    }

    public void ProximityExit()
    {
        transform.DOLocalMoveX(0, 0.25f);
    }
}
