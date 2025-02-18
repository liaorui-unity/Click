
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：udp发送数据
// 创建时间：2019-08-06 11:45:07
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public class UDPSendManager : Singleton<UDPSendManager>
	{
        private UDPSendSocket m_socket = null;

     
        
        /// <summary>
        /// 消息唯一码，自增长，用于校验消息的连续性
        /// </summary>
        private int m_msg_code_id = 0;

        public bool Init()
        {
            if (m_socket != null)
                return false;
            m_msg_code_id = 0;
            m_socket = new UDPSendSocket();
      
            bool result = m_socket.Init();
            if (!result)
            {
                this.Close();
                return false;
            }
            m_socket.OnClose += OnSocketClose;
            return true;
        }
        private void OnDestroy()
        {
            this.Close();
        }
        public void Close()
        {
            if(m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
        }
        private void OnSocketClose()
        {
            Debuger.Log("UDPNetSend 关闭");
            lock (ThreadScheduler.instance.LogicLock)
            {
                m_socket = null;
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">内容</param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="sync">是否同步方式</param>
        /// <returns></returns>
        public int Send(string msg, string ip, ushort port, bool sync =false)
        {
            if (m_socket == null)
            {
                this.Init();
            }
            if (m_socket == null) return 0;

            //增加机器id
            msg = this.AddHead(msg);
            //增加开始和结束标记
            msg = this.AddFlags(msg);

            //Debug.Log("send:" + msg);
            byte[] buf = Encoding.Default.GetBytes(msg);
            try
            {
                if (sync)
                    return m_socket.SendSync(buf, buf.Length, ip, port);
                else
                    return m_socket.SendAsync(buf, buf.Length, ip, port);
            }
            catch(System.Exception e)
            {
                Debuger.LogError(e.ToString());
                return 0;
            }
        }
        /// <summary>
        /// 增加协议头
        /// </summary>
        private string AddHead(string msg)
        {
            if(NetUtils.MsgAppendTime)
                msg = string.Format("{0}|{1}|{2}|{3}", NetUtils.DeviceGUID, ++m_msg_code_id, TimeUtils.MillisecondSince1970, msg);
            else
                msg = string.Format("{0}|{1}|{2}", NetUtils.DeviceGUID, ++m_msg_code_id, msg);
            return msg;
        }
        /// <summary>
        /// 添加标记，用于判断一个消息是否完整接收
        /// </summary>
        private string AddFlags(string msg)
        {
            return string.Format("{0}{1}{2}", NetUtils.UDP_START_FLAG, msg, NetUtils.UDP_END_FLAG);
        }
	}
}
