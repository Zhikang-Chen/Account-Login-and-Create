using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerDataManager : MonoBehaviour
{
    [System.Serializable]
    public class ConnectionData
    {
        public string User;
        public int ConnectID;
        public GameroomManager.GameroomData CurrentRoom = null;
    }


    //It's going to be pain to do deleting account
    private List<string> User = new List<string>();
    private List<string> Password = new List<string>();


    private LinkedList<ConnectionData> Connections = new LinkedList<ConnectionData>();

    private void Awake()
    {
        string fileName = "PlayerDataFile.csv";
        StreamReader streamReader = new StreamReader(fileName);
        string line;
        line = streamReader.ReadLine();
        while(line != null)
        {
            string[] loginData = line.Split(',');
            User.Add(loginData[0]);
            Password.Add(loginData[1]);
            line = streamReader.ReadLine();
        }
    }

    public void OnPlayerDisconnect(int id)
    {
        foreach (var data in Connections)
        {
            if (data.ConnectID == id)
            {
                Connections.Remove(data);
                return;
            }
        }
    }

    public bool PlayerLogin(int id,string user, string pass)
    {
        //No way to check is the user has already login
        //or check if the connection id has been in use


        for (int i = 0; i < User.Count; i++)
        {
            if (Password[i] == pass)
            {
                ConnectionData Data = new ConnectionData();
                Data.User = user;
                Data.ConnectID = id;
                Connections.AddLast(Data);
                return true;
            }
        }
        return false;
    }

    public bool CreateNewAccount(int id, string user, string pass)
    {
        if (User.Contains(user))
        {
            return false;
        }

        string fileName = "PlayerDataFile.csv";
        StreamReader sr = new StreamReader(fileName);
        List<string> allData = new List<string>();
        string line;
        line = sr.ReadLine();

        //Need to get everything inorder to append to the file
        //Might as well do the checking
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

        //Appending to the file
        //Maybe have some sort of buffer so it doesn't just delete everything if something goes wrong
        StreamWriter sw = new StreamWriter(fileName);
        foreach(string i in allData)
        {
            sw.WriteLine(i);
        }
        string newAccount = string.Format("{0},{1}", user, pass);
        sw.WriteLine(newAccount);
        sw.Close();

        //Add the new account to the list
        User.Add(user);
        Password.Add(pass);

        return true;
    }

    //WHY DID THE MADE THE ACCOUNT SYSTEM THIS WAY
    //I REGRET EVERYTHING
    public bool DeleteAccount(int id, string user, string pass)
    {
        return false;
    }
}
