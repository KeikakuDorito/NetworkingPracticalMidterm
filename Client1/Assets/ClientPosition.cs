using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Manages all movement of the Cubes of the two players

public class ClientPosition : MonoBehaviour
{
    public static ClientPosition instance;
    public static bool clientStarted = false;

    public static string ipInput = "127.0.0.1";

    public GameObject clientCube;
    public GameObject remoteCube;
    private static byte[] outBuffer = new byte[512];
    private static byte[] inBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static IPEndPoint listeningEP;
    private static Socket clientSoc;
    
    private static EndPoint remoteClient; //Receiving

    Vector3 currentPos;
    Vector3 lastPos;


    //[SerializeField] TMP_Text Input;
    //public Canvas typingCanvas;
  //  public TMP_Text Output = null;

    public static void StartClient()
    {

            //Try and Catch statement moved to InputFunctions.cs for a hacky way of doing error message

            //IPAddress ip = IPAddress.Parse("50.17.63.176"); //IPAddress of the Server
            IPAddress ip = IPAddress.Parse(ipInput);
            remoteEP = new IPEndPoint(ip, 8889);

            clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            clientSoc.Blocking = false;

            remoteClient = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

            try
            {
                clientSoc.Bind(remoteClient);
                clientSoc.Connect(remoteEP);

                Debug.Log("Waiting for data....");

                //server shutdown

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            Debug.Log("Establishing Connection");
            clientStarted = true;
    }
    

    void SendPosition() //Set The Position of the Blue Cube (Local Client)
    {
        try
        {
            outBuffer = Encoding.ASCII.GetBytes(clientCube.transform.position.x.ToString() + "," + clientCube.transform.position.y.ToString() + "," + clientCube.transform.position.z.ToString());

            Debug.Log("Sending Message to " + remoteEP.ToString());

            clientSoc.SendTo(outBuffer, remoteEP);
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }

    }


    void RecievePosition() //Set The Position of the Green Cube (Remote Client)
    {
        try
        {
            int recv = clientSoc.ReceiveFrom(inBuffer, ref remoteClient);

            string[] posData = Encoding.ASCII.GetString(inBuffer, 0, recv).Split(',');

            Debug.Log("Rec from: " + remoteClient.ToString() + " Data: " + float.Parse(posData[0]) + "," + float.Parse(posData[1]) + "," + float.Parse(posData[2]));

            //Set the position of the Cube from
            remoteCube.transform.position = new Vector3(float.Parse(posData[0]), float.Parse(posData[1]), float.Parse(posData[2]));
        }
        catch (Exception se)
        {
            Debug.Log("Exception: " + se.ToString());
        }
    }


    public void connectPosition(string serverAddress) //This function will be called by the Ip input thing when the client starts up
    {
        ipInput = serverAddress;
        StartClient();
    }




    // Start is called before the first frame update
    void Start()
    {

        if(instance == null)
        {
            instance = this;
        }

        currentPos = clientCube.transform.position;
        lastPos = clientCube.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (clientStarted)
        {
            currentPos = clientCube.transform.position;

            if (currentPos != lastPos)
            {
                SendPosition();
            }
            //SendPosition();

            lastPos = currentPos;

            RecievePosition();
        }
    }

   
}
