using System;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

public class UdpObject
{
    public Socket socket;
    public byte[] buffer = new byte[1024];
    public IPEndPoint EP;
    public void CloseUpd()
    {
        if (socket != null)
        {
            socket.Close();
        }
    }
}

public class RadioReport
{
   

    public UdpObject RevUdp;
    public UdpObject SendUdp;
    public bool isClose = false;
    
   
    public  static bool isConnetcing= false;//是否允许连接，当为true时，当前有主机且允许连接//
    private static string LocalIP = "";//本地IP地址//
    public static string HostIP = "";//服务器IP地址//
    public static string GetLocalIP
    {
        get
        {
            if (LocalIP == "")
            {
                SetLocalIP();
            }
            return LocalIP;
        }
    }

    //获取到本机的IP地址//
    public static void SetLocalIP()
    {
        if (LocalIP == "")
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;
                    if (uniCast.Count > 0)
                    {
                        foreach (UnicastIPAddressInformation uni in uniCast)
                        {
                            // IPList.Add(uni.Address.ToString());
                            //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4
                            if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                LocalIP = uni.Address.ToString();
                            }
                        }
                    }
                }
            }
        }
    }

    //获取本机IP地址方法2//
    public static string SetLocalIP1()
    {
        string ip = "";
        var strHostName =Dns.GetHostName();
        var ipEntyr =Dns.GetHostEntry(strHostName);
        var addr = ipEntyr.AddressList;
        
        ip = addr[1].ToString();
        LocalIP = ip;
        //Debug.Log(LocalIP);
        return ip;
    }


    public void InitUpd(int port)
    {
        RevUdp=new UdpObject();
        SendUdp=new UdpObject();
        try
        {
            //string local_ip = "127.0.0.1";// GetLocalIP.Substring(0, GetLocalIP.LastIndexOf(".")) + ".255";
            //IPAddress localIP= IPAddress.Parse(local_ip);

            SendUdp.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SendUdp.EP = new IPEndPoint(IPAddress.Broadcast, port);
            //SendUdp.EP = new IPEndPoint(localIP, port);
            SendUdp.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            RevUdp.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            RevUdp.EP = new IPEndPoint(IPAddress.Any, port);
            //RevUdp.EP = new IPEndPoint(localIP, port);
            RevUdp.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            RevUdp.socket.Bind(RevUdp.EP);
            Thread receiveThread = new Thread(new ThreadStart(RadioToGetHostIP));
            receiveThread.Start();

            //IPAddress groupAddress = IPAddress.Parse("239.0.84.11");
            //SendUdp.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //SendUdp.EP = new IPEndPoint(IPAddress.Broadcast, port);
            //SendUdp.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            //MulticastOption multicastOption = new MulticastOption(groupAddress);
            //SendUdp.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);

            //RevUdp.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //RevUdp.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            //RevUdp.EP = new IPEndPoint(IPAddress.Any, port);
            //RevUdp.socket.Bind(RevUdp.EP);
            //// MulticastOption multicastOption = new MulticastOption(groupAddress);
            //RevUdp.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);
            //Thread receiveThread = new Thread(new ThreadStart(RadioToGetHostIP));
            //receiveThread.Start();
        }
        catch (Exception e)
        {
            
            Debug.LogError("Udp通讯失败！！！"+e.Message);
        }
        

    }

  
 

    //当游戏退出，关闭线程//
    public  void Close()
    {
        RevUdp.CloseUpd();
        SendUdp.CloseUpd();
        isClose = true;
    }


    public  void RadioToSendOnce(RadioMsg _messenger)
    {
        try
        {
            SendUdp.buffer= CommonTools.Serialize<RadioMsg>(_messenger);
            SendUdp.socket.SendTo(SendUdp.buffer,SendUdp.EP);
            //Debug.Log("发出的主机IP： " + _messenger.hostIP+"  发送到的客户端IP："+_messenger.key);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void RadioToSendMore(RadioMsg value) 
    {
        int index = 0;
        while (index < 5) 
        {
            RadioToSendOnce(value);
            index++;
        }
    }

    //广播收到信息获取IP//
    private void RadioToGetHostIP()
    {
        while (!isClose)
        {
            try
            {
                EndPoint ep =RevUdp.EP; //获取发送广播主机的确切通信ip
                //Debug.Log("EP: "+ep);
                byte[] data = new byte[1024];
                int recv = RevUdp.socket.ReceiveFrom(data, ref ep); //同步调用，此处会被阻塞.
                //string stringData = Encoding.Default.GetString(data, 0, recv);
                // ReceiveSocket.ReceiveFrom(data);
                RadioMsg msg = CommonTools.Deserialize<RadioMsg>(data);
                if (msg != null)
                {
                    OnHandleMessage(msg,msg.key);

                    Debug.Log("收到的消息： " + msg.key);
                }
                
               
            }
            catch (Exception)
            {
                //Debug.Log("没有接收到信息");
            }
        }
    }
    
    //处理接收到的信息//

    private void OnHandleMessage(RadioMsg msg,string key)
    {
        switch (msg.state)
        {
            //收到获取主机IP的信息，当是主机时，发送主机IP的信息给客户端//
            case "_Get":
                if (isConnetcing)
                {
                    RadioToSendMore(new RadioMsg("_Set",GetLocalIP,key));
                }
                break;
            //收到主机IP信息，设置要连接的主机IP//
            case "_Set":
                if (key == GetLocalIP)
                {
                    RadioReport.HostIP = msg.hostIP;
                }
                break;

        }
    }

    public void OnStartToGetHost()
    {
        try
        {
            RadioToSendMore(new RadioMsg());
        }
        catch (Exception e)
        {
            
        }
    }
}

[Serializable]
public class RadioMsg
{
    public string state="";//判断操作符，根据所发信息的不同执行不同的操作//
    public string hostIP = "";//主机IP//
    public string key= "";//客户端IP,当客户端收到IP等于自身IP时，才执行方法//

    public RadioMsg()
    {
        state = "_Get";
        hostIP = "";
        key = RadioReport.GetLocalIP;
    }

    public RadioMsg(string state,string hostIP,string key)
    {
        this.state = state;
        this.hostIP = hostIP;
        this.key=key;
    }
}



[Serializable]
public class IPMessage
{
    
    public string IP;//本机IP地址//

    public IPMessage(string IP)
    {
        this.IP = IP;
    }

}


public class CommonTools
{
    /// <summary>
    /// 序列化class为byte[]
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static byte[] Serialize<T>(T obj) where T : class
    {
        IFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, obj);  //序列化到一个内存流中，不生成物理文件。
        stream.Position = 0;
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);  //以byte[]形式读取stream中数据。
        stream.Flush();
        stream.Close();
        return buffer;
    }

    /// <summary>
    /// 序列化byte[]为class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newT"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static T Deserialize<T>(byte[] buffer) where T : class
    {
        T newT = null;
        IFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(buffer);
        newT = (T)formatter.Deserialize(stream);

        stream.Flush();
        stream.Close();

        return newT;
    }
}