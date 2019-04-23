using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class UDPClient : MonoBehaviour {

    public ARMap armap;
    private MLInputController controller;

    Vector3 armap_pos = new Vector3(0, 0, 0);
    Vector3 world_origin = new Vector3(0, 0, 0);
    Quaternion armap_rot = new Quaternion(0, 0, 0, 0);


    Socket socket;
    EndPoint serverEnd;
    IPEndPoint ipEnd;
    Thread connectThread;

    string editString;

    void InitSocket() {
        ipEnd = new IPEndPoint(IPAddress.Parse("192.168.1.241"), 60123); 
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
        serverEnd = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
        SocketSend("hello");
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketSend(string sendStr) {
        byte[] sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(sendStr);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    byte ReceiveByte() {
        byte[] data = new byte[1];
        socket.ReceiveFrom(data, 1, SocketFlags.None, ref serverEnd);
        return data[0];
    }
    
    float ReceiveFloat() {
        byte[] data = new byte[4];
        socket.ReceiveFrom(data, 4, SocketFlags.None, ref serverEnd);
        return System.BitConverter.ToSingle(data, 0);
    }

    void SocketReceive() {
        while (true) {
            byte id = ReceiveByte();
            float x, y, z, w;
            if (id == 0) {
                x = ReceiveFloat();
                y = ReceiveFloat();
                z = ReceiveFloat();
                armap_pos = new Vector3(x, y, z);
                x = ReceiveFloat();
                y = ReceiveFloat();
                z = ReceiveFloat();
                w = ReceiveFloat();
                armap_rot = new Quaternion(x, y, z, w);
                //Debug.Log(armap_pos);
                //Debug.Log(armap_rot);
            }
        }
        
    }


    void Start() {
        InitSocket();
        MLInput.Start();
        controller = MLInput.GetController(MLInput.Hand.Left);
    }

    /*
    void OnGUI() {
        editString=GUI.TextField(new Rect(10,10,100,20),editString);
        if(GUI.Button(new Rect(10,30,60,20),"send"))
            SocketSend(editString);
    }
    */

    void Update() {
        if (controller != null) {
            world_origin = controller.Position;
        }
        System.Console.Write("%.5lf %.5lf %.5lf\n", controller.Position.x, controller.Position.y, controller.Position.z);
        //Debug.Log(controller.Position);
        //Debug.Log("   " + armap_pos);
        armap.transform.position = armap_pos + world_origin + new Vector3(0.3f, 0.3f, 0.3f);
        armap.transform.rotation = armap_rot;
    }

    void SocketQuit() {
        if(connectThread != null) {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if(socket != null)
            socket.Close();
    }

    void OnApplicationQuit() {
        SocketQuit();
    }
}
