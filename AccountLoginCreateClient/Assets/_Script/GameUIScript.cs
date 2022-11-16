using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUIScript : MonoBehaviour
{
    static public string CurrentRoomName = null;
    static public UnityEvent<bool> OnGameover = new UnityEvent<bool>();

    [SerializeField]
    private Text GameoverText;

    private void Awake()
    {
        GameoverText.gameObject.SetActive(false);
        OnGameover.AddListener(OnGameoverEvent);
    }

    public void LeaveRoom()
    {
        CurrentRoomName = null;
        NetworkedClient.SendMessageToHost("3");
        SceneManager.LoadScene("GameroomSearch");
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
}
