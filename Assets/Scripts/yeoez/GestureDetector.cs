/**
 * Detects the pinch and fold state of each finger to determine the overall hand gesture of an OVRHand. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GestureDetector : MonoBehaviour
{
    public OVRHand m_hand;
    public Collider index;
    public Collider middle;
    public Collider pinky;
    public Collider ring;
    private bool indexFold { get; set; } = false;
    private bool middleFold { get; set; } = false;
    private bool ringFold { get; set; } = false;
    private bool pinkyFold { get; set; } = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.Equals(index))
        {
            indexFold = true;
        }
        if (collision.Equals(middle))
        {
            middleFold = true;
        }
        if (collision.Equals(ring))
        {
            ringFold = true;
        }
        if (collision.Equals(pinky))
        {
            pinkyFold = true;
        }
    }    

    private void OnTriggerExit(Collider collision)
    {
        if (collision.Equals(index))
        {
            indexFold = false;
        }
        if (collision.Equals(middle))
        {
            middleFold = false;
        }
        if (collision.Equals(ring))
        {
            ringFold = false;
        }
        if (collision.Equals(pinky))
        {
            pinkyFold = false;
        }
    }

    public bool isGrabbing()
    {
        if (indexFold && middleFold && ringFold && pinkyFold)
        {
            return true;           
        }
        return false;
    }

    public bool isPointing()
    {
        return !isIndexFolding() && !isIndexPinching() && isMiddleFolding() && isRingFolding() && isPinkyFolding();
    }

    public bool isAnyPinching()
    {
        return isIndexPinching() || isMiddlePinching() || isRingPinching() || isPinkyPinching();
    }

    public bool isIndexFolding()
    {
        return indexFold;
    }
    public bool isMiddleFolding()
    {
        return middleFold;
    }
    public bool isRingFolding()
    {
        return ringFold;
    }
    public bool isPinkyFolding()
    {
        return pinkyFold;
    }
    public bool isIndexPinching()
    {
        return m_hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
    }
    public bool isMiddlePinching()
    {
        return m_hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);
    }
    public bool isRingPinching()
    {
        return m_hand.GetFingerIsPinching(OVRHand.HandFinger.Ring);
    }
    public bool isPinkyPinching()
    {
        return m_hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
    }
}
