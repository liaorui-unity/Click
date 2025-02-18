//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2024-12-26 10:16:25
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
	public interface INetSyncMessage
	{
		string sign { get; set; }

		void Init();

		bool OnPreCMDEvent(ValueState state, string value);
		bool OnPreRPCEvent(ValueState state, string value);    
		void OnCallStateEvent (ValueState state, string value);
	}

    public  class NetSyncMessage : INetSyncMessage
    {
        public string sign { get; set; }
		public NetEventCall netEventCall;

 
        public virtual void Init(){}

        public virtual void RequestEvent(ValueState state, string value)
		{
			netEventCall.CMDCallStateEvent(sign,state,value);
		}

        
        /// <summary>
		/// 服务器调用代码，在服务器中执行
		/// </summary>
		/// <param name="key"></param>
		/// <param name="state"></param>
		/// <param name="value"></param> <summary>
        public virtual bool OnPreCMDEvent(ValueState state, string value)
        {
           return true;
        }

        /// <summary>
        /// 客户端调用代码，在客户端中执行
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <param name="value"></param>
        public virtual bool OnPreRPCEvent(ValueState state, string value)
        {
            return true;
        }

          /// <summary>
        /// 客户端调用代码，在客户端中执行
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <param name="value"></param>
		public virtual void OnCallStateEvent( ValueState state, string value)
		{
		   
		}
    }
}
