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

   public class SyncPoint
   {
     public float x;
     public float y;
     public float z;

     public string ToSend()
     {
        return $"{x}_{y}_{z}";
     }

     public void ToRecv(string data)
     {
        string[] pos = data.Split('_');
        x = float.Parse(pos[0]);
        y = float.Parse(pos[1]);
        z = float.Parse(pos[2]);
     }
   }
  
    public class NetPlayer : NetPalyerCall
    {
        public PlayerInfo info;
        public int playerID;

        [SyncVar] public string messge;
        [SyncVar] public string group;
        [SyncVar] public string machine;


        void InitSelf()
        {
           if(isLocalPlayer)
           {
              NetManager.instance.localPlayer = this;
           }
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

            if(key == nameof(SyncPoint))
            {
                SyncPoint point = new SyncPoint();
                point.ToRecv(value);
                Heart.instance.Click((int)point.x,(int)point.y);
            }
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



        public void SendPoint(Vector3 pos)
        {
            SyncPoint point = new SyncPoint();
            point.x = pos.x;
            point.y = pos.y;
            point.z = pos.z;
            string data = point.ToSend();
            CMDSetupPlayerString(nameof(SyncPoint), data);
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
                group   = match.Groups[1].Value;
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
