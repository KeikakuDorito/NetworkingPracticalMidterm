using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;

public class TextChatMessages : MonoBehaviour
{

    public static TextChatMessages instance;
    private static EndPoint remoteClient;
    byte[] buffer = new byte[512];
    byte[] outbuffer = new byte[512];
    public GameObject inputField;
    private static Socket client;

    public static void TCPConnection()
    {
        String send;

        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint serverEp = new IPEndPoint(ip, 8888);
        client = new Socket(AddressFamily.InterNetwork, 
            SocketType.Stream, ProtocolType.Tcp);
        remoteClient = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

        //Attempting connection
        client.Connect(serverEp);
        client.Bind(remoteClient);

        
            
        

        

        
    }

    void RecieveText()
    {

        //string text = inputField.GetComponent<TMP_InputField>().text;

        int recv = client.Receive(buffer);
        Debug.Log("Recieved From " + remoteClient.ToString() + Encoding.ASCII.GetString(buffer, 0, recv));
   
        
    }


   public void SendText()
    {
        string text = inputField.GetComponent<TMP_InputField>().text;
        byte[] chat = Encoding.ASCII.GetBytes(text);
        client.Send(chat);

    }
    // Start is called before the first frame update
    void Start()
    {
        TCPConnection();  
    }

    // Update is called once per frame
    void Update()
    {
        if(inputField.GetComponent<TMP_InputField>().text != null && Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            SendText();
            inputField.GetComponent<TMP_InputField>().text = "";
        }

        RecieveText();
        
    }
}
