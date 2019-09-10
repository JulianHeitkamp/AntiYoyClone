using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public Tile tile;
    public GameObject treePrefab;
    private GameManager gm;

    public void NewTurn()
    {
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        float rnd = Random.Range(0f, 1f);
        if (rnd <= 0.5f)
        {
            List<Tile> neightbours = tile.GetNeigbours();
            int tileRND = Random.Range(0, neightbours.Count - 1);
            
        }
    }
}
