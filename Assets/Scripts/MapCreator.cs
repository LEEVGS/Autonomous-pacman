using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class MapCreator : MonoBehaviour
{
    [SerializeField] private Transform _mapParent;
    [SerializeField] private GameObject _wallObject;
    [SerializeField] private GameObject _pathObject;

    void Start()
    {
        ReadTiles();
        PlaceTiles();
    }
    private void ReadTiles()
    {
        //Map data in string (0 wall 1 floor) TODO: Read from file
        string data = @"0000000000000000000000000000
						0111111111111001111111111110
						0100001000001001000001000010
						0100001000001111000001000010
						0100001000001001000001000010
						0111111111111001111111111110
						0100001001000000001001000010
						0100001001000000001001000010
						0111111001111001111001111110
						0001001000001001000001001000
						0001001000001001000001001000
						0111001111111111111111001110
						0100001001000000001001000010
						0100001001000000001001000010
						0111111001000000001001111110
						0100001001000000001001000010
						0100001001000000001001000010
						0111001001111111111001001110
						0001001001000000001001001000
						0001001001000000001001001000
						0111111111111111111111111110
						0100001000001001000001000010
						0100001000001001000001000010
						0111001111111001111111001110
						0001001001000000001001001000
						0001001001000000001001001000
						0111111001111001111001111110
						0100001000001001000001000010
						0100001000001001000001000010
						0111111111111111111111111110
						0000000000000000000000000000";

        int X = 1, Y = GetHeight(data);
        using (StringReader reader = new StringReader(data))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                //Reset X every line
                X = 1;

                //For every line
                for (int i = 0; i < line.Length; ++i)
                {
                    //Create new tile
                    Tile newTile = new Tile(X, Y, TileManager.tiles.Count);

                    //Check if floor tile
                    if (line[i] == '1')
                    {
                        //Check if tile before him is also floor
                        if (i != 0 && line[i - 1] == '1')
                        {
                            //Give the tiles each other id
                            newTile.left = TileManager.tiles[TileManager.tiles.Count - 1];
                            TileManager.tiles[TileManager.tiles.Count - 1].right = newTile;

                            //Add connection count to each other
                            newTile.adjacentCount++;
                            TileManager.tiles[TileManager.tiles.Count - 1].adjacentCount++;
                        }
                    }
                    else
                    {
                        //Other char
                        if (line[i] != '0')
                        {
                            continue;
                        }
                        //Tile is wall
                        newTile.occupied = true;
                    }

                    //Check if their is a floor above the tile (Y<30)
                    //Get up neighbor index
                    int upNeighbor = TileManager.tiles.Count - line.Length;
                    if (Y < 30 && !newTile.occupied && !TileManager.tiles[upNeighbor].occupied)
                    {
                        //Give the tiles each other id
                        TileManager.tiles[upNeighbor].down = newTile;
                        newTile.up = TileManager.tiles[upNeighbor];

                        //Add connection count to each other
                        newTile.adjacentCount++;
                        TileManager.tiles[upNeighbor].adjacentCount++;
                    }

                    TileManager.tiles.Add(newTile);
                    X++;
                }

                Y--;
            }
        }

        //Set map size
        TileManager.width = X-1;
        TileManager.height = GetHeight(data);

        //Set intersection tiles
        foreach (Tile tile in TileManager.tiles)
        {
            if (tile.adjacentCount > 2)
                tile.isIntersection = true;
        }
    }
    private int GetHeight(string data)
    {
        //Count amounts of enters in file and adds 1
        int count = 0;
        int n = 0;
        while ((n = data.IndexOf('\n', n) + 1) != 0)
        {
            n++;
            count++;
        }
        return count + 1;
    }
    private void PlaceTiles()
    {
        foreach (Tile tile in TileManager.tiles)
        {
            //Set tile postion to vector
            Vector3 postion = Vector3.zero;
            postion.x = tile.x;
            postion.y = tile.y;

            //Check if wall or floor
            if (tile.occupied)
            {
                GameObject tempWall = Instantiate(_wallObject, postion, Quaternion.identity, _mapParent);
                tempWall.name = $"Wall(Y:{tile.y}, X:{tile.x})";
                //tempWall.name = $"Index: {tile.index}";
            }
            else
            {
                GameObject tempWall = Instantiate(_pathObject, postion, Quaternion.identity, _mapParent);
                tempWall.name = $"Wall(Y:{tile.y}, X:{tile.x})";
                //tempWall.name = $"Index: {tile.index}";
            }
        }
    }
}
