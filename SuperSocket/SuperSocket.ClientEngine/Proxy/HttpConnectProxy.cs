using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine.Proxy
{
    // Token: 0x02000018 RID: 24
    public class HttpConnectProxy : ProxyConnectorBase
    {
        // Token: 0x060000EA RID: 234 RVA: 0x00004250 File Offset: 0x00002450
        public HttpConnectProxy(EndPoint proxyEndPoint) : this(proxyEndPoint, 128, null)
        {
        }

        // Token: 0x060000EB RID: 235 RVA: 0x0000425F File Offset: 0x0000245F
        public HttpConnectProxy(EndPoint proxyEndPoint, string targetHostName) : this(proxyEndPoint, 128, targetHostName)
        {
        }

        // Token: 0x060000EC RID: 236 RVA: 0x0000426E File Offset: 0x0000246E
        public HttpConnectProxy(EndPoint proxyEndPoint, int receiveBufferSize, string targetHostName) : base(proxyEndPoint, targetHostName)
        {
            this.m_ReceiveBufferSize = receiveBufferSize;
        }

        // Token: 0x060000ED RID: 237 RVA: 0x00004280 File Offset: 0x00002480
        public override void Connect(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException("remoteEndPoint");
            }

            if (!(remoteEndPoint is IPEndPoint) && !(remoteEndPoint is DnsEndPoint))
            {
                throw new ArgumentException("remoteEndPoint must be IPEndPoint or DnsEndPoint", "remoteEndPoint");
            }

            try
            {
                base.ProxyEndPoint.ConnectAsync(null, new ConnectedCallback(this.ProcessConnect), remoteEndPoint);
            }
            catch (Exception innerException)
            {
                base.OnException(new Exception("Failed to connect proxy server", innerException));
            }
        }

        // Token: 0x060000EE RID: 238 RVA: 0x000042FC File Offset: 0x000024FC
        protected override void ProcessConnect(Socket socket, object targetEndPoint, SocketAsyncEventArgs e, Exception exception)
        {
            if (exception != null)
            {
                base.OnException(exception);
                return;
            }

            if (e != null && !base.ValidateAsyncResult(e))
            {
                return;
            }

            if (socket == null)
            {
                base.OnException(new SocketException(10053));
                return;
            }

            if (e == null)
            {
                e = new SocketAsyncEventArgs();
            }

            string s;
            if (targetEndPoint is DnsEndPoint)
            {
                DnsEndPoint dnsEndPoint = (DnsEndPoint)targetEndPoint;
                s = string.Format("CONNECT {0}:{1} HTTP/1.1\r\nHost: {0}:{1}\r\nProxy-Connection: Keep-Alive\r\n\r\n", dnsEndPoint.Host, dnsEndPoint.Port);
            }
            else
            {
                IPEndPoint ipendPoint = (IPEndPoint)targetEndPoint;
                s = string.Format("CONNECT {0}:{1} HTTP/1.1\r\nHost: {0}:{1}\r\nProxy-Connection: Keep-Alive\r\n\r\n", ipendPoint.Address, ipendPoint.Port);
            }

            byte[] bytes = ProxyConnectorBase.ASCIIEncoding.GetBytes(s);
            e.Completed += base.AsyncEventArgsCompleted;
            e.UserToken = new HttpConnectProxy.ConnectContext
            {
                Socket = socket,
                SearchState = new SearchMarkState<byte>(HttpConnectProxy.m_LineSeparator)
            };
            e.SetBuffer(bytes, 0, bytes.Length);
            base.StartSend(socket, e);
        }

        // Token: 0x060000EF RID: 239 RVA: 0x000043E4 File Offset: 0x000025E4
        protected override void ProcessSend(SocketAsyncEventArgs e)
        {
            if (!base.ValidateAsyncResult(e))
            {
                return;
            }

            HttpConnectProxy.ConnectContext connectContext = (HttpConnectProxy.ConnectContext)e.UserToken;
            byte[] array = new byte[this.m_ReceiveBufferSize];
            e.SetBuffer(array, 0, array.Length);
            this.StartReceive(connectContext.Socket, e);
        }

        // Token: 0x060000F0 RID: 240 RVA: 0x0000442C File Offset: 0x0000262C
        protected override void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (!base.ValidateAsyncResult(e))
            {
                return;
            }

            HttpConnectProxy.ConnectContext connectContext = (HttpConnectProxy.ConnectContext)e.UserToken;
            int matched = connectContext.SearchState.Matched;
            int num = e.Buffer.SearchMark(e.Offset, e.BytesTransferred, connectContext.SearchState);
            if (num < 0)
            {
                int num2 = e.Offset + e.BytesTransferred;
                if (num2 >= this.m_ReceiveBufferSize)
                {
                    base.OnException("receive buffer size has been exceeded");
                    return;
                }

                e.SetBuffer(num2, this.m_ReceiveBufferSize - num2);
                this.StartReceive(connectContext.Socket, e);
                return;
            }
            else
            {
                int num3 = (matched > 0) ? (e.Offset - matched) : (e.Offset + num);
                if (e.Offset + e.BytesTransferred > num3 + HttpConnectProxy.m_LineSeparator.Length)
                {
                    base.OnException("protocol error: more data has been received");
                    return;
                }

                string text = new StringReader(ProxyConnectorBase.ASCIIEncoding.GetString(e.Buffer, 0, num3)).ReadLine();
                if (string.IsNullOrEmpty(text))
                {
                    base.OnException("protocol error: invalid response");
                    return;
                }

                int num4 = text.IndexOf(' ');
                if (num4 <= 0 || text.Length <= num4 + 2)
                {
                    base.OnException("protocol error: invalid response");
                    return;
                }

                string value = text.Substring(0, num4);
                if (!"HTTP/1.1".Equals(value))
                {
                    base.OnException("protocol error: invalid protocol");
                    return;
                }

                int num5 = text.IndexOf(' ', num4 + 1);
                if (num5 < 0)
                {
                    base.OnException("protocol error: invalid response");
                    return;
                }

                int num6;
                if (!int.TryParse(text.Substring(num4 + 1, num5 - num4 - 1), out num6) || num6 > 299 || num6 < 200)
                {
                    base.OnException("the proxy server refused the connection");
                    return;
                }

                base.OnCompleted(new ProxyEventArgs(connectContext.Socket, base.TargetHostHame));
                return;
            }
        }

        // Token: 0x04000033 RID: 51
        private const string m_RequestTemplate = "CONNECT {0}:{1} HTTP/1.1\r\nHost: {0}:{1}\r\nProxy-Connection: Keep-Alive\r\n\r\n";

        // Token: 0x04000034 RID: 52
        private const string m_ResponsePrefix = "HTTP/1.1";

        // Token: 0x04000035 RID: 53
        private const char m_Space = ' ';

        // Token: 0x04000036 RID: 54
        private static byte[] m_LineSeparator = ProxyConnectorBase.ASCIIEncoding.GetBytes("\r\n\r\n");

        // Token: 0x04000037 RID: 55
        private int m_ReceiveBufferSize;

        // Token: 0x02000028 RID: 40
        private class ConnectContext
        {
            // Token: 0x1700003A RID: 58
            // (get) Token: 0x06000140 RID: 320 RVA: 0x0000596D File Offset: 0x00003B6D
            // (set) Token: 0x06000141 RID: 321 RVA: 0x00005975 File Offset: 0x00003B75
            public Socket Socket { get; set; }

            // Token: 0x1700003B RID: 59
            // (get) Token: 0x06000142 RID: 322 RVA: 0x0000597E File Offset: 0x00003B7E
            // (set) Token: 0x06000143 RID: 323 RVA: 0x00005986 File Offset: 0x00003B86
            public SearchMarkState<byte> SearchState { get; set; }
        }
    }
}