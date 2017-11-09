using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraInput : MonoBehaviour {
	public static readonly float T_SPEED = 5;
	public static readonly float R_SPEED = 5;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 rv = OVRInput.Get (OVRInput.Axis2D.SecondaryThumbstick);
		gameObject.transform.parent.Rotate (new Vector3 (0, R_SPEED*rv.x, 0));

		Vector2 tv = OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick) * T_SPEED;
		Vector3 t = gameObject.transform.forward * tv.y + gameObject.transform.right * tv.x;
		// Debug.Log (String.Format ("{0}", t));
		gameObject.transform.parent.Translate (t);
	}
}
