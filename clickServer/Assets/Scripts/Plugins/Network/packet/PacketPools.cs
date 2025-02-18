using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace fs
{
    /// packet特性
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class PacketAttribute : System.Attribute
    {
        /// <summary>
        /// 协议id
        /// </summary>
        public ushort Id = 0;
    }

    class PacketInfo
    {
        /// <summary>
        /// 协议id
        /// </summary>
        public ushort Id = 0;
        /// <summary>
        /// 类型
        /// </summary>
        public System.Type Type;
    }

    /// <summary>
    /// 协议对象池
    /// @author hannibal
    /// @time 2016-8-12
    /// </summary>
    public class PacketPools
    {
        /// <summary>
        /// 反射出的协议信息
        /// </summary>
        private static Dictionary<ushort, PacketInfo> m_packet_infos = new Dictionary<ushort, PacketInfo>();
        /// <summary>
        /// 对象池
        /// </summary>
        private static Dictionary<ushort, List<PacketBase>> m_packet_pools = new Dictionary<ushort, List<PacketBase>>();
#if DEBUG
        private static Dictionary<ushort, long> m_new_count = new Dictionary<ushort, long>();
#endif
        public static PacketBase Get(ushort id)
        {
            PacketBase packet = null;
            List<PacketBase> list;
            if (m_packet_pools.TryGetValue(id, out list) && list.Count > 0)
            {
                packet = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
            }
            else
            {
                PacketInfo packet_info = GetPacketInfo(id);
                if(packet_info == null)
                {
                    Debuger.LogError("PacketPools - 没有找到协议对应的类信息，请在类{0}添加特性PacketAttribute");
                    return null;
                }
                packet = ReflectionUtils.CreateInstance<PacketBase>(packet_info.Type);
                if(packet == null)
                {
                    Debuger.LogError("PacketPools - 创建协议失败:" + packet_info.Type.ToString());
                    return null;
                }
#if DEBUG
                //统计分配次数
                long count = 0;
                if (!m_new_count.TryGetValue(id, out count))
                    m_new_count.Add(id, 1);
                else
                    m_new_count[id] = ++count;
#endif
            }
            packet.Init();
            packet.header = id;
            return packet;
        }
        public static void Recover(PacketBase packet)
        {
            ushort id = packet.header;
            List<PacketBase> list;
            if (!m_packet_pools.TryGetValue(id, out list))
            {
                list = new List<PacketBase>();
                m_packet_pools.Add(id, list);
            }
            if (!list.Contains(packet)) list.Add(packet);
        }
        
        public static string ToString(bool is_print)
        {
#if DEBUG
            System.Text.StringBuilder st = new System.Text.StringBuilder();
            st.AppendLine("PacketPools使用情况:");
            foreach (var obj in m_new_count)
            {
                ushort id = obj.Key;
                string one_line = id + " New次数:" + obj.Value;
                List<PacketBase> list;
                if (m_packet_pools.TryGetValue(id, out list))
                {
                    one_line += " 空闲数量:" + list.Count;
                }
                st.AppendLine(one_line);
            }
            if (is_print) Debuger.Log(st.ToString());
            return st.ToString();
#else
            return string.Empty;
#endif
        }

        #region 协议信息
        private static void AddPacketInfo(PacketInfo info)
        {
            if (m_packet_infos.ContainsKey(info.Id))
            {
                Debuger.LogError("PacketPools::AddPacketInfo - same id is register:" + info.Id.ToString());
                return;
            }

            m_packet_infos.Add(info.Id, info);
        }
        private static PacketInfo GetPacketInfo(ushort id)
        {
            PacketInfo info = null;
            if (!m_packet_infos.TryGetValue(id, out info))
            {//可能没有提取特性
                ExtractAttribute(ReflectionUtils.GetCSharpAssembly());
                if (!m_packet_infos.TryGetValue(id, out info))
                {
                    return null;
                }
            }
            return info;
        }
        private static bool HasPacketInfo(ushort id)
        {
            return m_packet_infos.ContainsKey(id);
        }
        /// <summary>
        /// 提取特性
        /// </summary>
        private static void ExtractAttribute(System.Reflection.Assembly assembly)
        {
            //float start_time = Time.realtimeSinceStartup;
            //外部程序集
            List<System.Type> types = AttributeUtils.FindType<PacketBase>(assembly, false);
            if (types != null)
            {
                foreach (System.Type type in types)
                {
                    PacketAttribute attr = AttributeUtils.GetClassAttribute<PacketAttribute>(type);
                    if (attr == null) continue;
                    
                    AddPacketInfo(new PacketInfo() { Id = attr.Id, Type = type});
                }
            }
            //Debuger.Log("PacketPools:ExtractAttribute 提取特性用时:" + (Time.realtimeSinceStartup - start_time));
        }
#endregion
    }
}