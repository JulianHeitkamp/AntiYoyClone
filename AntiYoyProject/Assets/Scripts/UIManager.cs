using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public playerController pc;
    public PlayerStats ps;
    bool firstClickBuilding = true;
    bool firstClickUnit = true;
    public List<Tile> tilesToSpawnOn;
    public Button buyUnits;
    public Button buyBuildings;
    public Button toSpawn;
    public Text spawnCost;
    public bool spawningBuilding; //true when building, false when unit
    public BuildingToSpawn buildingToSpawn;
    public UnitsToSpawn unitToSpawn;
    GameManager gm;
    private List<GameObject> buildings;
    private List<GameObject> units;
    public Text goldTf;

    public Territory selected;
    public int buildingsLevel = -1;
    public int unitsLevel = -1;
    public Text currentTurnNameTf;
    public Unit[] unitsObjects;
    public Unit[] buildingsObjects;
    


    
    // Start is called before the first frame update
    void Start()
    {
        pc = FindObjectOfType<playerController>();
        ps = FindObjectOfType<PlayerStats>();
        buildings = new List<GameObject>();
        foreach (var b in GameObject.FindGameObjectsWithTag("BuildingsButton"))
        {
            buildings.Add(b);
            Debug.Log("adding to buildings");
        }
        units = new List<GameObject>();
        foreach (var u in GameObject.FindGameObjectsWithTag("UnitsButton"))
        {
            units.Add(u);

            Debug.Log("adding to units");
        }
            


        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        DisableAllUnits();
        //SwitchUnitsLevel();
        //SwitchBuildingsLevel();
    }
    public void SpawnUnit()
    {
        string[] cost = spawnCost.text.Split('$');
        if (selected.gold - int.Parse(cost[0]) >= 0)
        {
            tilesToSpawnOn = new List<Tile>();
            foreach (Tile t in selected.tiles)
            {
                List<Tile> temp = new List<Tile>();
                temp.AddRange(t.GetNeigbours());
                for (int i = 0; i < temp.Count; i++)
                {
                    if (tilesToSpawnOn.Contains(temp[i]))
                    {
                        continue;
                    }
                    if (temp[i].unit != null)
                    {
                        if (spawningBuilding == true)
                        {
                            if (temp[i].unit.level >= buildingsObjects[buildingsLevel].level)
                            {
                                continue;
                            }
                            else
                            {
                                tilesToSpawnOn.Add(temp[i]);
                            }
                        }
                        else
                        {
                            if (temp[i].unit.level >= unitsObjects[unitsLevel].level)
                            {
                                continue;
                            }
                            else
                            {
                                tilesToSpawnOn.Add(temp[i]);
                            }
                        } 
                    }
                    else
                    {
                        Debug.Log("no unit");
                        tilesToSpawnOn.Add(temp[i]);
                    }
                }

            }
            Debug.Log("number of tiles = " + tilesToSpawnOn.Count);
            for (int i = 0; i < tilesToSpawnOn.Count; i++)
            {
                SpriteRenderer sr = tilesToSpawnOn[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
                Color temp = sr.color;
                temp.a = 255f;
                sr.color = temp;
            }
            pc.spawningSomething = true;

        }
        else
        {
            DisableAllUnits();
        }
    }
    
    
    public void NextTurn()
    {
        gm.NextTurn();
    }

    public void SelectTerritory(Territory t)
    {
        Debug.Log("select territory in ui");
        if (t != null)
        {
            selected = t;
            UpdateGoldTf();
            buyUnits.enabled = true;
            buyUnits.GetComponent<Image>().enabled = true;
            buyBuildings.enabled = true;
            buyBuildings.GetComponent<Image>().enabled = true;
           
        }
        else
        {
            DisableAllUnits();
        }

    }
    public void UpdateGoldTf()
    {
        if (selected != null)
        {

            goldTf.text = selected.gold.ToString();

        }
    }

    public void SwitchBuildingsLevel()
    {
        if (buildingsLevel >= 2)
        {
            buildingsLevel = -1;
        }
        buildingsLevel++;
        unitsLevel = -1;
        buildingToSpawn = (BuildingToSpawn) buildingsLevel;
        toSpawn.enabled = true;
        spawningBuilding = true;
        toSpawn.GetComponent<Image>().enabled = true;
        spawnCost.enabled = true;
        Debug.Log("buildingslevel = " + buildingsLevel);
        toSpawn.image.sprite = buildingsObjects[buildingsLevel].sprite;
        spawnCost.text = buildingsObjects[buildingsLevel].cost + "$";

    }
    public void SwitchUnitsLevel()
    {
        if (unitsLevel >= 3)
        {
            unitsLevel = -1;
        }
        unitsLevel++;
        buildingsLevel = -1;
        unitToSpawn = (UnitsToSpawn)unitsLevel - 1;
        spawningBuilding = false;
        toSpawn.enabled = true;
        toSpawn.GetComponent<Image>().enabled = true;
        spawnCost.enabled = true;


        Debug.Log("unitslevel"+ unitsLevel);
        toSpawn.image.sprite = unitsObjects[unitsLevel].sprite;
        spawnCost.text = unitsObjects[unitsLevel].cost + "$";
    }
    public void DisableAllUnits()
    {
        buyBuildings.enabled = false;
        buyBuildings.GetComponent<Image>().enabled = false;
        buyUnits.enabled = false;
        buyUnits.GetComponent<Image>().enabled = false;
        toSpawn.enabled = false;
        toSpawn.GetComponent<Image>().enabled = false;
        spawnCost.enabled = false;
        Debug.Log("reseting color");
        Debug.Log("number of tiles = " + tilesToSpawnOn.Count);
        for (int i = 0; i < tilesToSpawnOn.Count; i++)
        {
            tilesToSpawnOn[i].SetTileType(tilesToSpawnOn[i].type);
        }
        unitsLevel = -1;
        buildingsLevel = -1;
    }
    public void MoveUnit(Tile tile)
    {
        tilesToSpawnOn = new List<Tile>();
        
            List<Tile> temp = new List<Tile>();
            temp.AddRange(tile.GetNeigbours());
            for (int i = 0; i < temp.Count; i++)
            {
                if (tilesToSpawnOn.Contains(temp[i]))
                {
                    continue;
                }
                if (temp[i].unit != null)
                {

                    if (temp[i].unit.level >= tile.unit.level)
                    {
                        continue;
                    }
                    else
                    {
                        tilesToSpawnOn.Add(temp[i]);
                    }


                }

                else
                {
                    Debug.Log("no unit");
                    tilesToSpawnOn.Add(temp[i]);
                }
            

        }
        Debug.Log("number of tiles = " + tilesToSpawnOn.Count);
        for (int i = 0; i < tilesToSpawnOn.Count; i++)
        {
            SpriteRenderer sr = tilesToSpawnOn[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
            Color c = sr.color;
            c.a = 255f;
            sr.color = c;
        }
    }
    public void UpdateCurrentPlayer(string name)
    {
        currentTurnNameTf.text = name;
    }
}
public enum BuildingToSpawn
{
    nullth, first, second
};
public enum UnitsToSpawn
{
    nullth, first, second, third
};
