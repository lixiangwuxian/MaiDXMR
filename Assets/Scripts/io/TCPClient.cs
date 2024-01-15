using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TCPClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    public string serverDomain = "p775tm-lxwx"; // ������������
    public string serverIp;
    public int serverPort = 10890;               // �������Ķ˿�

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
                tcpClient.Close(); // �رվɵ�����
            }

            tcpClient = new TcpClient();  // �����µ�TCP�ͻ���
            Debug.Log("Attempting to connect to server " + serverDomain + ", port " + serverPort);

            // ��������������
            IPAddress[] addressList = Dns.GetHostAddresses(serverDomain);
            serverIp = addressList[0].ToString();

            tcpClient.Connect(serverIp, serverPort); // ���ӷ�����
            stream = tcpClient.GetStream(); // ��ȡ������
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
            ConnectToServer(); // ������������
        }

        if (tcpClient.Connected)
        {
            try
            {
                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
                stream.Write(bytesToSend, 0, bytesToSend.Length); // ��������
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
            stream.Close(); // �ر�������
        if (tcpClient != null)
            tcpClient.Close(); // �ر�TCP�ͻ���
    }
}
