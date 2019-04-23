using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Screen : MonoBehaviour {

    private MLInputController controller;
	// Use this for initialization
	void Start () {
        MLInput.Start();
        controller = MLInput.GetController(MLInput.Hand.Left);
        Debug.Log(controller.Position);
        Debug.Log(controller.Orientation);
    }

    void OnDestroy()
    {
        MLInput.Stop();
    }

    // Update is called once per frame
    void Update () {
        transform.position = controller.Position;
        transform.rotation = controller.Orientation;
	}
}
