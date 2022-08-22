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
    //public Texture2D Image;
    public Image Image;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
