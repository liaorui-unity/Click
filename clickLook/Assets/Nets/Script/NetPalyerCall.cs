//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2024-07-23 11:15:20
//=======================================================
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Sailfish
{
	public class NetPalyerCall : NetworkBehaviour
    {
        public UnityAction<string, string> callString;
        public UnityAction<string, float> callFloat;
        public UnityAction<string, int> callInt;


        [Command(ignoreAuthority = true)]
        public void CMDSetupPlayerString(string key, string value)
        {
            RpcSetupPlayerString(key, value);
        }

        [ClientRpc]
        public void RpcSetupPlayerString(string key, string value)
        {
            SyncPlayerString(key, value);
        }

        protected virtual void SyncPlayerString(string key, string value)
        {
            // 重写这个方法，处理同步字符串
        }



        [Command(ignoreAuthority = true)]
        public void CMDSetupPlayerFloat(string key, float value)
        {
            RpcSetupPlayerFloat(key, value);
        }

        [ClientRpc]
        public void RpcSetupPlayerFloat(string key, float value)
        {
            SyncPlayerFloat(key, value);
        }

        protected virtual void SyncPlayerFloat(string key, float value)
        {
            // 重写这个方法，处理同步字符串
        }


        [Command(ignoreAuthority = true)]
        public void CMDSetupPlayerInt(string key, int value)
        {
            RpcSetupPlayerInt(key, value);
        }

        [ClientRpc]
        public void RpcSetupPlayerInt( string key, int value)
        {
            SyncPlayerInt(key, value);
        }



        protected virtual void SyncPlayerInt(string key, int value)
        {
            // 重写这个方法，处理同步字符串
        }
    }
}
