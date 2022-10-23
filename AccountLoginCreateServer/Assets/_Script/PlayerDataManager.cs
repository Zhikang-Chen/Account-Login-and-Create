using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerDataManager : MonoBehaviour
{
    private class PlayerData
    {
        public string User;
        public string Password;
        public int ConnectionID;
        
    }

    //public class ConnectionData
    //{
    //    PlayerData Player;
    //    //Gameroom
    //    //Etc
    //}

    // I SPENT HALF AN HOUR MAKING A LINKED LIST TURNS OUT THIS IS A THING
    // I AM SO DUMB
    [SerializeField]
    LinkedList<PlayerData> Connections = new LinkedList<PlayerData>();

    //List<PlayerData> Accounts = new List<PlayerData>();

    private void Awake()
    {
        string fileName = "PlayerDataFile.csv";
        StreamReader streamReader = new StreamReader(fileName);
        string line;
        line = streamReader.ReadLine();
        while(line != null)
        {
            string[] loginData = line.Split(',');
            PlayerData Data = new PlayerData();
            Data.User = loginData[0];
            Data.Password = loginData[1];
            Data.ConnectionID = -1;
            Connections.AddLast(Data);
            line = streamReader.ReadLine();
        }

        //if (line == null)
        //{
        //    return;
        //}

        ////Starting a link list
        //string[] loginData = line.Split(',');
        //Accounts = new PlayerData();
        //Accounts.User = loginData[0];
        //Accounts.Password = loginData[1];
        //line = streamReader.ReadLine();


        //PlayerData currentData = null;
        //PlayerData previousData = Accounts;

        ////Adding to the link list
        //while (line != null)
        //{
        //    loginData = line.Split(',');
        //    currentData = new PlayerData();
        //    currentData.User = loginData[0];
        //    currentData.Password = loginData[1];
        //    currentData.NextData = previousData;
        //    previousData = currentData;
        //    line = streamReader.ReadLine();
        //}

        //currentData = null;
        //previousData = null;

        //while(Accounts.NextData != null)
        //{

        //}
    }
    public void OnPlayerDisconnect(int id)
    {
        //Player.Remove(id);
    }

    public bool PlayerLogin(int id,string user, string pass)
    {
        //if(Player.ContainsKey(id))
        //{
        //    return false;
        //}


        //string fileName = "PlayerDataFile.csv";
        //StreamReader sr = new StreamReader(fileName);
        //string line;
        //line = sr.ReadLine();

        foreach (var data in Connections)
        {
            if (data.User == user)
            {
                if (data.Password == pass)
                {
                    data.ConnectionID = id;
                    return true;
                }
            }
        }

        //while (line != null)
        //{
        //    string[] loginData = line.Split(',');
        //    if(loginData.Length == 2)
        //    {
        //        if(loginData[0] == user)
        //        {
        //            if (loginData[1] == pass)
        //            {
        //                sr.Close();
        //                return true;
        //            }
        //            else
        //            {
        //                sr.Close();
        //                return false;
        //            }
        //        }

        //    }


        //    line = sr.ReadLine();
        //}

        //sr.Close();
        return false;
    }

    public bool CreateNewAccount(int id, string user, string pass)
    {
        // This is so bad
        // It would be way better to append it instead
        // Why can't I use it like in python
        // This is actually hurting me
        // AAAAAAAAAAAAAAAAAA

        string fileName = "PlayerDataFile.csv";
        StreamReader sr = new StreamReader(fileName);
        List<string> allData = new List<string>();
        string line;
        line = sr.ReadLine();
        while (line != null)
        {
            allData.Add(line);
            if(line.Split(',')[0] == user)
            {
                sr.Close();
                return false;
            }
            line = sr.ReadLine();
        }
        sr.Close();

        StreamWriter sw = new StreamWriter(fileName);
        foreach(string i in allData)
        {
            sw.WriteLine(i);
        }
        string newAccount = string.Format("{0},{1}", user, pass);
        sw.WriteLine(newAccount);
        sw.Close();

        PlayerData Data = new PlayerData();
        Data.User = user;
        Data.Password = pass;
        Data.ConnectionID = -1;
        Connections.AddLast(Data);

        return true;
    }
}
