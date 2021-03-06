﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

class zzNetworkInfo
{
    public struct Info
    {
        public IPAddress IP;
        public IPAddress broadcastIP;

        public override string ToString()
        {
            return "IP:" + IP.ToString()+"\n"
                + "Broadcast IP:" + broadcastIP.ToString();
        }
    }

    static Byte[] tilde(Byte[] pByte)
    {
        Byte[] lOut = new Byte[pByte.Length];
        for (int i = 0; i < pByte.Length; ++i)
        {
            lOut[i] = (Byte)~pByte[i];
        }
        return lOut;
    }

    static Byte[] or(Byte[] pByte1, Byte[] pByte2)
    {
        if (pByte1.Length != pByte2.Length)
            return null;
        Byte[] lOut = new Byte[pByte1.Length];
        for (int i = 0; i < lOut.Length; ++i)
        {
            lOut[i] = (Byte)(pByte1[i] | pByte2[i]);
        }
        return lOut;
    }

    static IPAddress getBroadcastIP(IPAddress pIP, IPAddress pIPv4Mask)
    {
        var lBroadcast = or(pIP.GetAddressBytes(), tilde(pIPv4Mask.GetAddressBytes()));
        if (lBroadcast == null)
        {
            throw new Exception("pByte1.Length!=pByte2.Length.IP:" + pIP
                + " IPv4Mask:" + pIPv4Mask);
        }
        return new IPAddress(lBroadcast);
    }

    public static Info[]  getNetworkInfo()
    {
        //本地计算机上的网络接口的对象,我的电脑里面以太网网络连接有两个虚拟机的接口和一个本地接口
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        List<Info> lOut = new List<Info>();
        foreach (NetworkInterface adapter in nics)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                && adapter.Supports(NetworkInterfaceComponent.IPv4))
            {
                //Console.WriteLine("网络适配器名称：" + adapter.Name);
                //Console.WriteLine("网络适配器标识符：" + adapter.Id);
                //Console.WriteLine("适配器连接状态：" + adapter.OperationalStatus.ToString());

                //IP配置信息
                IPInterfaceProperties lIPProperties = adapter.GetIPProperties();     
                //if (ip.UnicastAddresses.Count > 0)
                foreach (var lUnicastAddresses in lIPProperties.UnicastAddresses)
                {
                    //Console.WriteLine("IP地址:" + ip.UnicastAddresses[0].Address.ToString());
                    //Console.WriteLine("子网掩码:" + ip.UnicastAddresses[0].IPv4Mask.ToString());
                    var lIPAddress = lUnicastAddresses.Address;
                    var lIPv4Mask = lUnicastAddresses.IPv4Mask;
                    var lIP = lUnicastAddresses.Address.GetAddressBytes();
                    if (lIPAddress.GetAddressBytes().Length > 4
                        || lIPv4Mask == null)
                        continue;
                    Info lInfo = new Info();
                    lInfo.IP = lUnicastAddresses.Address;
                    lInfo.broadcastIP = getBroadcastIP(lIPAddress, lIPv4Mask);

                    lOut.Add(lInfo);
                }

            }
        }

        return lOut.ToArray();
    }
}