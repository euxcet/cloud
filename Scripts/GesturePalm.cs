using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class GesturePalm : MonoBehaviour {
	Controller _controller;
	Frame _frame;
	Hand _hand;
	// LMListener _lisener;
	public LeapProvider _provider;

	// Use this for initialization
	void Start () {
		_controller = new Controller();
		// _lisener = new LMListener();
		// _controller.Connect += _lisener.OnServiceConnect;
		// _controller.Device += _lisener.OnConnect;
		// _controller.FrameReady += _lisener.OnFrame;
		// _provider = FindObjectOfType<LeapProvider>();
		
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = _provider.CurrentFrame;
		if (frame != null) {
			Debug.Log("# Leap Motion get frame!");
			Debug.Log("# hands num = " + frame.Hands.Count);
		}
	}
}
