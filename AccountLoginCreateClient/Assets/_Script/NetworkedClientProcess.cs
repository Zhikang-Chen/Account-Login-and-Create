using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClientProcess : MonoBehaviour
{
    static public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(NetworkedClient.hostID, NetworkedClient.connectionID, NetworkedClient.reliableChannelID, buffer, msg.Length * sizeof(char), out NetworkedClient.error);
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
                        ProcessRecievedMsg(command[1], NetworkedClient.connectionID);
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
        else if (messagetype == MessageType.Reply)
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
}
