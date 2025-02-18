using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fs
{
    /// <summary>
    /// 链接socket管理器
    /// @author hannibal
    /// @time 2016-8-17
    /// </summary>
    public class NetConnectManager : Singleton<NetConnectManager>
    {
        private long m_share_conn_idx = 0;
        private Dictionary<long, TCPNetConnecter> m_connectedes = null;

        private ByteArray m_send_by = null;

        public NetConnectManager()
        {
            m_connectedes = new Dictionary<long, TCPNetConnecter>();
            m_send_by = NetUtils.AllocSendPacket();
        }
        
        public void Update()
        {
            foreach (var obj in m_connectedes)
            {
                obj.Value.Update();
            }
            foreach (var obj in m_connectedes)
            {
                if (!obj.Value.Valid)//底层已经销毁
                {
                    obj.Value.Destroy();
                    m_connectedes.Remove(obj.Key);
                    break;
                }
            }
        }
        /// <summary>
        /// 连接主机
        /// </summary>
        public long ConnectTo(string ip, ushort port, TCPNetBase.OnConnectedFunction connected, TCPNetBase.OnReceiveFunction receive, TCPNetBase.OnCloseFunction close)
        {
            TCPNetConnecter socket = new TCPNetConnecter();
            socket.Setup();
            socket.conn_idx = ++m_share_conn_idx;
            m_connectedes.Add(socket.conn_idx, socket);
            socket.Connect(ip, port, connected, receive, close);
            return socket.conn_idx;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close(long conn_idx)
        {
            TCPNetConnecter socket;
            if (m_connectedes.TryGetValue(conn_idx, out socket))
            {
                socket.Destroy();
            }
            m_connectedes.Remove(conn_idx);
        }
        public void CloseAll()
        {
            foreach (var obj in m_connectedes)
                obj.Value.Destroy();
            m_connectedes.Clear();
        }

        /// <summary>
        /// 发消息
        /// </summary>
        public int Send(long conn_idx, PacketBase packet)
        {
            Debuger.Log("发送协议:" + packet.header);
            m_send_by.Clear();
            m_send_by.WriteUShort(0);//先写入长度占位
            packet.Write(m_send_by);
            int len = this.Send(conn_idx, m_send_by);
            PacketPools.Recover(packet);
            return len;
        }
        private int Send(long conn_idx, ByteArray by)
        {
            TCPNetConnecter socket;
            if (m_connectedes.TryGetValue(conn_idx, out socket))
            {
                return socket.Send(conn_idx, by);
            }
            return 0;
        }
    }
}