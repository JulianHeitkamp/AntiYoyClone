using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexagonGrid : MonoBehaviour
{
    public int width;
    public int height;

    public float tileOffset;
    public float xOffset;
    public float yOffset;

    public GameObject hexPrefab;
    public GameObject treePrefab;

    public List<Tile> map;

    public List<Tile> tP1;
    public List<Tile> tP2;
    public List<Tile> tP3;
    public List<Tile> tP4;

    GameManager gm;

    public Unit treeUnit;
    public void DrawMap(int playerCount)
    {
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        map = new List<Tile>();
        tP1 = new List<Tile>();
        tP2 = new List<Tile>();
        tP3 = new List<Tile>();
        tP4 = new List<Tile>();
        for (int y = -height/2; y < height/2; y++)
        {
            for (int x = -width/2; x < width/2; x++)
            {
                float xPos = x * xOffset;
                if (y%2 == 1|| y%2 == -1 )
                {
                    xPos += xOffset / 2f;
                }
                GameObject newTile = Instantiate(hexPrefab, new Vector3(xPos, y * yOffset), Quaternion.identity);
                newTile.transform.parent = this.transform;
                newTile.transform.name = x + "_" + y;
                Tile t = newTile.GetComponent<Tile>();
                t.SetTileType(TileType.neutral);
                t.position = new Vector2(x, y);
                t.AssignCoordinates(new Vector2(x, y));
                t.grid = this;
                if (x <= -width / 2 + 2 && y <= -height / 2 + 1 && playerCount >= 1)
                {
                    t.SetTileType(TileType.p1);
                    tP1.Add(newTile.GetComponent<Tile>());
                }
                if (x <= -width / 2 + 2 && y >= height / 2 - 2 && playerCount >= 2 )
                {
                    t.SetTileType(TileType.p2);
                    tP2.Add(newTile.GetComponent<Tile>());
                }
                if (x >= width / 2 - 3 && y >= height / 2 - 2 && playerCount >= 3)
                {
                    t.SetTileType(TileType.p3);
                    tP3.Add(newTile.GetComponent<Tile>());
                }
                if (x >= width / 2 - 3 && y <= -height / 2 + 1 && playerCount >= 4)
                {
                    t.SetTileType(TileType.p4);
                    tP4.Add(newTile.GetComponent<Tile>());
                }
                map.Add(t);

            }
        }
        // spawning trees
        Debug.Log("starting to spwn trees");
        for (int i = 0; i < map.Count; i++)
        {
            float rnd = Random.Range(0f,1f);
            if (rnd <= 0.05)
            {
                GameObject treeGO = Instantiate(treeUnit.objectToSpawn);
                Tile t = map[i];
                t.unit = treeUnit;
                treeGO.transform.parent = map[i].transform;
                treeGO.transform.localPosition = new Vector3(0, 0, 0);
                treeGO.GetComponent<TreeBehaviour>().tile = map[i];
                gm.trees.Add(treeGO.GetComponent<TreeBehaviour>());
            }
        }
    }
    public Tile FindTileByCoordinates(Vector2 coordinates)
    {
        foreach (Tile t in map)
        {
            if (t.position.x == coordinates.x && t.position.y == coordinates.y)
            {
                return t;
            }
        }
        return null;
    }
    

    
}
