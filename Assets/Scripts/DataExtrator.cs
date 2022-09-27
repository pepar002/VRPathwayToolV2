using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;
using UnityEngine.Events;

/// <summary>
/// Singleton DataExtractor class
/// Handles sending API requests and fetching data from KEGG database.
/// </summary>
public class DataExtrator : MonoBehaviour
{
    public static DataExtrator Instance { get; private set; }
    public const string URI = "https://rest.kegg.jp";

    private List<string> pathwayIds; // list of entry ids for the pathways related to pyruvate (ko00620)
    private Dictionary<string, XmlDocument> pathwayXmls;

    public Dictionary<string, XmlDocument> PathwayXmls { get => pathwayXmls; }

    public List<string> RelatedPathwayIds { get => pathwayIds; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        pathwayIds = new List<string>();
        pathwayXmls = new Dictionary<string, XmlDocument>();

        // Calls an API request to get pyrvuate pathway
        LoadPyruvatePathway();
    }

    private void Start()
    {
        Debug.Log("Pyruvate Pathway loaded: " + (pathwayIds.Count==13 && pathwayXmls.Count>=1).ToString());
        StartCoroutine(LoadRelatedPathways());
    }

    public void LoadPyruvatePathway() {
        // Get the xmlreponse for the first/default pathway (pyruvate)
        IEnumerator iterator = SendRequest(URI + "/get/ko00620/kgml", GetPathwayXml, true);
        while (iterator.MoveNext())
        {
            if (iterator.Current != null)
                _ = 0; // Do nothing
        }
    }

    // Loads pathways related or connected to pyruvate
    public IEnumerator LoadRelatedPathways() {
        foreach (string entryId in pathwayIds)
        {
            yield return StartCoroutine(SendRequest(String.Format("{0}/get/{1}/kgml", URI, entryId), GetPathwayXml));
        }
    }

    // sends a Web request to the given url
    public IEnumerator SendRequest(string url, UnityAction<string> onSuccess, bool sync = false) {

        #region Old http request code
        using UnityWebRequest request = UnityWebRequest.Get(url);

        // send a synchronous request and wait till response is received
        if (sync == true)
        {
            request.SendWebRequest();
            while (!request.isDone)
                yield return null;
        }
        else
        {
            yield return request.SendWebRequest();
        }


        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(": Error: " + request.error);
                yield return request.error;
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(": HTTP Error: " + request.error);
                Debug.Log(url);
                break;
            case UnityWebRequest.Result.Success:
                string response = request.downloadHandler.text;
                onSuccess(response);
                yield return response;
                break;
        }
        #endregion
    }


    // sends a request to get an image from a url
    public IEnumerator SendImageRequest(string url, UnityAction<Texture2D> onSuccess)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();


        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(": Error: " + request.error);
                Debug.Log("url: " + url);
                yield return request.error;
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(": HTTP Error: " + request.error);
                yield return request.error;
                break;
            case UnityWebRequest.Result.Success:
                Texture2D response = ((DownloadHandlerTexture)request.downloadHandler).texture;
                onSuccess(response);
                //Debug.Log(":\nReceived: " + request.downloadHandler.text);
                yield return request.downloadHandler;
                break;
        }
    }

    // processes the response string into an xml file
    public void GetPathwayXml(string xmlString) {
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(xmlString);

        XmlNode pathway = doc.SelectSingleNode("/pathway");

        // save the xml document for the pathway
        if (!pathwayXmls.ContainsKey(pathway.Attributes["name"].Value[5..])) {
            pathwayXmls.Add(pathway.Attributes["name"].Value[5..], doc);
     
        }

        // if pathway is pyruvate then record ids for all connected pathways
        if (pathway.Attributes["number"].Value == "00620") {
            XmlNodeList entries = doc.SelectNodes("/pathway/entry");
            // record all ids of pathways related to pyruvate metabolism
            foreach (XmlNode entry in entries)
            {
                if (entry.Attributes["type"].Value == "map")
                {
                    string entryId = entry.Attributes["name"].Value[5..];
                    if (!pathwayXmls.ContainsKey(entryId) && !pathwayIds.Contains(entryId)) {
                        pathwayIds.Add(entryId);                        
                    }
                }

            }
        }


    }
}
