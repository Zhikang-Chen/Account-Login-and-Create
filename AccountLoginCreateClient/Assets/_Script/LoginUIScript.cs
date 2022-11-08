using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LoginUIScript : MonoBehaviour
{
    [SerializeField]
    private InputField userNameInput;

    [SerializeField]
    private InputField passwordInput;

    [SerializeField]
    private FadingText fadingText;

    static public UnityEvent<bool> OnLoginEvent = new UnityEvent<bool>();
    static public UnityEvent<bool> OnAccountCreationEvent = new UnityEvent<bool>();


    //private NetworkedClient networkedClient;
    // Start is called before the first frame update
    public void Start()
    {
        OnLoginEvent.AddListener(OnLogin);
        OnAccountCreationEvent.AddListener(OnAccountCreation);
    }

    private void OnDestroy()
    {
        OnLoginEvent.RemoveListener(OnLogin);
        OnAccountCreationEvent.RemoveListener(OnAccountCreation);
    }

    public void LoginButtonPress()
    {
        Debug.Log(userNameInput.text);
        Debug.Log(passwordInput.text);
        string message = string.Format("{0},{1},{2}", 0,userNameInput.text, passwordInput.text);

        //Debug.Log(message);
        NetworkedClient.SendMessageToHost(message);
    }

    public void NewAccountButtonPress()
    {
        Debug.Log(userNameInput.text);
        Debug.Log(passwordInput.text);
        string message = string.Format("{0},{1},{2}", 1, userNameInput.text, passwordInput.text);
        NetworkedClient.SendMessageToHost(message);
    }

    public void OnLogin(bool success)
    {
        if (success)
        {
            fadingText.ChangeText("Logging In~");
            SceneManager.LoadScene("Gameroom");
        }
        else
        {
            fadingText.ChangeText("Invaild Password or User Name");
        }
        fadingText.InvokeText();
    }

    public void OnAccountCreation(bool success)
    {
        if (success)
        {
            fadingText.ChangeText("Account Creation Success");
        }
        else
        {
            fadingText.ChangeText("Account Creation Failed");
        }
        fadingText.InvokeText();
    }

}
