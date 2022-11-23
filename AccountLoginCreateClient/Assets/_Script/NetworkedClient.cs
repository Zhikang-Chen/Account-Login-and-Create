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

    static public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }



    static public void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
        string[] data = msg.Split(',');

        //Message from server
        if (data[0] == "S")
        {
            if (data[1] == "0")
            {
                //Start game
                GameroomUI.OnStart.Invoke();
            }
            else if (data[1] == "1")
            {
                //End game
                GameroomUI.OnEnd.Invoke();
            }
            else if (data[1] == "2")
            {
                //Update grid
                GridScript.UpdateGridAction(bool.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]));
            }
            else if (data[1] == "3")
            {
                //Gameover
                //bool.Parse(data[2]);
                GameUIScript.OnGameover.Invoke(bool.Parse(data[2]));
            }
            else if (data[1] == "4")
            {
                //Messages
                //GameUIScript.OnGameover.Invoke()
                GameUIScript.OnGetMessageEvent.Invoke(data[2]);
            }
            else if (data[1] == "5")
            {
                //Debug.Log(msg);
                string[] actions = msg.Split("|");

                for (int i = 1; i < actions.Length - 1; i++)
                {
                    //Debug.Log(actions);
                    string[] command = actions[i].Split("@");
                    ProcessRecievedMsg(command[1], connectionID);
                }
            }
            else if (data[1] == "6")
            {
                //string[] actions = msg.Split("|");

                ReplayManager.SaveReplayFiles(GameUIScript.CurrentRoomName, msg);
                //Save stuff from server
            }
        }
        //Reply from server 
        if (data[0] == "0")
        {
            bool successBool = bool.Parse(data[1]);
            //UIManager.OnLogin(successBool);
            LoginUIScript.OnLoginEvent.Invoke(successBool);
        }
        else if (data[0] == "1")
        {
            bool successBool = bool.Parse(data[1]);
            //UIManager.OnAccountCreation(successBool);
            LoginUIScript.OnAccountCreationEvent.Invoke(successBool);
        }
        else if (data[0] == "2")
        {
            bool successBool = bool.Parse(data[1]);
            GameroomSearchUIScript.JoinRoom(successBool);
        }
    }

    static public bool IsConnected()
    {
        return isConnected;
    }
}
