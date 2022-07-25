using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour
{
    [SerializeField] private int _xCoord;
    [SerializeField] private int _yCoord;

    public int xCoord { get => _xCoord; }
    public int yCoord { get => _yCoord; }
    GridDisplayUI grid;

    private void Awake()
    {
       grid = FindObjectOfType<GridDisplayUI>();
    }

    public void AddReferenceToGrid()
    {
        grid.RegisterGridItem(_xCoord, _yCoord, this);
    }
}
