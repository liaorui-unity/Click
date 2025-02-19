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
    public  Text run;

    public Text log;
    public Text id;
    public Button randomBtn;

    public Button capBtn;

    AndroidJavaClass unityPlayer;
    AndroidJavaObject currentActivity;



    ByteArray array = new ByteArray(1024*1024*1, 1024*1024*2);

    long appectID =0;

    private void Awake()
    {
        instance = this;
    }
    
    Coroutine sendCor=null;
    Coroutine shotCor=null;

    void Start()
    {
        capBtn.onClick.AddListener(() =>
        {
            try
            { 

                if(sendCor != null)
                {
                    StopCoroutine(sendCor);
                    StopCoroutine(shotCor);
                }
                sendCor = StartCoroutine(Send());
                shotCor = StartCoroutine(Screenshot());

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

        //  if(Application.platform == RuntimePlatform.WindowsEditor)
        //      NetConnectManager.instance.ConnectTo("127.0.0.1",5000, OnConnected,OnRect,OnClose);
        //  else 
        //    NetConnectManager.instance.ConnectTo("119.91.62.156",5000, OnConnected,OnRect,OnClose);

               NetConnectManager.instance.ConnectTo("192.168.0.157",5000, OnConnected,OnRect,OnClose);

         //NetConnectManager.instance.ConnectTo("127.0.0.1",5000, OnConnected,OnRect,OnClose);
       
    }

    
    public void OnConnected(long _)
    {
        Debug.Log("连接成功");
        appectID = _;
    }
    public void OnRect(long _,PacketBase packet)
    {
        Debug.Log("连接成功");
    }

    public void OnClose(long _)
    {
        Debug.Log("连接关闭");
    }
 
   
   string pathDir => Application.persistentDataPath + "/Screenshot";

    public void CaptureScreenshot()   
    {  
        if(Directory.Exists(pathDir) == false)
        {
            Directory.CreateDirectory(pathDir);
        }
        else 
        {
            Directory.Delete(pathDir,true);
            Directory.CreateDirectory(pathDir);
        }

        currentActivity.Call("CaptureScreenshot",pathDir);
    }

    IEnumerator Send()
    {
         //发送图片
        array.Clear();
        var packet    = PacketPools.Get(1002) as FunctionPackage;
        packet.type   = "Check";
        packet.Write(array);
        NetConnectManager.instance.Send(appectID,packet);

        while (true)
        {
            if(ids.Count > 1)
            {
                saveID = ids[0];
                ids.RemoveAt(0);
                GetScreenshotBytes();
            }
            
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator Screenshot()
    {
        while (true)
        {  
            if(Application.platform == RuntimePlatform.WindowsEditor)
            {
                ScreenCapture.CaptureScreenshot(path);
            }
            else 
            {
                currentActivity.Call("NextCapture",$"{fileID}.png");
            }
            fileID += 1;
            yield return new WaitForSeconds(0.75f);
        }
    }

    int saveFileID = 0;

    List<int> ids = new List<int>();
    public void RecvScreenshot(string file)
    {
        saveFileID = int.Parse(file);
        ids.Add(saveFileID);
    }

    public int fileID  = 0;
    private int saveID = 0;
    private int sendID = -1;

    public string path => $"{pathDir}/{saveID}.png";

    public void GetScreenshotBytes()
    {
       if(File.Exists(path))
       {
            if(sendID == saveID)
            {
                return;
            }

            sendID = saveID;
            
           
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            Debug.Log("shot length:" + bytes.Length);

            array.Clear();
            var packet    = PacketPools.Get(1001) as TexturePacket;
            packet.type   = "Texture";
            packet.width  = 1080;
            packet.height = 1920;
            packet.length = bytes.Length;
            packet.data   = bytes;
            packet.Write(array);
            NetConnectManager.instance.Send(appectID,packet);
       }
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
