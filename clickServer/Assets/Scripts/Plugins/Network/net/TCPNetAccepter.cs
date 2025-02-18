using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;  
using System;

namespace fs
{
    /// <summary>
    /// 服务端socket
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public sealed class TCPNetAccepter : TCPNetBase
    {
        private TCPServerSocket m_socket = null;
        private Dictionary<long, NetChannel> m_channels = new Dictionary<long, NetChannel>();

        private ByteArray m_send_by = null;

        public TCPNetAccepter()
            : base()
        {
            m_send_by = NetUtils.AllocSendPacket();
        }
        
        public override void Setup()
        {
            base.Setup();
            ClientSessionManager.instance.Setup();
        }

        public override void Destroy()
        {
            ClientSessionManager.instance.Destroy();
            base.Destroy();
        }
        public override void Close()
        {
            if (m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
            //需要放m_socket.Close()后，socket关闭时，内部回调的关闭事件HanldeCloseConnect不能正常执行
            foreach (var obj in m_channels)
            {
                obj.Value.Destroy();
                NetChannelPools.Despawn(obj.Value);
            }
            m_channels.Clear();
            base.Close();
        }
        public void CloseConn(long conn_idx)
        {
            if (m_socket != null && conn_idx > 0)
            {
                m_socket.CloseConn(conn_idx);
            }
        }
        public override void Update()
        {
            base.Update();
            ClientSessionManager.instance.Update();
        }

        public bool Listen(ushort port, TCPNetBase.OnAcceptFunction accept, TCPNetBase.OnReceiveFunction receive, TCPNetBase.OnCloseFunction close)
        {
            if(m_socket != null)
            {
                Debuger.LogError("服务器已经开启过");
                return false;
            }
            OnAccept = accept;
            OnReceive = receive;
            OnClose = close;

            m_socket = new TCPServerSocket();
            m_socket.OnOpen += OnAcceptConnect;
            m_socket.OnMessage += OnMessageReveived;
            m_socket.OnClose += OnConnectClose;
            m_socket.Start(port);
            return true;
        }

        public int Send(long conn_idx, PacketBase packet)
        {
            Debuger.Log("发送协议:" + packet.header);

            m_send_by.Clear();
            m_send_by.WriteUShort(0);//先写入长度占位
            packet.Write(m_send_by);
            int size = this.Send(conn_idx, m_send_by);
            PacketPools.Recover(packet);
            return size;
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

            m_socket.Send(conn_idx, by.GetBuffer(), 0, (int)by.Available);
            return (int)by.Available;
        }
        private void OnAcceptConnect(long conn_idx)
        {
            Debuger.Log("OnChannelAccept:" + conn_idx);
            lock (ThreadScheduler.instance.LogicLock)
            {
                if (ClientSessionManager.instance.IsConnectedFull())
                {
                    Debuger.LogWarning("连接已满");
                    this.CloseConn(conn_idx);
                    return;
                }

                NetChannel channel = NetChannelPools.Spawn();
                channel.Setup(this, conn_idx);
                m_channels.Add(channel.conn_idx, channel);
                
                ClientSessionManager.instance.AddAcceptSession(conn_idx);

                if (OnAccept != null) OnAccept(channel.conn_idx);
            }
        }
        private void OnMessageReveived(long conn_idx, byte[] by, int count)
        {
            lock (ThreadScheduler.instance.LogicLock)
            {
                NetChannel channel;
                if (m_channels.TryGetValue(conn_idx, out channel))
                {
                    channel.HandleReceive(by, count, AddPacket);
                }
            }
        }
        private void OnConnectClose(long conn_idx)
        {
            Debuger.Log("OnChannelClose:" + conn_idx);
            lock (ThreadScheduler.instance.LogicLock)
            {
                ClientSession session = ClientSessionManager.instance.GetSession(conn_idx);
                if (session != null)
                {
                    session.Logout();
                }
                ClientSessionManager.instance.CleanupSession(conn_idx);
                this.HanldeCloseConnect(conn_idx);
            }
        }
        /// <summary>
        /// 关闭链接：底层通知
        /// </summary>
        private void HanldeCloseConnect(long conn_idx)
        {
            NetChannel channel;
            if (m_channels.TryGetValue(conn_idx, out channel))
            {
                channel.Destroy();
                NetChannelPools.Despawn(channel);
            }
            m_channels.Remove(conn_idx);
            if (OnClose != null)
            {
                OnClose(conn_idx);
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
    }
}