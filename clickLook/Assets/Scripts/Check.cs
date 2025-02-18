using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using fs;
using Sailfish;
using UnityEngine;
using UnityEngine.UI;

public class Check : MonoBehaviour
{

    public static Check instance;

    public NetPlayer netPlayer;

    public Image rect;
    public Image point;

       TCPNetConnecter netConnecter = new TCPNetConnecter();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LANManager.instance.StartClient();
  
        netConnecter = new TCPNetConnecter();
        netConnecter.Setup();

        netConnecter.Connect("119.91.62.156",5000, OnConnected,OnRect,OnClose);
    }

    
    public void OnConnected(long _)
    {
        Debug.Log("连接成功");
    }
    public void OnRect(long _,PacketBase packet)
    {
        Debug.Log("接收成功");
    }

    public void OnClose(long _)
    {
        Debug.Log("连接关闭");
    }
 
    
    //检测鼠标是否点击在rect上
    void CheckPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.rectTransform, Input.mousePosition, null, out pos);
            if (rect.rectTransform.rect.Contains(pos))
            {
                point.rectTransform.anchoredPosition = pos;
                netPlayer.SendPoint(pos);
            }
        }
    }


    public void RecvBytes(byte[] bytes)
    {
        Debug.Log("接收到的数据转成:"+bytes.Length);
          //接收到的数据转成texture2D
            Texture2D texture = new Texture2D(1080, 1920);
            texture.LoadImage(bytes);
            rect.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
  

    // Update is called once per frame
    void Update()
    {
        CheckPoint();
    }
}
