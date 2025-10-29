using System;
using System.Collections.Generic;
using System.Text;

public class Packet
{
	public byte[] incoming_data;

	public List<byte> outgoing_data;

	private int read_iterator;

	public void SendToMaster()
	{
	}

	public void SendToGameServer()
	{
	}

	public Packet(byte[] incoming_data)
	{
		this.incoming_data = incoming_data;
	}

	public Packet()
	{
		outgoing_data = new List<byte>();
	}

	public void putByte(byte B)
	{
		outgoing_data.Add(B);
	}

	public void putShort(short S)
	{
		byte[] bytes = BitConverter.GetBytes(S);
		outgoing_data.Add(bytes[0]);
		outgoing_data.Add(bytes[1]);
	}

	public void putString(string str)
	{
		byte[] bytes = Encoding.Unicode.GetBytes(str);
		outgoing_data.Add((byte)bytes.Length);
		for (int i = 0; i < bytes.Length; i++)
		{
			outgoing_data.Add(bytes[i]);
		}
	}

	public byte getByte()
	{
		byte result = incoming_data[read_iterator];
		read_iterator++;
		return result;
	}

	public short getShort()
	{
		if (read_iterator < 0)
		{
			return 0;
		}
		if (read_iterator >= incoming_data.Length)
		{
			return 0;
		}
		if (read_iterator + 1 >= incoming_data.Length)
		{
			return 0;
		}
		short result = BitConverter.ToInt16(incoming_data, read_iterator);
		read_iterator += 2;
		return result;
	}

	public string getString()
	{
		int num = incoming_data[read_iterator];
		read_iterator++;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = incoming_data[read_iterator];
			read_iterator++;
		}
		return Encoding.Unicode.GetString(array);
	}

	public byte[] convert()
	{
		return outgoing_data.ToArray();
	}
}
