using System.Collections;
using System.Collections.Generic;
using System.IO;
using fs;
using Sailfish;
using UnityEngine;
using UnityEngine.UI;


public class Heart : MonoBehaviour
{  
    public static Heart instance;
    public  Text run;

    public Text log;
    public Button randomBtn;

    public Button capBtn;

    float cycle = 3;
    float runTime = 0;

    AndroidJavaClass unityPlayer;
    AndroidJavaObject currentActivity;

    TCPNetConnecter netConnecter;


    private void Awake()
    {
        instance = this;
        netConnecter = new TCPNetConnecter();
    }
    

    void Start()
    {
        capBtn.onClick.AddListener(() =>
        {
            try
            { 
              CaptureScreenshot();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        });

        randomBtn.onClick.AddListener(() =>
        {
            try
            {
              Click(Random.Range(100, 1000), Random.Range(100, 1000));
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        });

        if(Application.platform == RuntimePlatform.Android)
        {
            unityPlayer     = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        LANManager.instance.StartClient();

        netConnecter.Connect("119.91.62.156",5000, OnConnected,OnRect,OnClose);

        StartCoroutine(Send());
    }

    
    public void OnConnected(long _)
    {
        Debug.Log("连接成功");
    }
    public void OnRect(long _,PacketBase packet)
    {
        Debug.Log("连接成功");
    }

    public void OnClose(long _)
    {
        Debug.Log("连接关闭");
    }
 

    public void CaptureScreenshot()   
    {  
        currentActivity.Call("CaptureScreenshot",Application.persistentDataPath);
    }

    IEnumerator Send()
    {
        while (true)
        {
            if(Application.platform == RuntimePlatform.WindowsEditor)
            {
              ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/Screenshot.png");
            }
            yield return new WaitForSeconds(1);
            GetScreenshotBytes(Application.persistentDataPath + "/Screenshot.png");
        }
    }

    public void GetScreenshotBytes(string filePath)
    {
        string filename = Application.persistentDataPath + "/Screenshot.png";
        byte[] bytes = System.IO.File.ReadAllBytes(filename);

        Debug.Log("发送截图数据:"+bytes.Length);
        NetManager.instance.localPlayer.SendScreenshot(bytes);

        //bytes 转字节
         
        string str = System.Text.Encoding.Default.GetString(bytes);
        UDPLocalSendManager.instance.Send(str, "119.91.62.156", 47777, false);

       // netConnecter.Send(new PacketBase(1,bytes));
    }


    public void Click(int x,int y)
    {
        log.text = $"点击坐标：{x},{y}";

        if (unityPlayer == null)
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        }

        if (currentActivity == null)
        {
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        currentActivity.Call("setSimulateClick",x,y);
    }

    public void RecvClick(int x,int y)
    {
       Click(x,y);
    }
  

    // Update is called once per frame
    void Update()
    {
        //Time.time 转换为时间显示
        System.TimeSpan t = System.TimeSpan.FromSeconds(Time.time);
        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
        run.text =$"时间：{answer}";
    }

  
  

}
