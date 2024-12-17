using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkServerProcessing
{

    static Dictionary<int, Vector2> playerPositions = new Dictionary<int, Vector2>();


    #region Send and Receive Data Functions

    static public void BroadcastBalloonCreation(Vector2 position, int excludedClientID)
    {
        string message = $"{ServerToClientSignifiers.BalloonCreation},{position.x},{position.y}";

        foreach (var clientID in networkServer.idToConnectionLookup.Keys)
        {
            if (clientID != excludedClientID) // 跳过发送者
            {
                SendMessageToClient(message, clientID, TransportPipeline.ReliableAndInOrder);
            }
        }
    }

    static public void ReceivedMessageFromClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        Debug.Log($"Received Message: '{msg}' from Client ID: {clientConnectionID}, Pipeline: {pipeline}");
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.PlayerPosition)
        {
            float posX = float.Parse(csv[1]);
            float posY = float.Parse(csv[2]);
            UpdatePlayerPosition(clientConnectionID, new Vector2(posX, posY));
        }
        else
        {
            Debug.Log($"Unhandled Signifier: {signifier} from Client ID: {clientConnectionID}");
        }
    }


    static void UpdatePlayerPosition(int clientID, Vector2 position)
    {
        if (playerPositions.ContainsKey(clientID))
        {
            playerPositions[clientID] = position; // 更新已有玩家位置
        }
        else
        {
            playerPositions.Add(clientID, position); // 添加新玩家位置
        }

        Debug.Log($"Updated position for Client {clientID}: {position}");
     
    }

    public static void BroadcastPlayerPositions()
    {
        Debug.Log("Broadcasting Player Positions:");
        foreach (var kvp in playerPositions)
        {
            int clientID = kvp.Key;
            Vector2 position = kvp.Value;

            Debug.Log($"Client ID: {clientID}, Position: {position}");

            string message = $"{ServerToClientSignifiers.BalloonCreation},{clientID},{position.x},{position.y}";

            foreach (var targetClientID in networkServer.idToConnectionLookup.Keys)
            {
                SendMessageToClient(message, targetClientID, TransportPipeline.ReliableAndInOrder);
            }
        }
    }




    static public void SendMessageToClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID, pipeline);
    }






    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("Client connected, ID == " + clientConnectionID);
        playerPositions.Add(clientConnectionID, Vector2.zero);
        PrintConnectedClients();
    }

    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("Client disconnected, ID == " + clientConnectionID);
        if (playerPositions.ContainsKey(clientConnectionID))
            playerPositions.Remove(clientConnectionID);
        PrintConnectedClients();
    }

    // 打印当前所有已连接客户端的信息
    static void PrintConnectedClients()
    {
        Debug.Log("Current Connected Clients:");
        foreach (var clientID in networkServer.idToConnectionLookup.Keys)
        {
            Debug.Log($"Client ID: {clientID}, Position: {playerPositions.GetValueOrDefault(clientID, Vector2.zero)}");
        }
    }

   

    #endregion

    #region Setup
    static NetworkServer networkServer;
    static GameLogic gameLogic;

    static public void SetNetworkServer(NetworkServer NetworkServer)
    {
        networkServer = NetworkServer;
    }
    static public NetworkServer GetNetworkServer()
    {
        return networkServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }



    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int SendBalloonPosition = 1; // 客户端发送气球位置
    public const int PlayerPosition = 3; // 客户端发送玩家位置

}

static public class ServerToClientSignifiers
{
    public const int BalloonCreation = 2; // 服务器广播气球创建
}


#endregion

