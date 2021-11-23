﻿

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 简单的Unity 直接调用 Python 的方法
/// </summary>
public class LoadPython : MonoBehaviour
{
    public GameObject _publisher;
    private Vector3 _force = new Vector3(0, 0, 0);
    private Vector3 _lastForce = new Vector3(0, 0, 0);
    private string basePath = @"C:\Users\Lamfu\Documents\VScode_python\";
    // Start is called before the first frame update
    void Start()
    {
        //CallPythonHW(basePath+ "HelloWorld.py");
        CallPythonAddHW(basePath + "F2T.py", 0f, 0f);
    }
    private void Update()
    {
        _force = _publisher.GetComponent<Acceleration>().forceH2P_hand;
        Vector3 vectordifference = _lastForce - _force;
        if(vectordifference.sqrMagnitude >= 0.1f)
        {
            CallPythonAddHW(basePath + "F2T.py", _force.y, _force.z);
        }
        _lastForce = _force;
    }

    void CallPythonHW(string pyScriptPath)
    {
        CallPythonBase(pyScriptPath);
    }

    void CallPythonAddHW(string pyScriptPath, float a, float b)
    {
        CallPythonBase(pyScriptPath, a.ToString(), b.ToString());
    }

    /// <summary>
    /// Unity 调用 Python
    /// </summary>
    /// <param name="pyScriptPath">python 脚本路径</param>
    /// <param name="argvs">python 函数参数</param>
    public void CallPythonBase(string pyScriptPath, params string[] argvs)
    {
        Process process = new Process();

        // ptython 的解释器位置 python.exe
        process.StartInfo.FileName = @"C:\Users\Lamfu\anaconda3\python.exe ";

        // 判断是否有参数（也可不用添加这个判断）
        if (argvs != null)
        {
            // 添加参数 （组合成：python xxx/xxx/xxx/test.python param1 param2）
            foreach (string item in argvs)
            {
                pyScriptPath += " " + item;
            }
        }
        UnityEngine.Debug.Log(pyScriptPath);

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.Arguments = pyScriptPath;     // 路径+参数
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;        // 不显示执行窗口

        // 开始执行，获取执行输出，添加结果输出委托
        process.Start();
        process.BeginOutputReadLine();
        process.OutputDataReceived += new DataReceivedEventHandler(GetData);
        process.WaitForExit();
    }

    /// <summary>
    /// 输出结果委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void GetData(object sender, DataReceivedEventArgs e)
    {

        // 结果不为空才打印（后期可以自己添加类型不同的处理委托）
        if (string.IsNullOrEmpty(e.Data) == false)
        {
            UnityEngine.Debug.Log(e.Data);
        }
    }
}