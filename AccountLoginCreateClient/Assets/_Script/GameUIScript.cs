using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;

public class GameUIScript : MonoBehaviour
{
    static public string CurrentRoomName = null;
    static public UnityEvent<bool> OnGameover = new UnityEvent<bool>();
    static public UnityEvent<string> OnGetMessageEvent = new UnityEvent<string>();
    static public UnityEvent<string> OnReplayEvent = new UnityEvent<string>();

    [SerializeField]
    private Text GameoverText;

    [SerializeField]
    private Button SendButton;

    [SerializeField]
    private InputField TextInput;

    [SerializeField]
    private GameObject ChatBox;

    [SerializeField]
    private GameObject TextPrefab;

    private void Awake()
    {
        GameoverText.gameObject.SetActive(false);
        OnGameover.AddListener(OnGameoverEvent);
        OnGetMessageEvent.AddListener(GetMessage);
        //OnReplayEvent.AddListener(PlayReplay);

        SendButton.onClick.AddListener(SendMessage);
        GameroomUI.OnEnd.AddListener(LeaveRoom);

        string message = string.Format("6,{0}", CurrentRoomName);
        NetworkedClient.SendMessageToHost(message);
    }

    private void Start()
    {
        if(ReplayManager.file != null)
        {
            StartCoroutine(ReplayManager.ReplayFile(ReplayManager.file));
            ReplayManager.file = null;
        }
    }

    private void OnDestroy()
    {
        OnGameover.RemoveListener(OnGameoverEvent);
        OnGetMessageEvent.RemoveListener(GetMessage);
        GameroomUI.OnEnd.RemoveListener(LeaveRoom);

    }

    public void LeaveRoom()
    {
        CurrentRoomName = null;
        NetworkedClient.SendMessageToHost("3");

        if(SceneManager.sceneCount <= 1)
            SceneManager.LoadScene("GameroomSearch");
        else
            SceneManager.UnloadSceneAsync("GameroomSearch");
    }

    public void OnGameoverEvent(bool isWinner)
    {
        GameoverText.gameObject.SetActive(true);
        if (isWinner)
        {
            GameoverText.text = "You win";
        }
        else
        {
            GameoverText.text = "You lose";
        }
    }

    public void SendMessage()
    {
        string message = string.Format("5,{0},{1}", CurrentRoomName, TextInput.text);
        NetworkedClient.SendMessageToHost(message);
        TextInput.text = "";

    }

    public void GetMessage(string msg)
    {
        Debug.Log(msg);

        var chat = Instantiate(TextPrefab, ChatBox.transform).GetComponent<Text>();
        chat.text = msg;
    }


}
