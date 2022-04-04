using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReplayPerson : MonoBehaviour
{

    private ArrayList headPositions = new ArrayList();

    private ArrayList headQuaternions = new ArrayList();

    private ArrayList leftHandPositions = new ArrayList();

    private ArrayList leftHandQuarternions = new ArrayList();

    private ArrayList rightHandPositions = new ArrayList();

    private ArrayList rightHandQuaternions = new ArrayList();

    [SerializeField]
    private Transform Head;

    [SerializeField]
    private Transform leftHand;

    [SerializeField]
    private Transform rightHand;

    [SerializeField]
    private float t = 0.1f;

    [SerializeField]
    private bool isPlaying = false;

    [SerializeField]
    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        createDS();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying && frame < headPositions.Count)
        {
            if(t > 0)
            {
                t -= Time.deltaTime;
            }
            else
            {
                Head.transform.position = (Vector3)headPositions[frame];
                Head.transform.rotation = (Quaternion)headQuaternions[frame];
                leftHand.transform.position = (Vector3)leftHandPositions[frame];
                leftHand.transform.rotation = (Quaternion)leftHandQuarternions[frame];
                rightHand.transform.position = (Vector3)rightHandPositions[frame];
                rightHand.transform.rotation = (Quaternion)rightHandQuaternions[frame];
                frame++;
                t = 0.1f;
            }
        }
    }

    void createDS()
    {
        try
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("Recording.csv"))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    string[] lineArray = line.Split(',');
                    if (lineArray[0] != "PID")
                    {
                        Vector3 headPos = new Vector3(float.Parse(lineArray[1]), float.Parse(lineArray[2]), float.Parse(lineArray[3]));
                        Quaternion headRot = new Quaternion(float.Parse(lineArray[4]), float.Parse(lineArray[5]),
                            float.Parse(lineArray[6]), float.Parse(lineArray[7]));
                        Vector3 lhPos = new Vector3(float.Parse(lineArray[8]), float.Parse(lineArray[9]), float.Parse(lineArray[10]));
                        Quaternion lhRot = new Quaternion(float.Parse(lineArray[11]), float.Parse(lineArray[12]),
                            float.Parse(lineArray[13]), float.Parse(lineArray[14]));
                        Vector3 rhPos = new Vector3(float.Parse(lineArray[15]), float.Parse(lineArray[16]), float.Parse(lineArray[17]));
                        Quaternion rhRot = new Quaternion(float.Parse(lineArray[18]), float.Parse(lineArray[19]),
                            float.Parse(lineArray[20]), float.Parse(lineArray[21]));

                        headPositions.Add(headPos);
                        headQuaternions.Add(headRot);

                        leftHandPositions.Add(lhPos);
                        leftHandQuarternions.Add(lhRot);

                        rightHandPositions.Add(rhPos);
                        rightHandQuaternions.Add(rhRot);
                    }
                }

            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            print("The file could not be read:");
            print(e.Message);
            Debug.LogError("file not read");
        }
    }
}

