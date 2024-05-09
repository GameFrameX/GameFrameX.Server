using System;
using System.Net;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000008 RID: 8
    public interface IProxyConnector
    {
        // Token: 0x0600003D RID: 61
        void Connect(EndPoint remoteEndPoint);

        // Token: 0x14000001 RID: 1
        // (add) Token: 0x0600003E RID: 62
        // (remove) Token: 0x0600003F RID: 63
        event EventHandler<ProxyEventArgs> Completed;
    }
}