using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;


[RequireComponent(typeof(PlayerDataManager))]
public class NetworkedServer : MonoBehaviour
{
    //Singleton
    static private NetworkedServer _instance = null;
    static public NetworkedServer instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    PlayerDataManager playerDataManager;
    GameroomManager roomManager;

    static public int maxConnections = 1000;
    static int reliableChannelID;
    static int unreliableChannelID;
    static int hostID;
    static int socketPort = 5491;

    // Start is called before the first frame update
    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null);

        playerDataManager = GetComponent<PlayerDataManager>();
        roomManager = GetComponent<GameroomManager>();
    }
    // Update is called once per frame
    void Update()
    {
        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error = 0;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection, " + recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                ProcessRecievedMsg(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                roomManager.OnPlayerLeave(recConnectionID);
                playerDataManager.OnPlayerDisconnect(recConnectionID);
                Debug.Log("Disconnection, " + recConnectionID);
                break;
        }
    }

    static public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        //Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
        string[] data = msg.Split(',');
        SendMessageToClient("Data recieved", id);
        string reply = data[0];
        string result = "-1";
        if (data[0] == "0")
        {
            //Login 
            result = string.Format(",{0}", playerDataManager.PlayerLogin(id, data[1], data[2]).ToString());
        }
        else if (data[0] == "1")
        {
            //Create account
            result = string.Format(",{0}", playerDataManager.CreateNewAccount(id, data[1], data[2]).ToString());
        }
        else if (data[0] == "2")
        {
            //Create game room or find game room
            result = string.Format(",{0}", roomManager.CheckForGameroom(id, data[1]));
        }
        else if(data[0] == "3")
        {
            roomManager.OnPlayerLeave(id);
            result = string.Format(",{0}", true);
        }
        SendMessageToClient(reply + result, id);
    }

}
