using System.Collections;
using System.Collections.Generic;
using fs;
using UnityEngine;

public class TextureInfo : PacketBase
{
  
    public string m_Data;

    public override void Read(ByteArray by)
    {
        header = by.ReadUShort();
        m_Data = by.ReadString();
            //undo


    }
    public override void Write(ByteArray by)
    {
        base.Write(by);
        by.WriteString(m_Data);
    }
}
