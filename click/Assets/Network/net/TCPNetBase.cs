using System;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// socket基类
    /// @author hannibal
    /// @time 2016-5-30
    /// </summary>
    public class TCPNetBase
    {
        public delegate void OnAcceptFunction(long conn_idx);
        public delegate void OnConnectedFunction(long conn_idx);
        public delegate void OnReceiveFunction(long conn_idx, PacketBase packet);
        public delegate void OnCloseFunction(long conn_idx);

        public TCPNetBase.OnAcceptFunction OnAccept = null;
        public TCPNetBase.OnConnectedFunction OnConnected = null;
        public TCPNetBase.OnReceiveFunction OnReceive = null;
        public TCPNetBase.OnCloseFunction OnClose = null;

        /// <summary>
        /// unity很多功能不能在多线程下执行，这里缓存多线程环境接收的数据，在主线程下执行
        /// </summary>
        private SafeList<KeyValuePair<long, PacketBase>> m_RecvPackets = new SafeList<KeyValuePair<long, PacketBase>>();

        public virtual void Setup()
        {

        }
        /// <summary>
        /// 外部调用，销毁socket
        /// </summary>
        public virtual void Destroy()
        {
            Close();
        }

        public virtual void Update()
        {
            //派发消息包
            for (int i = 0; i < m_RecvPackets.Count; ++i)
            {
                KeyValuePair<long, PacketBase> p = m_RecvPackets[i];
                try
                {
                    if (OnReceive != null) OnReceive(p.Key, p.Value);
                }
                catch (System.Exception e)
                {
                    Debuger.LogException(e);
                }
                PacketPools.Recover(p.Value);
            }
            m_RecvPackets.Clear();
        }
        protected void AddPacket(long conn_idx, ushort header, ByteArray data)
        {
            Debuger.Log("收到协议:" + header);
            PacketBase packet = PacketPools.Get(header);
            packet.Read(data);

            m_RecvPackets.Add(new KeyValuePair<long, PacketBase>(conn_idx, packet));
        }

        private static long send_count = 0;
        public virtual int Send(long conn_idx, ByteArray by)
        {
            ++send_count;
            return 0;
        }
        /// <summary>
        /// 内部调用或底层触发
        /// </summary>
        public virtual void Close()
        {
            OnReceive = null;
            OnClose = null;
        }
        public virtual bool Valid
        {
            get { return false; }
        }
    }
}