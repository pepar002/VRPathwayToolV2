using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NodeSelector : PokeSelector
{
    private GameObject nodeChild;
    private PhotonView photonView;
    private PhotonView nodeChildPhotonView;

    private MeshRenderer mesh;
    private Color originalColour;

    private bool childActive;
    private bool exited;
    public bool showNodeChildOnStart;

    //private NetworkHead[] heads;
    //private Transform head;
    
    protected void Start()
    {
        photonView = GetComponent<PhotonView>();            

        mesh = GetComponent<MeshRenderer>();
        if (mesh.material.color != null)
        {
            originalColour = mesh.material.color;
        }
        childActive = false;
        exited = true;
    }

    private void Update()
    {     
        if (!nodeChild && PhotonNetwork.CurrentRoom != null)
        {
            InstantiateNodeChild();
            if (showNodeChildOnStart)
            {
                nodeChildPhotonView.RPC("ShowAll", RpcTarget.AllBuffered, true, GameObject.Find("CenterEyeAnchor").transform.position.x, GameObject.Find("CenterEyeAnchor").transform.position.y, GameObject.Find("CenterEyeAnchor").transform.position.z);
                photonView.RPC("SetShowing", RpcTarget.AllBuffered, showNodeChildOnStart);
            }
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "IndexTip" && ColliderHandIsPointing(collision))
        {
            FindPokingPlayerHead(collision);
            photonView.RPC("ChangeGrabbableColour", RpcTarget.AllBuffered, "green", 0f, 0f, 0f);
            if (exited)
            {
                if (childActive)
                {
                    photonView.RPC("SetShowing", RpcTarget.AllBuffered, false);
                    nodeChildPhotonView.RPC("ShowAll", RpcTarget.AllBuffered, false, 
                        pokingPlayerHead.transform.position.x, pokingPlayerHead.transform.position.y, 
                        pokingPlayerHead.transform.position.z);
                }
                else
                {
                    photonView.RPC("SetShowing", RpcTarget.AllBuffered, true);
                    nodeChildPhotonView.RPC("ShowAll", RpcTarget.AllBuffered, true, 
                        pokingPlayerHead.transform.position.x, pokingPlayerHead.transform.position.y,
                        pokingPlayerHead.transform.position.z);
                }
                photonView.RPC("SetExited", RpcTarget.AllBuffered, false);
            }
        }
    }    

    private void OnTriggerExit(Collider collision)
    {
        if (collision.name == "IndexTip" && ColliderHandIsPointing(collision))
        {
            photonView.RPC("ChangeGrabbableColour", RpcTarget.AllBuffered, "null", originalColour.r, originalColour.g, originalColour.b);
            photonView.RPC("SetExited", RpcTarget.AllBuffered, true);
        }        
    }

    public void InstantiateNodeChild()
    {
        nodeChild = PhotonNetwork.Instantiate("NodeChild", transform.position, transform.rotation, 0);
        nodeChildPhotonView = nodeChild.GetComponent<PhotonView>();
        nodeChildPhotonView.OwnershipTransfer = OwnershipOption.Takeover;
        nodeChildPhotonView.TransferOwnership(0);

        nodeChildPhotonView.transform.SetParent(this.transform);
        nodeChildPhotonView.transform.localPosition = Vector3.zero;
        nodeChildPhotonView.transform.localRotation = Quaternion.identity;

        nodeChildPhotonView.RPC("ShowAll", RpcTarget.AllBuffered, false, GameObject.Find("CenterEyeAnchor").transform.position.x, GameObject.Find("CenterEyeAnchor").transform.position.y, GameObject.Find("CenterEyeAnchor").transform.position.z);
        nodeChildPhotonView.RPC("SetNodeText", RpcTarget.AllBuffered, GetComponent<DataPoint>().ID());

        
        nodeChildPhotonView.RPC("SetImage", RpcTarget.AllBuffered, "ChemicalStructures/" + GetComponent<DataPoint>().ID());
        nodeChildPhotonView.RPC("SetScatterplot", RpcTarget.AllBuffered, "ChemicalStructures/" + GetComponent<DataPoint>().ID() + "_scatterplot");
        Sprite loadedScatterplot = Resources.Load<Sprite>("ChemicalStructures/" + GetComponent<DataPoint>().ID() + "_scatterplot");        
        
        // Transparent node if no scatterplot
        if (!loadedScatterplot)
        {
            MeshRenderer mr = this.GetComponent<MeshRenderer>();
            Color col = mr.material.color;
            col.a = 100 / 255f; // pass float value here
            mr.material.color = col;
        }     
        
                
        // Inform other clients of the instantiated node child
        photonView.RPC("NewNodeChildPVID", RpcTarget.AllBuffered, nodeChildPhotonView.ViewID);
    }

    public PhotonView NodeChildPhotonView()
    {
        return nodeChildPhotonView;
    }

    [PunRPC]
    void NewNodeChildPVID(int id)
    {
        nodeChildPhotonView = PhotonView.Find(id);
        nodeChild = nodeChildPhotonView.gameObject;
    }

    [PunRPC]
    void SetShowing(bool active)
    {
        childActive = active;
    }

    [PunRPC]
    void SetExited(bool active)
    {
        exited = active;
    }
}
