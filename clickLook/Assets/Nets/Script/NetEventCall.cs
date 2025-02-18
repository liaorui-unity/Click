//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2024-12-20 10:02:49
//=======================================================
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
	
	   public enum ValueState
	   {
		 Add,
		 Remove,
		 Update,

		 UseMain,
         Replace
	   }
	public class NetEventCall : NetworkBehaviour 
	{
		


        [Command(ignoreAuthority = true)]
		public void CMDCallStateEvent(string key,  ValueState  state , string value)
		{
			if(PreCMDEvent(key, state, value))
			   RPCCallStateEvent(key, state, value); 
		}

        public virtual bool PreCMDEvent(string key,  ValueState  state , string value)
		{
			RPCCallStateEvent(key, state, value); 
            return false;
		}

		[ClientRpc]
		public void RPCCallStateEvent(string key,  ValueState  state , string value)
		{	
			if(PreRPCEvent(key, state, value))
			   CallStateEvent(key, state, value);
		}

		public virtual bool PreRPCEvent(string key,  ValueState  state , string value)
		{
			CallStateEvent(key, state, value);
            return false;
		}

        public virtual void CallStateEvent(string key,  ValueState  state , string value)
		{

		}

	}
}
