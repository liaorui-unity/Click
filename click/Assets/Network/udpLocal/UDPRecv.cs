//=======================================================
// 作者：BrotherChen 
// 公司：广州纷享科技发展有限公司
// 描述：UDP接收测试
// 创建时间：2020-07-09 17:41:40
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
	public class UDPRecv 
	{
        public UdpObject RevUdp;

        private object m_sync_lock = new object();

        private bool isClose=false;

        public UDPRecv()
        {
            if (RevUdp == null)
            {
                RevUdp = new UdpObject();
            }
        }

        public event OnConnectClose OnClose;
        public event OnReceiveData OnMessage;

        public void Close()
        {
            isClose = true;
            RevUdp.CloseUpd();
        }
        public bool Init(ushort port)
        {
            try
            {
                RevUdp.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                RevUdp.EP = new IPEndPoint(IPAddress.Any, port);
                //IPAddress ip = IPAddress.Parse(NetUtils.GetLocalIpv4());
                //RevUdp.EP = new IPEndPoint(ip, port);
                RevUdp.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                RevUdp.socket.Bind(RevUdp.EP);
                Thread receiveThread = new Thread(new ThreadStart(OnReceive));
                receiveThread.Start();

                return true;
            }
            catch
            {
                return false;
            }
           
        }

        public delegate void OnConnectClose();
        public delegate void OnReceiveData(byte[] buf, int len);

        //广播收到信息获取IP//
        private void OnReceive()
        {
            while (!isClose)
            {
                try
                {
                    EndPoint ep = RevUdp.EP; //获取发送广播主机的确切通信ip
                                             //Debug.Log("EP: "+ep);
                    byte[] data = new byte[1024];
                    int recv = RevUdp.socket.ReceiveFrom(data, ref ep); //同步调用，此处会被阻塞.
                                                                        //string stringData = Encoding.Default.GetString(data, 0, recv);
                   if(OnMessage!=null)                                                     // ReceiveSocket.ReceiveFrom(data);
                    OnMessage(data, recv);
                   
                }
                catch (Exception)
                {
                    //Debug.Log("没有接收到信息");
                }
                

            }
        }
    }
}
