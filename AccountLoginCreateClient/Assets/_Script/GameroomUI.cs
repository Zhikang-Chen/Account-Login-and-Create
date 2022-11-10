using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameroomUI : MonoBehaviour
{
    [SerializeField]
    Text GameroomStateText;
    static public UnityEvent OnStart = new UnityEvent();
    static public UnityEvent OnEnd = new UnityEvent();

    private void Awake()
    {
        OnStart.AddListener(StartGame);
        OnEnd.AddListener(LeaveRoom);
    }

    static public void LeaveRoom()
    {
        NetworkedClient.SendMessageToHost("3");
        SceneManager.LoadScene("GameroomSearch");
    }

    public void StartGame()
    {
        GameroomStateText.text = "Found player~";
        StartCoroutine(Join());
        Debug.Log("Found Player");
    }

    public IEnumerator Join()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Game");
    }


}
