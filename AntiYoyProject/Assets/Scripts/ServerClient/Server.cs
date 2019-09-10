using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Unity.Collections;
using UnityEngine.SceneManagement;
using NetworkingConnection = Unity.Networking.Transport.NetworkConnection;

public class Server : MonoBehaviour
{
    private Dictionary<int, NetworkPlayer> playerDic = new Dictionary<int, NetworkPlayer>();
    private UdpNetworkDriver driver;
    private NetworkPipeline pipeline;
    private NetworkEndPoint endpoint;
    [SerializeField]
    public NativeList<NetworkConnection> connection;
    
    private ushort port = 9000;
    
    private GameObject gameStartBT;
    private bool started = false;
    public string ipv4;
    float timer = 0f;
    bool playerHasDisconnected = false;
    

    private void Start()
    {
        driver = new UdpNetworkDriver(new SimulatorUtility.Parameters { MaxPacketSize = 256, MaxPacketCount = 30, PacketDelayMs = 100 });
        pipeline = driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage), typeof(SimulatorPipelineStage));
        endpoint = new NetworkEndPoint();
        ipv4 = GetLocalIpAdress();
        
        
        gameStartBT = GameObject.FindGameObjectWithTag("GameStartBT");
        gameStartBT.SetActive(false);


    }
    
    public void StartServer()
    {
        endpoint = NetworkEndPoint.Parse(ipv4, port);
        if (driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to Port 9000");
        }
        else
        {
            driver.Listen();
            started = true;
            Debug.Log("Server started on ip : "+ipv4 +" and port : "+ port);
        }
        connection = new NativeList<NetworkConnection>(16,Allocator.Persistent);
        

        gameStartBT.SetActive(true);

    }

    // Server beendet sich
    private void OnDestroy()
    {
        driver.Dispose();
        connection.Dispose();
        playerDic.Clear();
    }

    private void Update()
    {
        if (!started)
        {
            return;
        }
        if (timer <= 25)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            SendCmdToAllClients("LEER");
            Debug.Log("Sending empty to keep connection");
        }
        
        driver.ScheduleUpdate().Complete();

        for (int i = 0; i < connection.Length; i++)
        {
            if (!connection[i].IsCreated)
            {
                connection.RemoveAtSwapBack(i);
                Debug.LogError("is created abfrage 1");
                --i;
            }
        }

        NetworkConnection con;

        while ((con = driver.Accept()) != default(NetworkConnection))
        {
            connection.Add(con);
            string cmdString = "AUTHENTICATE|";
            SendCmdToClient(cmdString, con);
            
        }

        DataStreamReader sr;

        for (int i = 0; i < connection.Length; i++)
        {
            if (!connection[i].IsCreated)
            {
                Debug.LogError("created abfrage");
                continue;
            }
            

            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connection[i], out sr)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    
                    Debug.Log("client " + i + " has connected");
                    
                    
                }

                if (cmd == NetworkEvent.Type.Disconnect)
                {
                    playerHasDisconnected = true;
                    playerDic.Remove(FindKeyByConID(i));
                    for (int j = 0; j < connection.Length; j++)
                    {
                        if (j != i)
                        {
                            
                        }
                    }
                   
                }
                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    byte[] data = sr.ReadBytesAsArray(ref readerCtx, sr.Length);
                    string cmdString = System.Text.Encoding.UTF8.GetString(data);
                    ProcessCmd(cmdString, i);
                }

            }
        }
        
        

    }
    private void PopulateConList(NativeList<NetworkConnection> temp)
    {
        connection.Clear();
        Debug.Log("Populating con List");
        Debug.Log("new con list = ");
        for (int i = 0; i < temp.Length; i++)
        {
            connection.Add(temp[i]);
            Debug.Log(i);
        }
        temp.Clear();
    }

    private void ProcessCmd(string cmd, int id)
    {
        string[] splitString = cmd.Split('|');
        if (splitString[0] == "AUTHENTICATE")
        {
            string name = splitString[1];
            AuthenticatePlayer(id, name);
            SendClientList();

            Debug.Log("Authenticating new Player with id : " + id + " and name : " + name);
        }
        if (splitString[0] == "NEXT_TURN")
        {
            SendCmdToAllClients(cmd);
        }
        if (splitString[0] == "MAP")
        {
            string cmdString = "";
            for (int i = 0; i < splitString.Length; i++)
            {
                cmdString += splitString[i];
            }
            SendCmdToAllClients(cmdString);
        }
    }
    private void SendCmdToClient(string cmd, NetworkConnection con)
    {
        byte[] cmdByte = System.Text.Encoding.UTF8.GetBytes(cmd);
        using (var writer = new DataStreamWriter(cmdByte.Length, Allocator.Temp))
        {
            writer.Write(cmdByte);
            con.Send(driver, writer);

        }
    }
    private void SendCmdToAllClients(string cmd)
    {
        byte[] cmdByte = System.Text.Encoding.UTF8.GetBytes(cmd);
        using (var writer = new DataStreamWriter(cmdByte.Length, Allocator.Temp))
        {
            writer.Write(cmdByte);
            for (int i = 0; i < connection.Length; i++)
            {
                connection[i].Send(driver, writer);
            }
        }
    }
    private void AuthenticatePlayer(int connectionId,string name)
    {
        NetworkPlayer player = new NetworkPlayer(playerDic.Count, connectionId, name);
        playerDic.Add(playerDic.Count, player);
        string cmdString = "";
        cmdString += "ASSIGN_ID|";
        cmdString += playerDic.Count-1;
        SendCmdToClient(cmdString, connection[connectionId]);

    }

    private void SendClientList()
    {
        string cmdString = "";
        cmdString += "SENDCLIENTS";

        for (int i = 0; i < playerDic.Count; i++)
        {
            NetworkPlayer current;
            playerDic.TryGetValue(i, out current);
            cmdString += "|" + current.name;
            cmdString += "|" + i;
        }

        SendCmdToAllClients(cmdString);
    }
    
    public void GameStart()
    {
        string cmdString = "";
        cmdString += "GAMESTART|";

        cmdString += "1";
        
        SendCmdToAllClients(cmdString);
    }

    private string GetLocalIpAdress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
            Debug.LogError("No Ipv4 found");
            return null;
    }
    private int FindKeyByConID(int connectionId)
    {
        int key = int.MinValue;
        foreach (KeyValuePair<int, NetworkPlayer> pair in playerDic)
        {
            if (pair.Value.connectionId == connectionId)
            {
                key = pair.Key;
                break;

            }
        }
        return key;

    }
}






















