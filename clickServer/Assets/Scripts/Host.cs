using System.Collections;
using System.Collections.Generic;
using fs;
using UnityEngine;
using UnityEngine.UI;

public class Host : MonoBehaviour
{
    public Image image;
    TCPNetAccepter tCPNetAccepter;
    public long ckeckId;
    public long controlId;

    public Text controlIP;
    public Text checkIP;


    public Text recvTxt;


    // Start is called before the first frame update
    void Start()
    {
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
                Debug.Log("接收到监控包:"+_);
                ckeckId = _;
                checkIP.text = $"连接:{_}";
            }
            else if(functionPackage.type == "Control")
            {
                Debug.Log("接收到控制包:"+_);
                controlId = _;
                controlIP.text = $"连接:{_}";
            }
        }
        else if(packet is TexturePacket texturePacket)
        {
            tCPNetAccepter.Send(controlId,texturePacket);
            recvTxt.text =$"接收数据:{texturePacket.length}";
        }
        else if(packet is MousePackage mousePackage)
        {
            tCPNetAccepter.Send(ckeckId,mousePackage);

        }
    }

    public void OnClose(long _)
    {
        Debug.Log("连接关闭");
        if(ckeckId == _)
        {
            ckeckId  = -1;
            checkIP.text = "未连接";
        }
        else if(controlId == _)
        {
            controlId = -1;
            controlIP.text = "未连接";
        }
    }
 

    void FixedUpdate()
    {
        tCPNetAccepter?.Update();
    }

    
    void OnDestroy() 
    {
         tCPNetAccepter?.Close();
    }

}
