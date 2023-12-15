// NetworkController.cs

using UnityEngine;
using System.Net.Sockets;
using System.Text;
using TMPro;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour
{
    public TMP_InputField ipInputField; // Reference to the input field for IP
    string serverIp = "";
    public int serverPort = 12345;
    public TextMeshProUGUI inputData;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    void OnDestroy()
    {
        // Close the client when the GameObject is destroyed
        if (client != null)
        {
            client.Close();
        }
    }

    void Update()
    {
        // Check for received data from the server
        if (client != null && client.Connected)
        {
            ReceiveDataFromServer();
        }
    }

    private bool ConnectToServer()
    {
        // Get the server IP from the input field
        try
        {
            // Connect to the server
            client = new TcpClient(serverIp, serverPort);

            if (client != null && client.Connected)
            {
                stream = client.GetStream();
                Debug.Log("Connected to server.");
                return true;
            }
            else
            {
                Debug.LogError("Error connecting to server: Client is null or not connected.");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
            return false;
        }
    }

    private void SendCommandToServer()
    {
        // Check if the client is connected
        if (client == null || !client.Connected)
        {
            Debug.LogError("Not connected to server.");
            return;
        }

        // Get the command from the input field
        string command = inputData.text;
        byte[] data = Encoding.ASCII.GetBytes(command);

    // Remove the last byte
    // her nedense son byte ? isaretine donusuyordu
    if (data.Length > 0)
    {
        Array.Resize(ref data, data.Length - 1);
    }

        try
        {
            // Send the command to the server
            stream.Write(data, 0, data.Length);
            Debug.Log("Command sent to server: " + command);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error sending command: " + e.Message);
        }
    }

    private void ReceiveDataFromServer()
    {
        try
        {
            if (client == null)
            {
                Debug.LogError("Client is null.");
                return;
            }

            if (!client.Connected)
            {
                Debug.LogError("Client is not connected.");
                return;
            }

            if (stream == null)
            {
                Debug.LogError("Stream is null.");
                return;
            }

            if (stream.DataAvailable)
            {
                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                if (bytesRead > 0)
                {
                    string receivedData = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);

                    // Update UI on the main thread
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        if (receivedData != null)
                        {
                            Debug.Log("Received data from server: " + receivedData);
                        }
                        else
                        {
                            Debug.LogError("receivedData is null (inside Enqueue).");
                        }
                    });
                }
                else
                {
                    Debug.LogWarning("Received 0 bytes from server.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error receiving data from server: " + e.Message);
        }
    }

    public void ButtonPressed()
    {
        serverIp = ipInputField.text;
        
        if (!string.IsNullOrEmpty(serverIp) && ConnectToServer())
        {
            Debug.LogWarning("Trying to connect: " + serverIp);
            // Call this method when the button is pressed to send the command
            SendCommandToServer();
        } else
        {
            Debug.LogError("IP field is empty!");
        }
    }

    public void NextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}

