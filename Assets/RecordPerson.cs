using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordPerson : MonoBehaviour
{
    [SerializeField]
    private int Recording = 1;

    [SerializeField]
    private Transform person;

    [SerializeField]
    private Transform head, leftHand, rightHand;

    private ArrayList lines;

    [SerializeField]
    private bool isRecording = false;

    private float t = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        lines = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRecording == true)
        {
            if (t > 0)
            {
                t -= Time.deltaTime;
            }
            else
            {
                t = 0.1f;
                // add line
                string newLine = Recording + "," + head.position.x + "," + head.position.y + "," + head.position.z
                    + "," + head.rotation.x + "," + head.rotation.y + "," + head.rotation.z + "," + head.rotation.w +
                    "," + leftHand.position.x + "," + leftHand.position.y + "," + leftHand.position.z
                    + "," + leftHand.rotation.x + "," + leftHand.rotation.y + "," + leftHand.rotation.z + "," + leftHand.rotation.w
                    + "," + rightHand.position.x + "," + rightHand.position.y + "," + rightHand.position.z
                    + "," + rightHand.rotation.x + "," + rightHand.rotation.y + "," + rightHand.rotation.z + "," + rightHand.rotation.w;
                lines.Add(newLine);

            }
        }
        else
        {
            if (lines.Count > 0)
            {
                OutputCSV();
                lines = new ArrayList();
                print("Written successfully!!");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isRecording = !isRecording;
        }
    }



    public void OutputCSV()
    {
        // output experiment data
        string filename = "Recording.csv";
        StreamWriter writer = null;
        if (!System.IO.File.Exists(filename))
        {
            Debug.Log("Could not find text file! Did not record positional data");
        }
        else
        {
            writer = System.IO.File.AppendText(filename);
        }
        foreach (string line in lines)
        {
            writer.WriteLine(line);
        }
        if (!File.Exists(filename))
        {
            File.Create(filename).Close();
        }
        writer.Close();
    }
}
