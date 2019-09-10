using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public HexagonGrid map;

    public InputField IpField;
    public InputField NameField;
    Server server;
    Client client;
    public int turn;
    int playerCount;
    public GameObject playerPrefab;
    PlayerStats stats;
    UIManager ui;

    public List<TreeBehaviour> trees;
    public List<TreeBehaviour> addedTrees;



    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        server = GetComponent<Server>();
        client = GetComponent<Client>();
        IpField.text = server.ipv4;
        
    }
    public void StartServer()
    {
        server.StartServer();
    }
    public void Join()
    {
        if (IpField.text == "")
        {
            Debug.Log("Pls enter the hosts ip");
            return;
        }
        if (NameField.text == "")
        {
            Debug.Log("pls enter a Name");
            return;
        }
        client.Join(IpField.text,NameField.text);  

    }
    public void NextTurn()// function to connect to a button. tells the server that a turn has ended
    {
        Debug.Log("current player = " + turn);
        Debug.Log("my id is: " + client.me.id);
        if (turn == client.me.id)
        {
            ui.UpdateCurrentPlayer("You");
            client.EndTurn(); 
            Debug.Log("New Turn");
            
        }
        else
        {
            NetworkPlayer player;
            client.playerDic.TryGetValue(turn,out player);
            ui.UpdateCurrentPlayer(player.name);
        }

    }
    public void NewTurn() // getting notified by the server that a new turn has Started
    {
        turn++;
        if (turn >= playerCount-1)
        {
            
            foreach (var tree in trees)
            {
                tree.NewTurn();
            }
            foreach (var tree in addedTrees)
            {
                trees.Add(tree);
            }
            turn = 0;
        }
        Debug.Log("Current player = " + turn);
        if (turn == client.me.id)
        {
            stats.NextTurn();
        }
    }
    private void OnLevelWasLoaded(int level)
    {

        if (level == 1)
        {
            trees = new List<TreeBehaviour>();
            addedTrees = new List<TreeBehaviour>();
            turn = 0;
            playerCount = client.playerDic.Count;
            ui = GameObject.FindWithTag("UI").GetComponent<UIManager>();
            map = GameObject.FindWithTag("Map").GetComponent<HexagonGrid>();

            if (client.me.id == 0)
            {
                map.DrawMap(playerCount);
            }
            
            GameObject player = Instantiate(playerPrefab,new Vector3(0,0,0),Quaternion.identity);
            stats = player.GetComponent<PlayerStats>();
            stats.id = client.me.id;
            player.GetComponent<playerController>().id = client.me.id;
            switch (client.me.id)
            {
                case 0:
                    stats.GameStart(client.me.id, map.tP1);
                    
                    break;
                case 1:
                    stats.GameStart(client.me.id, map.tP2);
                    break;
                case 2:
                    stats.GameStart(client.me.id, map.tP3);
                    break;
                case 3:
                    stats.GameStart(client.me.id, map.tP4);
                    break;
                default:
                    break;
            }
            if (turn == client.me.id)
            {
                stats.NextTurn();
                ui.UpdateCurrentPlayer("You");

            }
            else
            {
                NetworkPlayer p;
                client.playerDic.TryGetValue(turn, out p);
                ui.UpdateCurrentPlayer(p.name);
            }
            if (client.me.id == 0)
            {

                
                client.SendMapToClients(map.map, map.width, map.height);

            }
        }
    }
}
