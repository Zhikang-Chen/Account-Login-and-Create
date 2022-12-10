using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayManager : MonoBehaviour
{
    static public string file = null;

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

    static public List<string> GetReplayFiles()
    {
        List<string> ReplayFilesName = new List<string>();
        System.IO.DirectoryInfo files = new System.IO.DirectoryInfo("Replay/");
        foreach (var i in files.GetFiles())
        {
            Debug.Log(i.FullName);
            ReplayFilesName.Add(i.Name);
        }
        return ReplayFilesName;
    }

    public void PlayReplayFiles(string FileName)
    {
        //GameUIScript.OnReplayEvent.AddListener(PlayReplay);
        //ReplayFile(FileName));
        file = FileName;
        SceneManager.LoadScene("Game");

    }

    static public IEnumerator ReplayFile(string FileName)
    {
        yield return new WaitForFixedUpdate();

        string fileName = string.Format("Replay/{0}", FileName);
        StreamReader sr = new StreamReader(fileName);
        if (sr != null)
        {
            string line;
            line = sr.ReadLine();

            while (line != null)
            {
                var data = line.Split("@");
                yield return new WaitForSeconds(float.Parse(data[0]));
                NetworkedClientProcess.ProcessRecievedMsg(data[1], NetworkedClient.connectionID);
                line = sr.ReadLine();
            }
        }
    }
}
