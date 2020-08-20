using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.IO;
using System;

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
    private Button VRKeyboardToggle;
    [SerializeField]
    private Canvas VRChatCanvas;
    [SerializeField]
    private GameObject keyboard;

    private bool keyboardActive;

    void Start()
    {
        keyboard.SetActive(false);
        keyboardActive = false;
        GlobalInformation.chatmanager = this;
    }

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

    public void UserDisconnected(string user)
    {
        Debug.Log("Disconnected");
        if (vrReady)
        {
            VRChatDisplay.text += string.Format("\n{0} left...", user);
        }
        else
        {
            NonVRChatDisplay.text += string.Format("\n{0} left...", user);
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i<senders.Length; i++)
        {
            msgs += string.Format("{0}\n{1}: {2}", msgs, senders[i], messages[i]);
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
                VRKeyboardToggle.interactable = true;
            }
            else
            {
                NonVRChatDisplay.text += "\n\nNow typing in channel: " + channel + "... \n ";
                NonVRSendButton.interactable = true;
                NonVRChatInput.interactable = true;
            }
            Debug.Log("joined channel: " + channel);
            subscribedChannels++;
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
                VRKeyboardToggle.interactable = false;
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
        //chatClient.PublishMessage(user, "left...");
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
        message = NonVRChatInput.text;
        NonVRChatInput.text = null;
        ResumeMovement();
        Debug.Log(message);
        chatClient.PublishMessage(currentChannel, message);
    }

    public void Send(string message)
    {
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
        if(!GlobalInformation.mobileReady)
            XRSet.GetComponent<Movement>().blocked = false;
        Debug.Log("Movement resumed");
    }
    public void toggleKeyboard()
    {
        Debug.Log("toggle Keyboard called");
        if (!keyboardActive)
        {
            showKeyboard();
            VRKeyboardToggle.GetComponentInChildren<Text>().text = "Hide Keyboard";
        }
        else
        {
            hideKeyboard();
            VRKeyboardToggle.GetComponentInChildren<Text>().text = "Show Keyboard";
        }
    }
    public void showKeyboard() {
        keyboardActive = true;
        keyboard.SetActive(true);
    }
    public void hideKeyboard() {
        keyboardActive = false;
        keyboard.SetActive(false);
    }
    public void showMobileKeyboard()
    {
        try { TouchScreenKeyboard.Open(NonVRChatInput.text, default); } catch (Exception e) { Debug.Log("Could not open mobiel keyboard: " + e); }
    }
    public void hideMobileKeyboard()
    {

    }
}
