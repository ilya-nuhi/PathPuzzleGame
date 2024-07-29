using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each tile in level.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private List<TilePathData> _tilePathDataList;
    public List<TilePathData> TilePathDataList => _tilePathDataList;
    public int y;
    public int x;
    public int currentRotation = 0;

    // reference rotation is used for solving path without changing the current rotation.
    public int referenceRotation = 0;

    public void RotateReferenceTile()
    {
        referenceRotation = (referenceRotation + 1) % 4;
    }

    public void SetReferenceRotation(){
        referenceRotation = currentRotation;
    }

    public List<(int, int)> GetPaths()
    {
        var paths = new List<(int, int)>();
        foreach (var path in _tilePathDataList)
        {
            int rotatedPointX = (path.PointX + 2 * referenceRotation) % 8;
            int rotatedPointY = (path.PointY + 2 * referenceRotation) % 8;
            paths.Add((rotatedPointX, rotatedPointY));
        }
        return paths;
    }

}