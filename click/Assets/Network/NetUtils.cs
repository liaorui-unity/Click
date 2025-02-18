using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace fs
{
    public class NetUtils
    {
        /// <summary>
        /// udp协议开始标记
        /// </summary>
        public const string UDP_START_FLAG = "$&$";
        /// <summary>
        /// udp协议结束标记
        /// </summary>
        public const string UDP_END_FLAG = "$#$";

        /// <summary>
        /// 协议内容分割符号
        /// </summary>
        public const char MsgSplit = '|';

        /// <summary>
        /// 消息附带发送时间
        /// </summary>
        public const bool MsgAppendTime = false;
        
     
        /// <summary>
        /// 解析1位
        /// </summary>
        /// <returns></returns>
        public static string ParseOneBit(ref string msg)
        {
            int index = msg.IndexOf(NetUtils.MsgSplit);
            if (index <= 0)
            {//最后一位
                return msg;
            }
            string str = msg.Substring(0, index);
            msg = msg.Substring(index + NetUtils.MsgSplit.ToString().Length);
            return str;
        }

        /// <summary>
        /// 唯一网络识别id
        /// </summary>
        private static string m_DeviceGUID = string.Empty;
        public static string DeviceGUID
        {
            get
            {
                if (m_DeviceGUID == string.Empty)
                    m_DeviceGUID = UnityEngine.Random.Range(1, int.MaxValue).ToString();
                return m_DeviceGUID;
            }
            set
            {
                m_DeviceGUID = value;
            }
        }

        private static ByteArray tmpSendBy = new ByteArray(1024, 1024*1024);
        public static ByteArray AllocSendPacket()
        {
            tmpSendBy.Clear();
            tmpSendBy.WriteUShort(0);//协议头，包长度
            return tmpSendBy;
        }

        /// <summary>
        /// 获得本地ipv4
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIpv4()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            try
            {
                foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                    {
                        AddressIP = _IPAddress.ToString();
                        break;
                    }
                }
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                AddressIP = string.Empty;
            }
            return AddressIP;
        }

#if UNITY_STANDALONE
        /// <summary>
        /// 判断是否插入网线
        /// </summary>
        [DllImport("winInet.dll ")]
        private static extern bool InternetGetConnectedState(ref int dwFlag,int dwReserved);
        public static bool CheckNetState()
        {
            int dwFlag = 0;
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
#endif
    }

    /// <summary>
    /// 网络事件
    /// </summary>
    public class NetEventID
    {
        /// <summary>
        /// 网络连接断开
        /// </summary>
        public const string NET_DISCONNECT = "NET_DISCONNECT";
        /// <summary>
        /// 服务器连接成功
        /// </summary>
        public const string CONNECT_SUCCEED = "CONNECT_SUCCEED";
        /// <summary>
        /// 服务器连接失败
        /// </summary>
        public const string CONNECT_FAILED = "CONNECT_FAILED";     
    }
}