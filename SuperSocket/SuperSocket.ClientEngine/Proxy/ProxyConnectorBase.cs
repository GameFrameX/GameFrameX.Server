using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SuperSocket.ClientEngine.Proxy
{
    // Token: 0x02000019 RID: 25
    public abstract class ProxyConnectorBase : IProxyConnector
    {
        // Token: 0x1700002F RID: 47
        // (get) Token: 0x060000F1 RID: 241 RVA: 0x000045EE File Offset: 0x000027EE
        // (set) Token: 0x060000F2 RID: 242 RVA: 0x000045F6 File Offset: 0x000027F6
        public EndPoint ProxyEndPoint { get; private set; }

        // Token: 0x17000030 RID: 48
        // (get) Token: 0x060000F3 RID: 243 RVA: 0x000045FF File Offset: 0x000027FF
        // (set) Token: 0x060000F4 RID: 244 RVA: 0x00004607 File Offset: 0x00002807
        public string TargetHostHame { get; private set; }

        // Token: 0x060000F5 RID: 245 RVA: 0x00004610 File Offset: 0x00002810
        public ProxyConnectorBase(EndPoint proxyEndPoint) : this(proxyEndPoint, null)
        {
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x0000461A File Offset: 0x0000281A
        public ProxyConnectorBase(EndPoint proxyEndPoint, string targetHostHame)
        {
            this.ProxyEndPoint = proxyEndPoint;
            this.TargetHostHame = targetHostHame;
        }

        // Token: 0x060000F7 RID: 247
        public abstract void Connect(EndPoint remoteEndPoint);

        // Token: 0x1400000A RID: 10
        // (add) Token: 0x060000F8 RID: 248 RVA: 0x00004630 File Offset: 0x00002830
        // (remove) Token: 0x060000F9 RID: 249 RVA: 0x00004649 File Offset: 0x00002849
        public event EventHandler<ProxyEventArgs> Completed
        {
            add { this.m_Completed = (EventHandler<ProxyEventArgs>)Delegate.Combine(this.m_Completed, value); }
            remove { this.m_Completed = (EventHandler<ProxyEventArgs>)Delegate.Remove(this.m_Completed, value); }
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00004662 File Offset: 0x00002862
        protected void OnCompleted(ProxyEventArgs args)
        {
            if (this.m_Completed == null)
            {
                return;
            }

            this.m_Completed(this, args);
        }

        // Token: 0x060000FB RID: 251 RVA: 0x0000467A File Offset: 0x0000287A
        protected void OnException(Exception exception)
        {
            this.OnCompleted(new ProxyEventArgs(exception));
        }

        // Token: 0x060000FC RID: 252 RVA: 0x00004688 File Offset: 0x00002888
        protected void OnException(string exception)
        {
            this.OnCompleted(new ProxyEventArgs(new Exception(exception)));
        }

        // Token: 0x060000FD RID: 253 RVA: 0x0000469C File Offset: 0x0000289C
        protected bool ValidateAsyncResult(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                SocketException ex = new SocketException((int)e.SocketError);
                this.OnCompleted(new ProxyEventArgs(new Exception(ex.Message, ex)));
                return false;
            }

            return true;
        }

        // Token: 0x060000FE RID: 254 RVA: 0x000046D7 File Offset: 0x000028D7
        protected void AsyncEventArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Send)
            {
                this.ProcessSend(e);
                return;
            }

            this.ProcessReceive(e);
        }

        // Token: 0x060000FF RID: 255 RVA: 0x000046F4 File Offset: 0x000028F4
        protected void StartSend(Socket socket, SocketAsyncEventArgs e)
        {
            bool flag = false;
            try
            {
                flag = socket.SendAsync(e);
            }
            catch (Exception ex)
            {
                this.OnException(new Exception(ex.Message, ex));
                return;
            }

            if (!flag)
            {
                this.ProcessSend(e);
            }
        }

        // Token: 0x06000100 RID: 256 RVA: 0x0000473C File Offset: 0x0000293C
        protected virtual void StartReceive(Socket socket, SocketAsyncEventArgs e)
        {
            bool flag = false;
            try
            {
                flag = socket.ReceiveAsync(e);
            }
            catch (Exception ex)
            {
                this.OnException(new Exception(ex.Message, ex));
                return;
            }

            if (!flag)
            {
                this.ProcessReceive(e);
            }
        }

        // Token: 0x06000101 RID: 257
        protected abstract void ProcessConnect(Socket socket, object targetEndPoint, SocketAsyncEventArgs e, Exception exception);

        // Token: 0x06000102 RID: 258
        protected abstract void ProcessSend(SocketAsyncEventArgs e);

        // Token: 0x06000103 RID: 259
        protected abstract void ProcessReceive(SocketAsyncEventArgs e);

        // Token: 0x0400003A RID: 58
        protected static Encoding ASCIIEncoding = new ASCIIEncoding();

        // Token: 0x0400003B RID: 59
        private EventHandler<ProxyEventArgs> m_Completed;
    }
}