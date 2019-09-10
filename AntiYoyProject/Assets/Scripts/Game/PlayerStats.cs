using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int id;
    public List<Territory> territories;
    UIManager ui;
    Unit treeUnit;
    public Unit mainUnit;

    public GameObject mainPF;


    // Start is called before the first frame update
    void Start()
    {


    }

    public void GameStart(int _id, List<Tile> initialTerr)
    {
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        territories = new List<Territory>();

        Debug.Log("setting initial territory");

        id = _id;
        Territory t = new Territory(id, initialTerr);
        
        territories.Add(t);
        NextTurn();

        id = _id;
        NewTerritory(initialTerr);
        ui.SelectTerritory(territories[0]);
        
        

    }
    public void NewTerritory(List<Tile>  _tiles)
    {
        Debug.Log("new territory");
        Territory t = new Territory(id, _tiles);
        List<Tile> emptyTiles = new List<Tile>();
        for (int i = 0; i < t.tiles.Count - 1; i++)
        {
            if (t.tiles[i].unit == null)
            {
                emptyTiles.Add(t.tiles[i]);
            }
        }
        if (emptyTiles.Count > 0)
        {
            int rnd = Random.Range(0, emptyTiles.Count-1);
            GameObject mainGO = Instantiate(mainUnit.objectToSpawn);
            emptyTiles[rnd].unit = mainUnit;
            Debug.LogError("spawning main unit");
            mainGO.transform.parent = emptyTiles[rnd].transform;
            mainGO.transform.localPosition = new Vector3(0, 0, 0);

        }
        else
        {
            int rnd = Random.Range(0, t.tiles.Count - 1);
            Destroy(t.tiles[rnd].unit);
            GameObject mainGo = Instantiate(mainUnit.objectToSpawn);

            t.tiles[rnd].unit = mainUnit;
            Debug.LogError("spawning main unit");
            mainGo.transform.parent = t.tiles[rnd].transform;
            mainGo.transform.localPosition = new Vector3(0, 0, 0);
        }
        territories.Add(t);


    }
    public void NextTurn()
    {
        foreach (Territory t in territories)
        {
            t.NewTurn();
        }
        ui.UpdateGoldTf();
    }

}
