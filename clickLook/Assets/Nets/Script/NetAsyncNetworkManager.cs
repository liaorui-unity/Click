using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Sailfish
{
    public class NetAsyncNetworkManager : NetworkManager
    {
        public static void InitNetwork()
        {
            Instantiate(Resources.Load("NetworkManager"));
        }

        GameObject goManager;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (goManager == null)
            {
                goManager = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Manager"));
                NetworkServer.Spawn(goManager);
            }

            var massage    = conn.authenticationData.ToString();
            var tempPlayer = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "NetPlayer"));
            var lanPlayer  = tempPlayer.GetComponent<NetPlayer>();
            lanPlayer.messge = massage;
        
           NetworkServer.AddPlayerForConnection(conn, tempPlayer);
        }



        public string GetGroup(string messge)
        {
                Match match = Regex.Match(messge, @"\[(\d)\]Group_\[(\d)\]Machine");

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return string.Empty;
        }
    }
}

