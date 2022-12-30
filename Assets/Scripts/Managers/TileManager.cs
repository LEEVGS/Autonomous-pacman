using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static List<Tile> tiles = new List<Tile>();
    public static int height = 0;
    public static int width = 0;

    /// <summary>
    /// Returns the index of the tile by position
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    public int Index(int X, int Y)
    {
        //Check if position is in bounds and return if so
        if (X >= 1 && X <= width && Y <= height && Y >= 1)
        {
            return (height - Y) * width + X - 1;
        }

        //Else return closest valid index
        if (X < 1) X = 1;
        if (X > width) X = width;
        if (Y < 1) Y = 1;
        if (Y > height) Y = height;

        return (height - Y) * width + X - 1;
    }

    public int Index(Tile tile)
    {
        return (height - tile.y) * width + tile.x - 1;
    }

    /// <summary>
    /// Returns distance between two tiles
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    public float distance(Tile tile1, Tile tile2)
    {
        return Mathf.Sqrt(Mathf.Pow(tile1.x - tile2.x, 2) + Mathf.Pow(tile1.y - tile2.y, 2));
    }
}
