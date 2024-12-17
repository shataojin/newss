using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkClientProcessing
{

    #region Send and Receive Data Functions

    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log($"Message Received from Server: {msg}");
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.BalloonCreation)
        {
            int playerID = int.Parse(csv[1]);
            float posX = float.Parse(csv[2]);
            float posY = float.Parse(csv[3]);

            gameLogic.CreateBalloon(new Vector2(posX, posY), playerID);
        }
        else
        {
            Debug.Log($"Unhandled Signifier: {signifier}");
        }
    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
    }
    static public bool IsConnectedToServer()
    {
        return networkClient.IsConnected();
    }
    static public void ConnectTozServer()
    {
        networkClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkClient.Disconnect();
    }

    #endregion

    #region Setup
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
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
    public const int PlayerPosition = 3;      // 客户端发送玩家位置

}


static public class ServerToClientSignifiers
{
    public const int BalloonCreation = 2; // 服务器广播气球创建
}


#endregion

