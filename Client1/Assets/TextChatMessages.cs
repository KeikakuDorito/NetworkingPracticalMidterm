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
    private static byte[] buffer = new byte[512];
    private static byte[] outbuffer = new byte[512];
    public GameObject inputField;
    private static Socket client = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

    public static void TCPConnection()
    {

        //String send;

        IPAddress ip = IPAddress.Parse("127.0.0.1");

            //Connection
        client.Connect(ip, 8888);
        Debug.Log("Connected to Server!");
        
        //client.Bind(new IPEndPoint(ip, 0));


        client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(RecieveText), client);

    }

    private static void RecieveText(IAsyncResult results)
    {

        //string text = inputField.GetComponent<TMP_InputField>().text;

        try
        {
            Socket socket = (Socket)results.AsyncState;
            int recv = socket.EndReceive(results);

            Debug.Log("Recieved From " + remoteClient.ToString() + Encoding.ASCII.GetString(buffer, 0, recv));


            socket.BeginReceive(buffer, 0, buffer.Length, 0,
                    new AsyncCallback(RecieveText), socket);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }


    }


   public void SendText()
    {
        string text = inputField.GetComponent<TMP_InputField>().text;
        outbuffer = Encoding.ASCII.GetBytes(text);
        client.Send(outbuffer);
        
    }


    // Start is called before the first frame update
    void Start()
    {
        TCPConnection();

    }

    // Update is called once per frame
    void Update()
    {
        if(inputField.GetComponent<TMP_InputField>().text != "" && Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            SendText();
            inputField.GetComponent<TMP_InputField>().text = "";
        }
        
    }
}
