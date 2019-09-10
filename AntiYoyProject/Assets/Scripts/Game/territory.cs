using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Territory
{
    int playerId;
    public List<Tile> tiles;
    public int gold = 4;

    public GameObject mainPF;

    public Territory(int id, List<Tile> _tiles)
    {
        tiles = new List<Tile>();
        playerId = id;
        Debug.Log("Tiles Count: " + _tiles.Count);
        
        for (int i = 0; i < _tiles.Count; i++)
        {

            tiles.Add(_tiles[i]);
            tiles[i].territory = this;
        }
       
        
    }
    public void AddTile(Tile t)
    {
        tiles.Add(t);
    }
    public void NewTurn()
    {
        foreach (Tile t in tiles)
        {
            gold++;
        }
    }

}
