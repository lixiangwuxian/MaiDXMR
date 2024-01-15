using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TCPClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    public string serverDomain = "p775tm-lxwx"; // 服务器的域名
    public string serverIp;
    public int serverPort = 10890;               // 服务器的端口

    void Start()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        try
        {
            if (tcpClient != null)
            {
                tcpClient.Close(); // 关闭旧的连接
            }

            tcpClient = new TcpClient();  // 创建新的TCP客户端
            Debug.Log("Attempting to connect to server " + serverDomain + ", port " + serverPort);

            // 解析服务器域名
            IPAddress[] addressList = Dns.GetHostAddresses(serverDomain);
            serverIp = addressList[0].ToString();

            tcpClient.Connect(serverIp, serverPort); // 连接服务器
            stream = tcpClient.GetStream(); // 获取网络流
            Debug.Log("Connected to server with IP " + serverIp);
        }
        catch (Exception err)
        {
            Debug.LogError("Error connecting to server: " + err.ToString());
        }
    }

    public void SendString(string message)
    {
        if (tcpClient == null || !tcpClient.Connected)
        {
            Debug.LogWarning("Connection lost. Attempting to reconnect...");
            ConnectToServer(); // 尝试重新连接
        }

        if (tcpClient.Connected)
        {
            try
            {
                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
                stream.Write(bytesToSend, 0, bytesToSend.Length); // 发送数据
                Debug.Log("Message sent: " + message);
            }
            catch (Exception err)
            {
                Debug.LogError("Error sending message: " + err.ToString());
            }
        }
        else
        {
            Debug.LogError("Unable to send message. Connection is not established.");
        }
    }

    public void SendNumber(int number)
    {
        string message = number.ToString();
        SendString(message);
    }

    void OnApplicationQuit()
    {
        if (stream != null)
            stream.Close(); // 关闭网络流
        if (tcpClient != null)
            tcpClient.Close(); // 关闭TCP客户端
    }
}
