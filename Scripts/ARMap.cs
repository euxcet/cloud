using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.UI;

public class ARMap : MonoBehaviour {

	public GameObject optitrack;
    public Text debug;

	// Use this for initialization
	void Start () {
		debug.text = GetIP(ADDRESSFAM.IPv4);
        Debug.Log(LocalIPAddress());
	}

	// Update is called once per frame
	void Update () {

		

		//if(!optitrack) { return; }

        // GameObject tmp = optitrack;

        // Vector3 pos = new Vector3(0,0,0);
        // int count = 0;
        /*
        foreach(Transform child in optitrack.transform) {
            if(child.position != Vector3.zero) {
                this.transform.position = child.position;
                return;

                // pos += child.position;
                // count++;
            }
        }
        */
        this.transform.position = new Vector3(0.77780f, -0.2648f, 1.5820f);

        // Debug.Log(pos);
        // if(pos == Vector3.zero) { return; }
        // this.transform.position = pos / (float)count;

    }

    public string GetIP(ADDRESSFAM Addfam)
    {
        //Return null if ADDRESSFAM is Ipv6 but Os does not support it
        if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
        {
            return null;
        }

        string output = "";

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    //IPv4
                    if (Addfam == ADDRESSFAM.IPv4)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }

                    //IPv6
                    else if (Addfam == ADDRESSFAM.IPv6)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
        }
        return output;
    }

    public  string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}

public enum ADDRESSFAM
{
    IPv4, IPv6
}
