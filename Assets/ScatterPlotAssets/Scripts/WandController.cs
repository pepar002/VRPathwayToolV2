using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Linq;


public interface SGrabbable
{
    int GetPriority();

    // return true if this grabbable is actually grabbable and should attach to the controller
    bool OnGrab(WandController controller);
    void OnRelease(WandController controller);
    void OnDrag(WandController controller);

    void OnEnter(WandController controller);
    void OnExit(WandController controller);
}

public interface Brushable
{
    void OnBrush(WandController controller, Vector3 position, bool is3D);
    void OnBrushRelease(WandController controller);
    void OnDetailOnDemand(WandController controller, Vector3 position, Vector3 localPosition);
    void OnDetailOnDemandRelease(WandController controller);

}
/*
 * This class deals with interactions based on the Oculus Rift controller interactions and Hand interactions
 */
public class WandController : MonoBehaviour
{
    // the controller this component is attached to
    public OVRInput.Controller OculusController;

    // the active controller in the scene
    private OVRInput.Controller activeController;

    // The hand object attached to this component
    private OVRHand hand;

    //public GestureDetector gesture;

    public bool isOculusRift = false;
    //Debug test
    GameObject brushingPoint;

    Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    Valve.VR.EVRButtonId padButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    bool isTouchDown;

    Valve.VR.SteamVR_TrackedObject trackedObject;
    
    Collider intersectingCollider;
    List<Collider> intersectingGrabbables = new List<Collider>();
    
    List<GameObject> draggingObjects = new List<GameObject>();

    Collider brushableCollider;

    List<Vector3> tracking = new List<Vector3>();

    //touch pad interaction
    float previousYValuePad = 0f;
    float incrementYValuePad = 0f;
    float yvaluePadTouchDown = 0f;

    GameObject currentBrushView = null;
    GameObject currentDetailView = null;

    public Material theBrushingMaterial;

    public Vector3 Velocity
    {
        get
        {
            return tracking[0] - tracking[tracking.Count - 1];
        }
    }

    void Start()
    {
        brushingPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        brushingPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.0f);

