using System;
using UnityEngine;

public class ColorFlyweight
{
	public static readonly int N_SHADES = 128;

	private Color[] reds;
	private Color[] blues;

	private ColorFlyweight ()
	{
		reds = new Color[N_SHADES];
		blues = new Color[N_SHADES];
		for (int i = 0; i < N_SHADES; i++) {
			float alpha = i / (float)N_SHADES;

			// 1 - alpha goes from 1 to 0 finely
			// idea: make values near zero look purple by mixing in blue (resp. red) that fades out
			// as i -> N_SHADES.
			reds [i] = new Color (1, 0, 1 - alpha, alpha);
			blues [i] = new Color (1 - alpha, 0, 1, alpha);
		}
	}

	private static ColorFlyweight instance;

	public static ColorFlyweight Instance {
		get {
			if(instance == null) {
				instance = new ColorFlyweight ();
			}
			return instance;
		}
	}

	private int IndexOf(float f) {
		return (int)Math.Floor (f * N_SHADES);
	}

	public Color Red(float alpha) {
		return reds [IndexOf(alpha)];
	}

	public Color Blue(float alpha) {
		return blues [IndexOf (alpha)];
	}
}