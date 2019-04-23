using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Mail {

    public enum IDENTIFIER {
        NONE, GET_IMAGE_DATA, GET_CURSOR
    };

    public IDENTIFIER identifier;
    public int width;
    public int height;
    public int x;
    public int y;
    public float scale;
    public byte[] data;
    public float dx;
    public float dy;

    public Mail() {
        identifier = NONE;
    }
}

public class Net : MonoBehaviour
{
    private Socket server;
    private TcpListener listener;
    private TcpClient clientSocket;
    private NetworkStream stream;

    private Mail mail;

    private const int BUFFER_SIZE = 2048;
    private const int NAME_SIZE = 20;
    private const int FORMAT_SIZE = 5;
    private const int CORRECTION_BYTE = 100;

    private const int IMAGE_WIDTH = 800;
    private const int IMAGE_HEIGHT = 800;



    // Use this for initialization
    void Start() {
        IPAddress address = IPAddress.Parse("0.0.0.0");
        int port = 2000;
        listener = new TcpListener(address, port);
        listener.Start();

        Thread socketThread = new Thread(new ThreadStart(CreateSocket));
        socketThread.Start();
        mail = new Mail();
    }

    void WaitSocket() {
        if (clientSocket != null)
            clientSocket.Close();
        clientSocket = listener.AcceptTcpClient();
        stream = clientSocket.GetStream();
    }

    byte GetByte() {
        byte[] data = new byte[BUFFER_SIZE];
        while (true) {
            if (stream.Read(data, 0, 1) == 1)
                break;
        }
        return data[0];
    }

    byte[] GetBytes(int size) {
        byte[] data = new byte[size];
        byte[] buffer = new byte[BUFFER_SIZE];
        for (int offset = 0; offset < size;) {
            int bytes = stream.Read(data, offset, Mathf.Min(size - offset, BUFFER_SIZE));
            offset += bytes;
        }
        return data;
    }

    void SendBytes(byte[] data, int size) {
        clientSocket.Send(data, size, SocketFlags.None);
    }

    string GetString(int size) {
        byte[] data = GetBytes(size);
        int length = StripLength(data, size);
        return Encoding.UTF8.GetString(data, 0, length);
    }

    int GetShort() {
        byte[] data = GetBytes(2);
        return System.BitConverter.ToUInt16(data, 0);
    }

    void SendByte(byte data) {
        clientSocket.Send(BitConverter.GetBytes(data), 1, SocketFlags.None);
    }

    int GetInt() {
        byte[] data = GetBytes(4);
        return (int)System.BitConverter.ToUInt32(data, 0);
    }

    void SendInt(int data) {
        clientSocket.Send(BitConverter.GetBytes(data), 4, SocketFlags.None);
    }

    double GetDouble() {
        byte[] data = GetBytes(8);
        return System.BitConverter.ToDouble(data, 0);
    }

    void SendDouble(double data) {
        clientSocket.Send(BitConverter.GetBytes(data), 8, SocketFlags.None);
    }

    int StripLength(byte[] data, int size) {
        while (size > 0 && data[size - 1] == 0)
            size--;
        return size;
    }

    void GetImageData() {
        string imageName = GetString(NAME_SIZE);
        string format = GetString(FORMAT_SIZE);
        mail.width = GetShort();
        mail.height = GetShort();
        IMAGE_HEIGHT = IMAGE_WIDTH * mail.height / mail.width;
        int size = GetInt();
        mail.data = GetBytes(size);
        mail.flag = GET_IMAGE_DATA;
    }

    void GetCursor() {
        mail.x = GetShort();
        mail.y = GetShort();
        mail.flag = GET_CURSOR;
    }

    void ReceiveMessage() {
        while (true) {
            byte identifier = GetByte();
            switch (identifier) {
                case 0: GetImageData(); break;
                case 4: GetCursor(); break;
                default: break;
            }
            byte t = GetByte();
            if (t != CORRECTION_BYTE) {
                Debug.LogError("Correction byte is wrong.");
            }
        }
    }

    void CreateSocket() {
        WaitSocket();
        ReceiveMessage();
    }

    void OnDestroy() {
        if (listener != null) {
            listener.Stop();
        }
        if (clientSocket != null) {
            clientSocket.Close();
        }
    }

    void InitSprite() {
        Texture2D texture = new Texture2D(mail.width, mail.height);
        texture.LoadImage(mail.data);

        Sprite sprite = Sprite.Create(texture,
            new Rect(0, 0, mail.width, mail.height),
            new Vector2(0.5f, 0.5f));
        GameObject.Find("I").GetComponent<SpriteRenderer>().sprite = sprite;
        GameObject.Find("Imask").GetComponent<SpriteMask>().sprite = sprite;
        GameObject.Find("Imask").GetComponent<Transform>().localScale = new Vector3(1.0f * IMAGE_WIDTH / mail.width, 1.0f * IMAGE_HEIGHT / mail.height, 1);
        Resources.UnloadUnusedAssets();
        mail.identifier = None;
    }


    void MoveCursor() {
        Texture2D texture = new Texture2D(mail.width, mail.height);
        texture.LoadImage(mail.data);

        for (int i = -20; i <= 20; i++)
            for (int j = -20; j <= 20; j++)
                texture.SetPixel(mail.x + i, mail.y + j, Color.red);

        texture.Apply();

        Sprite newSprite = Sprite.Create(texture,
            new Rect(0, 0, mail.width, mail.height),
            new Vector2(0.5f, 0.5f));
        GameObject.Find("I").GetComponent<SpriteRenderer>().sprite = newSprite;
        GameObject.Find("Imask").GetComponent<SpriteMask>().sprite = newSprite;
        mail.identifier = None;
    }


    // Update is called once per frame
    void Update()
    {
        switch (mail.identifier) {
            case GET_IMAGE_DATA:
                InitSprite();
                break;
            case GET_CURSOR:
                MoveCursor();
                break;
            default: break;
        }
    }
}
