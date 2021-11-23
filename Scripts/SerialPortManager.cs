using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

public class SerialPortManager : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 57600;
    public Parity parity = Parity.None;
    public int dataBits = 32;
    public StopBits stopBits = StopBits.None;
    SerialPort sp = null;
    Thread dataReceiveThread;
    public List<byte> listReceive = new List<byte>();
    char[] strchar = new char[100];
    string str;
    private Int16[] feedbackAngleRaw = {0,0};

    void Start()
    {
        OpenPort();
        dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
        dataReceiveThread.Start();
    }
    public void OpenPort()
    {
        //
        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        sp.ReadTimeout = 400;
        try
        {
            sp.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    /**
     * @brief: Get the angle of motors
     * @params: id Logical ID of motor, 0 for YAW, 1 for PITCH
     */
    public float angle(int id)
    {
        return (float)feedbackAngleRaw[id] / 8192.0f;
    }
    void OnApplicationQuit()
    {
        ClosePort();
    }

    public void ClosePort()
    {
        try
        {
            sp.Close();
            dataReceiveThread.Abort();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void Update()
    {
        PrintData();
    }

    //
    void PrintData()
    {
        for (int i = 0; i < listReceive.Count; i++)
        {
            strchar[i] = (char)(listReceive[i]);
            str = new string(strchar);
        }
        //Debug.Log(str);
    }

    //
    void DataReceiveFunction()
    {
        byte[] buffer = new byte[1024];
        int bytes = 0;
        while (true)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    bytes = sp.Read(buffer, 1, 4);//
                    if (bytes == 0)
                    {
                        continue;
                    }
                    else
                    {
                        feedbackAngleRaw[0] = (Int16)(buffer[0] << 8 | buffer[1]);
                        feedbackAngleRaw[1] = (Int16)(buffer[2] << 8 | buffer[3]);
                        Debug.Log("yes! ");
                        Debug.Log(buffer[0]);
                        Debug.Log(buffer[1]);
                        Debug.Log(buffer[2]);
                        Debug.Log(buffer[3]);
                        Debug.Log(feedbackAngleRaw[0]);
                        Debug.Log(feedbackAngleRaw[1]);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ThreadAbortException))
                    {
                    }
                }
            }
            Thread.Sleep(10);
        }
    }

    //
    public void WriteData(string dataStr)
    {
        if (sp.IsOpen)
        {
            sp.Write(dataStr);
        }
    }
}