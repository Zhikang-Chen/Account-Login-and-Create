using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameroomManager : MonoBehaviour
{

    [System.Serializable]
    public class Game
    {
        public enum State
        {
            Empty = 0,
            Player1 = 1,
            Player2 = 2
        }
        public List<List<State>> PositionData = new List<List<State>>();

        public bool IsPlayer1Turn = true;

        public GameroomData Gameroom = null;

        public Game(GameroomData room)
        {
            Gameroom = room;
            for (int i = 0; i < 3; i++)
            {
                List<State> Col = new List<State>();
                for (int i2 = 0; i2 < 3; i2++)
                {
                    Col.Add(0);
                }
                PositionData.Add(Col);
            }
            Debug.Log(PositionData);
        }

        public void CheckForWinner()
        {
            if ((PositionData[0][0] != 0) &&
                (PositionData[0][0] == PositionData[1][1]) &&
                (PositionData[1][1] == PositionData[2][2]))
            {
                OnGameover(PositionData[0][0]);
            }
            else if ((PositionData[2][0] != 0) &&
                    (PositionData[2][0] == PositionData[1][1]) &&
                    (PositionData[1][1] == PositionData[0][2]))
            {
                OnGameover(PositionData[2][0]);
            }

            for (int i = 0; i < 3; i++)
            {
                //Vertical
                if ((PositionData[i][0] != 0) &&
                    (PositionData[i][0] == PositionData[i][1]) &&
                    (PositionData[i][1] == PositionData[i][2]))
                {
                    OnGameover(PositionData[i][0]);
                }

                //Horizontal
                else if ((PositionData[0][i] != 0) &&
                        (PositionData[0][i] == PositionData[1][i]) &&
                        (PositionData[1][i] == PositionData[2][i]))
                {
                    OnGameover(PositionData[0][i]);
                }
            }
        }
        public bool OnPlayerAction(int row, int col)
        {
            if (PositionData[row][col] == State.Empty)
            {
                if (IsPlayer1Turn)
                {
                    PositionData[row][col] = State.Player1;
                    IsPlayer1Turn = !IsPlayer1Turn;
                    CheckForWinner();
                    return true;
                }
                else
                {
                    PositionData[row][col] = State.Player2;
                    IsPlayer1Turn = !IsPlayer1Turn;
                    CheckForWinner();
                    return true;
                }
            }
            return false;
        }

        public void OnGameover(State winnter)
        {
            string address = "S,3,";
            switch (winnter)
            {
                case State.Empty:
                    break;
                case State.Player1:
                    NetworkedServer.SendMessageToClient(address + true.ToString(), Gameroom.Player1);
                    NetworkedServer.SendMessageToClient(address + false.ToString(), Gameroom.Player2);
                    break;
                case State.Player2:
                    NetworkedServer.SendMessageToClient(address + false.ToString(), Gameroom.Player1);
                    NetworkedServer.SendMessageToClient(address + true.ToString(), Gameroom.Player2);
                    break;
            }
        }
    }

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

        public Game CurrentGame = null;

        public bool OnPlayerAction(int id, int row, int col)
        {
            bool result = false;
            string address = "S,2";
            string message = string.Format(",{0},{1},{2}", CurrentGame.IsPlayer1Turn, row, col);
            if (Player1 == id && CurrentGame.IsPlayer1Turn)
            {
                result = CurrentGame.OnPlayerAction(row, col);
            }
            else if(Player2 == id && !CurrentGame.IsPlayer1Turn)
            {
                result = CurrentGame.OnPlayerAction(row, col);
            }

            if (result)
            {
                NetworkedServer.SendMessageToClient(address + message, Player1);
                NetworkedServer.SendMessageToClient(address + message, Player2);
            }

            return result;
        }
    }

    //Using list because it's serializable
    //Change to LinkedList later
    [SerializeField]
    private List<GameroomData> Gamerooms = new List<GameroomData>();
    private int MaxRoom = NetworkedServer.maxConnections / 2;

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
        room.CurrentGame = new Game(room);
        string address = "S,0";
        string msg = true.ToString();
        NetworkedServer.SendMessageToClient(address, id);
    }

    public bool OnPlayerAction(int id, string GameroomName, int row, int col)
    {
        foreach (var data in Gamerooms)
        {
            if (data.RoomName == GameroomName)
            {
                //data.OnPlayerAction(id, row, col);
                return data.OnPlayerAction(id, row, col);
            }
        }
        return false;
    }

    
}