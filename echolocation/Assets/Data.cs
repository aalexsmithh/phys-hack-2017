using System;
using System.IO;
using UnityEngine;

public class Data {
	char[] m_data;
	int m_side;

	public Data (char[] data, int side) {
		m_data = data;
		m_side = side;
	}

	public char Point(int t, int x, int y, int z) {
		return m_data [t * x * y * z];
	}

	public int Duration() {
		return m_data.GetLength (0);
	}

	public int Side() {
		return m_side;
	}
}

public interface DataReader {
	Data Read();
}

/**
 * A DataReader that reads a matrix whose dimensions are fixed.
 * In that case, the input matrix file is just a giant list of values with no header.
 */
public class FixedDataReader : DataReader {
	readonly BinaryReader m_input;
	readonly int m_q, m_ts;

	/**
	 * path: file to read from.
	 * q: the total number of voxels in a cubic frame.
	 * ts: the number of time steps that were simulated.
	 */
	public FixedDataReader(BinaryReader input, int q, int ts) {
		m_input = input;
		m_q = q;
		m_ts = ts;
	}

	public Data Read() {
		char[] data2d = new char[m_ts * m_q];

		Debug.Log ("Reading char data...");

		const int BUF_SIZE = 128 * 1024;
		char[] buf = new char[BUF_SIZE];
		int p = 0;

		for (int i = 0; i < m_ts; i++) {
			for (int j = 0; j < m_q; j += BUF_SIZE) {
				int n_read = m_input.Read (buf, 0, BUF_SIZE);
				// Debug.Log (String.Format ("m_q*i = {0}, n_read = {1}", m_q * i, n_read));
				// Array.Copy (buf, 0, data2d, m_q * i, n_read); 
				Buffer.BlockCopy (buf, 0, data2d, m_q * i + j, n_read);
			}
//			for(int j = 0; j < n_read; j++) {
//				data2d [i, j] = buf[j];
//
//				p = (int)(Math.Floor (100.0 * j / n_read));
//				Debug.Log (String.Format ("Read {0}% of the loop buffer.", p));
//			}

			p = (int)(Math.Floor (100.0 * i / m_ts));
			Debug.Log (String.Format ("Read {0}% of the data.", p));
		}

		return new Data (data2d, (int)(Math.Pow(m_q, 1f/3f)));
	}
}

/**
 * A DataReader that reads a header telling us the dimensions of the matrix
 * followed by a matrix of that size.
 */
public class HeaderDataReader : DataReader {
	readonly BinaryReader m_src;

	public HeaderDataReader(BinaryReader src) {
		m_src = src;
	}

	public Data Read() {
		// open the file and read the header

		int q = m_src.ReadInt32 ();
		int ts = m_src.ReadInt32 ();
		Debug.Log (String.Format ("q={0}, ts={1}", q, ts));

		Data d = new FixedDataReader (m_src, q, ts).Read();

		return d;
	}
}


/**
 * Normalizes and converts floats to chars.
 */
public class CharNormalizer {
	readonly BinaryReader m_src;
	readonly BinaryWriter m_dst;
	readonly int m_shades;
	readonly long m_count;

	public const int BUF_SIZE = 8192;

	/**
	 * The stream underlying the BinaryReader must be seekable, since we read it twice!
	 */
	public CharNormalizer(BinaryReader src, BinaryWriter dst, int shades, long count) {
		m_src = src;
		m_dst = dst;
		m_shades = shades;
		m_count = count;
	}

	public void Normalize() {
		long now = m_src.BaseStream.Position;

		// set up buffer for bulk copying
		float[] interp_buf = new float[BUF_SIZE];
		byte[] raw_buf = new byte[BUF_SIZE * 4];

		// first find the maximal value
		// runs in constant memory
		float m = 0;

		Debug.Log (String.Format ("m_count%BUF_SIZE: {0},", m_count % BUF_SIZE));
		for(long i = 0; i < m_count; i += BUF_SIZE) {
			int n_read = m_src.Read (raw_buf, 0, BUF_SIZE * 4);
			if (n_read % 4 != 0)
				Debug.LogError ("uh oh we have some fucked up data here");
			n_read /= 4;
			Buffer.BlockCopy (raw_buf, 0, interp_buf, 0, n_read);

			for (int j = 0; j < n_read; j++) {
				float f = interp_buf [j];
				f = Math.Abs(f);
				if (f > m)
					m = f;
			}
		}

		Debug.Log (String.Format ("Found maximum value: {0}", m));

		// return to the beginning of the file.
		m_src.BaseStream.Seek (now, SeekOrigin.Begin);
		for(long i = 0; i < m_count; i += BUF_SIZE) {
			int n_read = m_src.Read (raw_buf, 0, BUF_SIZE * 4);
			Buffer.BlockCopy (raw_buf, 0, interp_buf, 0, n_read);

			if (n_read % 4 != 0)
				Debug.LogError ("uh oh we have some fucked up data here");
			n_read /= 4;

			for (int j = 0; j < n_read; j++) {
				float f = interp_buf [j];
				if (float.IsNaN (f))
					f = 0;
				f = (float)Math.Floor(f / m * m_shades);
				if (f < -128 || f > 127)
					Debug.LogError ("Normalization produced abnormal result.");
				byte c = (byte)f;
				raw_buf [j] = c;
			}

			m_dst.Write (raw_buf, 0, n_read);
			m_dst.Flush ();
		}

		Debug.Log ("Finished normalization.");
	}
}