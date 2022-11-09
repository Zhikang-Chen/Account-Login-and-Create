using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameroomSearhUIScript : MonoBehaviour
{
    [SerializeField]
    private InputField nameInput;

    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private FadingText fadingText;

    static public int GameroomID = -1;

    private void Awake()
    {
        joinButton.onClick.AddListener(OnJoin);
    }

    public void OnJoin()
    {
        string message = string.Format("{0},{1}", 2, nameInput.text);
        NetworkedClient.SendMessageToHost(message);
    }

    public void OnExit()
    {
        string message = string.Format("{0},{1}", 2, nameInput.text);
        GameroomID = -1;
        NetworkedClient.SendMessageToHost(message);
    }

    static public void JoinRoom(int roomID)
    {
        GameroomID = roomID;
        SceneManager.LoadScene("Gameroom");
    }
}
