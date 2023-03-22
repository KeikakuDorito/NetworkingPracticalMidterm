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
    public TMP_Text chatText = null;
    private static Socket client = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

    public static string ipInput = "127.0.0.1";

    public  void TCPConnection()
    {

        //String send;

        IPAddress ip = IPAddress.Parse(ipInput);

            //Connection
        client.Connect(ip, 8888);
        Debug.Log("Connected to Server!");
        
        //client.Bind(new IPEndPoint(ip, 0));

        
        client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(RecieveText), client);

    }

    public void RecieveText(IAsyncResult results)
    {

        //string text = inputField.GetComponent<TMP_InputField>().text;
        Socket socket = (Socket)results.AsyncState;
        int recv = socket.EndReceive(results);

        Debug.Log("Recieved: " + Encoding.ASCII.GetString(buffer, 0, recv));

        string msg = Encoding.ASCII.GetString(buffer, 0, recv);
        if (msg != "")
        {
            chatText.text += msg + "\n"; //set chat msg
        }
        
        socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(RecieveText), socket);

    }


   public void SendText()
    {
        
        string text = inputField.GetComponent<TMP_InputField>().text;
        outbuffer = Encoding.ASCII.GetBytes(text);
        client.Send(outbuffer);
        
    }

    public void connectChat(string serverAddress)
    {
        ipInput = serverAddress;
        TCPConnection(); //Begin Connection to server
    }


    void initiateDisconnect()
    {
        outbuffer = Encoding.ASCII.GetBytes("/quit");
        client.Send(outbuffer);
    }


    void OnApplicationQuit()
    {
        initiateDisconnect();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
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
