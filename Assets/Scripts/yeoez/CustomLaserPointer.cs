/**
 * Projects a laser pointer from the finger tip of the index finger of the hand. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class CustomLaserPointer : MonoBehaviour
{    
    public PhotonView photonView;

    private OVRSkeleton skeleton;
    private OVRHand m_hand;

    private GameObject laserRoot;
    private Vector3 hitPoint; // Point where the raycast hits
    private GameObject cube;

    void Start()
    {
        m_hand = GetComponentInParent<OVRHand>();
        skeleton = GetComponentInParent<OVRCustomSkeleton>();

        // Find the index tip bone
        List<OVRBone> fingerBones = new List<OVRBone>(skeleton.Bones);

        foreach (OVRBone bone in fingerBones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip)
            {
                laserRoot = bone.Transform.gameObject;
            }
        }

        // Create cube at the tip of the index to orientate pointing direction
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<MeshRenderer>().enabled = false;
        cube.GetComponent<Collider>().isTrigger = true;
        cube.name = "cube";
        cube.transform.SetParent(laserRoot.transform);
        cube.transform.localScale = Vector3.zero;

        if (m_hand.HandType == OVRHand.Hand.HandLeft)
        {
            cube.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
        }
        if (m_hand.HandType == OVRHand.Hand.HandRight)
        {
            cube.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
    }

    public void ShootLaser()
    {
        //if (Application.platform == RuntimePlatform.Android)
        //{
            RaycastHit hit;

            // Send out a raycast from the set root
            if (Physics.Raycast(laserRoot.transform.position, cube.transform.forward, out hit, 100))
            {
                if (!hit.collider.CompareTag("Laser") && !hit.collider.CompareTag("Player") && hit.collider.gameObject.name != "cube")
                {
                    hitPoint = hit.point;
                    transform.position = Vector3.Lerp(laserRoot.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
                    transform.LookAt(hitPoint); // Rotate laser facing the hit point
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y,
                        hit.distance + 0.01f); // Scale laser so it fits exactly between the controller & the hit point
                }
            }
            else
            {
                transform.position = Vector3.Lerp(laserRoot.transform.position, laserRoot.transform.position, .5f);
                transform.LookAt(cube.transform.forward);
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y,
                    int.MaxValue);
            }
        //}
    }

    public void ShowLaser(bool active)
    {  
        if (active)
        {
            ShootLaser();
        }
        else
        {
            // Collider remains active when gameObject is active which is undesired. Instead, move the laser to another location.
            transform.position = new Vector3(-9999f, -9999f, -9999f);
        }
    }
}