using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Client : MonoBehaviour
{

    public GameObject mycube;
    private static byte[] outBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static Socket clinetSoc;
    Vector3 Temp;
    bool change = true;
    float[] pos;



    [SerializeField] TMP_Text Input;
    //public Canvas typingCanvas;
  //  public TMP_Text Output = null;
    string wrong = "";

    public static void Startclient()
    {


        try
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            remoteEP = new IPEndPoint(ip, 8889);

            clinetSoc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Debug.Log("Establishing Connection");
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }



    }
    // Start is called before the first frame update
    void Start()
    {
        mycube = GameObject.Find("Cube");
        Startclient();
        
    }

    // Update is called once per frame
    void Update()
    {
        //pos = new float[] { mycube.transform.position.x, mycube.transform.position.y, mycube.transform.position.z };
        //if (mycube.transform.position != Temp && change)
        //{
        //    outBuffer = new byte[pos.Length * 4];
        //    Buffer.BlockCopy(pos, 0, outBuffer, 0,outBuffer.Length);
        //    clinetSoc.SendTo(outBuffer, remoteEP);
        //    change = false;
        //    Debug.Log(pos[0] + "," + pos[1] + "," + pos[2]);
        //}

        outBuffer = Encoding.ASCII.GetBytes(mycube.transform.position.x.ToString() + "," + mycube.transform.position.y.ToString() + "," + mycube.transform.position.z.ToString());
        clinetSoc.SendTo(outBuffer, remoteEP);

        if (!change) 
        { 
            Temp = mycube.transform.position;
            change = true;
        }
       

        
    }


    public void ConnectedToServer()
    {
        if (Input.text == "127.0.0.1")
        {
           
        }
    }
}
