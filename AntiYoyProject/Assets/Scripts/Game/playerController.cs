using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    Camera cam;
    public LayerMask mapLayer;
    PlayerStats stats;
    UIManager ui;
    public int id;
    public bool spawningSomething = false;
    GameManager gm;
    Client client;
    void Start()
    {
        stats = GetComponent<PlayerStats>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        client = gm.GetComponent<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.turn == client.me.id)
        {
            if (Input.GetMouseButtonDown(0))
            {

                Debug.Log("pressing left mouse");
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 cursorPos2D = new Vector2(cursorPosition.x, cursorPosition.y);
                RaycastHit2D hit = Physics2D.Raycast(cursorPos2D, Camera.main.transform.forward, 1000f, mapLayer);
                Debug.Log(spawningSomething);
                if (spawningSomething == false)
                {
                    if (hit.transform != null)
                    { 
                        Territory t = hit.transform.parent.gameObject.GetComponent<Tile>().territory;

                        if (stats.territories.Contains(t))
                        {
                            ui.SelectTerritory(t);
                            Unit u = hit.transform.parent.GetComponent<Tile>().unit;
                            if (u != null)
                            {
                                if (u.type == true)
                                {
                                    Debug.Log("moving a unit");
                                }
                            } 
                        }


                    }
                }
                else
                {
                    Tile t = hit.transform.parent.gameObject.GetComponent<Tile>();
                    if (ui.tilesToSpawnOn.Contains(t))
                    {
                        Debug.Log("it does contain");
                        if (ui.spawningBuilding == true)
                        {
                            Debug.Log("Spawning building is true");
                            GameObject objectToSpawn = Instantiate(ui.buildingsObjects[ui.buildingsLevel].objectToSpawn, t.transform);
                            Territory terr = ui.selected;
                            if (id == 0)
                            {
                                t.SetTileType(TileType.p1);

                            }
                            else if (id == 1)
                            {
                                t.SetTileType(TileType.p2);
                            }
                            else if (id == 2)
                            {
                                t.SetTileType(TileType.p3);
                            }
                            else
                            {
                                t.SetTileType(TileType.p4);
                            }
                            if (t.unit != null)
                            {
                                Destroy(t.transform.GetChild(3).gameObject);
                            }
                            t.unit = ui.buildingsObjects[ui.buildingsLevel];
                            t.territory = terr;
                            terr.AddTile(t);
                            terr.gold -= ui.buildingsObjects[ui.buildingsLevel].cost;
                            ui.UpdateGoldTf();
                            ui.DisableAllUnits();
                            spawningSomething = false;
                        }
                        else
                        {
                            Debug.Log("unitslevel while spawning" + ui.unitsLevel);

                            GameObject objectToSpawn = Instantiate(ui.unitsObjects[ui.unitsLevel].objectToSpawn, t.transform);
                            Territory terr = ui.selected;
                            if (id == 0)
                            {
                                t.SetTileType(TileType.p1);

                            }
                            else if (id == 1)
                            {
                                t.SetTileType(TileType.p2);
                            }
                            else if (id == 2)
                            {
                                t.SetTileType(TileType.p3);
                            }
                            else
                            {
                                t.SetTileType(TileType.p4);
                            }
                            if (t.unit != null)
                            {
                                Destroy(t.transform.GetChild(3).gameObject);
                            }
                            t.unit = ui.unitsObjects[ui.unitsLevel];
                            t.territory = terr;
                            terr.AddTile(t);
                            terr.gold -= ui.unitsObjects[ui.unitsLevel].cost;
                            ui.UpdateGoldTf();
                            ui.DisableAllUnits();
                            spawningSomething = false;

                        }


                    }
                }
            } 
        }
    }
}
