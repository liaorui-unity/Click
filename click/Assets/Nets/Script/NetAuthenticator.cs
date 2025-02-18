//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2024-07-23 09:04:02
//=======================================================
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{


    public class AuthRequestMessage : MessageBase
    {
        public string message;
    }

    public class AuthResponseMessage : MessageBase
    {
        public byte code;
        public string message;
    }

    public class NetAuthenticator : NetworkAuthenticator
    {

        public override void OnStartServer()
        {
            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        public override void OnStartClient()
        {
            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }



        public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
        {
            AuthResponseMessage authResponseMessage = new AuthResponseMessage
            {
                code = 100,
                message = msg.message
            };

            conn.authenticationData = msg.message;
            conn.Send(authResponseMessage);
            OnServerAuthenticated.Invoke(conn);
        }



        public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                OnClientAuthenticated.Invoke(conn);
            }
        }

        public override void OnServerAuthenticate(NetworkConnection conn)
        {
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                message = JsonUtility.ToJson(LANManager.instance.selfInfo)
            };
            conn.Send(authRequestMessage);
        }

        public override void OnClientAuthenticate(NetworkConnection conn)
        {
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                message = JsonUtility.ToJson(LANManager.instance.selfInfo)
            };
            conn.Send(authRequestMessage);
        }
    }
}
