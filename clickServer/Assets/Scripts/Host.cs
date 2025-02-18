using System.Collections;
using System.Collections.Generic;
using fs;
using Sailfish;
using UnityEngine;

public class Host : MonoBehaviour
{

TCPNetAccepter tCPNetAccepter;
     public LANManager manager;

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
        Debug.Log("连接成功");
    }

    public void OnClose(long _)
    {
        Debug.Log("连接关闭");
    }
 

    // Update is called once per frame
    void Update()
    {
        
    }
}
