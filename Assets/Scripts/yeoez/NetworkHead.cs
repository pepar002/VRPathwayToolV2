/**
 * The player's head on the network.  Sets the head to the correct eye anchor. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using UnityEngine;
using System.Collections;
using Photon.Pun;
public class NetworkHead : MonoBehaviour
{
    private Transform playerGlobal;
    private Transform playerLocal;

    //public GameObject[] avatars;
    //private GameObject currentAvatar;
    //private int currentAvatarIndex;
    public bool isNarrator = false;    
    public Material narratorMaterial;
    public Material spectatorMaterial;
    
    [SerializeField]
    private MeshRenderer meshRenderer;

    void Start()
    {
        var photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            playerGlobal = GameObject.Find("OVRPlayerController").transform;
            playerLocal = playerGlobal.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");

            transform.SetParent(playerLocal);
            transform.forward = playerLocal.forward;
            transform.localPosition = Vector3.zero;
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            //currentAvatarIndex = 0;
            //currentAvatar = avatars[0];
            //for (int i = 1; i < avatars.Length; i++)
            //{
            //    avatars[i].SetActive(false);
            //}
            
        }
    }

    private void Update()
    {
        if (isNarrator)
        {
            GetComponent<PhotonView>().RPC("SetIsNarrator", RpcTarget.AllBuffered, true);
        }
        if (!isNarrator)
        {
            GetComponent<PhotonView>().RPC("SetIsNarrator", RpcTarget.AllBuffered, false);
        }
    }

    [PunRPC]
    void SetIsNarrator(bool isNarrator)
    {
        if (isNarrator)
        {
            this.isNarrator = true;
            var mats = meshRenderer.materials;
            mats[0] = narratorMaterial;
            meshRenderer.materials = mats;
        } else
        {
            this.isNarrator = false;
            var mats = meshRenderer.materials;
            mats[0] = spectatorMaterial;
            meshRenderer.materials = mats;
        }
    }

    //[PunRPC]
    //void ChangeAvatarHead(bool forward)
    //{
    //    if (forward)
    //    {
    //        if (currentAvatarIndex == avatars.Length - 1)
    //        {
    //            currentAvatarIndex = 0;
    //        }
    //        else
    //        {
    //            currentAvatarIndex++;
    //        }
    //    }
    //    else
    //    {
    //        if (currentAvatarIndex == 0)
    //        {
    //            currentAvatarIndex = avatars.Length - 1;
    //        }
    //        else
    //        {
    //            currentAvatarIndex--;
    //        }
    //    }

    //    bool currentActive = currentAvatar.activeInHierarchy;
    //    currentAvatar.SetActive(false);
    //    currentAvatar = avatars[currentAvatarIndex];
    //    currentAvatar.SetActive(currentActive);
    //}
}