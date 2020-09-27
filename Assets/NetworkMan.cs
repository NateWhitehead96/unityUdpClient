﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class NetworkMan : MonoBehaviour
{
    
    public GameObject prefab;
    private GameObject spawnedObject;

    //private string[] IDList;

    public UdpClient udp;
    // Start is called before the first frame update
    void Start()
    {
        udp = new UdpClient();

        udp.Connect("localhost", 12345);
        //udp.Connect("3.129.208.0", 12345);

        Byte[] sendBytes = Encoding.ASCII.GetBytes("connect");
      
        udp.Send(sendBytes, sendBytes.Length);

        udp.BeginReceive(new AsyncCallback(OnReceived), udp);

        InvokeRepeating("HeartBeat", 1, 1);
    }

    void OnDestroy(){
        udp.Dispose();
    }


    public enum commands{
        NEW_CLIENT,
        UPDATE
    };
    
    [Serializable]
    public class Message{
        public commands cmd;
        public Player player;
    }
    
    [Serializable]
    public class Player{
        [Serializable]
        public struct receivedColor{
            public float R;
            public float G;
            public float B;
        }
        public string id;
        public receivedColor color;
        public bool spawned = false;
        public GameObject playerObject;
    }

    [Serializable]
    public class NewPlayer{
        
    }

    [Serializable]
    public class GameState{
        public Player[] players;
    }

    public List<Player> playerList;

    public Message latestMessage;
    public GameState lastestGameState;
    void OnReceived(IAsyncResult result){
        // this is what had been passed into BeginReceive as the second parameter:
        UdpClient socket = result.AsyncState as UdpClient;
        
        // points towards whoever had sent the message:
        IPEndPoint source = new IPEndPoint(0, 0);

        // get the actual message and fill out the source:
        byte[] message = socket.EndReceive(result, ref source);
        
        // do what you'd like with `message` here:
        string returnData = Encoding.ASCII.GetString(message);
        Debug.Log("Got this: " + returnData);
        
        latestMessage = JsonUtility.FromJson<Message>(returnData);
        try{
            switch(latestMessage.cmd){
                case commands.NEW_CLIENT:
                    Player tempPlayer = new Player();
                    tempPlayer.id = latestMessage.player.id;
                    playerList.Add(tempPlayer);
                    //SpawnPlayers();
                    break;
                case commands.UPDATE:
                    lastestGameState = JsonUtility.FromJson<GameState>(returnData);
                    UpdatePlayers();
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        catch (Exception e){
            Debug.Log(e.ToString());
        }
        
        // schedule the next receive operation once reading is done:
        socket.BeginReceive(new AsyncCallback(OnReceived), socket);
    }

    void SpawnPlayers(){
        //Player player = new Player();
        //player.id = lastestGameState.players[0].id;
        //player.color = lastestGameState.players[0].color;
        //spawnedObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        //Renderer renderer = spawnedObject.GetComponent<Renderer>();
        //renderer.material.color = new Color(lastestGameState.players[0].color.R, lastestGameState.players[0].color.B, lastestGameState.players[0].color.G);

        //for (int i = 0; i < lastestGameState.players.Length; i++)
        //{
        //    //IDList[i] = lastestGameState.players[i].id;
        //if (lastestGameState.players[i].spawned == false)
        //{
        //    spawnedObject = Instantiate(prefab, new Vector3(3, 0, 0), Quaternion.identity);
        //    lastestGameState.players[i].playerObject = spawnedObject;
        //    lastestGameState.players[i].spawned = true;
        //    Debug.Log(lastestGameState.players.Length);
        //}
        //}

        foreach (Player player in playerList) // go through each player in the player list
        {
            if (player.spawned == false) // do the spawn
            {
                spawnedObject = Instantiate(prefab, new Vector3(UnityEngine.Random.Range(-5, 5), 0, 0), Quaternion.identity);
                player.playerObject = spawnedObject;
                player.spawned = true;
                //Debug.Log(lastestGameState.players.Length);
            }
        }
        
    }

    void UpdatePlayers(){
        //if(spawnedObject.scene.IsValid())
        //{
        //    Renderer renderer = spawnedObject.GetComponent<Renderer>();
        //    renderer.material.color = new Color(lastestGameState.players[0].color.R, lastestGameState.players[0].color.B, lastestGameState.players[0].color.G);

        //}
        for (int i = 0; i < lastestGameState.players.Length; i++)
        {
            foreach (Player player in playerList)
            {
                if(player.id == lastestGameState.players[i].id) // match up id's and update the colours
                {
                    Renderer renderer = player.playerObject.GetComponent<Renderer>();
                    player.color = lastestGameState.players[i].color;
                    renderer.material.color = new Color(player.color.R, player.color.G, player.color.B);
                }
            }
            
        }

        
    }

    void DestroyPlayers(){
    }
    
    void HeartBeat(){
        Byte[] sendBytes = Encoding.ASCII.GetBytes("heartbeat");
        udp.Send(sendBytes, sendBytes.Length);
    }

    void Update()
    {
        SpawnPlayers();
        UpdatePlayers();
        DestroyPlayers();
    }
}