        brushingPoint.GetComponent<SphereCollider>().enabled = false;// isTrigger = false;
        brushingPoint.GetComponent<MeshRenderer>().material = theBrushingMaterial;

    }

    void Awake()
    {

        trackedObject = GetComponent<Valve.VR.SteamVR_TrackedObject>();
        tracking.AddRange(Enumerable.Repeat<Vector3>(Vector3.zero, 10));
    }

    public void PropergateOnGrab(GameObject g)
    {
        if (g.GetComponent<SGrabbable>() != null && g.GetComponent<SGrabbable>().OnGrab(this))
        {
            draggingObjects.Add(g.gameObject);
        }
    }

    /*
     * On every update check for the active controller and check for any instantiated interactions
     */
    void Update()
    {
        activeController = OVRInput.GetActiveController(); // will always get active controller
        bool gripDown = false;
        bool gripUp = false;
        bool gripping = false ;

        #region if controller type is Oculus Touch
        if (OculusController == activeController &&  OculusController == OVRInput.Controller.Touch || OculusController == OVRInput.Controller.LTouch || OculusController == OVRInput.Controller.RTouch)
        {
            // update the sphere colliders
            UpdateSphereColliders(gameObject, gameObject.GetComponent<SphereCollider>());
            
            // Check the current status of Oculus controller buttons to determing whether to grab or not
            gripDown = isOculusRift ?
    OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OculusController) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger, OculusController)
    : false;

            gripUp = isOculusRift ?
                OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OculusController) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger, OculusController)
                : false;

            gripping = isOculusRift ?
                OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OculusController) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger, OculusController)
                : false;

        }
        #endregion

        #region if controller type is Hands
        else if (OculusController == OVRInput.Controller.LHand || OculusController == OVRInput.Controller.RHand || OculusController == OVRInput.Controller.Hands)
        {
            hand = gameObject.GetComponent<OVRHand>();

            UpdateSphereColliders(gameObject, gameObject.GetComponent<SphereCollider>());
            //gesture = GetComponentInChildren<GestureDetector>();

            // If a hand is making a pinching gesture then it is assumed that it's grabbing (Gestures can be altered)
            if (hand && hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                // it should get to grabbing ofc
                gripDown = true;
                gripUp = false;
                gripping = true;
            }
            else {
                gripDown = false;
                gripUp = true;
                gripping = false;
            }
        }
        #endregion


        /* Check if object can be dragged and compare the objects dragging/grabbing priority
         * against other draggable object in the proximity
         */
        if (gripDown && intersectingGrabbables.Any(x => x!= null) && draggingObjects.Count == 0)
        {
            var potentialDrags = intersectingGrabbables.Where(x => x != null).ToList();
            potentialDrags.Sort((x, y) => y.GetComponent<SGrabbable>().GetPriority() - x.GetComponent<SGrabbable>().GetPriority());
            if (potentialDrags.Count() > 0)
            {
                PropergateOnGrab(potentialDrags.First().gameObject);
            }            
        }
        else if (gripUp && draggingObjects.Count > 0)
        {
            draggingObjects.Where(x => x != null).ForEach(x => x.GetComponent<SGrabbable>().OnRelease(this));
            draggingObjects.Clear();
        }
        else if (gripping && draggingObjects.Count > 0)
        {
            draggingObjects.Where(x => x != null).ForEach(x => x.GetComponent<SGrabbable>().OnDrag(this));            
        }
        
        if (draggingObjects.Count > 0)
        {
            if (!isOculusRift)
                isOculusRift = false;
        }

        bool padPressDown = isOculusRift ? OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OculusController) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick, OculusController)
           : false;

        bool padPressUp = isOculusRift ? OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OculusController) || OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick, OculusController)
          : false;

        #region details on demand
        //detail on demand actions
        if (VisualisationAttributes.detailsOnDemand)
        {
            if (padPressDown)
            {
                bool detail3Dscatterplots = false;
                GameObject[] listCandidatesBrush3D = GameObject.FindGameObjectsWithTag("Scatterplot3D");
                for (int i = 0; i < listCandidatesBrush3D.Length; i++)
                {
                    {
                        if (Vector3.Distance(listCandidatesBrush3D[i].transform.position, transform.position) < 0.3f)
                        {
                            detail3Dscatterplots = true;
                            brushingPoint.gameObject.SetActive(true);

                            currentDetailView = listCandidatesBrush3D[i];
                            brushingPoint.transform.position = transform.position + transform.forward * 0.1f;
                            brushingPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                            if (currentDetailView.GetComponent<Visualization>() != null)
                            {
                                currentDetailView.GetComponent<Visualization>().OnDetailOnDemand(this, 
                                    brushingPoint.transform.position, 
                                    currentDetailView.transform.InverseTransformPoint(brushingPoint.transform.position),
                                    true);
                            }
                            else
                            {
                                Debug.Log("the object is null/...");
                            }
                        }
                    }
                }
                if (!detail3Dscatterplots)
                {
                    RaycastHit hit;
                    Ray downRay = new Ray(transform.position, transform.forward);
                    if (Physics.Raycast(downRay, out hit))
                    {
                        if (hit.transform.gameObject.GetComponent<Brushable>() != null)
                        {
                            brushingPoint.gameObject.SetActive(true);
                            currentDetailView = hit.transform.gameObject;
                            brushingPoint.transform.position = hit.point;
                            brushingPoint.transform.rotation = currentDetailView.transform.rotation;
                            brushingPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.0f);
                            
                            currentDetailView.GetComponent<Visualization>().OnDetailOnDemand(
                                this, 
                                hit.point, 
                                currentDetailView.transform.InverseTransformPoint(hit.point),
                                false);
                        }

                    }
                }
            }
            if (padPressUp)
            {
                if (currentDetailView != null)
                {
                    currentDetailView.GetComponent<Visualization>().OnDetailOnDemand(null, Vector3.zero, Vector3.zero,false);

                    currentDetailView.GetComponent<Visualization>().OnDetailOnDemandRelease(this);
                    currentDetailView = null;
                    brushingPoint.gameObject.SetActive(false);

                }
            }
        }
