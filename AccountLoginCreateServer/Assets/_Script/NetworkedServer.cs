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


    static public PlayerDataManager playerDataManager;
    static public GameroomManager roomManager;

    static public int maxConnections = 1000;
    static public int reliableChannelID;
    static public int unreliableChannelID;
    static public int hostID;
    static public int socketPort = 5491;

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
                NetworkedServerProcess.ProcessRecievedMsg(msg, recConnectionID);
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
        NetworkTransport.Send(NetworkedServer.hostID, id, NetworkedServer.reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    static public void SendMessageToClient(NetworkedServerProcess.MessageType type, NetworkedServerProcess.ServerToClientMessageSignifiers signifier, string msg, int id)
    {
        string fullMsg = ((int)type).ToString() + "," + ((int)signifier).ToString() + "," + msg;
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(fullMsg);
        NetworkTransport.Send(NetworkedServer.hostID, id, NetworkedServer.reliableChannelID, buffer, fullMsg.Length * sizeof(char), out error);
    }

    static public void SendMessageToClient(NetworkedServerProcess.MessageType type, NetworkedServerProcess.ServerToClientMessageSignifiers signifier, int id)
    {
        string fullMsg = ((int)type).ToString() + "," + ((int)signifier).ToString();
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(fullMsg);
        NetworkTransport.Send(NetworkedServer.hostID, id, NetworkedServer.reliableChannelID, buffer, fullMsg.Length * sizeof(char), out error);
    }

}
