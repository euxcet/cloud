using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class Map : MonoBehaviour {

    private MLInputController controller;
    public Text text;
    // Use this for initialization
    void Start()
    {
        MLInput.Start();

        controller = MLInput.GetController(MLInput.Hand.Left);
    }

    void OnDestroy()
    {
        MLInput.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null) { return; }

        MLInputControllerTouchpadGesture gesture = controller.TouchpadGesture;
        if (gesture != null)
        {
            text.text = gesture.Radius.ToString();
        } else
        {
            text.text = "Can't get the gesture of controller";
        }
        
        

        transform.position = controller.Position + new Vector3(0.3f, 1.0f, 0.05f);
        transform.rotation = controller.Orientation;
    }
}
