using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridScript : MonoBehaviour
{
    static public UnityAction<bool, int, int> UpdateGridAction = null;

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
        UpdateGridAction = UpdateGrid;
    }

    private void UpdateGrid(bool isPlayer1, int row, int col)
    {
        PositionData[col][row].UpdateSlotState(isPlayer1);
    }

    private void OnDestroy()
    {
        UpdateGridAction = null;
    }
}
