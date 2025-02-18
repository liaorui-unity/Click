
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：udp接收数据
// 创建时间：2019-08-06 11:45:07
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    public class UDPRecvManager : MonoSingleton<UDPRecvManager>
    {
        /// <summary>
        /// bind端口
        /// </summary>
        private ushort m_port = 0;

        private UDPRecvSocket m_socket = null;

      
        /// <summary>
        /// 接收到的数据
        /// </summary>
        private string m_recv_message = string.Empty;
        /// <summary>
        /// 接收协议回调
        /// </summary>
        private System.Action<string, string> m_receive_fun = null;

        /// <summary>
        /// 保存每台机器的消息码，用于校验是否有丢包
        /// </summary>
        private Dictionary<string, int> m_machine_code = new Dictionary<string, int>();

        /// <summary>
        /// 发送者是否接受广播消息
        /// </summary>
        private bool m_myself_receive = false;

        public bool Init(ushort port, System.Action<string, string> receive_fun)
        {
            Debuger.Log("UDPRecvManager 初始化端口:" + port);
            if (m_socket != null)
                return false;

            m_port = port;
            m_receive_fun = receive_fun;
            m_recv_message = string.Empty;
            m_machine_code.Clear();

            m_socket = new UDPRecvSocket();
          

            bool result = m_socket.Init(port);
            if (!result)
            {
                this.Close();
                return false;
            }
            m_socket.OnClose += OnSocketClose;
            m_socket.OnMessage += OnReceive;
            return true;
        }
        public void Close()
        {
            Debuger.Log("UDPRecvManager 关闭");
            if (m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
        }
        private void OnDestroy()
        {
            this.Close();
        }
        private void Update()
        {
            lock (ThreadScheduler.instance.LogicLock)
            {
                this.ParseMessage();
            }
        }
        private void OnSocketClose()
        {
            lock (ThreadScheduler.instance.LogicLock)
            {
                this.Close();
                //自动重新初始化
                this.Init(m_port, m_receive_fun);
            }
        }
        private void OnReceive(byte[] buf, int len)
        {
            string msg = Encoding.Default.GetString(buf, 0, len);
            //Debug.LogWarning("收到数据:" + msg);
            lock(ThreadScheduler.instance.LogicLock)
            {
                m_recv_message += msg;
            }
        }
        private void ParseMessage()
        {
            if (string.IsNullOrEmpty(m_recv_message))
                return;

            //没有接收完全
            if (!(m_recv_message.IndexOf(NetUtils.UDP_START_FLAG) >= 0 && m_recv_message.IndexOf(NetUtils.UDP_END_FLAG) >= 0))
                return;

            int start_index = m_recv_message.IndexOf(NetUtils.UDP_START_FLAG);
            int end_index = m_recv_message.IndexOf(NetUtils.UDP_END_FLAG);
            if(end_index < start_index)
            {//数据异常
                Debuger.LogWarning("出现异常数据:" + m_recv_message);
                //定位到正常数据
                m_recv_message = m_recv_message.Substring(0, start_index);
                this.ParseMessage();
                return;
            }

            //提取一个协议内容
            string msg = m_recv_message.Substring(start_index + NetUtils.UDP_START_FLAG.Length, end_index - start_index - NetUtils.UDP_START_FLAG.Length);
            this.ParseOneMessage(msg);

            //去掉已经处理的数据，重新解析
            m_recv_message = m_recv_message.Substring(end_index + NetUtils.UDP_END_FLAG.Length);
            this.ParseMessage();
        }
        /// <summary>
        /// 解析单个消息
        /// </summary>
        private void ParseOneMessage(string message)
        {
            //Debuger.Log("recv:" + message);

            string msg = message;
            //第1位：机台guid
            string send_machine_guid = NetUtils.ParseOneBit(ref msg);
            //第2位：消息码
            string str = NetUtils.ParseOneBit(ref msg);
            int msg_code = str.ToInt();
            bool result = this.CheckMsgCodeValid(send_machine_guid, msg_code);
            if (!result)
            {
                //Debuger.LogWarning(string.Format("消息码不连续:{0}/{1},msg:{2}", m_machine_code[send_machine_guid], msg_code, message));
                //重新纠正
                m_machine_code[send_machine_guid] = msg_code;
            }
            //是否有带时间
            if(NetUtils.MsgAppendTime)
            {
                str = NetUtils.ParseOneBit(ref msg);
                long send_time = str.ToLong();
                Debuger.Log("消息延迟时间:" + (TimeUtils.MillisecondSince1970 - send_time));
            }

            //派发消息
            try
            {
                //是发送者自己
                //if (!m_myself_receive && send_machine_guid == NetUtils.DeviceGUID)
                //    return;

                if (m_receive_fun != null) m_receive_fun.Invoke(send_machine_guid, msg);
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
            }
        }
        
        /// <summary>
        /// 验证消息码是否正确
        /// </summary>
        /// <param name="machine_id"></param>
        /// <param name="msg_id"></param>
        /// <returns></returns>
        private bool CheckMsgCodeValid(string machine_id, int msg_code)
        {
            if (!m_machine_code.ContainsKey(machine_id))
            {
                m_machine_code.Add(machine_id, msg_code);
                return true;
            }

            int old_msg_code = m_machine_code[machine_id];
            if (old_msg_code + 1 != msg_code)
            {
                return false;
            }

            m_machine_code[machine_id] = msg_code;
            return true;
        }
    }
}
