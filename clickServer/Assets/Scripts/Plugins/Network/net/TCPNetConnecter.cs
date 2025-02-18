using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

namespace fs
{
    /// <summary>
    /// 客户端socket
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public sealed class TCPNetConnecter : TCPNetBase
    {
        private long m_conn_idx = 0;
        private TCPClientSocket m_socket = null;
        private NetChannel m_channel = null;

        public TCPNetConnecter()
            : base()
        {
        }

        public override void Setup()
        {
            base.Setup();
        }

        public override void Destroy()
        {
            if (m_socket != null)
            {//socket只有外部调用时才主动关闭，否则底层会先自己关闭
                m_socket.Close();
                m_socket = null;
            }
            if (m_channel != null)
            {
                m_channel.Destroy();
                NetChannelPools.Despawn(m_channel);
                m_channel = null;
            }
            base.Destroy();
        }
        public override void Close()
        {
            HanldeCloseConnect();
            base.Close();
        }

        public override void Update()
        {
            //TODO:心跳

            base.Update();
        }

        public long Connect(string ip, ushort port, TCPNetBase.OnConnectedFunction connected, TCPNetBase.OnReceiveFunction receive, TCPNetBase.OnCloseFunction close)
        {
            OnConnected = connected;
            OnReceive = receive;
            OnClose = close;

            m_socket = new TCPClientSocket();
            m_socket.OnOpen += OnAcceptConnect;
            m_socket.OnMessage += OnMessageReveived;
            m_socket.OnClose += OnConnectClose;
            m_socket.Connect(ip, port);

            return m_conn_idx;
        }

        public override int Send(long conn_idx, ByteArray by)
        {
            base.Send(conn_idx, by);

            if (m_socket == null) return 0;

            if (by.Available >= SocketID.SendRecvMaxSize)
            {
                by.Skip(SocketID.PacketHeadSize);
                ushort header = by.ReadUShort();
                Debuger.LogError("发送数据量过大:" + header);
                return 0;
            }
            int data_len = by.Available - SocketID.PacketHeadSize;
            by.ModifyUShort((ushort)data_len, 0);

            m_socket.Send(by.GetBuffer(), 0, (int)by.Available);
            return (int)by.Available;
        }
        private void OnAcceptConnect(long conn_idx)
        {
            Debuger.Log("OnChannelConnect:" + conn_idx);
            lock (ThreadScheduler.instance.LogicLock)
            {
                m_channel = NetChannelPools.Spawn();
                m_channel.Setup(this, m_conn_idx);

                if (OnConnected != null) OnConnected(m_channel.conn_idx);
            }
        }
        private void OnMessageReveived(long conn_idx, byte[] by, int count)
        {
            lock (ThreadScheduler.instance.LogicLock)
            {
                m_channel.HandleReceive(by, count, AddPacket);
            }
        }
        private void OnConnectClose(long conn_idx)
        {
            Debuger.Log("OnChannelClose:" + conn_idx);
            lock (ThreadScheduler.instance.LogicLock)
            {
                this.Close();
            }
        }
        /// <summary>
        /// 关闭链接：底层通知
        /// </summary>
        private void HanldeCloseConnect()
        {
            if (m_channel != null)
            {
                m_channel.Destroy();
                NetChannelPools.Despawn(m_channel);
                m_channel = null;
            }
            if (OnClose != null)
            {
                OnClose(conn_idx);
                OnClose = null;
            }
        }

        public override bool Valid
        {
            get
            {
                if (m_socket == null) return false;
                return true;
            }
        }
        public long conn_idx
        {
            get { return m_conn_idx; }
            set { m_conn_idx = value; }
        }
    }
}