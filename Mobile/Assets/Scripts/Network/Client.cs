using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class Client : MonoBehaviour
{
    // UI
    public InputField userInput;
    public InputField ipInput;
    public GameObject loginForm;
    public TMPro.TextMeshProUGUI infoText;
    public TMPro.TextMeshProUGUI killCooldownText;
    public GameObject TimerGO;

    // Network
    public string server;
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private UdpClient udpClient;
    private int udpCounter;
    private UdpServer udpServer;

    // Player Objects
    public GameObject playerPrefab;
    public GameObject otherPlayerPrefab;
    private GameObject player;
    private AU_PlayerController playerController;
    private int id;
    private bool imposter;
    private Dictionary<int, ClientTransform> otherClientsData;
    private ConcurrentDictionary<int, GameObject> otherClientsObjects;
    private float lerpSpeed = 5f;
    private static Color purple = new Color(166f/255f, 60f/255f, 176f/255f);
    private Color[] colors = { Color.red, Color.white, Color.green, Color.cyan, purple, Color.yellow };
    private ConcurrentQueue<object> outgoingData = new ConcurrentQueue<object>();
    private int? _killedPlayerId;
    private int? killedPlayerId
    {
        get { return _killedPlayerId; }
        set
        {
            if (_killedPlayerId != value)
            {
                _killedPlayerId = value;                
                object wrapper = new { actionType = "kill", id = _killedPlayerId };
                outgoingData.Enqueue(wrapper);
                Task.Run(() => SendTcpComm());
            }
        }
    }

    private void Awake()
    {
        udpClient = new UdpClient();
        udpCounter = 0;
        udpServer = new UdpServer(UdpReceived);
        otherClientsData = new Dictionary<int, ClientTransform>();
        otherClientsObjects = new ConcurrentDictionary<int, GameObject>();
    }

    private void OnDestroy()
    {
        tcpClient.Close();
        udpClient.Close();
    }

    private void UdpReceived(string payload)
    {
        if(payload.Length > 0) {
            //Debug.Log($"UdpReceived: {payload}");
            otherClientsData = JsonConvert.DeserializeObject<Dictionary<int, ClientTransform>>(payload);
        }
    }

    private void HandleAction(Action action)
    {
        Debug.Log($"Size of otherClientsObjects: {otherClientsObjects.Count}");

        foreach (var pair in otherClientsObjects)
        {
            Debug.Log($"Key: {pair.Key}, Value: {pair.Value}");
        }

        //Debug.Log($"Action received: {action.actionType}");
        //Debug.Log($"Action received: {action.id}");
        string type = action.actionType.Trim().ToLower();
        if (type == "kill")
        {   
            Debug.Log($"Player {action.id} was killed!");
            
            if (action.id == id)
            {
                Debug.Log("I was killed!");
                player.GetComponent<AU_PlayerController>().Die();
                return;
            }

            AU_PlayerController otherPlayerController = otherClientsObjects[action.id].GetComponent<AU_PlayerController>();
            otherPlayerController.Die();

            // set character sprite to invisible
            Transform sprite = otherClientsObjects[action.id].transform.GetChild(0);
            if (action.id != id) // if i m not the dead player
            {
                SpriteRenderer myAvatarSprite = sprite.GetComponent<SpriteRenderer>();
                myAvatarSprite.color = Color.clear;
            }

            Transform part = sprite.transform.GetChild(0);
            SpriteRenderer myPartSprite = part.GetComponent<SpriteRenderer>();
            myPartSprite.color = Color.clear;

        }
    }

    public async Task ListenTcpComm()
    {
        while (true)
        {
            try
            {
                if (tcpClient == null || !tcpClient.Connected)
                {
                    Debug.LogError("TCP client is not connected.");
                }

                var responsePayload = Utils.ReadData(tcpStream);
                var jsonResponse = Encoding.ASCII.GetString(responsePayload);

                if (jsonResponse.Length > 0)
                {
                    Debug.Log($"Server TCP response: {jsonResponse}");
                    var action = JsonUtility.FromJson<Action>(jsonResponse);
                    HandleAction(action);
                } else {
                    Debug.LogError("No data received from server.");
                }

                await Task.Delay(100);

            } catch (Exception ex) {
                Debug.LogError($"Error in ListenTcpComm: {ex.Message}");
            } finally {
                await Task.Delay(100); // Always wait a bit before the next iteration to avoid maxing out CPU usage
            }
        }
    }

    public void SendObject<T>(T obj)
    {
        if (tcpClient == null || !tcpClient.Connected)
        {
            Debug.LogError("TCP client is not connected.");
            return;
        }

        Debug.Log($"Sending object: {obj}");

        string request = JsonConvert.SerializeObject(obj);
        var requestPayload = Encoding.ASCII.GetBytes(request);
        tcpStream.Write(requestPayload, 0, requestPayload.Length);
    }

    public async Task SendTcpComm()
    {
        try
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                Debug.LogError("TCP client is not connected.");
            }

            // Dequeue and send data from the outgoingData queue
            if (outgoingData.TryDequeue(out object data))
            {
                SendObject(data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in SendTcpComm: {ex.Message}");
        }
    }

    public void OnLoginPress()
    {
        Debug.Log($"User input: {userInput.text}");
        Debug.Log($"Server input: {ipInput.text}");
        server = ipInput.text;

        tcpClient = new TcpClient(server, Utils.SERVER_TCP_PORT);
        tcpStream = tcpClient.GetStream();

        //Debug.Log($"User input: {userInput.text}");
        var request = new LoginRequest(userInput.text);
        var jsonRequest = JsonUtility.ToJson(request);
        var requestPayload = Encoding.ASCII.GetBytes(jsonRequest);
        tcpStream.Write(requestPayload, 0, requestPayload.Length);

        var responsePayload = Utils.ReadData(tcpStream);
        var jsonResponse = Encoding.ASCII.GetString(responsePayload);
        var response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

        id = response.id;
        imposter = response.imposter;
        //Debug.Log($"Login response: {response.id}");
        //Login response: {"id":0,"imposter":true}
        Debug.Log($"Login response: {jsonResponse}");
        //Debug.Log($"Login response: {response.imposter}");

        if (response.id == -1) {
            Debug.Log("Game is already started!");
        }


        player = Instantiate(playerPrefab);
        changePlayerColor(player, colors[response.color]);
        playerController = player.GetComponent<AU_PlayerController>();
        playerController.isImposter = response.imposter;
        playerController.OnPlayerKilled.AddListener(HandlePlayerKilled);

        if (imposter) TimerGO.SetActive(true);
        loginForm.SetActive(false);
        infoText.text = $"id: {response.id} \nimposter: {response.imposter} \n ";

        //udpServer.Start(Utils.CLIENT_UDP_PORT);
        // ayni bilgisayarda test etmek icin bunu kullanacagiz sonra silinecek
        udpServer.Start(Utils.CLIENT_UDP_PORT+response.id);

        Task.Run(() => ListenTcpComm());
    }

    private void HandlePlayerKilled(int killedPlayerId)
    {
        // Store the killed player's ID
        this.killedPlayerId = killedPlayerId;
    }

    private void Update()
    {   
        if (imposter)
        {
            killCooldownText.text = $"Kill Cooldown: {playerController.GetKillCooldownTimer()}";
        }

        if (player != null)
        {
            var playerData = new PlayerDataPacket(player.transform.position,
                id,
                udpCounter);

            udpCounter++;
            var jsonRequest = JsonUtility.ToJson(playerData);
            var requestPayload = Encoding.ASCII.GetBytes(jsonRequest);
            udpClient.Send(requestPayload, requestPayload.Length, server, Utils.SERVER_UDP_PORT);
            
            foreach (var clientData in otherClientsData)
            {   
                if (clientData.Key != id)
                {
                    GameObject clientGo;

                    // if client does not exist, create it
                    if (!otherClientsObjects.TryGetValue(clientData.Key, out clientGo))
                    {
                        Debug.LogWarning($"Creating new client {clientData.Key}");
                        //clientGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        clientGo = Instantiate(otherPlayerPrefab);
                        clientGo.transform.parent = transform;
                        clientGo.transform.position = Vector3.zero;
                        
                        bool added = otherClientsObjects.TryAdd(clientData.Key, clientGo);
                        if (added) {
                            Debug.Log("Added the item successfully.");
                        } else {
                            Debug.Log("Failed to add the item. A value with the same key already exists.");
                        }

                        AU_PlayerController otherPlayerController = clientGo.GetComponent<AU_PlayerController>();
                        if (otherPlayerController != null)
                        {
                            otherPlayerController.id = clientData.Key;
                            otherPlayerController.OnPlayerKilled.AddListener(HandlePlayerKilled);
                            changePlayerColor(clientGo, colors[clientData.Value.color]);
                        }
                    }
                    
                    var clientGoTransform = clientGo.transform;
                    // Lerping: Smoothly interpolate between old and new positions
                    clientGoTransform.position = Vector3.Lerp(
                        clientGoTransform.position,     // Current position
                        clientData.Value.position,      // Target position (new position from otherClientsData)
                        Time.deltaTime * lerpSpeed // Interpolation factor based on fixedDeltaTime and lerpSpeed
                    );

                    Vector2 movementVector = clientData.Value.position - clientGoTransform.position;

                    // Update the animation
                    Animator otherClientAnimator = clientGo.GetComponent<Animator>();
                    otherClientAnimator.SetFloat("Speed", movementVector.magnitude);

                    // Update the scale based on the direction of movement
                    if (movementVector.x != 0)
                    {
                        Transform otherClientAvatar = clientGo.transform;
                        otherClientAvatar.localScale = new Vector2(Mathf.Sign(movementVector.x), 1);
                    }

                    /*var clientGoTransform = clientGo.transform;
                    clientGoTransform.position = clientData.Value.position;
                    Debug.Log($"Position {clientGoTransform.position}");*/
                }
            }
        }
    }

    private void changePlayerColor(GameObject go, Color color)
    {
        Transform avatar = go.transform.GetChild(0);
        SpriteRenderer avatarSprite = avatar.GetComponent<SpriteRenderer>();
        avatarSprite.color = color;
    }
}