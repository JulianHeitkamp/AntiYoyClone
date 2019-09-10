
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Unity.Collections;
 using UnityEngine.SceneManagement;

 public class Client : MonoBehaviour
{
    public Dictionary<int,NetworkPlayer> playerDic = new Dictionary<int, NetworkPlayer>();

    private UdpNetworkDriver driver;
    private NetworkPipeline pipeline;
    private NetworkEndPoint endpoint;
    private NetworkConnection connection;
    public NetworkPlayer me;
    GameManager gm;
    private bool started = false;
    

    private void Start()
    {
        driver = new UdpNetworkDriver(new SimulatorUtility.Parameters{MaxPacketSize = 256, MaxPacketCount = 30, PacketDelayMs = 100});
        pipeline = driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage), typeof(SimulatorPipelineStage));
        connection = default;

        gm = GetComponent<GameManager>();
        endpoint = new NetworkEndPoint();
        
    }

    public void Join(string ip, string name)
    {
        endpoint = NetworkEndPoint.Parse(ip, 9000);
        connection = driver.Connect(endpoint);
        started = true;
        me = new NetworkPlayer(0,0,name);
    }

    private void OnDestroy()
    {
        driver.Dispose();
    }

    private void Update()
    {
        if (!started)
        {
            return;
        }
        driver.ScheduleUpdate().Complete();
        if (!connection.IsCreated)
        {
            return;
        }

        DataStreamReader sr;

        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver,out sr)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
            }
            
            if (cmd == NetworkEvent.Type.Disconnect)
            {
            }
            if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                byte[] data = sr.ReadBytesAsArray(ref readerCtx, sr.Length);
                string cmdString = System.Text.Encoding.UTF8.GetString(data);
                
                ProcessCommand(cmdString);
            }
        }
    }

    private void ProcessCommand(string cmd )
    {
        string[] splitString = cmd.Split('|');

        if (splitString[0] == "LEER")
        {
            SendCmdToServer("LEER");
        }
        if (splitString[0] == "AUTHENTICATE")
        {
            AuthenticateToServer();
        }
        if (splitString[0] == "ASSIGN_ID")
        {
            me.id = int.Parse(splitString[1]);
        }
        if (splitString[0] == "SENDCLIENTS")
        {
            playerDic.Clear();
            for (int i = 1; i < splitString.Length; i += 2)
            {
                int id = playerDic.Count;
                NetworkPlayer current = new NetworkPlayer(id, int.Parse(splitString[i + 1]), splitString[i]);
                playerDic.Add(playerDic.Count, current);
            }

            for (int i = 0; i < playerDic.Count; i++)
            {
                NetworkPlayer current = playerDic[i];
                // playerDic.TryGetValue(i, out current);
            }
        }

        if (splitString[0] == "GAMESTART")
        {
            SceneManager.LoadScene(int.Parse(splitString[1]));
        }
        if (splitString[0] == "NEXT_TURN")
        {
            gm.NewTurn();
        }
        if (splitString[0] =="MAP")
        {
            string test = "";
            for (int i = 0; i < splitString.Length; i++)
            {
                test += splitString[i];
            }
            Debug.Log(test);
        }
        
    }
    private void SendCmdToServer(string cmd)
    {
        byte[] cmdByte = System.Text.Encoding.UTF8.GetBytes(cmd);
        using (var writer = new DataStreamWriter(cmdByte.Length, Allocator.Temp))
        {
            writer.Write(cmdByte);
            connection.Send(driver, writer);
        }
    }

    private void AuthenticateToServer()
    {
        string cmdString = "AUTHENTICATE|";
        cmdString += me.name;
        SendCmdToServer(cmdString);

    }
    public void SendMapToClients(List<Tile> map, int width, int height)
    {
        string cmdString = "MAP|";
        cmdString += width + "|" +height;
        for (int i = 0; i < map.Count; i++)
        {
            cmdString += "|"+map[i].type +"|"+ map[i].unit;
        }
        SendCmdToServer(cmdString);
    }
    public void EndTurn()
    {
        string cmdString = "NEXT_TURN|";
        SendCmdToServer(cmdString);
    }
}
[System.Serializable]
public class NetworkPlayer
{
    public int id;
    public int connectionId;
    public string name;
    public NetworkPlayer(int _id, int _connectionId, string _name)
    {
        id = _id;
        connectionId = _connectionId;
        name = _name;
    }

}





















