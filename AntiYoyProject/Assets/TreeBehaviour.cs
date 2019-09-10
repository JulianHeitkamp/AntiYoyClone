using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour
{
    public Tile tile;
    public GameObject treePrefab;
    GameManager gm;

    public Unit treeUnit;
    public void NewTurn()
    {
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        float rnd = Random.Range(0f, 1f);
        if (rnd <= 0.1)
        {
            List<Tile> neighbours = tile.GetNeigbours();
            int tileRND = Random.Range(0, neighbours.Count-1);
            Debug.Log("tileRND = " + tileRND);
            Tile n = neighbours[tileRND];
            Debug.Log("tile is = " + n);
            if (n.unit == null)
            {
                GameObject tree = Instantiate(treeUnit.objectToSpawn);
                n.unit = treeUnit;
                tree.transform.parent = n.transform;
                tree.transform.localPosition = new Vector3(0, 0, 0);
                gm.addedTrees.Add(tree.GetComponent<TreeBehaviour>());
            }
            
        }
    }
}
