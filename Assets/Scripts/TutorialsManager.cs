using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialsManager : MonoBehaviour
{
    public VideoClip[] videoClips;
    public GameObject graph;
    public VideoPlayer videoPlayer;
    public GameObject leftHandRayInteractor;
    public GameObject rightHandRayInterator;
    public GameObject headcamera;
    public GameObject backButton;
    public GameObject forwardButton;
    public TextAsset tutorialsSource;
    public TMPro.TMP_Text tutorialDescription;

    private int videoClipIndex;
    private string[] tutorials;

    private void Awake()
    {
        videoClipIndex = 0;
        tutorials = tutorialsSource.text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
    }

    // Play first tutorial upon startup
    void Start()
    {
        PlayVideo(0);
    }

    // hide graph and enable ray hand interactors whenver tutorials are open
    private void OnEnable()
    {
        this.transform.position = new Vector3(headcamera.transform.position.x + 1.0f, headcamera.transform.position.y, headcamera.transform.position.z);

        graph.GetComponent<VRige.VRige_Graph_Creator>().hideNodes(true);
        leftHandRayInteractor.SetActive(true);
        rightHandRayInterator.SetActive(true);

    }

    private void OnDisable()
    {
        graph.GetComponent<VRige.VRige_Graph_Creator>().hideNodes(false);
        leftHandRayInteractor.SetActive(false);
        rightHandRayInterator.SetActive(false);
    }

    private void Update()
    {
        // Enable/Disble back and forward buttons if at video 1 or last video
        if (videoClipIndex == 0)
        {
            backButton.SetActive(false);
            forwardButton.SetActive(true);
        }
        else if(videoClipIndex == videoClips.Length - 1)
        {
            backButton.SetActive(true);
            forwardButton.SetActive(false);
        }
        else
        {
            backButton.SetActive(true);
            forwardButton.SetActive(true);
        }

        Vector3 v = transform.position - headcamera.transform.position;
        Quaternion q = Quaternion.LookRotation(v);
        transform.rotation = q;
    }

    // Show tutorial video and info for a given index, graph is disabled.
    public void PlayVideo(int index) {
        this.gameObject.SetActive(true);
        videoClipIndex = index;
        tutorialDescription.text = tutorials[index];
        videoPlayer.clip = videoClips[videoClipIndex];
    }

    // Plays the next tutorial clip
    public void PlayNext()
    {
        videoClipIndex++;
        if (videoClipIndex >= videoClips.Length)
        {
            videoClipIndex %= videoClips.Length;
        }
        PlayVideo(videoClipIndex);
    }

    // Plays the previous tutorial clip
    public void PlayPrev()
    {
        videoClipIndex--;
        PlayVideo(videoClipIndex);
    }

    public void CloseTutorial()
    {
        this.gameObject.SetActive(false);
        
    }
}
