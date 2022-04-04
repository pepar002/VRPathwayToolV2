using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognised;
}

public class ValemGestureDetector : MonoBehaviour
{
    public float threshold = 0.05f;
    private OVRCustomSkeleton skeleton ;
    public List<Gesture> gestures;
    public bool debugMode = true;
    private List<OVRBone> fingerBones;
    private Gesture previousGesture = new Gesture();


    // Update is called once per frame
    void Update()
    {
        if (skeleton == null)
        {
            skeleton = gameObject.GetComponent<OVRCustomSkeleton>();
        }

        if (fingerBones == null && skeleton != null)
        {
            fingerBones = new List<OVRBone>(skeleton.Bones);
        }
        
        if (debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }

        Gesture currentGesture = Recognise();
        bool hasRecognised = !currentGesture.Equals(new Gesture());
        // Check if new gesture
        if (hasRecognised && !currentGesture.Equals(previousGesture))
        {
            //Debug.Log(currentGesture.name);
            previousGesture = currentGesture;
            currentGesture.onRecognised.Invoke();
        }
    }

    void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            // finger position relative to root
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerDatas = data;
        gestures.Add(g);
    }

    Gesture Recognise()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if (distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }
}
