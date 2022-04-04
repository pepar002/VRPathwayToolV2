/**
 * Selector that triggers only when the hand pokes the selector with a pointing gesture. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PokeSelector : MonoBehaviour
{
    protected NetworkHead[] heads;
    protected NetworkHead pokingPlayerHead;
    protected bool ColliderHandIsPointing(Collider collision)
    {
        GameObject hand = collision.gameObject.GetComponentInParent<OVRHand>().gameObject;
        GestureDetector gesture = hand.GetComponentInChildren<GestureDetector>();
        if (gesture)
        {
            return gesture.isPointing();
        }
        return false;
    }

    protected void FindPokingPlayerHead(Collider collision)
    {
        // Find the head of the player which selected the node to orientate the child objects
        heads = collision.gameObject.transform.root.GetComponentsInChildren<NetworkHead>();
        foreach (var child in heads)
        {
            if (child.GetComponent<PhotonView>().Owner == collision.gameObject.GetComponent<PhotonView>().Owner)
            {
                pokingPlayerHead = child;
            }
        }
        
    }
}
