using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Unity.VisualScripting;

public class Client : MonoBehaviour
{
    // UI
    public InputField userInput;
    public InputField ipInput;
    public InputField deviceInput;
    public GameObject loginForm;
    public Canvas canvas2;
    public GameObject votingForm;
    public GameObject UIControls;
    public Canvas UIControlsCanvas;
    public TMPro.TextMeshProUGUI infoText;
    public TMPro.TextMeshProUGUI killCooldownText;
    public GameObject TimerGO;
    public TMPro.TMP_Dropdown devicesDropdown;
    private bool infoTextUpdated = false;
    public TMPro.TextMeshProUGUI gameEndText;
    public GameObject GameEndGo;
    private bool GameEndTextActive = false;

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
    public ConcurrentDictionary<int, ClientTransform> otherPlayersData;
    private float lerpSpeed = 5f;
    public static bool reportInProgress = false;
    private Vector3 UIControlsPosition;


    private ConcurrentQueue<object> outgoingData = new ConcurrentQueue<object>();
    private int? _votedPlayerId;
    private int? _reportedPlayerId;

    private int? reportedPlayerId
    {
        get { return _reportedPlayerId; }
        set
        {
            if (_reportedPlayerId != value)
            {
                Debug.Log("Sending reported id");
                _reportedPlayerId = value;                
                object wrapper = new { actionType = Utils.ActionType.Report, id = _reportedPlayerId };
                outgoingData.Enqueue(wrapper);
                Task.Run(() => SendTcpComm());
            }
        }
    }

    /*private int? votedPlayerId
    {
        get { return _votedPlayerId; }
        set
        {
            if (_reportedPlayerId != value)
            {
                Debug.Log("Sending voted id");
                _votedPlayerId = value;                
                object wrapper = new { actionType = Utils.ActionType.Voted, id = _votedPlayerId };
                outgoingData.Enqueue(wrapper);
                Task.Run(() => SendTcpComm());
            }
        }
    }*/

    private void Awake()
    {
        votingForm.SetActive(false);
        canvas2.gameObject.SetActive(false);
        udpClient = new UdpClient();
        udpCounter = 0;
        udpServer = new UdpServer(UdpReceived);
        otherClientsData = new Dictionary<int, ClientTransform>();
        otherClientsObjects = new ConcurrentDictionary<int, GameObject>();
        otherPlayersData = new ConcurrentDictionary<int, ClientTransform>();
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
        //Debug.Log($"Size of otherClientsObjects: {otherClientsObjects.Count}");

        /*foreach (var pair in otherClientsObjects)
        {
            Debug.Log($"Key: {pair.Key}, Value: {pair.Value}");
        }*/

        //Debug.Log($"Action received: {action.actionType}");
        //Debug.Log($"Action received: {action.id}");

        Utils.ActionType actionType = action.actionType;

        // KILL
        if (actionType == Utils.ActionType.Killed)
        {   
            KillPlayer(action.id);
            UpdateInfoText();
        }
        
        // REPORT
        else if (actionType == Utils.ActionType.Report)
        {
            Debug.Log($"Player {action.id} was reported!");
            
            if (!playerController.isDead)
            {                
                //playerController.reportingInProgress = true;
                reportInProgress = true;

                votingForm.SetActive(true);
                canvas2.gameObject.SetActive(true);
                Debug.Log("bbbbbbbbbbbb");

                if (votingForm.GetComponentInChildren<VotingListControl>().time <= 0f)
                {
                    int voted;
                    if(!int.TryParse(votingForm.GetComponentInChildren<VotingListControl>().VotedPlayer.text.ToString(), out voted)){
                        voted = -1;
                    }
                    Debug.Log("Voting screen: Voted player: " + voted);

                    if (voted != -1)
                    {
                        //playerController.OnPlayerVoted.Invoke(voted);

                    }
                }

                reportInProgress = false;
                
                Debug.Log("ssssssssssssssss");
                //playerController.reportingInProgress = false;
            }
            else{
                Debug.Log("I was dead!");
            }

            
            
            // set character sprite to invisible
            //Destroy(otherClientsObjects[action.id]); 
        }

        /*else if (actionType == Utils.ActionType.VoteKill)
        {
            Debug.Log("VoteKill: " + action.id);
            KillPlayer(action.id);
        }*/

        else if (actionType == Utils.ActionType.BackStart)
        {
            player.transform.position = Vector3.zero;
        }

        else if (actionType == Utils.ActionType.GameOver )
        {
            Debug.Log("GameEnd: " + action.id);
            
            if (action.id == 0) 
            {
                gameEndText.text = "Imposter \nWin!";
            }
            else
            {
                gameEndText.text = "Crewmates \nWin!";
            }

            GameEndTextActive = true;
        }
    }

