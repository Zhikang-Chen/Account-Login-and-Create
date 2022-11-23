using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ReplayUIScrupt : MonoBehaviour
{
    [SerializeField]
    GameObject content;

    [SerializeField]
    ReplaySelectionButton buttonPrefab;

    [SerializeField]
    ReplayManager replay;

    List<GameObject> Buttons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach(var file in ReplayManager.GetReplayFiles())
        {
            var tempRef = Instantiate(buttonPrefab.gameObject, content.transform);
            tempRef.GetComponent<ReplaySelectionButton>().fileName = file;
            tempRef.GetComponent<ReplaySelectionButton>().replay = replay;
            Buttons.Add(tempRef);
        }
    }


    public void ExitUI()
    {
        SceneManager.LoadScene("GameroomSearch");
    }
}
