using System;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000004 RID: 4
    // (Invoke) Token: 0x06000023 RID: 35
    public delegate void ConnectedCallback(Socket socket, object state, SocketAsyncEventArgs e, Exception exception);
}