    private void KillPlayer(int playerId)
    {
        Debug.Log($"Player {playerId} was killed!");
            
            if (playerId == id)
            {
                Debug.Log("I was killed!");
                player.GetComponent<AU_PlayerController>().Die();
                return;
            }

            // set character sprite to invisible for other players

            // get players canvas and set it to invisible
            otherClientsObjects[playerId].GetComponentInChildren<Canvas>().enabled = false;

            AU_PlayerController otherPlayerController = otherClientsObjects[playerId].GetComponent<AU_PlayerController>();
            otherPlayerController.Die();

            // set character sprite to invisible
            Transform sprite = otherClientsObjects[playerId].transform.GetChild(0);
            if (playerId != id) // if i m not the dead player
            {
                SpriteRenderer myAvatarSprite = sprite.GetComponent<SpriteRenderer>();
                myAvatarSprite.color = Color.clear;
            }

            otherPlayersData.Remove(playerId, out var ignore);

            Transform part = sprite.transform.GetChild(0);
            SpriteRenderer myPartSprite = part.GetComponent<SpriteRenderer>();
            myPartSprite.color = Color.clear;
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
                Debug.Log("tcp json response" + jsonResponse);

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
        server = ipInput.text;
        string deviceName = devicesDropdown.options[devicesDropdown.value].text;

        Debug.Log($"User input: {userInput.text}");
        Debug.Log($"Server input: {ipInput.text}");
        Debug.Log($"Device input: {deviceName}");

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
        changePlayerColor(player, Utils.colors[response.color]);

        playerController = player.GetComponent<AU_PlayerController>();
        playerController.setDeviceName(deviceName == "None" ? "" : deviceName);
        playerController.isImposter = response.imposter;
        playerController.id = response.id;
        playerController.isLocalPlayer = true;
        playerController.SetName(userInput.text);
        //Debug.Log("numRemainingTask: " + response.numRemainingTask);

        // create a list that containts numRemainingTask number of ints
        // each int is a taskID, task ids are between 0-21 and they are unique
        if ( ! playerController.isImposter)
        {
            playerController.taskIDs = Enumerable.Range(0, response.numRemainingTask)
                .Select(_ => UnityEngine.Random.Range(0, 21))
                .Distinct()
                .ToList();
        }

        // fill list by hand
        /*playerController.taskIDs = new List<int>();
        playerController.taskIDs.Add(0);
        playerController.taskIDs.Add(15);
        playerController.taskIDs.Add(9);*/

        playerController.initializeTaskPointers();
        
        playerController.OnPlayerKilled.AddListener(HandlePlayerKilled);
        playerController.OnPlayerReported.AddListener(HandlePlayerReported);
        //OnPlayerVoted.AddListener(HandlePlayerVoted);
        playerController.OnPlayerDoneTask.AddListener(HandlePlayerDoneTask);

        VotingListControl votingListControl = votingForm.GetComponentInChildren<VotingListControl>();
        votingListControl.OnPlayerVoted.AddListener(HandlePlayerVoted);
        
        if (imposter) TimerGO.SetActive(true);
        loginForm.SetActive(false);
        UIControls.SetActive(true);
        UIControlsPosition = UIControls.transform.position;

        Debug.Log("UIControlsPosition:");
        Debug.Log(UIControls.transform.position.x + " " + UIControls.transform.position.y + " " + UIControls.transform.position.z);


        UpdateInfoText();

        //udpServer.Start(Utils.CLIENT_UDP_PORT);
        // ayni bilgisayarda test etmek icin bunu kullanacagiz sonra silinecek
        udpServer.Start(Utils.CLIENT_UDP_PORT+response.id);

        Task.Run(() => ListenTcpComm());
    }

