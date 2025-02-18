using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Net.Http.Headers;

namespace Sailfish
{

  
    public class NetPlayer : NetPalyerCall
    {
        public PlayerInfo info;
        public int playerID;

        [SyncVar] public string messge;
        [SyncVar] public string group;
        [SyncVar] public string machine;


        void InitSelf()
        {
          
        }


        protected override void SyncPlayerString(string key, string value)
        {
            if (key == nameof(messge))
            {
                messge = value;
                info   = JsonUtility.FromJson<PlayerInfo>(value);
                // 使用正则表达式匹配
                Match match = Regex.Match(messge, @"\[(\d)\]Group_\[(\d)\]Machine");

                if (match.Success)
                {
                    group   = match.Groups[1].Value;
                    machine = match.Groups[2].Value;

                    playerID = (int.Parse(group)-1) * 2 + int.Parse(machine);
                }
                this.gameObject.name = $"[{machine}]Machine";
            }
            
        }

        protected override void SyncPlayerInt(string key, int value)
        {
          
        }


        public void SendScreenshot(byte[] bytes)
        {
            CMDSetupPlayerString(bytes);
        }
     
        [Command(ignoreAuthority = true)]
        public void CMDSetupPlayerString(byte[] bytes)
        {
            RPCSetupPlayerString(bytes);
        }

        [ClientRpc]       
        public void RPCSetupPlayerString(byte[] bytes)
        {
         
        }


      

        /// <summary>
        /// 自己同步自己的数据到各个机台
        /// </summary>
        async void Start()
        {
           InitSelf();

            await Task.Delay(100);

            Debug.Log("messge:" + messge);
            InitPlayerInit();
            NetManager.instance.AddPlayer(this);
        }


        void InitPlayerInit()
        {
            info = JsonUtility.FromJson<PlayerInfo>(messge);
            // 使用正则表达式匹配
            Match match = Regex.Match(messge, @"\[(\d)\]Group_\[(\d)\]Machine");

            if (match.Success)
            {
                group = match.Groups[1].Value;
                machine = match.Groups[2].Value;

                playerID = int.Parse(group) * 2 + int.Parse(machine);
            }
            this.gameObject.name = $"[{machine}]Machine";
        }


  
 


        void OnDestroy()
        {
            NetManager.instance.RemovePlayer(this);
        }
    }
}
