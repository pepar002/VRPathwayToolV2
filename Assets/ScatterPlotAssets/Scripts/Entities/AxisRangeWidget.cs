using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class AxisRangeWidget : MonoBehaviour, SGrabbable {

    [SerializeField]
    float axisOffset = 2.0f;

    [SerializeField]
    UnityEvent OnEntered;

    [SerializeField]
    UnityEvent OnExited;

    SAxis parentAxis;

    public Vector3 initialScale;
    Vector3 rescaled = Vector3.one;

    [Serializable]
    public class OnValueChanged : UnityEvent<float> { };
    public OnValueChanged onValueChanged;
        

    // Use this for initialization
    void Start () {
        parentAxis = GetComponentInParent<SAxis>();
        //initialScale = transform.localScale;
        rescaled = initialScale;
        rescaled.x *= 2f;
        rescaled.z *= 2f;
    }
	
	// Update is called once per frame
	void Update () { }

    public void OnEnter(WandController controller)
    {
        OnEntered.Invoke();
    }

    public void OnExit(WandController controller)
    {
        OnExited.Invoke();
    }

    public bool OnGrab(WandController controller)
    {
        return true;
    }

    public void OnRelease(WandController controller)
    { }

    /*
     * Linear interpolation between the controller (collider) and the axis widget
     * whenever its dragging
     */
    public void OnDrag(WandController controller)
    {
        // Get the sphere child gameobject attached to controller (getting the sphere collider component of the controller
        // will yield the same tranform.position as the controller rather than the collider itself)

        float offset = parentAxis.CalculateLinearMapping(controller.GetSphere().transform);
        Vector3 axisOffset = new Vector3(transform.localPosition.x, 0, 0);
        transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, -0.5f, 0),
                                               new Vector3(transform.localPosition.x, 0.5f, 0), 
                                          offset);
        parentAxis.isDirty = true;

        onValueChanged.Invoke(transform.localPosition.y);
    }
	
    public int GetPriority()
    {
        return 10;
    }

    public void ProximityEnter()
    {
        transform.DOKill(true);
        transform.DOLocalMoveX(-axisOffset, 0.35f).SetEase(Ease.OutBack);
        transform.DOScale(rescaled, 0.35f).SetEase(Ease.OutBack);
    }

    public void ProximityExit()
    {
        transform.DOKill(true);
        transform.DOLocalMoveX(0, 0.25f);
        transform.DOScale(initialScale, 0.25f);
    }
}
