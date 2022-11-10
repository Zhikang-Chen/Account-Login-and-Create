using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameroomManager : MonoBehaviour
{
    [System.Serializable]
    public class GameroomData
    {
        public enum RoomState
        {
            Empty = 0,
            Waiting = 1,
            Playing = 2
        }

        public RoomState CurrentState = RoomState.Empty;
        public string RoomName;


        public int Player1 = -1;
        public bool Player1Ready = false;

        public int Player2 = -1;
        public bool Player2Ready = false;
    }

    //Using list because it's serializable
    //Change to LinkedList later
    [SerializeField]
    private List<GameroomData> Gamerooms = new List<GameroomData>();
    private int MaxRoom = NetworkedServer.maxConnections / 2;

    private void Awake()
    {
        //MakeRooms();
    }

    public void MakeRooms()
    {
        for (int i = 0; i < MaxRoom; i++)
        {
            GameroomData newRoom = new GameroomData();
            Gamerooms.Add(newRoom);
        }
    }

    public void OnPlayerLeave(int id)
    {
        foreach (var room in Gamerooms)
        {
            if (room.Player1 == id)
            {
                room.Player1 = -1;
                room.Player1Ready = false;

                if (room.CurrentState == GameroomData.RoomState.Playing)
                {
                    EndGame(room.Player2, room);
                }
                //StartGame(id, room);
            }

            if (room.Player2 == id)
            {
                room.Player2 = -1;
                room.Player2Ready = false;

                if(room.CurrentState == GameroomData.RoomState.Playing)
                {
                    EndGame(room.Player1, room);
                }

                //StartGame(id, room);
            }

            if (room.Player1 == -1 && room.Player2 == -1)
            {
                Gamerooms.Remove(room);
                break;
            }
        }
    }

    public bool CheckForGameroom(int id, string name)
    {
        foreach (var data in Gamerooms)
        {
            if (data.RoomName == name)
            {
                if (data.Player2 == -1)
                {
                    //Connections.Remove(data);
                    data.Player2 = id;
                    if (data.Player1 != -1)
                    {
                        StartGame(data.Player1, data);
                        StartGame(data.Player2, data);
                        data.CurrentState = GameroomData.RoomState.Playing;
                        return true;
                    }
                    return false;
                }
                else if(data.Player1 == -1)
                {
                    data.Player1 = id;
                    if(data.Player2 != -1)
                    {
                        StartGame(data.Player1, data);
                        StartGame(data.Player2, data);
                        data.CurrentState = GameroomData.RoomState.Playing;
                        return true;
                    }
                    return false;
                }
            }
        }

        GameroomData newRoom = new GameroomData();
        newRoom.RoomName = name;
        newRoom.Player1 = id;
        Gamerooms.Add(newRoom);
        return false;
    }

    //public void OnReadyUp(int id, GameroomData room)
    //{
    //    if (room.Player1 == id)
    //    {
    //        room.Player1Ready = true;
    //    }
    //    else if (room.Player2 == id)
    //    {
    //        room.Player2Ready = true;
    //    }
    //}

    public void EndGame(int id, GameroomData room)
    {
        string address = "S,1";
        string msg = true.ToString();
        NetworkedServer.SendMessageToClient(address, id);
    }

    public void StartGame(int id, GameroomData room)
    {
        string address = "S,0";
        string msg = true.ToString();
        NetworkedServer.SendMessageToClient(address, id);
    }
    //public void StartMatch(int id, GameroomData room)
    //{

    //    string address = "S, 1";
    //    NetworkedServer.SendMessageToClient(address, id);
    //}
}
