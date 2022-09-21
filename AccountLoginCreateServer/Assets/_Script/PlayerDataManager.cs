using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerDataManager : MonoBehaviour
{
    public bool PlayerLogin(string user, string pass)
    {
        string fileName = "PlayerDataFile.csv";
        StreamReader sr = new StreamReader(fileName);
        string line;
        line = sr.ReadLine();
        while (line != null)
        {
            string[] loginData = line.Split(',');
            if(loginData.Length == 2)
            {
                if(loginData[0] == user)
                {
                    if (loginData[1] == pass)
                    {
                        sr.Close();
                        return true;
                    }
                    else
                    {
                        sr.Close();
                        return false;
                    }
                }

            }


            line = sr.ReadLine();
        }
        sr.Close();
        return false;
    }

    public bool CreateNewAccount(string user, string pass)
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
        return true;
    }
}
