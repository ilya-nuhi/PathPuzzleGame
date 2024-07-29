using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelHealthChecker : MonoBehaviour
{
    [SerializeField] private Level level;
    [SerializeField] private GameObject caseLevelGO;

    private Tile[,] tileGrid;

    private void Start() {
        tileGrid = new Tile[level.columnLength, level.rowLength];
        for (int i = 0; i < level.columnLength; i++) {
            for (int j = 0; j < level.rowLength; j++) {
                string tileName = "Tile" + ((i * level.rowLength) + j + 1);
                Transform tileTransform = caseLevelGO.transform.Find(tileName);
                if (tileTransform != null) {
                    tileGrid[i, j] = tileTransform.GetComponent<Tile>();
                    tileGrid[i, j].y = i;
                    tileGrid[i, j].x = j;
                } else {
                    Debug.LogError("Tile " + tileName + " not found");
                }
            }
        }
    }

    public void CheckHealth() {
        Debug.Log("Starting CheckHealth");
        SetReferenceRotations();
        if (CheckHealthHelper(level.startingTile, level.startingPoint)) {
            Debug.Log("Level is solvable.");
        } else {
            Debug.Log("Level is not solvable.");
        }
    }

    

    private void SetReferenceRotations()
    {
        foreach (Tile tile in tileGrid) {
            tile.SetReferenceRotation();
        }
    }


    private bool CheckHealthHelper(Tile tile, int entryPoint, HashSet<(Tile, int)> visitedPoints = null, bool showSolution = false) {
        if (visitedPoints == null)
            visitedPoints = new HashSet<(Tile, int)>();

        Debug.Log($"Checking tile at position ({tile.y}, {tile.x}) with entry point {entryPoint}");

        if (visitedPoints.Contains((tile, entryPoint))) {
            Debug.Log($"Already visited tile at position ({tile.y}, {tile.x}) with entry point {entryPoint}");
            return false;
        }

        visitedPoints.Add((tile, entryPoint));
        // Rotating tile 4 times to check every possiblity. After this loop tile turns back to its original position.
        for(int i = 0; i < 4; i++){
            foreach (var path in tile.GetPaths()) {
                if (path.Item1 == entryPoint || path.Item2 == entryPoint) {
                    int destinationPoint = (path.Item1 == entryPoint) ? path.Item2 : path.Item1;
                    Debug.Log($"Found path from {entryPoint} to {destinationPoint} on tile at position ({tile.y}, {tile.x})");

                    // If the next entry point is on the upper side and we exceed the boundaries, then we reached the win area.
                    if ((destinationPoint / 2 == 0) && !CheckBoundaries(tile.y + 1, tile.x)) {
                        Debug.Log($"Reached win area from tile at position ({tile.y}, {tile.x}) with entry point {destinationPoint}");
                        if(showSolution){
                            ShowSolutionPath(visitedPoints);
                        }
                        else{
                            PrintSolution(visitedPoints);
                        }
                        
                        return true;
                    }

                    // Get the neighboring tile based on nextEntryPoint
                    (Tile nextTile, int nextEntryPoint) = GetNextTileAndPoint(tile, destinationPoint);
                    if (nextTile != null && CheckHealthHelper(nextTile, nextEntryPoint, visitedPoints, showSolution)) {
                        return true;
                    }
                }
            }
            // We can let the tile used multiple times, however we can't let it rotate after multiple use because
            // it is the part of the current solution now.
            if(CheckForMultipleUse(tile, visitedPoints)){
                Debug.Log("This tile is already used. Skipping rotations.");
                break;
            }
            tile.RotateReferenceTile();
            Debug.Log("Rotating Tile.");
        }
        

        // If the current way has no solution, we are removing the point
        visitedPoints.Remove((tile, entryPoint));
        Debug.Log($"No solution found from tile at position ({tile.y}, {tile.x}) with entry point {entryPoint}");
        return false;
    }

    public void ShowSolution(){
        SetReferenceRotations();
        if(!CheckHealthHelper(level.startingTile, level.startingPoint, showSolution:true)) {
            Debug.Log("Level is not solvable.");
        }
    }

    private void ShowSolutionPath(HashSet<(Tile, int)> visitedPoints)
    {
        foreach(var point in visitedPoints){
            Tile currentTile = point.Item1;
            for(int i = 0; i < (currentTile.referenceRotation - currentTile.currentRotation + 4) % 4; i++){
                currentTile.transform.Rotate(0, 0, -90);
            }
            currentTile.currentRotation = currentTile.referenceRotation;
        }
    }


    private bool CheckForMultipleUse(Tile tile, HashSet<(Tile, int)> visitedPoints)
    {
        int tileCount = 0;
        foreach (var point in visitedPoints){
            if(point.Item1 == tile) {
                tileCount++;
                if (tileCount > 1) {
                    return true;
                }
            }
        }
        return false;
    }


    private void PrintSolution(HashSet<(Tile, int)> visitedPoints)
    {
        Debug.Log("Solution Path:");
        foreach (var point in visitedPoints) {
            Debug.Log($"{point.Item1.gameObject.name}:({point.Item1.y}, {point.Item1.x}) point: {point.Item2} with Rotation Index: {point.Item1.referenceRotation}");
        }
    }


    private (Tile, int) GetNextTileAndPoint(Tile tile, int destinationPoint) {
        Tile nextTile = null;
        int nextEntryPoint = -1;

        // Destination and new entry points sum is equal to 9 if the tiles are matched horizontally, 5 if matched vertically.
        if (destinationPoint / 2 == 0 && CheckBoundaries(tile.y + 1, tile.x)) {
            nextTile = tileGrid[tile.y + 1, tile.x];
            nextEntryPoint = 5 - destinationPoint;
        } else if (destinationPoint / 2 == 1 && CheckBoundaries(tile.y, tile.x + 1)) {
            nextTile = tileGrid[tile.y, tile.x + 1];
            nextEntryPoint = 9 - destinationPoint;
        } else if (destinationPoint / 2 == 2 && CheckBoundaries(tile.y - 1, tile.x)) {
            nextTile = tileGrid[tile.y - 1, tile.x];
            nextEntryPoint = 5 - destinationPoint;
        } else if (destinationPoint / 2 == 3 && CheckBoundaries(tile.y, tile.x - 1)) {
            nextTile = tileGrid[tile.y, tile.x - 1];
            nextEntryPoint = 9 - destinationPoint;
        }

        Debug.Log(nextTile != null
            ? $"Moving to next tile at position ({nextTile.y}, {nextTile.x}) with next entry point {nextEntryPoint} from tile at position ({tile.y}, {tile.x}) with entry point {destinationPoint}"
            : $"No valid next tile found from tile at position ({tile.y}, {tile.x}) with entry point {destinationPoint}");

        return (nextTile, nextEntryPoint);
    }

    private bool CheckBoundaries(int y, int x) {
        bool inBounds = y < level.columnLength && x < level.rowLength && y >= 0 && x >= 0;
        Debug.Log($"CheckBoundaries for position ({y}, {x}): {inBounds}");
        return inBounds;
    }
}
