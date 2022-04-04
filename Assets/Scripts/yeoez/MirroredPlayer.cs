using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirroredPlayer : MonoBehaviour
{
    //public GameObject[] avatars;
    public GameObject mirroredHead;
    public GameObject right;
    public GameObject left;

    private SkinnedMeshRenderer leftMesh;
    private SkinnedMeshRenderer rightMesh;

    private GameObject networkL;
    private GameObject networkR;

    private SkinnedMeshRenderer networkHandMesh;

    private int currentAvatarIndex;

    private void Start()
    {
        //currentAvatarIndex = 0;
        //mirroredHead = avatars[currentAvatarIndex];
        //for (int i = 1; i < avatars.Length; i++)
        //{
        //    avatars[i].SetActive(false);
        //}
    }
    void Update()
    {
        if (networkL == null)
        {
            networkL = GameObject.Find("NetworkHand_L");
            if (networkL)
            {

                left.GetComponent<OVRSkeleton>().SetDataProvider(networkL);
                leftMesh = left.GetComponentInChildren<SkinnedMeshRenderer>();
                networkHandMesh = networkL.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }
        else
        {
            left.transform.rotation = Quaternion.Euler(6.232f, -275.341f, 176.907f);
            leftMesh.material = networkHandMesh.material;
        }
        if (networkR == null)
        {
            networkR = GameObject.Find("NetworkHand_R");
            if (networkR)
            {

                right.GetComponent<OVRSkeleton>().SetDataProvider(networkR);
                rightMesh = right.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }
        else
        {
            right.transform.rotation = Quaternion.Euler(0, 74.339f, 0);
            rightMesh.material = networkHandMesh.material;
        }
    }

    //public void ChangeAvatar(bool forward)
    //{
    //    avatars[currentAvatarIndex].SetActive(false);
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

    //    mirroredHead = avatars[currentAvatarIndex];
    //    mirroredHead.SetActive(true);
    //} 
}
