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
    public  UdpClient udpClient;
    public UdpClient udpServer;
    public  string serverDomain = "192.168.6.124"; // ������������
    public  string serverIp;
    public  int serverPort = 10890;               // �������Ķ˿�
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
        //send
        udpClient = new UdpClient();  // ����UDP�ͻ���
        Debug.Log("Starting create udp client, send to " + serverDomain + ", port is " + serverPort);
        IPAddress[] addressList = Dns.GetHostAddresses(serverDomain);
        serverIp = addressList[0].ToString();
        Debug.Log("Server Ip is " + serverIp.ToString());
        //listen
        udpServer = new UdpClient(listenPort);
        Debug.Log("Starting create udp server, listen port is " + listenPort);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        //InputUpdater.Instance.SetTimer(SendKeyStates, 240);
    }
    public void SendString(string message)
    {
        try
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
            udpClient.Send(bytesToSend, bytesToSend.Length, serverIp, serverPort);
        }
        catch (System.Exception err)
        {
            Debug.LogError(err.ToString());
        }
    }

    //inputType->0 is touch,1 is key
    public void SendInputState(int inputType,int keyCode,bool isPressed)
    {
        byte[] byteArray = new byte[3]; // ���ڴ洢�����byte����
        byteArray[0] = (byte)inputType;
        byteArray[1] = (byte)keyCode;
        byteArray[2] = (byte)(isPressed?1:0);
        try
        {
            udpClient.Send(byteArray, byteArray.Length, serverIp, serverPort);
        }
        catch (System.Exception err)
        {
            Debug.LogError(err.ToString());
        }
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedBytes = udpServer.Receive(ref remoteEndPoint);
                // �����ﴦ����յ�������
                Debug.Log("Received data from " + remoteEndPoint );
                 ScreenManager.instance.jobs.Enqueue(() => {
                    ScreenManager.instance.UpdateScreen(receivedBytes);
                });
            }
            catch (Exception ex)
            {
                if (udpServer == null || !udpServer.Client.Connected)
                {
                    // �� UdpClient ���ر�ʱ�˳�ѭ��
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

        // �ӽ��յ�����������ȡ������Ϣ
        //int length = BitConverter.ToInt32(receivedBytes, 0);

       // if (receivedBytes.Length - 4 < length)
        //{
        //    Debug.LogError("Received data is shorter than the expected length.");
        //    return;
        //}

        // ��ȡʵ�ʵ���Ϣ����
        //byte[] messageBytes = new byte[length];
        //Array.Copy(receivedBytes, 4, messageBytes, 0, length);
        ScreenManager.instance.jobs.Enqueue(() => {
            ScreenManager.instance.UpdateScreen(receivedBytes);
        });

    }

    public void SendKeyStates(bool[] state)
    {
        byte[] byteArray = new byte[8]; // ���ڴ洢�����byte����

        for (int i = 0; i < 60; i++)
        {
            if (state[i])
            {
                // ����byteArray����Ӧ��λ
                byteArray[i / 8] |= (byte)(1 << (i % 8));
            }
        }
        udpClient.Send(byteArray, byteArray.Length, serverIp, serverPort);
    }
    public void UpdateKeyState(int key,bool state)
    {
        string message = "UpdateKeyState:" + key.ToString() + ":" + state.ToString();
        SendString(message);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();  // �ر�UDP�ͻ���
    }
}
