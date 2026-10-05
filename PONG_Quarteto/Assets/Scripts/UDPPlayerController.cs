using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;

public class UDPlayerController : MonoBehaviour
{
    UdpClient client;
    Thread receiveThread;
    IPEndPoint serverEP;
    int myId = -1;
    Vector3 remotePos = Vector3.zero;

    private float ballX;
    private float ballY;

    public GameObject[] players;
    public GameObject remoteCube1;
    public GameObject remoteCube2;
    public GameObject remoteCube3;
    public GameObject remoteCube4;
    
    public GameObject localBall;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
