using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;

public class UdpServerTwoClients : MonoBehaviour
{
    UdpClient server;
    IPEndPoint anyEP;
    Thread receiveThread;
    Dictionary<string, int> clientIds = new Dictionary<string, int>();
    int nextId = 0;
    private List<int> idList = new List<int>(4);

    void Start()
    {
        server = new UdpClient(5001);
        anyEP = new IPEndPoint(IPAddress.Any, 0);
        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
        Debug.Log("Servidor iniciado na porta 5001");
    }

    void ReceiveData()
    {
        while (true)
        {
            byte[] data = server.Receive(ref anyEP);
            string msg = Encoding.UTF8.GetString(data);
            string key = anyEP.Address + ":" + anyEP.Port;

            if (!clientIds.ContainsKey(key))
            {
                clientIds[key] = nextId++; //Pegar o id e verificar se ele já tá na lista
                foreach (int idListed in idList)
                {
                    if (clientIds[key] == idListed)
                    {
                        clientIds[key] = Random.Range(0, 4);
                    }
                }

                string assignMsg = "ASSIGN:" + clientIds[key];
                server.Send(Encoding.UTF8.GetBytes(assignMsg), assignMsg.Length, anyEP);
            }

            int id = clientIds[key];
            if (msg.StartsWith("POS:"))
            {
                string coords = msg.Substring(4);
                string broadcast = $"POS:{id};{coords}";
                byte[] bdata = Encoding.UTF8.GetBytes(broadcast);

                foreach (var kvp in clientIds)
                {
                    var parts = kvp.Key.Split(':');
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
                    server.Send(bdata, bdata.Length, ep);
                }
            }

            if (msg.StartsWith("BPOS:"))
            {
                string coords = msg.Substring(5);
                string[] parts = coords.Split(';');
                string x = parts[0];
                string y = parts[1];

                string broadcast = $"BPOS: {x}; {y}";
                byte[] bdata = Encoding.UTF8.GetBytes(broadcast);
                
                foreach (var kvp in clientIds)
                {
                    var part = kvp.Key.Split(':');
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(part[0]), int.Parse(part[1]));
                    server.Send(bdata, bdata.Length, ep);
                }
            }

            if (msg.StartsWith("QUIT:"))
            {
                string quitMsg = msg.Substring(5);
                string[] parts = quitMsg.Split(' ');
                int idToRemove = int.Parse(parts[1]);

                Debug.Log($"Client {idToRemove} quit");
                clientIds.Remove(idToRemove.ToString());
            }
        }
    }
}