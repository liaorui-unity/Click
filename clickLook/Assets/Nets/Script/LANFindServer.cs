//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：2024-07-23 15:45:26
//=======================================================
using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sailfish
{
	public class LANFindServer : MonoBehaviour 
	{
        public NetworkDiscovery networkDiscovery;

        public Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

#if UNITY_EDITOR
        void OnValidate()
        {
            if (networkDiscovery == null)
            {
                networkDiscovery = GetComponent<NetworkDiscovery>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
                UnityEditor.Undo.RecordObjects(new UnityEngine.Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
            }
        }
#endif
  

        public bool isHasServer()
        {
            return discoveredServers.Count > 0;
        }

        public void StartHost()
        {
            NetworkManager.singleton.StartHost();
            networkDiscovery?.AdvertiseServer();
        }


        public void FindServers()
        {
            discoveredServers.Clear();
            networkDiscovery?.StartDiscovery();
        }

        public void StopDiscovery()
        {
            networkDiscovery?.StopDiscovery();
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            //if(discoveredServers.TryGetValue(info.serverId, out ServerResponse server))
            {
                discoveredServers[info.serverId] = info;
                return;
            }
        }


        public void Connect()
        {
            foreach (var item in discoveredServers?.Values)
            {
                StopDiscovery();
                discoveredServers.Clear();
                NetworkManager.singleton.StartClient(item.uri);
                break;
            }
        }
    }
}
