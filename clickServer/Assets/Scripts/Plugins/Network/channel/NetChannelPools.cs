using System;
using System.Collections.Generic;
using System.Text;

namespace fs
{
    /// <summary>
    /// 通道对象池
    /// @author hannibal
    /// @time 2016-5-24
    /// </summary>
    public class NetChannelPools
    {
        private static int m_total_new_count = 0;
        private static List<NetChannel> m_channel_pools = new List<NetChannel>();

        public static NetChannel Spawn()
        {
            NetChannel obj = null;
            if (m_channel_pools.Count > 0)
            {
                obj = m_channel_pools[m_channel_pools.Count - 1];
                m_channel_pools.RemoveAt(m_channel_pools.Count - 1);
                return obj;
            }
            else
            {
                ++m_total_new_count;
                obj = new NetChannel();
                return obj;
            }
        }
        public static void Despawn(NetChannel obj)
        {
            if (obj == null) return;
            if (!m_channel_pools.Contains(obj))
            {
                m_channel_pools.Add(obj);
            }
        }
        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("NetChannelPools使用情况:");
            st.AppendLine("New次数:" + m_total_new_count + " 空闲数量:" + m_channel_pools.Count);
            if (is_print) Debuger.Log(st.ToString());
            return st.ToString();
        }
    }
}