    private void HandlePlayerKilled(int killedPlayerId)
    {
        object wrapper = new { actionType = Utils.ActionType.Killed, id = killedPlayerId };
        outgoingData.Enqueue(wrapper);
        Task.Run(() => SendTcpComm());
        UpdateInfoText();
    }

    private void HandlePlayerReported(int reportedPlayerId)
    {
        // Store the reported player's ID
        this.reportedPlayerId = reportedPlayerId;
    }
    private void HandlePlayerVoted(int votedPlayerId)
    {
        Debug.Log("Sending voted id in handle player voted");
        object wrapper = new { actionType = Utils.ActionType.Voted, id = votedPlayerId };
        outgoingData.Enqueue(wrapper);
        Task.Run(() => SendTcpComm());
    }

    private void HandlePlayerDoneTask(int taskDonePlayerId)
    {
        object wrapper = new { actionType = Utils.ActionType.TaskDone, id = taskDonePlayerId};
        outgoingData.Enqueue(wrapper);
        Task.Run(() => SendTcpComm());
        UpdateInfoText();
    }

    private void Update()
    {   
        if (imposter)
        {
            killCooldownText.text = $"Kill Cooldown: {playerController.GetKillCooldownTimer()}";
        }

        if (infoTextUpdated)
        {
            UpdateInfoText();
            infoTextUpdated = false;
        }

        if (GameEndTextActive)
        {
            GameEndGo.SetActive(true);
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

            bool fillPlayerData = false;
            if (otherPlayersData.IsEmpty)
            {
                fillPlayerData = true;
            }
            
            foreach (var clientData in otherClientsData)
            {   
                if (clientData.Key != id)
                {
                    GameObject clientGo;

                    if (fillPlayerData)
                    {
                        Debug.Log($"Filling player data: {clientData.Key}");
                        otherPlayersData.TryAdd(clientData.Key,clientData.Value);
                        Debug.Log("Update client name: " + clientData.Value.name);
                    }

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
                            otherPlayerController.OnPlayerReported.AddListener(HandlePlayerReported);
                            //playerController.OnPlayerVoted.AddListener(HandlePlayerVoted);
                            changePlayerColor(clientGo, Utils.colors[clientData.Value.color]);
                            otherPlayerController.SetName(clientData.Value.name);
                        }

                        UpdateInfoText();
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

                        // get name canvas of the other player change its direction like above, otherwise names are reversed
                        Transform otherClientName = clientGo.transform.GetChild(1);
                        otherClientName.localScale = new Vector2(Mathf.Sign(movementVector.x), 1);
                    }

                    /*var clientGoTransform = clientGo.transform;
                    clientGoTransform.position = clientData.Value.position;
                    Debug.Log($"Position {clientGoTransform.position}");*/
                }
            }
            fillPlayerData = false;
        }
    }

    private void FixedUpdate()
    {
        /*if ( ! reportInProgress && UIControlsPosition != null)
        {   
            UIControls.transform.position = UIControlsPosition;
            Debug.Log(UIControls.transform.position.x + " " + UIControls.transform.position.y + " " + UIControls.transform.position.z);
            
        } else {
            UIControls.transform.position = new Vector3(-5000, -5000, 0);
        }*/
        
                //Debug.Log(tempPosition.x + " " + tempPosition.y + " " + tempPosition.z);

                /* -523.2076 229.2329 -2.177725 */

        if ( ! reportInProgress)
        {   
            CanvasGroup canvasGroup = UIControls.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;  
        } else {
            CanvasGroup canvasGroup = UIControls.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
                

    }

    private void changePlayerColor(GameObject go, Color color)
    {
        Transform avatar = go.transform.GetChild(0);
        SpriteRenderer avatarSprite = avatar.GetComponent<SpriteRenderer>();
        avatarSprite.color = color;
    }

    private void UpdateInfoText()
    {
        string role = playerController.isImposter ? "Imposter" : "Crewmate";

        if ( ! playerController.isImposter) {
            string taskIDsString = string.Join(", ", playerController.taskIDs.ToArray());
            infoText.text = $"id: {playerController.id} \nrole: {role} \ntasks: {taskIDsString} ";
        } else {
            infoText.text = $"id: {playerController.id} \nrole: {role} \nremaining: {otherPlayersData.Count}";
        }

        infoTextUpdated = true;
    }

    public void TryReconnect()
    {
        BluetoothPlayerMovement.IsConnected = false;
    }
}