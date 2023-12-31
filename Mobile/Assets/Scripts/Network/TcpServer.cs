﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TcpServer
{
    public delegate byte[] AfterReceiveFunction(string payload);

    public AfterReceiveFunction func;

    public TcpServer(AfterReceiveFunction func)
    {
        this.func = func;
    }

    public void Start(int port)
    {
        Debug.Log($"Starting TCP server on port: {port}");

        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        listener.BeginAcceptTcpClient(TcpReceived, listener);
    }

    void TcpReceived(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener) ar.AsyncState;
        listener.BeginAcceptTcpClient(TcpReceived, ar.AsyncState);

        using (TcpClient client = listener.EndAcceptTcpClient(ar))
        using (var nwStream = client.GetStream())
        {
            var payload = Utils.ReadData(nwStream);
            if (payload.Length > 0)
            {
                var requestJson = Encoding.ASCII.GetString(payload);
                Debug.Log($"Received: {requestJson}");
                var responsePayload = func(requestJson);

                nwStream.Write(responsePayload, 0, responsePayload.Length);
            }
        }
    }
}