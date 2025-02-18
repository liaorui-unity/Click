using System;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 协议id
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class PacketID
    {
        //
        //	网络层保留的协议号区间
        //
        public const int PROTOCOL_RESERVED_LOW = 0;             //	net 保留的协议号，最小值
        public const int PROTOCOL_RESERVED_HIGH = 999;          //	net 保留的协议号，最大值
        public const int MSG_APPLAYER_BASE = 1000;              //	应用层协议起始号码
        public const int MSG_APPLAYER_PER_INTERVAL = 1000;      //  消息起始结束间隔


        //	内部id
        public const int MSG_BASE_INTERNAL = MSG_APPLAYER_BASE + 100;
        public const int MSG_BASE_C2S = MSG_APPLAYER_BASE + 1000;
        public const int MSG_BASE_S2C = MSG_APPLAYER_BASE + 5000;
    }
    /// <summary>
    /// 协议基类
    /// </summary>
    public class PacketBase
    {
        public ushort header { get; set; }

        public PacketBase()
        {
        }
        public virtual void Init()
        {
            //初始化数据：packet属于对象池，重用时需要把上次数据重置
        }
        public virtual void Read(ByteArray by)
        {
            //undo
        }
        public virtual void Write(ByteArray by)
        {
            by.WriteUShort(header);
        }
    }
    public class PackBaseC2S : PacketBase
    {
        public PackBaseC2S()
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
        }
    }
    public class PackBaseS2C : PacketBase
    {
        public PackBaseS2C()
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
        }
    }
}