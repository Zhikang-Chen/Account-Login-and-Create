using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class NetworkedServerProcess : MonoBehaviour
{
    static public void ProcessRecievedMsg(string msg, int id)
    {
        //Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
        string[] data = msg.Split(',');
        //SendMessageToClient("Data recieved", id);
        string reply = data[0];
        string result = "-1";
        if (data[0] == "0")
        {
            //Login 
            result = string.Format(",{0}", NetworkedServer.playerDataManager.PlayerLogin(id, data[1], data[2]).ToString());
        }
        else if (data[0] == "1")
        {
            //Create account
            result = string.Format(",{0}", NetworkedServer.playerDataManager.CreateNewAccount(id, data[1], data[2]).ToString());
        }
        else if (data[0] == "2")
        {
            //Create game room or find game room
            result = string.Format(",{0}", NetworkedServer.roomManager.CheckForGameroom(id, data[1]));
        }
        else if (data[0] == "3")
        {
            NetworkedServer.roomManager.OnPlayerLeave(id);
            result = string.Format(",{0}", true);
        }
        else if (data[0] == "4")
        {
            result = string.Format(",{0}", NetworkedServer.roomManager.OnPlayerAction(id, data[1], int.Parse(data[2]), int.Parse(data[3])));
        }
        else if (data[0] == "5")
        {
            result = string.Format(",{0}", NetworkedServer.roomManager.OnPlayerMessage(id, data[1], data[2]));
        }
        else if (data[0] == "6")
        {
            result = string.Format(",{0}", NetworkedServer.roomManager.SyncUp(id, data[1]));
        }
        NetworkedServer.SendMessageToClient("1," + reply + result, id);
    }

    public enum ServerToClientMessageSignifiers
    {
        StartGame = 0,
        EndGame = 1,
        UpdateGride = 2,
        Gameover = 3,
        GetChatMessage = 4,
        SyncUp = 5,
        SaveReplay = 6
    }

    public enum ServerToClientReplySignifiers
    {
        Login = 0,
        CreateAccount = 1,
        RoomSearch = 2,
    }

    public enum MessageType
    {
        Message = 0,
        Reply = 1
    }
}
