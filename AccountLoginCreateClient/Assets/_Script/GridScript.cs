using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    static public bool IsPlayer1Turn = true;

    [SerializeField]
    SlotScript SlotPrefab;

    // This is the position the server knows
    [SerializeField]
    public List<List<SlotScript>> PositionData = new List<List<SlotScript>>();

    private void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            List<SlotScript> Col = new List<SlotScript>();
            for (int i2 = 0; i2 < 3; i2++)
            {
                var tempRef = Instantiate(SlotPrefab.gameObject, this.transform);
                var tempCompRef = tempRef.GetComponent<SlotScript>();
                tempCompRef.Position = new Vector2Int(i2, i);
                Col.Add(tempCompRef);
            }
            PositionData.Add(Col);
        }
        Debug.Log(PositionData);
    }


    // Update is called once per frame
    void Update()
    {
        CheckForWinner();
    }

    void CheckForWinner()
    {

        //foreach (var row in PositionData)
        //{
        //    if ((row[0].CurrentState != 0) && (row[0].CurrentState == row[1].CurrentState) && (row[1].CurrentState == row[2].CurrentState))
        //    {
        //        Debug.Log(row[0]);
        //    }
        //}

        if ((PositionData[0][0].CurrentState != 0) &&
            (PositionData[0][0].CurrentState == PositionData[1][1].CurrentState) &&
            (PositionData[1][1].CurrentState == PositionData[2][2].CurrentState))
        {
            Debug.Log(PositionData[0][0].CurrentState);
        }
        else if ((PositionData[2][0].CurrentState != 0) &&
                (PositionData[2][0].CurrentState == PositionData[1][1].CurrentState) &&
                (PositionData[1][1].CurrentState == PositionData[0][2].CurrentState))
        {
            Debug.Log(PositionData[2][0].CurrentState);
        }

        for (int i = 0; i < 3; i++)
        {
            //Vertical
            if ((PositionData[i][0].CurrentState != 0) &&
            (PositionData[i][0].CurrentState == PositionData[i][1].CurrentState) &&
            (PositionData[i][1].CurrentState == PositionData[i][2].CurrentState))
            {
                Debug.Log(PositionData[i][0].CurrentState);
            }

            //Horizontal
            else if ((PositionData[0][i].CurrentState != 0) &&
                (PositionData[0][i].CurrentState == PositionData[1][i].CurrentState) &&
                (PositionData[1][i].CurrentState == PositionData[2][i].CurrentState))
            {
                Debug.Log(PositionData[i][0].CurrentState);
            }
        }
    }
}
