using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinnedNodeHandler : MonoBehaviour
{
    public GameObject playerLocation;
    public Transform player;

    public bool displayAll = false;
    public bool displayPinned = false;
    public List<SAxis> allAxis;
    public List<SAxis> pinnedAxis;
    public List<int> pinnedData;

    // Start is called before the first frame update
    void Start()
    {
        pinnedData = new List<int>();
        pinnedData.Add(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePlayer()
    {
        player.position = playerLocation.transform.position;
        player.rotation = Quaternion.Euler(player.rotation.x, player.rotation.y + 90, player.rotation.z);
    }

    public void DisplayAllNodes()
    {
        displayAll = true;
        allAxis = ScatterPlotSceneManager.Instance.SpawnAllGraphs();
        MovePlayer();
    }

    public void DisplayPinnedNodes()
    {
        displayPinned = true;
        pinnedAxis = ScatterPlotSceneManager.Instance.SpawnPinnedGraphs(pinnedData);
        MovePlayer();
    }
    public void ReturnPlayer()
    {
        ResetDisplay();
        player.position = new Vector3(0, 0, 0);
        player.rotation = Quaternion.Euler(0, 0, 0);
    }
    public void ResetDisplay()
    {
        if (displayAll)
        {
            foreach (SAxis axis in allAxis)
            {
                Destroy(axis.gameObject);
            }
        }else if (displayPinned)
        {
            foreach (SAxis axis in pinnedAxis)
            {
                Destroy(axis.gameObject);
            }
        }
        displayAll = false;
        displayPinned = false;
    }

    public void PinNode(int id)
    {
        pinnedData.Add(id);
    }

    public void UnPinNode(int id)
    {
        pinnedData.Remove(id);
    }
}
