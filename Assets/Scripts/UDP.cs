using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


struct Data
{
    public float x;
    public float y;
    public float z;

    public float q;
    public float w;
    public float e;
    public float r;
}

public class UDP : MonoBehaviour
{
    UdpClient udp;
    
    string returnData = "";
    bool precessData = false;

    private IPEndPoint broadcastAddress;

    private IPAddress address;

    private NetworkDiscovery _nd;

    private bool running;

    public InputField IpAddressText;

    void Start()
    {
        //thread.Start();
        udp = new UdpClient();

        /*address = IPManager.GetIP(ADDRESSFAM.IPv4);

        var bytes = address.GetAddressBytes();

        bytes[3] = 255;*/

        //address = new IPAddress(new byte[] {192, 168, 0, 255});

        running = false;

        //_nd = GetComponent<NetworkDiscovery>();
    }

    public void StartSend()
    {
        if (System.Net.IPAddress.TryParse(IpAddressText.text, out var throwawayIpAddress))
        {
            broadcastAddress = new IPEndPoint(throwawayIpAddress, 7325);
            udp.ExclusiveAddressUse = false;

            running = true;
        }
    }

    byte[] getBytes(Data str)
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(str, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    void Update()
    {
        if (!running)
        {
            return;
        }

        var d = new Data();

        var p = transform.position;
        var r = transform.rotation;

        d.x = p.x;
        d.y = p.y;
        d.z = p.z;
        
        d.q = r.x;
        d.w = r.y;
        d.e = r.z;
        d.r = r.w;

        var datagram = getBytes(d);
        var AC = new AsyncCallback(SendIt);

        udp.BeginSend(datagram, datagram.Length, broadcastAddress, AC, udp);

        //Debug.Log(address.ToString());
    }

    void SendIt(IAsyncResult result)
    {
        udp.EndSend(result);
    }
}
