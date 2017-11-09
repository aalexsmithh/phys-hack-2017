using System;
using UnityEngine;

public class VoxelTime
{
	public static readonly float V_SPEED = 12;
	public readonly float T0;
	public readonly int MAX_FRAMES;

	private float last_time = -1;
	private int last_result = 0;

	public VoxelTime (float t0, int max_frames)
	{
		T0 = t0;
		MAX_FRAMES = max_frames;
	}

	public int GetFrameNumber(float t) {
		if (last_time == t)
			return last_result;
		
		int f = (int)((t - T0) * V_SPEED) % (MAX_FRAMES * 2);
		if (f >= MAX_FRAMES)
			f = 2 * MAX_FRAMES - f - 1;

		last_time = t;
		last_result = f;
		Debug.Log (String.Format ("t = {0}", f));
		return f;
	}
}