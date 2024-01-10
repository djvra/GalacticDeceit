using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
    public static int SERVER_TCP_PORT = 9000;
    public static int SERVER_UDP_PORT = 9001;
    public static int CLIENT_UDP_PORT = 9002;
    public TMPro.TextMeshProUGUI infoText;
    private static Color purple = new Color(166f/255f, 60f/255f, 176f/255f);
    public static Color[] colors = { Color.red, Color.white, Color.green, Color.cyan, purple, Color.yellow };

    public enum ActionType { Login, TaskDone, Killed, Report, Voted, GameStarted, GameOver, VoteKill, BackStart };

    public static byte[] ReadData(NetworkStream stream)
    {
        byte[] buffer = new byte[128];
        using (MemoryStream ms = new MemoryStream())
        {
            do
            {
                stream.Read(buffer, 0, buffer.Length);
                ms.Write(buffer,0,buffer.Length);
            } while (stream.DataAvailable);

            return ms.ToArray();
        }
    }
}