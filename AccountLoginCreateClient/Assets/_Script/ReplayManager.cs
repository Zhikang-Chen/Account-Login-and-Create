using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //GetReplayFiles();
    }

    static public void SaveReplayFiles(string roomName, string ReplayData)
    {
        string fileName = string.Format("Replay/{0}-{1}.txt", roomName, DateTime.Now.ToString("yyyyMMddHHmmssffff"));
        StreamWriter sw = new StreamWriter(fileName);
        string[] actions = ReplayData.Split("|");

        for (int i = 1; i < actions.Length - 1; i++)
        {
            sw.WriteLine(actions[i]);
        }
        sw.Close();
    }

    static public void GetReplayFiles()
    {
        List<string> ReplayFilesName = new List<string>();
        System.IO.DirectoryInfo files = new System.IO.DirectoryInfo("Replay/");
        foreach (var i in files.GetFiles())
        {
            Debug.Log(i.FullName);
            ReplayFilesName.Add(i.Name);
        }
        //return ReplayFilesName;
    }
}
