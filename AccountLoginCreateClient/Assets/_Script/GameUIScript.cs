using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIScript : MonoBehaviour
{
    public void LeaveRoom()
    {
        NetworkedClient.SendMessageToHost("3");
        SceneManager.LoadScene("GameroomSearch");
    }
}
