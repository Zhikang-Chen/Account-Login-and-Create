using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    [SerializeField]
    private InputField userNameInput;

    [SerializeField]
    private InputField passwordInput;

    [SerializeField]
    private Button loginButton;

    [SerializeField]
    private Button newAccountButton;

    [SerializeField]
    private FadingText fadingText;

    private NetworkedClient networkedClient;
    // Start is called before the first frame update
    public void Start()
    {
        networkedClient = FindObjectOfType<NetworkedClient>();
        if(networkedClient == null)
        {
            Debug.LogError("NetworedClient Not Found");
        }

        loginButton.onClick.AddListener(LoginButtonPress);
        newAccountButton.onClick.AddListener(NewAccountButtonPress);
    }

    void LoginButtonPress()
    {
        Debug.Log(userNameInput.text);
        Debug.Log(passwordInput.text);
        string message = string.Format("{0},{1},{2}", 0,userNameInput.text, passwordInput.text);

        //Debug.Log(message);
        networkedClient.SendMessageToHost(message);
    }

    void NewAccountButtonPress()
    {
        Debug.Log(userNameInput.text);
        Debug.Log(passwordInput.text);
        string message = string.Format("{0},{1},{2}", 1, userNameInput.text, passwordInput.text);
        networkedClient.SendMessageToHost(message);
    }

    public void OnLogin(bool success)
    {
        if (success)
        {
            fadingText.ChangeText("Logging In~");
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
