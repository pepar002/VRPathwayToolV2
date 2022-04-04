/**
 * Controls the state of all node labels in the pathway.  
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PathwayController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] pathways;

    [SerializeField]
    private TextMesh pathwayName;

    GameObject currentPathway;
    int currentPathwayIndex;

    private bool labelsActive;

    

    private void Start()
    {
        // Show the first pathway only
        currentPathwayIndex = 0;
        currentPathway = pathways[currentPathwayIndex];
        for (int i = 1; i < pathways.Length; i++)
        {
            pathways[i].gameObject.SetActive(false);
        }

        pathwayName.text = currentPathway.name;
        labelsActive = false;
    }

    public void ToggleNodesLabel()
    {
        NodeSelector[] pathwayNodes = currentPathway.GetComponentsInChildren<NodeSelector>();
        foreach (var node in pathwayNodes)
        {
            if (node.NodeChildPhotonView() == null)
            {
                node.InstantiateNodeChild();
            }
            if (!labelsActive)
            {
                node.NodeChildPhotonView().RPC("ShowNodeText", RpcTarget.AllBuffered, true);
            }
            else
            {
                node.NodeChildPhotonView().RPC("ShowNodeText", RpcTarget.AllBuffered, false);
            }
        }

        if (!labelsActive)
        {
            labelsActive = true;
        }
        else
        {
            labelsActive = false;
        }
    }

    public void ChangePathway(bool forward)
    {
        GetComponent<PhotonView>().RPC("PUNChangePathway", RpcTarget.AllBuffered, forward);
    }

    [PunRPC]
    void PUNChangePathway(bool forward)
    {
        if (forward)
        {
            if (currentPathwayIndex == pathways.Length - 1)
            {
                currentPathwayIndex = 0;
            }
            else
            {
                currentPathwayIndex++;
            }
        }
        else
        {
            if (currentPathwayIndex == 0)
            {
                currentPathwayIndex = pathways.Length - 1;
            }
            else
            {
                currentPathwayIndex--;
            }
        }
        currentPathway.gameObject.SetActive(false);

        currentPathway = pathways[currentPathwayIndex];
        currentPathway.gameObject.SetActive(true);

    }
}
