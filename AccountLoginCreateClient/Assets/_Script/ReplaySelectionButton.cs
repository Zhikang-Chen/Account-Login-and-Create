using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ReplaySelectionButton : MonoBehaviour
{
    public string fileName = null;
    public ReplayManager replay;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnSelection);
    }

    private void Start()
    {
        GetComponentInChildren<TMP_Text>().text = fileName;
        
    }

    public void OnSelection()
    {
        replay.PlayReplayFiles(fileName);
        
    }



}
