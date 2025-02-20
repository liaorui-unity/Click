using System.Collections;
using System.Collections.Generic;
using fs;
using UnityEngine;

[PacketAttribute(Id = 1003)]
public class MousePackage : PacketBase
{
    public int x;
    public int y;
  
    public override void Read(ByteArray by)
    {
        base.Read(by);
        x = by.ReadInt();
        y = by.ReadInt();
    }
    public override void Write(ByteArray by)
    {
        base.Write(by);
        by.WriteInt(x);
        by.WriteInt(y);
    }
}

[PacketAttribute(Id = 1002)]
public class FunctionPackage : PacketBase
{
    public string type;
  
    public override void Read(ByteArray by)
    {
        base.Read(by);
        type   = by.ReadString();
    }
    public override void Write(ByteArray by)
    {
        base.Write(by);
        by.WriteString(type);
    }
}


[PacketAttribute(Id = 1001)]
public class TexturePacket :PacketBase
{
    public string type;

    public int width;

    public int height;
    public int length;
    public byte[] data;

    public override void Read(ByteArray by)
    {
        base.Read(by);
        type   = by.ReadString();
        width  = by.ReadInt();
        height = by.ReadInt();
        length = by.ReadInt();
        
        Debug.Log("发送图片数据:"+length);
        data = new byte[length];
        by.Read(ref data,length);
    }

    public override void Write(ByteArray by)
    {
        base.Write(by);
        by.WriteString(type);
        by.WriteInt(width);
        by.WriteInt(height);
        by.WriteInt(data.Length);

        Debug.Log("发送图片数据:"+data.Length);
        by.WriteBytes(data,data.Length);
    }
}