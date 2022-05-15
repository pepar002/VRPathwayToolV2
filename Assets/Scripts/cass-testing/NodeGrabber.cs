using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OculusSampleFramework;

public class NodeGrabber : OVRGrabber
{
    public LaserPointer laserPointerObj;

    public OVRHand m_hand;
    public GestureDetector gesture;
    public GameObject palmUI;

    private Vector3 previousPosition;
    private Quaternion lastRot;
    private Vector3 linearVelocity;
    private Vector3 angularVelocity;



    protected override void Start()
    {
        if (transform.parent.name == "RightHandAnchor")
        {
            palmUI = Instantiate((GameObject)Resources.Load("PalmUI"), gameObject.transform);
        }

        gameObject.layer = LayerMask.NameToLayer("UI");
        base.Start();
        m_hand = gameObject.GetComponent<OVRHand>();

        gesture = GetComponentInChildren<GestureDetector>();

        if (laserPointerObj != null && laserPointerObj.isActiveAndEnabled)
        {
            //laserPointer = laserPointerObj.GetComponent<CustomLaserPointer>();
        }
        previousPosition = transform.position;
    }

    override public void Update()
    {

        base.Update();

        CheckGesture();

        if (m_grabbedObj)
        {
            m_grabbedObj.GetComponent<NodeGrabbable>().ChangeGrabbableColour( "green", 0f, 0f, 0f);
        }
        previousPosition = transform.position;
        lastRot = transform.rotation;
        if (transform.parent.name == "RightHandAnchor")
        {
            CheckAngle();
        }
            
    }

    void CheckAngle()
    {

        float rotationX = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
        //float rotationZ = Mathf.Atan2(transform.up.x, transform.up.z) * Mathf.Rad2Deg;

        //Debug.Log("X: " + rotationX);
        //Debug.Log("Z: " + rotationZ);

         bool x = rotationX > -120f && rotationX < -60f;
        //bool z = rotation.z > -20f && rotation.z < 20f;*//*

        //Debug.Log(x);
        if (x)
         {
            palmUI.SetActive(true);
         }
         else
         {
             palmUI.SetActive(false);
         }
        //Debug.Log(GetCurrentRotation());

    }

    void CheckGesture()
    {
        if (!m_hand.IsSystemGestureInProgress)
        {
            if (!m_grabbedObj && (gesture.isMiddlePinching() || gesture.isGrabbing()) && m_grabCandidates.Count > 0 && !gesture.isPointing())
            {
                
                GrabBegin();
            }

            if (!gesture.isAnyPinching() && !gesture.isGrabbing() && m_grabbedObj)
            {
                m_grabbedObj.GetComponent<NodeGrabbable>().ResetColour();
                GrabEnd();
            }

            if (laserPointerObj != null && isActiveAndEnabled)
            {
                if (gesture.isPointing())
                {
                }
                else
                {
                }
            }
            /*if(!m_grabbedObj && gesture.isIndexFolding())
            {
                print("TESTING FINGER PRESS");
            }*/
        }
    }

    protected override void GrabBegin()
    {
        float closestMagSq = float.MaxValue;
        OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
        foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];

                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                NodeGrabber previousGrabber = (NodeGrabber)closestGrabbable.grabbedBy;
                previousGrabber.OffhandGrabbed(closestGrabbable);

                previousGrabber.GrabVolumeEnable(true);
            }

            m_grabbedObj = closestGrabbable;

            // Added to transfer ownership of children of the grabbed object to the grabbing client
            /*
            m_grabbedObj.GetComponent<PhotonView>().TransferOwnership(photonView.Owner);
            PhotonView[] pvs;
            NodeSelector node = m_grabbedObj.GetComponent<NodeSelector>();
            if (node && node.NodeChildPhotonView())
            {
                node.NodeChildPhotonView().gameObject.transform.SetParent(m_grabbedObj.transform);
                node.NodeChildPhotonView().TransferOwnership(photonView.Owner);
                pvs = node.NodeChildPhotonView().gameObject.GetComponentsInChildren<PhotonView>();

            }
            else
            {
                pvs = m_grabbedObj.GetComponentsInChildren<PhotonView>();
            }
            foreach (var pv in pvs)
            {
                pv.TransferOwnership(photonView.Owner);
            }
            */

            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            if (m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if (m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }
            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            SetPlayerIgnoreCollision(m_grabbedObj.gameObject, true);

            if (m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }
    /*public override void GrabEnd()
    {
        linearVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
        Quaternion dif = lastRot * Quaternion.Inverse(transform.rotation);
        float angle;
        Vector3 axis;
        dif.ToAngleAxis(out angle, out axis);
        angularVelocity = axis * angle / Time.deltaTime;
        linearVelocity.Scale(new Vector3(0.7f, 0.7f, 0.7f));

        // Velocity of hand when release object is slow. Considered placing the object rather than throwing.
        if (linearVelocity.x < 0.15f && linearVelocity.y < 0.15f && linearVelocity.z < 0.15f)
        {
            linearVelocity = Vector3.zero;
        }
        angularVelocity = Vector3.zero;

        GrabbableRelease(linearVelocity, angularVelocity);

        GrabVolumeEnable(true);
    }*/
}