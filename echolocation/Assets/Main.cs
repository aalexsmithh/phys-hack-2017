using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Main : MonoBehaviour {
	int SIDE, STEPS;
	Data data;
	readonly float DELTA = 1; // [UU]

	public Material voxelMaterial;

	public VoxelTime VOXEL_TIME;

	readonly string NORMAL_DATA = "C:\\Users\\owner\\Documents\\MATLAB\\normal.bin";
	readonly string RAW_DATA = "C:\\Users\\owner\\Documents\\MATLAB\\temp_sensor_data.bin";

	// instantiated by unity!
	public GameObject template;

	private void LoadExistingData() {
		FileStream input = new FileStream (NORMAL_DATA, FileMode.Open);
		BinaryReader br = new BinaryReader (input);
		DataReader dr = new HeaderDataReader (br);
		data = dr.Read ();
		br.Close ();
	}

	private void LoadData() {
		if (File.Exists (NORMAL_DATA)) {
			Debug.Log ("Found normalized data! Loading...");
			LoadExistingData ();
		} else {
			Debug.Log ("No normalized data found. Normalizing...");
			FileStream fs_input = new FileStream (RAW_DATA, FileMode.Open);
			FileStream fs_output = new FileStream (NORMAL_DATA, FileMode.CreateNew);
			BinaryReader br_input = new BinaryReader (fs_input);
			BinaryWriter br_output = new BinaryWriter (fs_output);

			// we need to write the header into the output file
			// first we put the Q
			const int side = 64;
			br_output.Write ((int)(side * side * side));
			// next we put the number of timesteps
			br_output.Write (769);

			long n_samples = fs_input.Length / 4;
			if (fs_input.Length % 4 != 0)
				Debug.LogError ("Uh oh the data is fucked.");
			// because the fs_input is just a giant list of floats, dividing in four gives the number of samples.

			CharNormalizer normalizer = new CharNormalizer (br_input, br_output, ColorFlyweight.N_SHADES, n_samples);
			normalizer.Normalize ();
			br_input.Close ();
			br_output.Close ();

			LoadExistingData ();
		}
		Debug.Log ("data loaded!");
	}

	// Use this for initialization
	void Start () {
		LoadData ();

		// load the data
		VOXEL_TIME = new VoxelTime (Time.time, data.Duration());

		// construct the template cube that is Instantiated for each voxel.
		template.transform.localScale = new Vector3 (DELTA, DELTA, DELTA);

		for (int i = 0; i < data.Side (); i++) {
			for (int j = 0; j < data.Side (); j++) {
				for (int k = 0; k < data.Side (); k++) {
					Vector3 p = new Vector3 (DELTA * i, DELTA * j, DELTA * k);
					// instantiate the cube at its position.
					GameObject inst = GameObject.Instantiate (template, p, Quaternion.identity);
					inst.transform.parent = gameObject.transform;
					AudioVoxel v = inst.GetComponent<AudioVoxel>();
					v.VOXEL_TIME = VOXEL_TIME;
					v.data = data;
					v.DELTA = DELTA;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
