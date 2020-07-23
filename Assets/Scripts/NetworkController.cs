using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Linq;

public class NetworkController : MonoBehaviourPunCallbacks
{

    public int RoomSize;
    public int multiplayerSceneIndex;
    public TextMeshPro logtext;
    public GameObject XR;
    public Vector3 spawnPos;
    public GameObject leftHandP;
    public GameObject rightHandP;
    public GameObject headP;
    public string roomName;

    //GameObject world;
    //GameObject carafe;
    //GameObject guitar;
    //GameObject mondrian;
    //GameObject typewriter;

    //private bool joined = false;


    // Start is called before the first frame update
    void Start()
    {
        roomName = GlobalInformation.currScene+"room1";
        //DontDestroyOnLoad(this);
        PhotonNetwork.ConnectUsingSettings(); //connecting to Master Server
        spawnPos.Set(0, 1, 0);
        Debug.Log("Vr activated: " + GlobalInformation.vrReady);
        Debug.LogWarning("Sendrate: " + PhotonNetwork.SendRate + "\nSerializationRate: " + PhotonNetwork.SerializationRate);


    }

    //Is called once connected to Master Server
    public override void OnConnectedToMaster()
    {
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
        logtext.text = "We are now connected to the " + PhotonNetwork.CloudRegion + " server!";
        PhotonNetwork.JoinRoom(roomName);
        PhotonNetwork.NickName = GlobalInformation.username;
    }

    //Called if joining failed, probably because there is no room
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room");
        logtext.text = "Failed to join random room";
        CreateRoom();   //Creating new room because no other can be found
    }

    void CreateRoom()
    {
        Debug.Log("Creating new Room now!");
        logtext.text = "Creating new Room now";
        RoomOptions roomops = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom(roomName, roomops);   //creating room with aforementioned parameters
        Debug.Log("Room has been created");
        logtext.text = "Room has been created";

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... something is wrong");
        logtext.text = "Failed to create room... something is wrong";
    }

    //---------------------------------------------------

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom);
        logtext.text = "Joined room: " + PhotonNetwork.CurrentRoom;

        if (PhotonNetwork.IsMasterClient)
        {
            //GameObject world = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "World"), new Vector3(2, -100, 0), Quaternion.identity);
            // GameObject guitar = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Guitar"), new Vector3(0, -100, 0), Quaternion.identity);
            // GameObject carafe = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Carafe"), new Vector3(-2, -100, 0), Quaternion.identity);
            //GameObject beCube = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "BejoyntCube"), spawnPos, Quaternion.identity);
            //GameObject carafeSpawner = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "CarafeSpawner"), new Vector3(1,1,-1), Quaternion.identity);
            //carafeSpawner.transform.localRotation = new Quaternion(0,180,0,0);
            //GameObject worldSpawner = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "WorldSpawner"), new Vector3(0,1,-1), Quaternion.identity);
            //worldSpawner.transform.localRotation = new Quaternion(0, -90, 0, 0);
            //GameObject guitarSpawner = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "GuitarSpawner"), new Vector3(-1, 1, -1), Quaternion.identity);
            //guitarSpawner.transform.localRotation = new Quaternion(0, 90, 0, 0);
            Debug.Log("Starting Game");
            logtext.text = "Starting Game";
        }
        logtext.text = "Sucessfully joined room: " + PhotonNetwork.CurrentRoom;

        GameObject head = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "head"), Vector3.zero, Quaternion.identity);
        head.transform.parent = headP.transform;
        //head.transform.parent = FindObjectsOfType<WebXRCamera>().First(camera => camera.isActiveAndEnabled).transform;
        head.transform.localPosition = Vector3.zero;
        head.transform.localRotation = Quaternion.identity;
        //head.GetComponent<PhotonView>().RequestOwnership();

        Debug.Log("NetworkController VRready: " + GlobalInformation.vrReady);
        if (GlobalInformation.vrReady)
        {
            GameObject handLeft = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "hand_left"), Vector3.zero, Quaternion.identity);
            handLeft.transform.parent = leftHandP.transform;
            handLeft.transform.localPosition = Vector3.zero;
            handLeft.transform.localRotation = Quaternion.identity;
            //handLeft.GetComponent<PhotonView>().RequestOwnership();

            GameObject handRight = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "hand_right"), Vector3.zero, Quaternion.identity);
            handRight.transform.parent = rightHandP.transform;
            handRight.transform.localPosition = Vector3.zero;
            handRight.transform.localRotation = Quaternion.identity;
            //handRight.GetComponent<PhotonView>().RequestOwnership();
        }
        else
        {
            GameObject crosshair = PhotonNetwork.Instantiate(Path.Combine("Prefabs","crosshair"), Vector3.zero, Quaternion.identity);
            crosshair.transform.parent = XR.transform.GetChild(0).GetChild(0).transform;
            crosshair.transform.localRotation = Quaternion.identity;
            crosshair.transform.localPosition = new Vector3(0,0,1f);
        }

        XR.GetComponent<Movement>().Initialise();
        FindObjectsOfType<GameObject>().First(obj => obj.name == "ChatManager").GetComponent<ChatManager>().Initialise();
        Debug.Log("Initialise Function called");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected cause: " + cause);
        logtext.text = "Disconnected, cause: " + cause;
        base.OnDisconnected(cause);
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer + " sucessfully joined " + PhotonNetwork.CurrentRoom);
        logtext.text = ("New Player '"+ newPlayer.NickName + "' sucessfully joined.\n" + PhotonNetwork.CurrentRoom);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer + " left room " + PhotonNetwork.CurrentRoom);
        logtext.text = ("Player '" + otherPlayer.NickName + "' left room.\n" + PhotonNetwork.CurrentRoom);
    }

    //------------------------------------------------------------

}
