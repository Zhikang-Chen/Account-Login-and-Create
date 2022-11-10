using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public enum State
    {
        Empty = 0,
        Player1 = 1,
        Player2 = 2
    }

    public State CurrentState = State.Empty; 

    [SerializeField]
    Sprite OImage;

    [SerializeField]
    Sprite XImage;

    [SerializeField]
    Image TargetImage;

    //public uint Col;
    //public uint Row;

    // This is the position the client knows
    public Vector2Int Position;

    private void Awake()
    {
        //TargetImage = GetComponent<Image>();
        TargetImage.gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(Click);
    }

    private void Click()
    {
        if(CurrentState == State.Empty)
        {
            if(GridScript.IsPlayer1Turn)
            {
                TargetImage.gameObject.SetActive(true);
                CurrentState = State.Player1;
                TargetImage.sprite = XImage;
                GridScript.IsPlayer1Turn = !GridScript.IsPlayer1Turn;
            }
            else
            {
                TargetImage.gameObject.SetActive(true);
                CurrentState = State.Player2;
                TargetImage.sprite = OImage;
                GridScript.IsPlayer1Turn = !GridScript.IsPlayer1Turn;
            }
        }
    }
}
