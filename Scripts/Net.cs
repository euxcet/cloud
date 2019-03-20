using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Mail {
    public int flag;
    public int width;
    public int height;
    public int x;
    public int y;
    public float scale;
    public byte[] data;

    public Mail() {
        flag = 0;
        width = 0;
        height = 0;
        scale = 0.0f;
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
    private int IMAGE_WIDTH = 800;
    private int IMAGE_HEIGHT = 800;

    // Use this for initialization
    void Start()
    {
        IPAddress address = IPAddress.Parse("0.0.0.0");
        int port = 2000;
        listener = new TcpListener(address, port);
        listener.Start();

        Thread socketThread = new Thread(new ThreadStart(CreateSocket));
        socketThread.Start();
        mail = new Mail();
    }

    void WaitSocket()
    {
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

    string GetString(int size) {
        byte[] data = GetBytes(size);
        int length = StripLength(data, size);
        return Encoding.UTF8.GetString(data, 0, length);
    }

    int GetShort() {
        byte[] data = GetBytes(2);
        return System.BitConverter.ToUInt16(data, 0);
    }

    int GetInt() {
        byte[] data = GetBytes(4);
        return (int)System.BitConverter.ToUInt32(data, 0);
    }

    double GetDouble()
    {
        byte[] data = GetBytes(8);
        return System.BitConverter.ToDouble(data, 0);
    }

    int StripLength(byte[] data, int size) {
        while (size > 0 && data[size - 1] == 0) {
            size--;
        }
        return size;
    }

    void GetImageData() {
        string imageName = GetString(NAME_SIZE);
        Debug.Log(imageName);
        string format = GetString(FORMAT_SIZE);
        Debug.Log(format);
        mail.width = GetShort();
        mail.height = GetShort();
        Debug.Log(mail.width);
        Debug.Log(mail.height);
        IMAGE_HEIGHT = IMAGE_WIDTH * mail.height / mail.width;
        int size = GetInt();
        Debug.Log(size);
        mail.data = GetBytes(size);
        mail.flag = 1;
    }

    void GetMode() {
        byte type = GetByte();
    }

    void GetEnlarge() {
        mail.x = GetShort();
        mail.y = GetShort();
        mail.flag = 2;
    }

    void GetEnlargeScale() {
        mail.scale = (float)GetDouble();
        mail.flag = 3;
    }

    void GetCursor() {
        mail.x = GetShort();
        mail.y = GetShort();
        mail.flag = 4;
    }

    void ReceiveMessage()
    {
        while (true) {
            byte identifier = GetByte();
            Debug.LogError(identifier);
            switch(identifier) {
                case 0:
                    GetImageData();
                    break;
                case 1:
                    GetMode();
                    break;
                case 2:
                    GetEnlarge();
                    break;
                case 3:
                    GetEnlargeScale();
                    break;
                case 4:
                    GetCursor();
                    break;
                default:
                    break;
            }
            byte t = GetByte();
            if (t != 100) {
                Debug.LogError("Correction byte is wrong.");
            }
        }
    }

    void CreateSocket()
    {
        Debug.Log("Listening");
        WaitSocket();
        Debug.Log("Connected");
        ReceiveMessage();
    }

    void OnDestroy()
    {
        if (listener != null)
        {
            listener.Stop();
        }
        if (clientSocket != null)
        {
            clientSocket.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mail.flag == 1)
        {
            Debug.Log("Init sprite");
            Texture2D texture = new Texture2D(mail.width, mail.height);
            texture.LoadImage(mail.data);



            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, mail.width, mail.height),
                new Vector2(0.5f, 0.5f));
            GameObject.Find("I").GetComponent<SpriteRenderer>().sprite = sprite;
            GameObject.Find("Imask").GetComponent<SpriteMask>().sprite = sprite;
            GameObject.Find("Imask").GetComponent<Transform>().localScale = new Vector3(1.0f * IMAGE_WIDTH / mail.width, 1.0f * IMAGE_HEIGHT / mail.height, 1);
            Resources.UnloadUnusedAssets();
            mail.flag = 0;
        }
        else if (mail.flag == 3) {
            Debug.Log("Enlarge");
            float x = 1.0f * mail.x * IMAGE_WIDTH / mail.width - IMAGE_WIDTH / 2;
            float y = 1.0f * mail.y * IMAGE_HEIGHT / mail.height - IMAGE_HEIGHT / 2;
            /*
            Debug.Log(mail.x);
            Debug.Log(mail.y);
            Debug.Log(mail.width);
            Debug.Log(mail.height);
            Debug.Log(IMAGE_WIDTH);
            Debug.Log(IMAGE_HEIGHT);
            Debug.Log(x);
            Debug.Log(y);
            */
            Debug.Log(mail.scale);
            GameObject.Find("I").GetComponent<Transform>().localScale = new Vector3(mail.scale, mail.scale, 1.0f);
            //GameObject.Find("I").GetComponent<Transform>().localPosition = new Vector3(x - x * mail.scale, y - y * mail.scale, 0f);
            mail.flag = 0;
        }
        else if (mail.flag == 4) {
            Debug.Log("Move cursor");

            /*
            GameObject.Find("cursor").GetComponent<Transform>().localPosition = new Vector3(-49.5f + x / 100 , -50.0f + y / 100, -0.1f);
            */
            /*
            Sprite sprite = GameObject.Find("I").GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            Debug.Log(texture);
            */

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
            mail.flag = 0;
        }
    }
}