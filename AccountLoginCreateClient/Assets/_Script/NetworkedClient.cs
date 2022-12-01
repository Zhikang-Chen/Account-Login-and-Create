using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{
    //Singleton
    static private NetworkedClient _instance = null;
    static public NetworkedClient instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //static LoginUIManager UIManager;

    public static int connectionID;
    static int maxConnections = 1000;
    static int reliableChannelID;
    static int unreliableChannelID;
    static int hostID;
    static int socketPort = 5491;
    static byte error;
    static bool isConnected = false;
    static int ourClientID;

    // Start is called before the first frame update
    void Start()
    {
        //UIManager = GetComponent<LoginUIManager>();
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNetworkConnection();
    }

    static private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
        else
        {
            Connect();
        }
    }

    static private void Connect()
    {
        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            string hostName = Dns.GetHostName();
            string ip = Dns.GetHostByName(hostName).AddressList[1].ToString();
            Debug.Log(ip);

            if (ip != null)
            {
                NetworkTransport.Init();

                ConnectionConfig config = new ConnectionConfig();
                reliableChannelID = config.AddChannel(QosType.Reliable);
                unreliableChannelID = config.AddChannel(QosType.Unreliable);
                HostTopology topology = new HostTopology(config, maxConnections);
                hostID = NetworkTransport.AddHost(topology, 0);
                Debug.Log("Socket open.  Host ID = " + hostID);

                connectionID = NetworkTransport.Connect(hostID, ip, socketPort, 0, out error); // server is local on network

                if (error == 0)
                {
                    isConnected = true;
                    Debug.Log("Connected, id = " + connectionID);
                }
            }
            else
            {
                Debug.Log("Unable to get ip");
            }
        }
    }

    static public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }


    //Move everything below to another class

    static public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    static public void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
        string[] data = msg.Split(',');

        int type = int.Parse(data[0]);
        MessageType messagetype = (MessageType)type;

        //Message from server
        if (messagetype == MessageType.Message)
        {
            int signifier = int.Parse(data[1]);
            ServerToClientMessageSignifiers STCMS = (ServerToClientMessageSignifiers)signifier;

            switch (STCMS)
            {
                case ServerToClientMessageSignifiers.StartGame:
                    GameroomUI.OnStart.Invoke();
                    break;
                case ServerToClientMessageSignifiers.EndGame:
                    GameroomUI.OnEnd.Invoke();
                    break;
                case ServerToClientMessageSignifiers.UpdateGride:
                    GridScript.UpdateGridAction(bool.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]));
                    break;
                case ServerToClientMessageSignifiers.Gameover:
                    GameUIScript.OnGameover.Invoke(bool.Parse(data[2]));
                    break;
                case ServerToClientMessageSignifiers.GetChatMessage:
                    GameUIScript.OnGetMessageEvent.Invoke(data[2]);
                    break;
                case ServerToClientMessageSignifiers.SyncUp:
                    string[] actions = msg.Split("|");

                    for (int i = 1; i < actions.Length - 1; i++)
                    {
                        //Debug.Log(actions);
                        string[] command = actions[i].Split("@");
                        ProcessRecievedMsg(command[1], connectionID);
                    }
                    break;
                case ServerToClientMessageSignifiers.SaveReplay:
                    ReplayManager.SaveReplayFiles(GameUIScript.CurrentRoomName, msg);
                    break;

                default:
                    Debug.LogWarning("Invaild Signifier");
                    break;
            }
        }
        else if(messagetype == MessageType.Reply)
        {
            
            int signifier = int.Parse(data[1]);
            ServerToClientReplySignifiers STCRS = (ServerToClientReplySignifiers)signifier;
            bool successBool = bool.Parse(data[2]);


            switch (STCRS)
            {
                case ServerToClientReplySignifiers.Login:
                    LoginUIScript.OnLoginEvent.Invoke(successBool);
                    break;
                case ServerToClientReplySignifiers.CreateAccount:
                    LoginUIScript.OnAccountCreationEvent.Invoke(successBool);
                    break;
                case ServerToClientReplySignifiers.RoomSearch:
                    GameroomSearchUIScript.JoinRoom(successBool);
                    break;
                default:
                    Debug.LogWarning("Invaild Signifier");
                    break;
            }
        }
    }



    enum ServerToClientMessageSignifiers
    {
        StartGame = 0,
        EndGame = 1,
        UpdateGride = 2,
        Gameover = 3,
        GetChatMessage = 4,
        SyncUp = 5,
        SaveReplay = 6
    }

    enum ServerToClientReplySignifiers
    {
        Login = 0,
        CreateAccount = 1,
        RoomSearch = 2,
    }

    enum MessageType
    {
        Message = 0,
        Reply = 1
    }

    static public bool IsConnected()
    {
        return isConnected;
    }
}
