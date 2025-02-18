using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 客户端会话
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class ClientSession
    {
        /// <summary>
        /// 连接唯一id
        /// </summary>
        private long m_ConnID = 0;
        /// <summary>
        /// 状态
        /// </summary>
        private eSessionStatus m_SessionStatus;

        public void Setup(long conn_idx)
        {
            m_ConnID = conn_idx;
        }
        public void Destroy()
        {
            if (m_ConnID > 0)
            {
                this.OnLogout();
            }
            m_ConnID = 0;
        }
        
        /// <summary>
        /// 主动登出，或异常退出
        /// </summary>
        public void Logout()
        {
            if (m_SessionStatus == eSessionStatus.LOGOUTING) return;
            m_SessionStatus = eSessionStatus.LOGOUTING;
            if (m_ConnID > 0)
            {
                this.OnLogout();
            }
        }
        /// <summary>
        /// 被踢出
        /// </summary>
        public void Kickout()
        {
            if (m_SessionStatus == eSessionStatus.LOGOUTING) return;
            m_SessionStatus = eSessionStatus.LOGOUTING;

            //已经是正式连接的客户端，需要做后续清理工作
            if (m_ConnID > 0)
            {//需要清理
                this.OnLogout();
            }
        }
        /// <summary>
        /// 清理数据
        /// </summary>
        private void OnLogout()
        {
            m_ConnID = 0;
        }
        public long ConnID
        {
            get { return m_ConnID; }
        }

        public eSessionStatus SessionStatus
        {
            get { return m_SessionStatus; }
            set { m_SessionStatus = value; }
        }
    }

    public enum eSessionStatus
    {
        INVALID = 0,            // 非法值
        CREATED,                // 已创建
        LOGIN_DOING,            // 登录中
        ALREADY_LOGIN,          // 登录成功
        LOGIN_FAILED,           // 登录失败
        IN_GAMING,              // 游戏中
        DELAY_DISCONNECT,       // 延迟断线
        LOGOUTING,              // 登出中
    }
}