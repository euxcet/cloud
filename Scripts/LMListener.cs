using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class LMListener : MonoBehaviour {

	public void OnServiceConnect(object sender, ConnectionEventArgs args)
    {
    	Debug.Log("Leap Motion: Service Connected");
    }

    public void OnConnect(object sender, DeviceEventArgs args)
    {
    	Debug.Log("Leap Motion: Connected");
    }

    public void OnFrame(object sender, FrameEventArgs args)
    {
    	Debug.Log("Leap Motion: Frame Available.");
    }
}
