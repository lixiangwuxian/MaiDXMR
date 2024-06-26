using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
//adb logcat -s Unity

public class UDPClient : MonoBehaviour
{
    public static UDPClient instance;
    public  UdpClient udpAsClient;
    public UdpClient udpAsServer;
    public  IPAddress serverIp;
    public  int serverPort = 10890;               // 服务器的端口
    public int listenPort = 10890;
    private Thread receiveThread;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        //Debug.Log("Server Ip is " + serverIp.ToString());
        //listen
        udpAsServer = new UdpClient(listenPort);
        Debug.Log("Starting create udp server, listen port is " + listenPort);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        //udpClient = new UdpClient(serverDomain, serverPort);
        //InputUpdater.Instance.SetTimer(SendKeyStates, 240);
    }

    public void SendString(string message)
    {
        try
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
            if (udpAsClient != null)
            {
                udpAsClient.Send(bytesToSend, bytesToSend.Length, serverIp.ToString(), serverPort);
            }
        }
        catch (System.Exception err)
        {
            Debug.LogError(err.ToString());
        }
    }

    //inputType->0 is touch,1 is key
    public void SendInputState(int inputType,int keyCode,bool isPressed)
    {
        byte[] byteArray = new byte[3]; // 用于存储结果的byte数组
        byteArray[0] = (byte)inputType;
        byteArray[1] = (byte)keyCode;
        byteArray[2] = (byte)(isPressed?1:0);
        try
        {
            if (udpAsClient != null)
            {
                udpAsClient.Send(byteArray, byteArray.Length, serverIp.ToString(), serverPort);
            }
        }
        catch (System.Exception err)
        {
            Debug.LogError(err.ToString());
        }
    }

    private void ReceiveData()
    {
        Debug.Log("Start Receiving Data");
        while (true)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedBytes = udpAsServer.Receive(ref remoteEndPoint);
                // 在这里处理接收到的数据
                Debug.Log("Received data from " + remoteEndPoint );
                 ScreenManager.instance.jobs.Enqueue(() => {
                    ScreenManager.instance.UpdateScreen(receivedBytes);
                });
                if (udpAsClient == null)
                {
                    Debug.Log("Starting create udp client");
                    udpAsClient = new UdpClient(remoteEndPoint);  // 创建UDP客户端
                }
            }
            catch (Exception ex)
            {
                if (udpAsServer == null || !udpAsServer.Client.Connected)
                {
                    // 当 UdpClient 被关闭时退出循环
                    break;
                }
                Debug.LogError("Error receiving UDP data: " + ex.Message);
            }
        }
    }

    private void DecodeMessage(byte[] receivedBytes)
    {
        if (receivedBytes.Length < 4)
        {
            Debug.LogError("Received data is too short to contain length information.");
            return;
        }

        // 从接收到的数据中提取长度信息
        //int length = BitConverter.ToInt32(receivedBytes, 0);

       // if (receivedBytes.Length - 4 < length)
        //{
        //    Debug.LogError("Received data is shorter than the expected length.");
        //    return;
        //}

        // 提取实际的消息内容
        //byte[] messageBytes = new byte[length];
        //Array.Copy(receivedBytes, 4, messageBytes, 0, length);
        //ScreenManager.instance.jobs.Enqueue(() => {
        //    ScreenManager.instance.UpdateScreen(receivedBytes);
        //});

    }

    public void SendKeyStates(bool[] state)
    {
        byte[] byteArray = new byte[8]; // 用于存储结果的byte数组

        for (int i = 0; i < 60; i++)
        {
            if (state[i])
            {
                // 设置byteArray中相应的位
                byteArray[i / 8] |= (byte)(1 << (i % 8));
            }
        }
        if (udpAsClient != null)
        {
            udpAsClient.Send(byteArray, byteArray.Length, serverIp.ToString(), serverPort);
        }
    }
    public void UpdateKeyState(int key,bool state)
    {
        string message = "UpdateKeyState:" + key.ToString() + ":" + state.ToString();
        SendString(message);
    }

    void OnApplicationQuit()
    {
        udpAsClient.Close();  // 关闭UDP客户端
    }
}
