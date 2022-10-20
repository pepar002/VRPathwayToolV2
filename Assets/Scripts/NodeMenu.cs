using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public PinnedNodeHandler pinnedNodeHandler;
    public int spID;

    public TMPro.TMP_Text Name;
    public TMPro.TMP_Text Description;
    public TMPro.TMP_Text Formula;
    public TMPro.TMP_Text Ttest;
    public VRige.VirtualNode targetNode;
    //public Texture2D Image;
    public Image Image;


    public void CloseMenu()
    {
        targetNode.selectNode();
    }
        

    public void PinNode()
    {
        pinnedNodeHandler.PinNode(spID);
    }

    public void UnPinNode()
    {
        pinnedNodeHandler.UnPinNode(spID);
    }
}
