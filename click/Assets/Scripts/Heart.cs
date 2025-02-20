using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using fs;
using Sailfish;
using UnityEngine;
using UnityEngine.UI;


public class Heart : MonoBehaviour
{
    public static Heart instance;
    public Text run;

    public Text log;
    public Text id;
    public Button randomBtn;

    public Button capBtn;

    AndroidJavaClass unityPlayer;
    AndroidJavaObject currentActivity;

    ByteArray array = new ByteArray(1024 * 1024 * 1, 1024 * 1024 * 2);

    long appectID = 0;

    public bool isLink = false;

    private void Awake()
    {
        instance = this;
    }

    Coroutine sendCor = null;
    Coroutine shotCor = null;

    void Start()
    {
        capBtn.onClick.AddListener(() =>
        {
            try
            {
                if (sendCor != null)
                {
                    StopCoroutine(sendCor);
                    StopCoroutine(shotCor);
                }
  
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

        if (Application.platform == RuntimePlatform.Android)
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        // LANManager.instance.StartClient();
        StartCoroutine(Send());
    }

    public void Connect()
    {
        //   if(Application.platform == RuntimePlatform.WindowsEditor)
        //       NetConnectManager.instance.ConnectTo("127.0.0.1",5000, OnConnected,OnRect,OnClose);
        //   else 
        NetConnectManager.instance.ConnectTo("119.91.62.156",5000, OnConnected,OnRect,OnClose);

       // NetConnectManager.instance.ConnectTo("192.168.0.157", 5000, OnConnected, OnRect, OnClose);
        //NetConnectManager.instance.ConnectTo("127.0.0.1",5000, OnConnected,OnRect,OnClose);
    }
    
    

    public void OnConnected(long _)
    {
        Debug.Log("连接成功");
        appectID = _;
        isLink = true;
    }
    public void OnRect(long _, PacketBase packet)
    {
        Debug.Log("连接成功");
        if(packet is MousePackage)
        {
            var mouse = packet as MousePackage;
            RecvClick(mouse.x, mouse.y);
        }
    }

    public void OnClose(long _)
    {
        Debug.Log("连接关闭");
        appectID = 0;
        isLink = false;
        Replace();
    }


    string pathDir => Application.persistentDataPath + "/Screenshot";

    public void CaptureScreenshot()
    {
        if (Directory.Exists(pathDir) == false)
        {
            Directory.CreateDirectory(pathDir);
        }
        else
        {
            Directory.Delete(pathDir, true);
            Directory.CreateDirectory(pathDir);
        }

        currentActivity.Call("CaptureScreenshot", pathDir);
    }


    void PlayInfo()
    {
        array.Clear();
        var packet = PacketPools.Get(1002) as FunctionPackage;
        packet.type = "Check";
        packet.Write(array);
        NetConnectManager.instance.Send(appectID, packet);
    }



    public void RecvScreenshot(string byteStr)
    {
        //字符转换为字节
        bytes = System.Convert.FromBase64String(byteStr);
    }

    
    byte[] bytes = null;
    IEnumerator Send()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);

            if(bytes == null)
            {
                continue;
            }

            array.Clear();
            var packet = PacketPools.Get(1001) as TexturePacket;
            packet.type = "Texture";
            packet.width = 1080;
            packet.height = 1920;
            packet.length = bytes.Length;
            packet.data = bytes;
            packet.Write(array);
            NetConnectManager.instance.Send(appectID, packet);

            bytes = null;
        }
    }


    public void Click(int x, int y)
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

        currentActivity.Call("setSimulateClick", x, y);
    }

    public void RecvClick(int x, int y)
    {
        Click(x, y);
    }


    // Update is called once per frame
    void Update()
    {
        //Time.time 转换为时间显示
        System.TimeSpan t = System.TimeSpan.FromSeconds(Time.time);
        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
        run.text = $"时间：{answer}";

        CheckLink();

        NetConnectManager.instance.Update();
    }

    int StateID = 0;
    float runTime = 0;

    void Replace()
    {
        StateID = 0;
        runTime = 0;
    }

    void CheckLink()
    {
        if (isLink == false)
        {
            if (StateID == 0)
            {
                Connect();
                StateID = 1;
            }
        }
        else
        {
            if (StateID == 1)
            {
                runTime += Time.deltaTime;
                if (runTime > 3)
                {
                    StateID = 2;
                    runTime = 0;
                    PlayInfo();
                }
            }
            else if (StateID == 2)
            {
                StateID = 3;
            }
        }
    }


}
