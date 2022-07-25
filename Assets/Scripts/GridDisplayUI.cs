using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GridDisplayUI : MonoBehaviour
{
    public GridItem[,] gridItems;
    public List<GridItem> gridItemsToRegisterOnStart;

    private void Awake()
    {
        gridItems = new GridItem[3, 3];

    }

    private void Start()
    {
        for (int i = 0; i < gridItemsToRegisterOnStart.Count; i++)
        {
            gridItemsToRegisterOnStart[i].AddReferenceToGrid();
        }
    }

    public void RegisterGridItem(int x, int y, GridItem gridItem)
    {
        gridItems[x, y] = gridItem;
    }

    public void UpdateGrid(int x, int y, TicTacToeState state)
    {
        TextMeshProUGUI textMesh = gridItems[x,y].GetComponent<TextMeshProUGUI>();
        switch (state)
        {
            case TicTacToeState.circle:
                textMesh.text = "[O]";
                break;
            case TicTacToeState.cross:
                textMesh.text = "[X]";
                break;
        }
    }
}
