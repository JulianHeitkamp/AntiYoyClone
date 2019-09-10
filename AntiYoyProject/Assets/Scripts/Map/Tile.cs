using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum TileType
{
    empty, neutral,p1,p2,p3,p4,
}




public class Tile : MonoBehaviour
{
    public HexagonGrid grid;
    public Color[] tileColors;

    public TileType type = TileType.empty;
    public Vector2 position;
    Text coordText;
    public SpriteRenderer sr;
    public Unit unit;
    public Territory territory;
    public void SetTileType(TileType _type)
    {
        switch (_type)
        {
            case TileType.empty:
                type = _type;
                sr.enabled = false;
                sr.color = Color.black;                
                break;
            case TileType.neutral:
                type = _type;
                sr.enabled = true;
                sr.color = tileColors[0];
                break;
            case TileType.p1:
                type = _type;
                sr.enabled = true;
                sr.color = tileColors[1];
                break;
            case TileType.p2:
                type = _type;
                sr.enabled = true;
                sr.color = tileColors[2];
                break;
            case TileType.p3:
                type = _type;
                sr.enabled = true;
                sr.color = tileColors[3];
                break;
            case TileType.p4:
                type = _type;
                sr.enabled = true;
                sr.color = tileColors[4];
                break;
            default:
                break;
        }
    }
    public void AssignCoordinates(Vector2 coord)
    {
        coordText = GetComponentInChildren<Text>();
        coordText.text = coord.x + "_" + coord.y;
    }
    public List<Tile> GetNeigbours()
    {
        List<Tile> neighbours = new List<Tile>();
        if (position.y % 2 == 0)
        {
            Tile t = grid.FindTileByCoordinates(new Vector2(position.x, position.y - 1));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x - 1, position.y - 1));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x - 1, position.y));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x + 1, position.y));
            if (t != null)
            {
                neighbours.Add(t);

            }
            t = grid.FindTileByCoordinates(new Vector2(position.x - 1, position.y + 1));
            if (t != null) 
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x, position.y + 1));
            if (t != null)
            {
                neighbours.Add(t);
            }
            
        }
        else
        {
            Tile t = grid.FindTileByCoordinates(new Vector2(position.x + 1, position.y - 1));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x, position.y - 1));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x - 1, position.y));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x + 1, position.y));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x, position.y + 1));
            if (t != null)
            {
                neighbours.Add(t);
            }
            t = grid.FindTileByCoordinates(new Vector2(position.x + 1, position.y + 1));
            if (t != null)
            {
                neighbours.Add(t);
            }
        }
        return neighbours;
    }
}
