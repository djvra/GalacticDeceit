using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkManagerUIHelper : NetworkBehaviour
{
    [SerializeField] private Button disconnectBtn;
    [SerializeField] private Button listPlayersBtn;
    [SerializeField] private Button startGameBtn;

    public void PrintConnectedClients()
    {
        if (IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClientsList == null) 
            { // server
                Debug.Log("No connected clients.");
                return;
            }

            // host
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject != null)
                {
                    // Access information about connected clients
                    Debug.Log($"Player ID: {client.ClientId}");
                }
            }
        } else { // client
            Debug.Log("You are not a host or server.");
        }
    }

    public void Disconnect() {
        DisconnectServerRpc(); // serverRpcParams will be filled in automatically
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisconnectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (IsServer) 
        {
            Debug.LogWarning("Server Disconnect, everyone is disconnected");
            if (NetworkManager.ConnectedClients != null)
            {
                var senderClientId = serverRpcParams.Receive.SenderClientId;
                if (senderClientId != 0) // host id is always 0
                {
                    NetworkManager.DisconnectClient(senderClientId);
                    Debug.LogWarning($"Player ID: {senderClientId} disconnected.");
                } else {
                    foreach (var client in NetworkManager.ConnectedClients)
                    {
                        var clientId = client.Value.ClientId; // disconnect all clients first
                        if (client.Value.PlayerObject != null && clientId != 0) // server id is always 0
                        {
                            NetworkManager.DisconnectClient(clientId);
                            Debug.Log($"Player ID: {clientId}");
                        }

                        // then shutdown the server
                        NetworkManager.Singleton.Shutdown();
                        Debug.Log("Server shutdown.");
                    }
                }
            }
        } 
    }

    // Reference to the game scene name
    public string gameSceneName = "SampleScene";

    public void StartGame()
    { 
        if (IsServer && !string.IsNullOrEmpty(gameSceneName)) 
        {
            NetworkManager.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        }
        
    }
}
