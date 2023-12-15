using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button clientLocalBtn;
    [SerializeField] public TMP_InputField ipField; // Reference to the input field for IP
    private string localIp = GetLocalIPAddress();
    private string serverIp = "";   
    private ushort port = 9000;

    private void Awake() {
        serverBtn.onClick.AddListener( () => {
            Debug.Log($"Connecting to {localIp}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(localIp, port);
            NetworkManager.Singleton.StartServer();
        });
        hostBtn.onClick.AddListener( () => {
            Debug.Log($"Connecting to {localIp}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(localIp, port);
            NetworkManager.Singleton.StartHost();
        });
    }

    public void ClientStart() 
    {
        // Check if ipField is not null before accessing its text property
        if (ipField != null)
        {
            if (!string.IsNullOrEmpty(ipField.text))
            {
                serverIp = ipField.text;
                Debug.Log($"Connecting to {serverIp}");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(serverIp, port);
                NetworkManager.Singleton.StartClient();
            }
        }

        Debug.LogError("Cannot connect to server.");
    }

    public void ClientLocalStart() 
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(localIp, port);
        NetworkManager.Singleton.StartClient();
    }

    private static string GetLocalIPAddress()
    {
        foreach(NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if(ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                        
                    }
                }
            }  
        }

        return "127.0.0.1"; 
    }

}
