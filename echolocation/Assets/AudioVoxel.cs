using UnityEngine;
using System;

public class AudioVoxel : MonoBehaviour
{
	public Material material;
	public int x, y, z;
	public Data data;
	private double value;
	public VoxelTime VOXEL_TIME;
	public float DELTA;

	public AudioVoxel ()
	{
	}

	private static Color makeRed(float alpha) {
		return ColorFlyweight.Instance.Red (alpha);
	}

	private static Color makeBlue(float alpha) {
		return ColorFlyweight.Instance.Blue (alpha);
	}

	public void Start() {
		x = (int)Math.Round (gameObject.transform.position.x / DELTA);
		y = (int)Math.Round (gameObject.transform.position.y / DELTA);
		z = (int)Math.Round (gameObject.transform.position.z / DELTA);
	}

	public void Update() {
		// Debug.Log ("updating!");
		int n = VOXEL_TIME.GetFrameNumber(Time.time);
		value = data.Point(n, x, y, z);
		float f = (float)Math.Abs (value);
		if(value < 0) {
			material.color = makeRed(f);
		}
		else {
			material.color = makeBlue(f);
		}
	    // value is 0 for lo sound, 1 for hi sound.
	}
}