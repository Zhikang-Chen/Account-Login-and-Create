using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIScript : MonoBehaviour
{
    static public string CurrentRoomName = null;

    public void LeaveRoom()
    {
        CurrentRoomName = null;
        NetworkedClient.SendMessageToHost("3");
        SceneManager.LoadScene("GameroomSearch");
    }
}
