/**
 * Information components related to a metabolite. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NodeComponents : MonoBehaviour
{
    [SerializeField]
    private TextMesh nodeLabel;

    [SerializeField]
    private GameObject image;

    [SerializeField]
    private GameObject scatterplot;

    private Vector3 hmdCameraPos;
    private Vector3 imagePos;
    private Vector3 imageLocalPos;
    private Quaternion imageRot;
    private Quaternion imageLocalRot;

    private Vector3 scatterplotPos;
    private Vector3 scatterplotLocalPos;
    private Quaternion scatterplotRot;
    private Quaternion scatterplotLocalRot;

    private bool initalised = false;
    private void Update()
    {
        if (!initalised)
        {
            CustomGrabbable imageHandle = image.GetComponentInChildren<CustomGrabbable>();
            imagePos = imageHandle.transform.position;
            imageLocalPos = imageHandle.transform.localPosition;
            imageRot = imageHandle.transform.rotation;
            imageLocalRot = imageHandle.transform.localRotation;

            CustomGrabbable scatterplotHandle = image.GetComponentInChildren<CustomGrabbable>();
            scatterplotPos = scatterplotHandle.transform.position;
            scatterplotLocalPos = scatterplotHandle.transform.localPosition;
            scatterplotRot = scatterplotHandle.transform.rotation;
            scatterplotLocalRot = scatterplotHandle.transform.localRotation;

            GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Takeover;
            GetComponent<PhotonView>().TransferOwnership(0);

            PhotonView[] photonViews = GetComponentsInChildren<PhotonView>(true);
            foreach (PhotonView pv in photonViews)
            {
                pv.OwnershipTransfer = OwnershipOption.Takeover;
                pv.TransferOwnership(0);
            }
            initalised = true;
        }
    }

    [PunRPC]
    void ShowAll(bool active, float hmdX, float hmdY, float hmdZ)
    {
        hmdCameraPos = new Vector3(hmdX, hmdY, hmdZ);
        ShowNodeText(active);
        ShowImage(active);
        
        if (scatterplot)
        {  
            ShowScatterplot(active);
        } 
    }

    [PunRPC]
    void SetNodeText(string text)
    {
        nodeLabel.text = text;         
    }

    [PunRPC]
    void ShowNodeText(bool active)
    {
        nodeLabel.transform.parent.gameObject.SetActive(active);        
    }

    [PunRPC]
    void SetImage(string path)
    {
        Sprite loadedImage = Resources.Load<Sprite>(path);
        if (!loadedImage)
        {
            loadedImage = Resources.Load<Sprite>("ChemicalStructures/NoData");
        }
        SpriteRenderer spriteRenderer = image.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = loadedImage;
    }

    [PunRPC]
    void SetScatterplot(string path)
    {
        Sprite loadedImage = Resources.Load<Sprite>(path);
        if (!loadedImage)
        {
            ShowScatterplot(false);
            scatterplot = null;
        }
        else
        {
            SpriteRenderer spriteRenderer = scatterplot.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = loadedImage;
        }
    }

    [PunRPC]
    void ShowImage(bool active)
    {
        image.SetActive(active);
        if (active)
        {
            CustomGrabbable handle = image.GetComponentInChildren<CustomGrabbable>();
            handle.transform.position = imagePos;
            handle.transform.rotation = imageRot;
            handle.transform.localPosition = imageLocalPos;
            handle.transform.localRotation = imageLocalRot;
            OrientateToCamera(image);
        }
    }

    [PunRPC]
    void ShowScatterplot(bool active)
    {
        scatterplot.SetActive(active);
        if (active)
        {
            CustomGrabbable handle = scatterplot.GetComponentInChildren<CustomGrabbable>();
            handle.transform.position = scatterplotPos;
            handle.transform.rotation = scatterplotRot;
            handle.transform.localPosition = scatterplotLocalPos;
            handle.transform.localRotation = scatterplotLocalRot;
            OrientateToCamera(scatterplot);
        }
    }
    
    private void OrientateToCamera(GameObject go)
    {
        Vector3 v = transform.position - hmdCameraPos;
        Quaternion q = Quaternion.LookRotation(v);
        go.transform.rotation = q;
    }
}
