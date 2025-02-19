using System.Collections;
using System.Collections.Generic;
using fs;
using Sailfish;
using UnityEngine;
using UnityEngine.UI;

public class Host : MonoBehaviour
{

    public Image image;
    TCPNetAccepter tCPNetAccepter;
    public LANManager manager;

    public long ckeckId;
    public long controlId;

    // Start is called before the first frame update
    void Start()
    {
        manager.StartHost();
        tCPNetAccepter = new TCPNetAccepter();
        tCPNetAccepter.Listen(5000,OnConnected,OnRect,OnClose);

    }

    
    public void OnConnected(long _)
    {
        Debug.Log("连接成功");

    }
    public void OnRect(long _,PacketBase packet)
    {
        Debug.Log("接受成功");

        if(packet is FunctionPackage functionPackage)
        {
            if(functionPackage.type == "Check")
            {
                ckeckId = _;
            }
            else if(functionPackage.type == "Control")
            {
                controlId = _;
            }
        }
        else if(packet is TexturePacket texturePacket)
        {
            Debug.Log("接收到图片");
            tCPNetAccepter.Send(controlId,texturePacket);

            Texture2D texture = new Texture2D(texturePacket.width,texturePacket.height);
            texture.LoadImage(texturePacket.data);
            image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0.5f,0.5f));
            image.rectTransform.sizeDelta = new Vector2(540 ,960);

        }
    }

    public void OnClose(long _)
    {
        Debug.Log("连接关闭");
        if(ckeckId == _)
        {
            ckeckId  = -1;
        }
        else if(controlId == _)
        {
            controlId = -1;
        }
    }
 

 void Update()
 {
    tCPNetAccepter?.Update();
 }

 
}
