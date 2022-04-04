using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarController : MonoBehaviour
{
    public MirroredPlayer mirroredPlayer;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    ChangeHandMaterial(false);
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    ChangeHandMaterial(true);
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    ChangeAvatarHead(false);
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    ChangeAvatarHead(true);
        //}
    }

    public void SetAsNarrator()
    {

    }

    public void SetAsSpectator()
    {

    }

    //public void ChangeAvatarHead(bool forward)
    //{
    //    NetworkHead[] heads = (NetworkHead[])GameObject.FindObjectsOfType(typeof(NetworkHead));       
    //    foreach (var head in heads)
    //    {
    //        head.GetComponent<PhotonView>().RPC("ChangeAvatarHead", RpcTarget.AllBuffered, forward);           
    //    }
    //    mirroredPlayer.ChangeAvatar(forward);
    //}

    //public void ChangeHandMaterial(bool forward)
    //{
    //    NetworkHand[] hands = (NetworkHand[])GameObject.FindObjectsOfType(typeof(NetworkHand));
    //    foreach (var hand in hands)
    //    {
    //        hand.GetComponent<PhotonView>().RPC("ChangeHandMaterial", RpcTarget.AllBuffered, forward);
    //    }
    //}
}
