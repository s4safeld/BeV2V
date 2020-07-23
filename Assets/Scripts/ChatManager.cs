using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.IO;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    int subscribedChannels = 0;
    bool vrReady = false;
    bool initialised = false;

    string currentChannel;

    [SerializeField]
    private GameObject XRSet;

    [SerializeField]
    private Text NonVRChatDisplay;
    [SerializeField]
    private InputField NonVRChatInput;
    [SerializeField]
    private Button NonVRSendButton;
    [SerializeField]
    private Canvas NonVRChatCanvas;

    [SerializeField]
    private Text VRChatDisplay;
    [SerializeField]
    private InputField VRChatInput;
    [SerializeField]
    private Button VRSendButton;
    [SerializeField]
    private Canvas VRChatCanvas;

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.LogWarning("DebugReturn called with parameters: < Debuglevel:" + level + " and message: " + message + " > but was not implemented");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.LogWarning("OnChatStateChange called with parameters: < state:" + state + " > but was not implemented");
    }

    public void OnConnected()
    {
        currentChannel = PhotonNetwork.CurrentRoom.Name+"Chat";
        chatClient.Subscribe(currentChannel);
        
        if (vrReady)
        {
            VRChatDisplay.text += "Connected to: "+PhotonNetwork.CurrentRoom;
        }
        else
        {
            NonVRChatDisplay.text += "Connected to: " + PhotonNetwork.CurrentRoom;
        }
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
        if (vrReady)
        {
            VRChatDisplay.text += "Discconnected from: " + PhotonNetwork.CurrentRoom;
        }
        else
        {
            NonVRChatDisplay.text += "Disconnected from: " + PhotonNetwork.CurrentRoom;
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i<senders.Length; i++)
        {
            msgs += string.Format("\n{0}{1}: {2}", msgs, senders[i], messages[i]);
        }
        if (!vrReady)
        {
            NonVRChatDisplay.text += msgs;
        }
        else
        {
            VRChatDisplay.text += msgs;
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.LogWarning("OnPrivateMessage Called with parameters: < sender:" + sender + 
                                                                    ", message: " + message + 
                                                                    ", channelName:  " + channelName + 
                                                                    " > but was not implemented");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.LogWarning("OnStatusUpdate Called with parameters: < user: "+ user +
                                                                    ", status: "+status+
                                                                    ", gotMessage: "+gotMessage+
                                                                    ", message: " + message +
                                                                    "> but was not implemented");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (string channel in channels)
        {
            if (vrReady)
            {
                VRChatDisplay.text += "\n\nNow typing in channel: " + channel + "... \n ";
                VRSendButton.interactable = true;
                VRChatInput.interactable = true;
            }
            else
            {
                NonVRChatDisplay.text += "\n\nNow typing in channel: " + channel + "... \n ";
                NonVRSendButton.interactable = true;
                NonVRChatInput.interactable = true;
            }
            Debug.Log("joined channel: " + channel);
            subscribedChannels++;
            chatClient.PublishMessage(channel, "joined...");
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        foreach (string channel in channels)
        {
            Debug.Log("left channel: " + channel);
            subscribedChannels--;
            chatClient.PublishMessage(channel, "left...");
            if (vrReady)
            {
                VRChatDisplay.text += "left channel: " + channel;
                VRSendButton.interactable = false;
                VRChatInput.interactable = false;
            }
            else
            {
                NonVRChatDisplay.text += "left channel: " + channel;
                NonVRSendButton.interactable = false;
                NonVRChatInput.interactable = false;
            }
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //chatClient.PublishMessage(channel, "joined...");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //chatClient.PublishMessage(channel, "left...");
    }

    // Start is called before the first frame update
    public void Initialise()
    {
        if (!initialised)
        {
            initialised = true;

            vrReady = GlobalInformation.vrReady;
            chatClient = new ChatClient(this);
            chatClient.ChatRegion = "EU";
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.NickName));

            if (vrReady)
            {
                VRChatCanvas.gameObject.SetActive(true);
            }
            else
            {
                NonVRChatCanvas.gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (initialised)
        {
            chatClient.Service();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Send();
            }
        }
    }

    public void Send()
    {
        string message;
        if (!vrReady)
        {
            message = NonVRChatInput.text;
            NonVRChatInput.text = null;
        }
        else
        {
            message = VRChatInput.text;
            VRChatInput.text = null;
        }
        ResumeMovement();
        chatClient.PublishMessage(currentChannel, message);
    }
    public void BlockMovement()
    {
        XRSet.GetComponent<Movement>().blocked = true;
        Debug.Log("Movement blocked");
    }
    public void ResumeMovement()
    {
        XRSet.GetComponent<Movement>().blocked = false;
        Debug.Log("Movement resumed");
    }
}
