using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameroomSearchUIScript : MonoBehaviour
{
    [SerializeField]
    private InputField nameInput;

    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private FadingText fadingText;

    static public int GameroomID = -1;
    //static public string CurrentLobbyName = null;

    private void Awake()
    {
        joinButton.onClick.AddListener(OnJoin);
    }

    public void OnJoin()
    {
        string message = string.Format("{0},{1}", 2, nameInput.text);
        GameUIScript.CurrentRoomName = nameInput.text;
        NetworkedClient.SendMessageToHost(message);
    }

    //public void OnExit()
    //{
    //    string message = string.Format("{0},{1}", 2, nameInput.text);
    //    GameroomID = -1;
    //    NetworkedClient.SendMessageToHost(message);
    //    nameInput.text = null;
    //}

    static public void JoinRoom(bool FoundPlayer)
    {
        if(FoundPlayer)
            SceneManager.LoadScene("Game");
        else
            SceneManager.LoadScene("Gameroom");
    }

    public void OpenReplaySelection()
    {
        SceneManager.LoadScene("ReplaySelection");
    }
}
