using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkServerProcessing
{

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
        Debug.Log("Network msg received = " + msg + ", from connection id = " + clientConnectionID + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.SendBalloonPosition)
        {
            float posX = float.Parse(csv[1]);
            float posY = float.Parse(csv[2]);
            BroadcastBalloonCreation(new Vector2(posX, posY), clientConnectionID);
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
        Debug.Log("Client connection, ID == " + clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("Client disconnection, ID == " + clientConnectionID);
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
}

static public class ServerToClientSignifiers
{
    public const int BalloonCreation = 2; // 服务器广播气球创建
}


#endregion

