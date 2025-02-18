//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2024-07-23 09:25:00
//=======================================================
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Sailfish
{
    public class NetManager : NetPalyerCall
    {
        [SyncVar] public string message;
        public static NetManager instance;
        public List<NetPlayer>   netPlayers = new List<NetPlayer>();
        public Dictionary<string, GameObject> netGroups = new Dictionary<string, GameObject>();
        public NetPlayer localPlayer;

        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }


        protected override void SyncPlayerString(string key, string value)
        {
            if (key == nameof(message))
            {
                message = value;
            }
        }





        public void AddPlayer(NetPlayer netPlayer)
        {
            Debug.Log("netPlayer.messge" + netPlayer.messge);

            if (netGroups.TryGetValue(netPlayer.group, out var go) == false)
            {
                go = new GameObject(netPlayer.group);
                go.transform.SetParent(this.transform);
                go.name = $"[{netPlayer.group}]Group";
                netGroups.Add(netPlayer.group, go);
            }


            if (localPlayer == null)
            {
                if (LANManager.instance.selfInfo.group == netPlayer.info.group)
                {
                    localPlayer = netPlayer;
                }
            }

            netPlayer.transform.SetParent(go.transform);

            if (netPlayers.Contains(netPlayer) == false)
            {
                netPlayers.Add(netPlayer);
            }
        }

        public void RemovePlayer(NetPlayer netPlayer)
        {
            if (netPlayers.Contains(netPlayer))
            {
                netPlayers.Remove(netPlayer);
            }
        }


      

       
        public void Receive()
        {
        
        }

    



        /// <summary>
        /// 同步所有的数据
        /// </summary>
        public void Send()
        {
           // CmdSend();
        }




        void Start()
        {
            Debug.Log("开始同步服务端数据");
    
           // CmdSend();
        }
    }
}
