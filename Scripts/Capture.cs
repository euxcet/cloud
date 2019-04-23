using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.MagicLeap;
using System.Threading;

[RequireComponent(typeof(PrivilegeRequester))]
public class Capture : MonoBehaviour {

	private Thread captureThread;
	private bool isConnected;
	private PrivilegeRequester _privilegeRequester;

	void Awake () {
		_privilegeRequester = GetComponent<PrivilegeRequester>();
		_privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;
	}

	private void StartCapture() {
		Debug.Log(MLCamera.IsStarted);
		MLResult result = MLCamera.Start();
		if (result.IsOk) {
			result = MLCamera.Connect();
			MLCamera.OnRawImageAvailable += OnCaptureRawImageComplete;
			isConnected = true;
		}
		else {
			isConnected = false;
			Debug.Log("Error: Start camera failed.");
			Debug.Log(result.Code);
			if (result.Code == MLResultCode.PrivilegeDenied) {
				Debug.LogError("Error: Privilege Denied.");
			}
		}
	}


	private void HandlePrivilegesDone(MLResult result) {
		Debug.Log("Handle Privileges Done.");
		Debug.Log(result.Code);
		StartCapture();
	}

	private void CaptureThreadWorker() {
		if (MLCamera.IsStarted && isConnected) {
			MLResult result = MLCamera.CaptureRawImageAsync();
			if (!result.IsOk) {
				Debug.Log("Error: Capture Failed.");
			}
		}
	}

	
	public void TriggerCapture() {
		if (captureThread == null || !captureThread.IsAlive) {
			captureThread = new Thread(new ThreadStart(CaptureThreadWorker));
			captureThread.Start();
		}
	}
	
	void Update () {
		TriggerCapture();

	}

	private void OnCaptureRawImageComplete(byte[] imageData) {
		Texture2D texture = new Texture2D(8, 8);
		texture.LoadImage(imageData);
		Debug.Log(texture.width);
		Debug.Log(texture.height);
	}

	void OnDestroy()
	{
		if (_privilegeRequester != null) {
			_privilegeRequester.OnPrivilegesDone -= HandlePrivilegesDone;
		}
	}
}