#endregion
        
        tracking.RemoveAt(0);
        tracking.Add(transform.TransformPoint(new Vector3(0, -0.04f, 0)));

    }

    void OnTriggerEnter(Collider col)
    {
        if (draggingObjects.Count > 0)
            return;

        var grabble = col.GetComponent<SGrabbable>();
        if (grabble != null && !intersectingGrabbables.Contains(col))
        {
            Collider activeGrabbable = intersectingGrabbables.FirstOrDefault();
            intersectingGrabbables.Add(col);
            intersectingGrabbables.RemoveAll(x => x == null);
            intersectingGrabbables.Sort((x, y) => y.GetComponent<SGrabbable>().GetPriority() - x.GetComponent<SGrabbable>().GetPriority());
            if (intersectingGrabbables[0] == col){
                if (activeGrabbable != null && activeGrabbable != intersectingGrabbables[0])
                {
                    activeGrabbable.GetComponent<SGrabbable>().OnExit(this);
                }
                grabble.OnEnter(this);
            }
        }
        if (col.GetComponent<Brushable>() != null)
        {
            brushableCollider = col;
        }
    }

    void OnTriggerExit(Collider col)
    {
        intersectingGrabbables.RemoveAll(x => x == null);

        var grabbable = col.GetComponent<SGrabbable>();
        if (grabbable != null && intersectingGrabbables.Contains(col))
        {
            if (col == intersectingGrabbables[0]){
                grabbable.OnExit(this);
                intersectingGrabbables.RemoveAt(0);
                if (intersectingGrabbables.Count > 0){
                    intersectingGrabbables[0].GetComponent<SGrabbable>().OnEnter(this);
                }
            } else {
                intersectingGrabbables.Remove(col);
            }
            brushableCollider = null;
        }
    }

    void SetIntersectingCollider(Collider col)
    {
        if (col != null){
            if (intersectingCollider != null && col.GetComponent<SGrabbable>().GetPriority() >= intersectingCollider.GetComponent<SGrabbable>().GetPriority())
            {
                intersectingCollider.GetComponent<SGrabbable>().OnExit(this);
            }
            intersectingCollider = col;
            intersectingCollider.GetComponent<SGrabbable>().OnEnter(this);
            
        } else {
            intersectingCollider.GetComponent<SGrabbable>().OnExit(this);
            intersectingCollider = null;
        }        
    }

    void UpdateSphereColliders(GameObject controller, SphereCollider collider) {
        List<GameObject> allControllers = GameObject.FindGameObjectsWithTag("Controller").ToList();
        String name ="";

        switch (controller.name) {
            case "OVRLeftControllerPrefab":
            case "OVRRightControllerPrefab":
                name = "oculus_touch";
                break;
            case "OVRLeftHand":
            case "OVRRightHand":
                name = "hands";
                break;
            default:
                break;
        }

        if (controller)
        {
            collider.enabled = true;

            foreach (GameObject obj in allControllers)
            {
             
/*                if (!GameObject.ReferenceEquals(obj, controller))
                {
                    obj.GetComponent<SphereCollider>().enabled = true;
                }
*/
                switch (controller.name)
                {
                    case "OVRLeftControllerPrefab":
                    case "OVRRightControllerPrefab":
                        if (name == "oculus_touch")
                        {
                            obj.GetComponent<SphereCollider>().enabled = true;
                        }
                        else {
                            obj.GetComponent<SphereCollider>().enabled = false;
                        }
                        break;
                    case "OVRLeftHand":
                    case "OVRRightHand":
                        if (name == "hands")
                        {
                            obj.GetComponent<SphereCollider>().enabled = true;
                        }
                        else {
                            obj.GetComponent<SphereCollider>().enabled = false;
                        }
                        break;

                }
            }
        }

    }

    // if controller is not in use be it hand or oculus touch, then disable its sphere collider.
    void colliderOff() {
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    void colliderOn() {
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    // Grab the sphere game object attached to the contoller gameobject
    public GameObject GetSphere()
    {
        var transforms = this.GetComponentsInChildren<Transform>();
        String childObjName = "Sphere";

        foreach (Transform child in transforms)
        {
            if (child.name == childObjName)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    public bool IsDragging(SGrabbable grab)
    {
        return draggingObjects.Any(x => x.GetComponent<SGrabbable>() == grab);
    }

    public bool IsDragging()
    {
        return draggingObjects.Count > 0;
    }

    IEnumerator ShakeCoroutine()
    {
        for (int i = 0; i < 15; ++i)
        {
            //controller.TriggerHapticPulse((ushort)(3900 * (15 - i) / 15.0f));
            yield return new WaitForEndOfFrame();
        }
    }

    //length is how long the vibration should go for
    //strength is vibration strength from 0-1
    IEnumerator TriggerHaptics(float length, float strength) {
        if (!isOculusRift)
        {
            for (float i = 0; i < length; i += Time.deltaTime)
            {
                //controller.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void Shake()
    {
        if (!isOculusRift)
        StartCoroutine(ShakeCoroutine());
    }

    public void OnApplicationQuit()
    {
       
    }

}
