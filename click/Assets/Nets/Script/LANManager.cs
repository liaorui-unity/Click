//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2024-05-30 16:54:40
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.Video;
using Mirror.Discovery;
using Mirror;
using fs;

namespace Sailfish
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string group;
        public string name;
        public string ip;
        public double time;
    }


    public class LANManager : MonoBehaviour
    {
        public static LANManager _instance;
        public static LANManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LANManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("LANManager");
                        _instance = go.AddComponent<LANManager>();
                    }
                }
                return _instance;
            }
        }


        public LANFindServer findServer;
        public PlayerInfo selfInfo;

 
        public bool isOpenHost = false;


        bool isFindHost = false;
        bool isLink = false;

        void Start()
        {
            AddSelf();
        }

        void AddSelf()
        {
            selfInfo      = new PlayerInfo();
            selfInfo.ip   = NetUtils.GetLocalIpv4();
            selfInfo.name = NetUtils.DeviceGUID;
            selfInfo.time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 1000000;
        }



        public void StartHost()
        {
            findServer.StartHost();
        }

        public void StartClient()
        {
          //  findServer.Connect();
            NetworkManager.singleton.networkAddress = "119.91.62.156";
            NetworkManager.singleton.StartClient();
        }

        public void FindHost()
        {
            findServer.FindServers();
        }




        void Update()
        {
            if (!NetworkClient.isConnected)
            {  
                // if (!isFindHost)
                // {
                //     isLink     = false;
                //     isFindHost = true;
                //     FindHost();
                //     Debug.Log("重新查找主机");
                // }
            }
            else
            {
                if (!isLink)
                {
                   if(findServer.isHasServer())
                   {
                       isLink = true;

                       StartClient();
                        Debug.Log("重新连接");
                                               isFindHost = false;
                   }
                }
            }
        }


    }
}
