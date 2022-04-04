/**
 * Changes the mesh colour of an object by cycling through a list of colours. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ColourCycler : MonoBehaviour
{
    private PhotonView photonView;
    private MeshRenderer meshRenderer;
    private Color[] colours = { Color.red, Color.yellow, Color.green, Color.black, Color.magenta };
    private int ColourPtr = 0;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ChangeColour()
    {
        photonView.RPC("PunChangeColour", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void PunChangeColour()
    {
        meshRenderer.material.color = colours[ColourPtr];
        ColourPtr++;
        if (ColourPtr == 5)
        {
            ColourPtr = 0;
        }
    }
}
