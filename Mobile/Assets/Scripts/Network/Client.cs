using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Client : MonoBehaviour
{
    public InputField userInput;
    public string server;
    public GameObject playerPrefab;
    public GameObject otherPlayerPrefab;
    private GameObject player;
    public GameObject loginForm;
    private int id;
    private TcpClient tcpClient;
    private UdpClient udpClient;
    private int udpCounter;
    private UdpServer udpServer;
    private Dictionary<int, ClientTransform> otherClientsData;
    private Dictionary<int, GameObject> otherClientsObjects;
    private float lerpSpeed = 5f;

    private void Awake()
    {
        tcpClient = new TcpClient(server, Utils.SERVER_TCP_PORT);
        udpClient = new UdpClient();
        udpCounter = 0;
        udpServer = new UdpServer(UdpReceived);
        otherClientsData = new Dictionary<int, ClientTransform>();
        otherClientsObjects = new Dictionary<int, GameObject>();
    }

    private void UdpReceived(string payload)
    {
        if(payload.Length > 0) {
            otherClientsData = JsonConvert.DeserializeObject<Dictionary<int, ClientTransform>>(payload);
        }
    }

    private void OnDestroy()
    {
        tcpClient.Close();
        udpClient.Close();
    }

    public void OnLoginPress()
    {
        using (var nwStream = tcpClient.GetStream())
        {
            Debug.Log($"User input: {userInput.text}");
            var request = new LoginRequest(userInput.text);
            var jsonRequest = JsonUtility.ToJson(request);
            var requestPayload = Encoding.ASCII.GetBytes(jsonRequest);
            nwStream.Write(requestPayload, 0, requestPayload.Length);

            var responsePayload = Utils.ReadData(nwStream);
            var jsonResponse = Encoding.ASCII.GetString(responsePayload);
            var response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

            id = response.id;
            Debug.Log($"Login response: {response.id}");
            player = Instantiate(playerPrefab);            
            loginForm.SetActive(false);
            //udpServer.Start(Utils.CLIENT_UDP_PORT);
            // ayni bilgisayarda test etmek icin bunu kullanacagiz sonra silinecek
            udpServer.Start(Utils.CLIENT_UDP_PORT+response.id);
        }
    }

    private void Update()
    {   
        if (player != null)
        {
            var playerData = new PlayerDataPacket(new ClientTransform(player.transform.position, player.transform.rotation),
                id,
                udpCounter);
            udpCounter++;
            //Debug.Log($"udpCounter {udpCounter}");
            var jsonRequest = JsonUtility.ToJson(playerData);
            var requestPayload = Encoding.ASCII.GetBytes(jsonRequest);
            udpClient.Send(requestPayload, requestPayload.Length, server, Utils.SERVER_UDP_PORT);
            foreach (var clientData in otherClientsData)
            {
                //Debug.Log($"Client key {clientData.Key}");
                //Debug.Log($"Client value {clientData.Value}");
                if (clientData.Key != id)
                {
                    //Debug.LogError($"Client key: {clientData.Key}");
                    GameObject clientGo;

                    // if client does not exist, create it
                    if (!otherClientsObjects.TryGetValue(clientData.Key, out clientGo))
                    {
                        Debug.LogWarning($"Creating new client {clientData.Key}");
                        //clientGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        clientGo = Instantiate(otherPlayerPrefab);
                        clientGo.transform.parent = transform;
                        clientGo.transform.position = Vector3.zero;
                        otherClientsObjects.Add(clientData.Key, clientGo);
                    }

                    
                    var clientGoTransform = clientGo.transform;
                        // Lerping: Smoothly interpolate between old and new positions
                        clientGoTransform.position = Vector3.Lerp(
                        clientGoTransform.position,     // Current position
                        clientData.Value.position,      // Target position (new position from otherClientsData)
                        Time.deltaTime * lerpSpeed // Interpolation factor based on fixedDeltaTime and lerpSpeed
                    );

                    /*var clientGoTransform = clientGo.transform;
                    clientGoTransform.position = clientData.Value.position;
                    Debug.Log($"Position {clientGoTransform.position}");*/
                    clientGoTransform.rotation = clientData.Value.rotation;
                }
            }
        }
    }
}