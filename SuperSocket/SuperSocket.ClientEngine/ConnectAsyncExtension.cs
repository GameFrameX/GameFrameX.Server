using System;
using System.Net;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000005 RID: 5
    public static class ConnectAsyncExtension
    {
        // Token: 0x06000026 RID: 38 RVA: 0x00002AE0 File Offset: 0x00000CE0
        private static void SocketAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= ConnectAsyncExtension.SocketAsyncEventCompleted;
            ConnectAsyncExtension.ConnectToken connectToken = (ConnectAsyncExtension.ConnectToken)e.UserToken;
            e.UserToken = null;
            connectToken.Callback(sender as Socket, connectToken.State, e, null);
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002B2B File Offset: 0x00000D2B
        private static SocketAsyncEventArgs CreateSocketAsyncEventArgs(EndPoint remoteEndPoint, ConnectedCallback callback, object state)
        {
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            socketAsyncEventArgs.UserToken = new ConnectAsyncExtension.ConnectToken
            {
                State = state,
                Callback = callback
            };
            socketAsyncEventArgs.RemoteEndPoint = remoteEndPoint;
            socketAsyncEventArgs.Completed += ConnectAsyncExtension.SocketAsyncEventCompleted;
            return socketAsyncEventArgs;
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00002B64 File Offset: 0x00000D64
        internal static bool PreferIPv4Stack()
        {
            return Environment.GetEnvironmentVariable("PREFER_IPv4_STACK") != null;
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00002B74 File Offset: 0x00000D74
        public static void ConnectAsync(this EndPoint remoteEndPoint, EndPoint localEndPoint, ConnectedCallback callback, object state)
        {
            SocketAsyncEventArgs socketAsyncEventArgs = ConnectAsyncExtension.CreateSocketAsyncEventArgs(remoteEndPoint, callback, state);
            if (localEndPoint != null)
            {
                Socket socket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.ExclusiveAddressUse = false;
                    socket.Bind(localEndPoint);
                }
                catch (Exception exception)
                {
                    callback(null, state, null, exception);
                    return;
                }

                socket.ConnectAsync(socketAsyncEventArgs);
                return;
            }

            Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, socketAsyncEventArgs);
        }

        // Token: 0x02000021 RID: 33
        private class ConnectToken
        {
            // Token: 0x17000035 RID: 53
            // (get) Token: 0x06000129 RID: 297 RVA: 0x000053F1 File Offset: 0x000035F1
            // (set) Token: 0x0600012A RID: 298 RVA: 0x000053F9 File Offset: 0x000035F9
            public object State { get; set; }

            // Token: 0x17000036 RID: 54
            // (get) Token: 0x0600012B RID: 299 RVA: 0x00005402 File Offset: 0x00003602
            // (set) Token: 0x0600012C RID: 300 RVA: 0x0000540A File Offset: 0x0000360A
            public ConnectedCallback Callback { get; set; }
        }
    }
}