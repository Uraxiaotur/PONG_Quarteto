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
    public GameObject localCube;
    public GameObject remoteCube1;
    public GameObject remoteCube2;
    public GameObject remoteCube3;
    
    public GameObject localBall;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        client = new UdpClient();
        serverEP = new IPEndPoint(IPAddress.Parse("10.57.1.50"), 5001);
        client.Connect(serverEP);
        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
        client.Send(Encoding.UTF8.GetBytes("HELLO"), 5);
    }

    void Update()
    {
        // Movimento local
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        localCube.transform.Translate(new Vector3(0, v, 0) * (Time.deltaTime * 5));

        // Envia posição
        string msg = "POS:" +
                     localCube.transform.position.x.ToString("F2", CultureInfo.InvariantCulture) + ";" +
                     localCube.transform.position.y.ToString("F2", CultureInfo.InvariantCulture);
        client.Send(Encoding.UTF8.GetBytes(msg), msg.Length);

        string msgBall = null;
        if (localBall && myId == 1)
        {
            msgBall = "BPOS:" +
                      localBall.transform.position.x.ToString("F2", CultureInfo.InvariantCulture) + ";" +
                      localBall.transform.position.y.ToString("F2", CultureInfo.InvariantCulture);
            client.Send(Encoding.UTF8.GetBytes(msgBall), msgBall.Length);
        }

        // Atualiza posição dos outros jogadores e bola se o id não for 1
        remoteCube1.transform.position = Vector3.Lerp(remoteCube1.transform.position, remotePos, Time.deltaTime * 10f);
        remoteCube2.transform.position = Vector3.Lerp(remoteCube2.transform.position, remotePos, Time.deltaTime * 10f);
        remoteCube3.transform.position = Vector3.Lerp(remoteCube3.transform.position, remotePos, Time.deltaTime * 10f);

        if (myId != 1)
        {
            localBall.transform.position = new Vector3(-ballX, ballY, 0);
        }
    }
    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            byte[] data = client.Receive(ref remoteEP);
            string msg = Encoding.UTF8.GetString(data);

            if (msg.StartsWith("BPOS:") && myId != 1)
            {
                string[] parts = msg.Substring(5).Split(';');
                ballX = float.Parse(parts[0], CultureInfo.InvariantCulture);
                ballY = float.Parse(parts[1], CultureInfo.InvariantCulture);
            }

            if (msg.StartsWith("ASSIGN:"))
            {
                myId = int.Parse(msg.Substring(7));
                Debug.Log("[Cliente] Meu ID = " + myId);

                switch (myId)
                {
                    case 1:
                        localCube = players[0];
                        break;

                    case 2:
                        localCube = players[1];
                        break;

                    case 3:
                        localCube = players[2];
                        break;

                    case 4:
                        localCube = players[3];
                        break;
                }
            }
            else if (msg.StartsWith("POS:"))
            {
                string[] parts = msg.Substring(4).Split(';');
                if (parts.Length == 3)
                {
                    int id = int.Parse(parts[0]);
                    if (id != myId)
                    {
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        remotePos = new Vector3(-x, y, 0);
                    }
                }
            }
        }
    }
    void OnApplicationQuit()
    {
        string quitMsg = $"QUIT: {myId}";
        client.Send(Encoding.UTF8.GetBytes(quitMsg), quitMsg.Length);

        receiveThread.Abort();
        client.Close();
    }
}
