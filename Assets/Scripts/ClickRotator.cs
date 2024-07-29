using System;
using UnityEngine;

public class ClickRotator : MonoBehaviour
{
    private Tile m_Tile;
    private void Awake() {
        m_Tile = GetComponent<Tile>();
    }
    private void OnMouseUpAsButton()
    {
        transform.Rotate(0, 0, -90);
        // Changing rotation index of the tile
        m_Tile.currentRotation = (m_Tile.currentRotation + 1) % 4;
    }
}
