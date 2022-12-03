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


    public static int connectionID;
    public static int maxConnections = 1000;
    public static int reliableChannelID;
    public static int unreliableChannelID;
    public static int hostID;
    public static int socketPort = 5491;
    public static byte error;
    public static bool isConnected = false;
    public static int ourClientID;
    

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
                    NetworkedClientProcess.ProcessRecievedMsg(msg, recConnectionID);
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
    static public bool IsConnected()
    {
        return isConnected;
    }

    //I have this here so it doesn't mess too much
    static public void SendMessageToHost(string msg)
    {
        NetworkedClientProcess.SendMessageToHost(msg);
    }

}
