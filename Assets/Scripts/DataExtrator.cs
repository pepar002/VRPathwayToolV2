using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;
using UnityEngine.Events;

public class DataExtrator : MonoBehaviour
{
    public static DataExtrator Instance { get; private set; }
    public const string URI = "https://rest.kegg.jp";

    private List<string> pathwayIds; // list of entry ids for the pathways related to pyruvate
    private Dictionary<string, XmlDocument> pathwayXmls;
    private Dictionary<string, string> pathwayKeys;


    public Dictionary<string, XmlDocument> PathwayXmls { get => pathwayXmls; }

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


    }

    private void Start()
    {
        Debug.Log("Pyruvate Pathway loaded: " + (pathwayIds.Count==13).ToString());
        Debug.Log(String.Format("Pathways size: {0}",pathwayXmls.Count));

        //LoadRelatedPathways();
    }

    public void LoadPyruvatePathway() {
        // Get the xmlreponse for the first/default pathway (pyruvate)
        IEnumerator iterator = SendRequest(URI + "/get/ko00620/kgml", GetPathwayXml);
        while (iterator.MoveNext())
        {
            if (iterator.Current != null)
                Debug.Log(iterator.Current);
        }
    }
    public void LoadRelatedPathways() {
        foreach (string entryId in pathwayIds)
        {
            StartCoroutine(SendRequest(String.Format("{0}/get/{1}/kgml", URI, entryId), GetPathwayXml, false));
        }

        StartCoroutine(WaitUntilPathwaysLoaded());
    }

    private void ProcessAllPathwayKeys()
    {
        foreach (string entryId in pathwayIds)
        {
            string[] pathwayContent = getPathwayContent(pathwayXmls[entryId]).ToArray();
            string contentURL ="";
            int size = pathwayContent.Length;
            int indexStart = 0;
            while (size > 100) {
                contentURL = URI + "/list/" + string.Join("+", pathwayContent, 0, 100);
                StartCoroutine(SendKeyRequest(contentURL, entryId, ProcessKeys));
                indexStart += 100;
                size -= 100;
            }

            contentURL = URI + "/list/" + string.Join("+", pathwayContent, indexStart, size);

            StartCoroutine(SendKeyRequest(contentURL, entryId, ProcessKeys));
        }

    }

    public IEnumerator SendRequest(string url, UnityAction<string> onSuccess, bool sync = true) {

        #region Old http request code
        using UnityWebRequest request = UnityWebRequest.Get(url);
        //Debug.Log("In coroutine with: " + url);


        //request.SendWebRequest();

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
                //yield return SendRequest(url, onSuccess, false);
                break;
            case UnityWebRequest.Result.Success:
                string response = request.downloadHandler.text;
                onSuccess(response);
                //Debug.Log(":\nReceived: " + response);
                yield return response;
                break;
        }
        #endregion
    }

    // send requests to ket key value pairs for pathway content
    // only happens when all related pathway xmls are present
    public IEnumerator SendKeyRequest(string url, string pathId, UnityAction<string, string> onSuccess) {


        using UnityWebRequest request = UnityWebRequest.Get(url);
        //yield return request.SendWebRequest();

        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(": Error: " + request.error);
                yield return request.error;
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(": HTTP Error: " + request.error);
                yield return request.error;
                break;
            case UnityWebRequest.Result.Success:
                string response = request.downloadHandler.text;
                onSuccess(pathId, response);
                //Debug.Log(":\nReceived: " + response);
                yield return response;
                break;
        }

    }

    private IEnumerator WaitUntilPathwaysLoaded()
    {
        yield return new WaitUntil(() => pathwayXmls.Count >= 14);

        yield return null;

        Debug.Log(pathwayXmls.Count +" " + pathwayIds.Count);

        ProcessAllPathwayKeys();
    }

    public IEnumerator SendImageRequest(string url, UnityAction<Texture2D> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url)){

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
                    Debug.Log(":\nReceived: " + response);
                    yield return request.downloadHandler.text;
                    break;
            }

        }
    }

    public void GetPathwayXml(string xmlString) {
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(xmlString);

        XmlNode pathway = doc.SelectSingleNode("/pathway");

        // save the xml document for the pathway
        if (!pathwayXmls.ContainsKey(pathway.Attributes["name"].Value[5..])) {
            pathwayXmls.Add(pathway.Attributes["name"].Value[5..], doc);
     
        }

        if (pathway.Attributes["number"].Value == "00620") {
            XmlNodeList entries = doc.SelectNodes("/pathway/entry");
            Debug.Log("In Data Extractor: "+entries.Count);
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

    private void ProcessKeys(string pathwayEntry, string key) {
        // TO DO process the received key text (append if already exists for each pathway)
    }

    // gets all compounds and enzymes entry ids from the pathway kml file
    // TODO can be done in getpathwayxml function (code reuse)
    private List<string> getPathwayContent(XmlDocument doc) {
        List<string> content = new List<string>();

        if (doc!= null) {
            XmlNodeList entries = doc.SelectNodes("/pathway/entry");

            foreach (XmlNode entry in entries) {
                if (entry.Attributes["type"].Value == "ortholog" || entry.Attributes["type"].Value == "compound")
                {
                    //Debug.Log(doc.SelectSingleNode("/pathway").Attributes["name"].Value + entry.Attributes["name"].Value);
                    content.Add(entry.Attributes["name"].Value.Split(':', ' ')[1]);
                }
            }
        }

        return content;
    }
}
