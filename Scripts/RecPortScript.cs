using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class RecPortScript : MonoBehaviour
{
	#region 串口参数,主要修改串口名与波特率
	/// <summary>
	/// 串口名
	/// </summary>
	string getPortName;
	/// <summary>
	/// 波特率
	/// </summary>
	int baudRate = 115200;
	private Parity parity = Parity.None;
	private int dataBits = 8;
	private StopBits stopBits = StopBits.One;
	SerialPort sp = null;
	#endregion

	#region 消息处理相关
	/// <summary>
	/// 缓存消息列表
	/// </summary>
	List<byte> bufferList = new List<byte>();
	/// <summary>
	/// 一条消息的长度
	/// </summary>
	int messageLen = 24;
	#endregion
	// Start is called before the first frame update
	void Start()
	{
		getPortName = "COM4";
		baudRate = 115200;

		OpenPort(getPortName, baudRate);
		StartCoroutine(DataReceiveFunction());
	}
	IEnumerator DataReceiveFunction()
	{

		while (true)
		{
			if (sp != null && sp.IsOpen)
			{
				try
				{
					RecAndProcessingFunction();
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}

	/// <summary>
	/// 读取并处理消息
	/// </summary>
	private void RecAndProcessingFunction()
	{
		//待读字节个数
		int n = sp.BytesToRead;
		//创建n个字节的缓存
		byte[] buf = new byte[n];
		//读到在数据存储到buf
		sp.Read(buf, 0, n);
		//1.缓存数据 不断地将接收到的数据加入到buffer链表中
		bufferList.AddRange(buf);
		//2.完整性判断 至少包含帧头（1字节）、类型（1字节）、功能位（22字节） 根据设计不同而不同
		while (bufferList.Count >= 2)
		{
			//2.1 查找数据头 根据帧头和类型
			if (bufferList[0] == 170 && bufferList[1] == 85)
			{
				//如果小于则说明数据区尚未接收完整，
				if (bufferList.Count < messageLen)
				{
					//跳出接收函数后之后继续接收数据
					break;
				}
				//得到一帧完整的数据，进行处理，在此之前可以使用校验位保证此帧数据完整性
				byte[] processingByteArray = new byte[messageLen];
				//从缓存池中拷贝到处理数组
				bufferList.CopyTo(0, processingByteArray, 0, messageLen);
				//处理一帧数据
				DataProcessingFunction(processingByteArray);
				//从缓存池移除处理完的这帧
				bufferList.RemoveRange(0, messageLen);
			}
			else
			{
				//帧头不正确时，清除第一个字节，继续检测下一个。
				bufferList.RemoveAt(0);
			}
		}
	}
	/// <summary>
	/// 数据处理
	/// </summary>
	private void DataProcessingFunction(byte[] dataBytes)
	{
		Debug.Log(byteToHexStr(dataBytes));
	}
	/// <summary>
	/// 字节数组转16进制字符串
	/// </summary>
	/// <param name="bytes"></param>
	/// <returns></returns>
	public static string byteToHexStr(byte[] bytes)
	{
		string returnStr = "";
		if (bytes != null)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				returnStr += bytes[i].ToString("X2");
				returnStr += "-";
			}
		}
		return returnStr;
	}

	#region 串口开启关闭相关
	//打开串口
	public void OpenPort(string DefaultPortName, int DefaultBaudRate)
	{
		sp = new SerialPort(DefaultPortName, DefaultBaudRate, parity, dataBits, stopBits);
		sp.ReadTimeout = 10;
		try
		{
			if (!sp.IsOpen)
			{
				sp.Open();
			}
		}

		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	//关闭串口
	public void ClosePort()
	{
		try
		{
			sp.Close();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}
	#endregion

	#region Unity
	private void OnApplicationQuit()
	{
		ClosePort();
	}
	private void OnDisable()
	{
		ClosePort();
	}
	#endregion
}

