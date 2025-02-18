//=======================================================
// 作者：BrotherChen 
// 公司：广州纷享科技发展有限公司
// 描述：UDP发送测试
// 创建时间：2020-07-09 17:41:16
//=======================================================
using System;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace fs
{
	public class UDPSend
	{
        public UdpObject SendUdp;

        public UDPSend()
        {
            if (SendUdp == null)
            {
                SendUdp = new UdpObject();
            }
        }

        public event OnConnectClose OnClose;

        public void Close()
        {
            SendUdp.CloseUpd();
        }
        public bool Init(ushort port)
        {
            try
            {
                SendUdp.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                SendUdp.EP = new IPEndPoint(IPAddress.Broadcast,port);
                //IPAddress ip = IPAddress.Parse(Dns.GetHostAddresses("localhost")[0].ToString());
                //SendUdp.EP = new IPEndPoint(ip, 50002);
                SendUdp.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                return true;
            }
            catch
            {
                return false;
            } 
        }

        public int SendAsync(byte[] buf, int len, string ip, ushort port)
        {
            try
            {
                //SendUdp.socket.SendTo(buf,len,SocketFlags.Broadcast,SendUdp.EP);
                SendUdp.socket.SendTo(buf, SendUdp.EP);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        public int SendSync(byte[] buf, int len, string ip, ushort port)
        {
            //SendUdp.socket.SendTo(buf, len, SocketFlags.Broadcast, SendUdp.EP);
            SendUdp.socket.SendTo(buf, SendUdp.EP);
            return 0;
        }

        public delegate void OnConnectClose();
    }
}
