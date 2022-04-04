/**
 * Connects the application to the Photon network and connects players to a room with their avatar head and hands. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using ExitGames.Client.Photon;
using System;

public class Network : MonoBehaviourPunCallbacks
{
    public delegate void PropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged);

    public static event PropertiesChanged RoomPropsChanged;
    public static event Action OnRoomCreated;
    public static event Action OnExistingRoomJoined;
    public static event Action OnPlayersChanged;

    private string room = "VRPathway";
    private string gameVersion = "0.1";

    private bool m_createdRoom = false;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }

        Debug.Log("Connecting...");
    }
    #region CONNECTION
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to master!");
        Debug.Log("Joining room...");

        PhotonNetwork.JoinRoom(room);

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room!");

        if (m_createdRoom)
        {
            Network.OnRoomCreated?.Invoke();
        }
        else
        {
            Network.OnExistingRoomJoined?.Invoke();
        }

        CreatePlayer();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Room join failed " + message);
        m_createdRoom = true;
        Debug.Log("Creating room...");
        PhotonNetwork.CreateRoom(room, new RoomOptions { MaxPlayers = 8, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("Got " + roomList.Count + " rooms.");
        foreach (RoomInfo room in roomList)
        {
            Debug.Log("Room: " + room.Name + ", " + room.PlayerCount);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayersChanged?.Invoke();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }
    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", Vector3.zero, Quaternion.identity, 0);
        GameObject leftHand = player.transform.Find("NetworkHand_L").gameObject;
        GameObject rightHand = player.transform.Find("NetworkHand_R").gameObject;
        leftHand.GetComponent<OVRSkeleton>().SetDataProvider(leftHand);
        rightHand.GetComponent<OVRSkeleton>().SetDataProvider(rightHand);      
        PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[PhotonNetwork.PlayerList.Length - 1]);
    }
    #endregion
    #region ROOM_PROPS

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        OnPlayersChanged?.Invoke();

    }
    public static bool SetCustomPropertySafe(string key, object newValue, WebFlags webFlags = null)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room == null || room.IsOffline)
        {
            return false;
        }

        ExitGames.Client.Photon.Hashtable props = room.CustomProperties;

        if (room.CustomProperties.ContainsKey(key))
        {
            props[key] = newValue;
        }
        else
        {
            props.Add(key, newValue);
        }
        return room.LoadBalancingClient.OpSetCustomPropertiesOfRoom(props/*, oldProps, webFlags);*/);
    }

    public static object GetCurrentRoomCustomProperty(string key)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room == null || room.IsOffline || !room.CustomProperties.ContainsKey(key))
        {
            return null;
        }
        else
        {
            return room.CustomProperties[key];
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        RoomPropsChanged?.Invoke(propertiesThatChanged);
    }
    #endregion
}