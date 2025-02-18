using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 会话管理器
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class ClientSessionManager : Singleton<ClientSessionManager>
    {
        private Dictionary<long, float> m_DicAcceptSession = new Dictionary<long, float>();
        private Dictionary<long, ClientSession> m_DicSession = new Dictionary<long, ClientSession>();

        private float m_CurTime = 0;

        public void Setup()
        {

        }
        public void Destroy()
        {
        }

        public void Update()
        {
            m_CurTime = Time.realtimeSinceStartup;
            this.TickKickout();
        }
        /// <summary>
        /// 踢掉无效链接
        /// </summary>
        private void TickKickout()
        {
            foreach (var obj in m_DicAcceptSession)
            {
                if (Time.realtimeSinceStartup - obj.Value >= 5)//5秒后还没有发握手协议，直接踢掉，防止占线
                {
                    KickoutSession(obj.Key);
                    break;
                }
            }
        }
        public bool AddAcceptSession(long conn_idx)
        {
            if (m_DicAcceptSession.ContainsKey(conn_idx))
                return false;
            m_DicAcceptSession.Add(conn_idx, m_CurTime);
            return true;
        }
        public bool CleanupAcceptSession(long conn_idx)
        {
            if (!m_DicAcceptSession.ContainsKey(conn_idx))
                return false;
            m_DicAcceptSession.Remove(conn_idx);
            return true;
        }
        public bool HasAcceptSession(long conn_idx)
        {
            if (!m_DicAcceptSession.ContainsKey(conn_idx))
                return false;
            return true;
        }
        public uint GetAcceptSessionCount()
        {
            return (uint)m_DicAcceptSession.Count;
        }

        public ClientSession AddSession(long conn_idx)
        {
            if (!m_DicAcceptSession.ContainsKey(conn_idx))
                return null;

            ClientSession session = new ClientSession();
            session.Setup(conn_idx);
            m_DicSession.Add(conn_idx, session);
            return session;
        }
        public bool CleanupSession(long conn_idx)
        {
            CleanupAcceptSession(conn_idx);

            ClientSession session;
            if (m_DicSession.TryGetValue(conn_idx, out session))
            {
                session.Destroy();
            }
            m_DicSession.Remove(conn_idx);
            return true;
        }
        public void CloseAllSession()
        {
            foreach (var obj in m_DicSession)
            {
                obj.Value.Destroy();
            }
            m_DicSession.Clear();
        }
        public ClientSession GetSession(long conn_idx)
        {
            ClientSession session;
            if (m_DicSession.TryGetValue(conn_idx, out session))
            {
                return session;
            }
            return null;
        }
        public uint GetClientSessionCount()
        {
            return (uint)m_DicSession.Count;
        }
        /// <summary>
        /// 连接是否已满
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedFull()
        {
            if (GetClientSessionCount() + GetAcceptSessionCount() >= 6)
                return true;
            return false;
        }
        /// <summary>
        /// 踢号
        /// </summary>
        public void KickoutSession(long conn_idx)
        {
            ClientSession session;
            if (m_DicSession.TryGetValue(conn_idx, out session))
            {
                session.Kickout();
            }
            CleanupSession(conn_idx);
            //TODO
            ///ServerNetManager.instance.CloseConn(conn_idx);
        }
    }
}