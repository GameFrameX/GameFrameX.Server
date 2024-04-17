using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000017 RID: 23
    public abstract class TcpClientSession : ClientSession
    {
        // Token: 0x1700002B RID: 43
        // (get) Token: 0x060000CE RID: 206 RVA: 0x00003C01 File Offset: 0x00001E01
        // (set) Token: 0x060000CF RID: 207 RVA: 0x00003C09 File Offset: 0x00001E09
        protected string HostName { get; private set; }

        // Token: 0x060000D0 RID: 208 RVA: 0x00003C12 File Offset: 0x00001E12
        public TcpClientSession()
        {
        }

        // Token: 0x1700002C RID: 44
        // (get) Token: 0x060000D1 RID: 209 RVA: 0x00003C1A File Offset: 0x00001E1A
        // (set) Token: 0x060000D2 RID: 210 RVA: 0x00003C22 File Offset: 0x00001E22
        public override EndPoint LocalEndPoint
        {
            get { return base.LocalEndPoint; }
            set
            {
                if (this.m_InConnecting || base.IsConnected)
                {
                    throw new Exception("You cannot set LocalEdnPoint after you start the connection.");
                }

                base.LocalEndPoint = value;
            }
        }

        // Token: 0x1700002D RID: 45
        // (get) Token: 0x060000D3 RID: 211 RVA: 0x00003C46 File Offset: 0x00001E46
        // (set) Token: 0x060000D4 RID: 212 RVA: 0x00003C50 File Offset: 0x00001E50
        public override int ReceiveBufferSize
        {
            get { return base.ReceiveBufferSize; }
            set
            {
                if (base.Buffer.Array != null)
                {
                    throw new Exception("ReceiveBufferSize cannot be set after the socket has been connected!");
                }

                base.ReceiveBufferSize = value;
            }
        }

        // Token: 0x060000D5 RID: 213 RVA: 0x00003C7F File Offset: 0x00001E7F
        protected virtual bool IsIgnorableException(Exception e)
        {
            return e is ObjectDisposedException || e is NullReferenceException;
        }

        // Token: 0x060000D6 RID: 214 RVA: 0x00003C96 File Offset: 0x00001E96
        protected bool IsIgnorableSocketError(int errorCode)
        {
            return errorCode == 10058 || errorCode == 10053 || errorCode == 10054 || errorCode == 995;
        }

        // Token: 0x060000D7 RID: 215
        protected abstract void SocketEventArgsCompleted(object sender, SocketAsyncEventArgs e);

        // Token: 0x060000D8 RID: 216 RVA: 0x00003CBC File Offset: 0x00001EBC
        public override void Connect(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException("remoteEndPoint");
            }

            DnsEndPoint dnsEndPoint = remoteEndPoint as DnsEndPoint;
            if (dnsEndPoint != null)
            {
                string host = dnsEndPoint.Host;
                if (!string.IsNullOrEmpty(host))
                {
                    this.HostName = host;
                }
            }

            if (this.m_InConnecting)
            {
                throw new Exception("The socket is connecting, cannot connect again!");
            }

            if (base.Client != null)
            {
                throw new Exception("The socket is connected, you needn't connect again!");
            }

            if (base.Proxy != null)
            {
                base.Proxy.Completed += this.Proxy_Completed;
                base.Proxy.Connect(remoteEndPoint);
                this.m_InConnecting = true;
                return;
            }

            this.m_InConnecting = true;
            remoteEndPoint.ConnectAsync(this.LocalEndPoint, new ConnectedCallback(this.ProcessConnect), null);
        }

        // Token: 0x060000D9 RID: 217 RVA: 0x00003D70 File Offset: 0x00001F70
        private void Proxy_Completed(object sender, ProxyEventArgs e)
        {
            base.Proxy.Completed -= this.Proxy_Completed;
            if (e.Connected)
            {
                SocketAsyncEventArgs socketAsyncEventArgs = null;
                if (e.TargetHostName != null)
                {
                    socketAsyncEventArgs = new SocketAsyncEventArgs();
                    socketAsyncEventArgs.RemoteEndPoint = new DnsEndPoint(e.TargetHostName, 0);
                }

                this.ProcessConnect(e.Socket, null, socketAsyncEventArgs, null);
                return;
            }

            this.OnError(new Exception("proxy error", e.Exception));
            this.m_InConnecting = false;
        }

        // Token: 0x060000DA RID: 218 RVA: 0x00003DEC File Offset: 0x00001FEC
        protected void ProcessConnect(Socket socket, object state, SocketAsyncEventArgs e, Exception exception)
        {
            if (exception != null)
            {
                this.m_InConnecting = false;
                this.OnError(exception);
                if (e != null)
                {
                    e.Dispose();
                }

                return;
            }

            if (e != null && e.SocketError != SocketError.Success)
            {
                this.m_InConnecting = false;
                this.OnError(new SocketException((int)e.SocketError));
                e.Dispose();
                return;
            }

            if (socket == null)
            {
                this.m_InConnecting = false;
                this.OnError(new SocketException(10053));
                return;
            }

            if (!socket.Connected)
            {
                this.m_InConnecting = false;
                SocketError errorCode = SocketError.HostUnreachable;
                this.OnError(new SocketException((int)errorCode));
                return;
            }

            if (e == null)
            {
                e = new SocketAsyncEventArgs();
            }

            e.Completed += this.SocketEventArgsCompleted;
            base.Client = socket;
            this.m_InConnecting = false;
            try
            {
                this.LocalEndPoint = socket.LocalEndPoint;
            }
            catch
            {
            }

            EndPoint endPoint = (e.RemoteEndPoint != null) ? e.RemoteEndPoint : socket.RemoteEndPoint;
            if (string.IsNullOrEmpty(this.HostName))
            {
                this.HostName = this.GetHostOfEndPoint(endPoint);
            }
            else
            {
                DnsEndPoint dnsEndPoint = endPoint as DnsEndPoint;
                if (dnsEndPoint != null)
                {
                    string host = dnsEndPoint.Host;
                    if (!string.IsNullOrEmpty(host) && !this.HostName.Equals(host, StringComparison.OrdinalIgnoreCase))
                    {
                        this.HostName = host;
                    }
                }
            }

            this.OnGetSocket(e);
        }

        // Token: 0x060000DB RID: 219 RVA: 0x00003F34 File Offset: 0x00002134
        private string GetHostOfEndPoint(EndPoint endPoint)
        {
            DnsEndPoint dnsEndPoint = endPoint as DnsEndPoint;
            if (dnsEndPoint != null)
            {
                return dnsEndPoint.Host;
            }

            IPEndPoint ipendPoint = endPoint as IPEndPoint;
            if (ipendPoint != null && ipendPoint.Address != null)
            {
                return ipendPoint.Address.ToString();
            }

            return string.Empty;
        }

        // Token: 0x060000DC RID: 220
        protected abstract void OnGetSocket(SocketAsyncEventArgs e);

        // Token: 0x060000DD RID: 221 RVA: 0x00003F75 File Offset: 0x00002175
        protected bool EnsureSocketClosed()
        {
            return this.EnsureSocketClosed(null);
        }

        // Token: 0x060000DE RID: 222 RVA: 0x00003F80 File Offset: 0x00002180
        protected bool EnsureSocketClosed(Socket prevClient)
        {
            Socket socket = base.Client;
            if (socket == null)
            {
                return false;
            }

            bool result = true;
            if (prevClient != null && prevClient != socket)
            {
                socket = prevClient;
                result = false;
            }
            else
            {
                base.Client = null;
                this.m_IsSending = 0;
            }

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            finally
            {
                try
                {
                    socket.Dispose();
                }
                catch
                {
                }
            }

            return result;
        }

        // Token: 0x060000DF RID: 223 RVA: 0x00003FFC File Offset: 0x000021FC
        private bool DetectConnected()
        {
            if (base.Client != null)
            {
                return true;
            }

            this.OnError(new SocketException(10057));
            return false;
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x0000401C File Offset: 0x0000221C
        private IBatchQueue<ArraySegment<byte>> GetSendingQueue()
        {
            if (this.m_SendingQueue != null)
            {
                return this.m_SendingQueue;
            }

            IBatchQueue<ArraySegment<byte>> sendingQueue;
            lock (this)
            {
                if (this.m_SendingQueue != null)
                {
                    sendingQueue = this.m_SendingQueue;
                }
                else
                {
                    this.m_SendingQueue = new ConcurrentBatchQueue<ArraySegment<byte>>(Math.Max(base.SendingQueueSize, 1024), (ArraySegment<byte> t) => t.Array == null || t.Count == 0);
                    sendingQueue = this.m_SendingQueue;
                }
            }

            return sendingQueue;
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x000040B4 File Offset: 0x000022B4
        private PosList<ArraySegment<byte>> GetSendingItems()
        {
            if (this.m_SendingItems == null)
            {
                this.m_SendingItems = new PosList<ArraySegment<byte>>();
            }

            return this.m_SendingItems;
        }

        // Token: 0x1700002E RID: 46
        // (get) Token: 0x060000E2 RID: 226 RVA: 0x000040CF File Offset: 0x000022CF
        protected bool IsSending
        {
            get { return this.m_IsSending == 1; }
        }

        // Token: 0x060000E3 RID: 227 RVA: 0x000040DC File Offset: 0x000022DC
        public override bool TrySend(ArraySegment<byte> segment)
        {
            if (segment.Array == null || segment.Count == 0)
            {
                throw new Exception("The data to be sent cannot be empty.");
            }

            if (!this.DetectConnected())
            {
                return true;
            }

            bool result = this.GetSendingQueue().Enqueue(segment);
            if (Interlocked.CompareExchange(ref this.m_IsSending, 1, 0) != 0)
            {
                return result;
            }

            this.DequeueSend();
            return result;
        }

        // Token: 0x060000E4 RID: 228 RVA: 0x00004138 File Offset: 0x00002338
        public override bool TrySend(IList<ArraySegment<byte>> segments)
        {
            if (segments == null || segments.Count == 0)
            {
                throw new ArgumentNullException("segments");
            }

            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i].Count == 0)
                {
                    throw new Exception("The data piece to be sent cannot be empty.");
                }
            }

            if (!this.DetectConnected())
            {
                return true;
            }

            bool result = this.GetSendingQueue().Enqueue(segments);
            if (Interlocked.CompareExchange(ref this.m_IsSending, 1, 0) != 0)
            {
                return result;
            }

            this.DequeueSend();
            return result;
        }

        // Token: 0x060000E5 RID: 229 RVA: 0x000041B8 File Offset: 0x000023B8
        private void DequeueSend()
        {
            PosList<ArraySegment<byte>> sendingItems = this.GetSendingItems();
            if (!this.m_SendingQueue.TryDequeue(sendingItems))
            {
                this.m_IsSending = 0;
                return;
            }

            this.SendInternal(sendingItems);
        }

        // Token: 0x060000E6 RID: 230
        protected abstract void SendInternal(PosList<ArraySegment<byte>> items);

        // Token: 0x060000E7 RID: 231 RVA: 0x000041EC File Offset: 0x000023EC
        protected void OnSendingCompleted()
        {
            PosList<ArraySegment<byte>> sendingItems = this.GetSendingItems();
            sendingItems.Clear();
            sendingItems.Position = 0;
            if (!this.m_SendingQueue.TryDequeue(sendingItems))
            {
                this.m_IsSending = 0;
                return;
            }

            this.SendInternal(sendingItems);
        }

        // Token: 0x060000E8 RID: 232 RVA: 0x0000422A File Offset: 0x0000242A
        public override void Close()
        {
            if (this.EnsureSocketClosed())
            {
                this.OnClosed();
            }
        }

        // Token: 0x0400002F RID: 47
        private bool m_InConnecting;

        // Token: 0x04000030 RID: 48
        private IBatchQueue<ArraySegment<byte>> m_SendingQueue;

        // Token: 0x04000031 RID: 49
        private PosList<ArraySegment<byte>> m_SendingItems;

        // Token: 0x04000032 RID: 50
        private int m_IsSending;
    }
}