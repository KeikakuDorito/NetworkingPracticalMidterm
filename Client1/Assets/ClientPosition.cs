using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClientPosition : MonoBehaviour
{

    public GameObject clientCube;
    public GameObject remoteCube;
    private static byte[] outBuffer = new byte[512];
    private static byte[] inBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static Socket clientSoc;
    
    private static EndPoint remoteClient; //Receiving


    //[SerializeField] TMP_Text Input;
    //public Canvas typingCanvas;
  //  public TMP_Text Output = null;

    public static void StartClient()
    {


        try
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            remoteEP = new IPEndPoint(ip, 8889);

            clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            clientSoc.Blocking = false;

            remoteClient = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                clientSoc.Bind(remoteEP);

                Debug.Log("Waiting for data....");

                //server shutdown

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            Debug.Log("Establishing Connection");
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }



    }
    

    public void SendPosition() //Set The Position of the Blue Cube (Local Client)
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


    public void RecievePosition() //Set The Position of the Green Cube (Remote Client)
    {
        try
        {
            int recv = clientSoc.ReceiveFrom(inBuffer, ref remoteClient);
            // server.SendTo()

            string[] posData = Encoding.ASCII.GetString(inBuffer, 0, recv).Split(',');

            Debug.Log("Rec from: " + remoteClient.ToString() + "Data: " + float.Parse(posData[0]) + "," + float.Parse(posData[1]) + "," + float.Parse(posData[2]));

            //Set the position of the Cube from
            remoteCube.transform.position = new Vector3(float.Parse(posData[0]), float.Parse(posData[1]), float.Parse(posData[2]));
        }
        catch (Exception se)
        {
            Debug.Log("Exception: " + se.ToString());
        }
    }


    public void ConnectedToServer()
    {
        /*
        if (Input.text == "127.0.0.1")
        {
           
        }
        */
    }




    // Start is called before the first frame update
    void Start()
    {
        StartClient();

    }

    // Update is called once per frame
    void Update()
    {

        //SendPosition();
        RecievePosition();

    }

   
}
