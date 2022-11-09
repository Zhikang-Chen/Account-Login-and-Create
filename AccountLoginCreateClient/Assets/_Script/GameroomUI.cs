using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameroomUI : MonoBehaviour
{
    Text GameroomStateText;
    static public UnityEvent OnUpdateGameroom = new UnityEvent();

    private void Awake()
    {
        OnUpdateGameroom.AddListener(UpdateRoomState);
    }

    public void LeaveRoom()
    {
        NetworkedClient.SendMessageToHost("3");
        SceneManager.LoadScene("GameroomSearch");
    }

    public void UpdateRoomState()
    {
        Debug.Log("Found Player");
    }
}
