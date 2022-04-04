/**
 * The player's hand on the network. Sets the hand to attach to the correct hand anchor.  
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkHand : MonoBehaviour
{
    private Transform playerGlobal;
    private Transform playerLocal;
    private PhotonView photonView;

    //private SkinnedMeshRenderer meshRenderer;
    //public Material[] materials;
    //private int currentMaterial;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            playerGlobal = GameObject.Find("OVRPlayerController").transform;
            if (GetComponent<OVRHand>().HandType == OVRHand.Hand.HandLeft)
            {
                playerLocal = playerGlobal.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor");
            }

            if (GetComponent<OVRHand>().HandType == OVRHand.Hand.HandRight)
            {
                playerLocal = playerGlobal.Find("OVRCameraRig/TrackingSpace/RightHandAnchor");
            }

            this.transform.SetParent(playerLocal);
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = new Vector3(1, 1, 1);
            //meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            //currentMaterial = 0;
        }
    }

    //[PunRPC]
    //void ChangeHandMaterial(bool forward)
    //{
    //    if (forward)
    //    {
    //        if (currentMaterial == materials.Length - 1)
    //        {
    //            currentMaterial = 0;
    //        }
    //        else
    //        {
    //            currentMaterial++;
    //        }
    //    } else
    //    {
    //        if (currentMaterial == 0)
    //        {
    //            currentMaterial = materials.Length - 1;
    //        }
    //        else
    //        {
    //            currentMaterial--;
    //        }
    //    }
    //    meshRenderer.material = materials[currentMaterial];            
    //}

}
