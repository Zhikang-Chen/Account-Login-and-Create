using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameroomUIScript : MonoBehaviour
{
    [SerializeField]
    private InputField nameInput;

    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private FadingText fadingText;

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
        NetworkedClient.SendMessageToHost(message);
    }


}
