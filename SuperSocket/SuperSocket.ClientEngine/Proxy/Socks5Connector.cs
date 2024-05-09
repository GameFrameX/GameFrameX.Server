using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine.Proxy
{
    // Token: 0x0200001D RID: 29
    public class Socks5Connector : ProxyConnectorBase
    {
        // Token: 0x06000118 RID: 280 RVA: 0x00004C8F File Offset: 0x00002E8F
        public Socks5Connector(EndPoint proxyEndPoint) : base(proxyEndPoint)
        {
        }

        // Token: 0x06000119 RID: 281 RVA: 0x00004C98 File Offset: 0x00002E98
        public Socks5Connector(EndPoint proxyEndPoint, string username, string password) : base(proxyEndPoint)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            byte[] array = new byte[3 + ProxyConnectorBase.ASCIIEncoding.GetMaxByteCount(username.Length) + (string.IsNullOrEmpty(password) ? 0 : ProxyConnectorBase.ASCIIEncoding.GetMaxByteCount(password.Length))];
            array[0] = 5;
            int bytes = ProxyConnectorBase.ASCIIEncoding.GetBytes(username, 0, username.Length, array, 2);
            if (bytes > 255)
            {
                throw new ArgumentException("the length of username cannot exceed 255", "username");
            }

            array[1] = (byte)bytes;
            int num = bytes + 2;
            if (!string.IsNullOrEmpty(password))
            {
                bytes = ProxyConnectorBase.ASCIIEncoding.GetBytes(password, 0, password.Length, array, num + 1);
                if (bytes > 255)
                {
                    throw new ArgumentException("the length of password cannot exceed 255", "password");
                }

                array[num] = (byte)bytes;
                num += bytes + 1;
            }
            else
            {
                array[num] = 0;
                num++;
            }

            this.m_UserNameAuthenRequest = new ArraySegment<byte>(array, 0, num);
        }

        // Token: 0x0600011A RID: 282 RVA: 0x00004D8C File Offset: 0x00002F8C
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

        // Token: 0x0600011B RID: 283 RVA: 0x00004E08 File Offset: 0x00003008
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

            e.UserToken = new Socks5Connector.SocksContext
            {
                TargetEndPoint = (EndPoint)targetEndPoint,
                Socket = socket,
                State = Socks5Connector.SocksState.NotAuthenticated
            };
            e.Completed += base.AsyncEventArgsCompleted;
            e.SetBuffer(Socks5Connector.m_AuthenHandshake, 0, Socks5Connector.m_AuthenHandshake.Length);
            base.StartSend(socket, e);
        }

        // Token: 0x0600011C RID: 284 RVA: 0x00004EA0 File Offset: 0x000030A0
        protected override void ProcessSend(SocketAsyncEventArgs e)
        {
            if (!base.ValidateAsyncResult(e))
            {
                return;
            }

            Socks5Connector.SocksContext socksContext = e.UserToken as Socks5Connector.SocksContext;
            if (socksContext.State == Socks5Connector.SocksState.NotAuthenticated)
            {
                e.SetBuffer(0, 2);
                this.StartReceive(socksContext.Socket, e);
                return;
            }

            if (socksContext.State == Socks5Connector.SocksState.Authenticating)
            {
                e.SetBuffer(0, 2);
                this.StartReceive(socksContext.Socket, e);
                return;
            }

            e.SetBuffer(0, e.Buffer.Length);
            this.StartReceive(socksContext.Socket, e);
        }

        // Token: 0x0600011D RID: 285 RVA: 0x00004F1C File Offset: 0x0000311C
        private bool ProcessAuthenticationResponse(Socket socket, SocketAsyncEventArgs e)
        {
            int num = e.BytesTransferred + e.Offset;
            if (num < 2)
            {
                e.SetBuffer(num, 2 - num);
                this.StartReceive(socket, e);
                return false;
            }

            if (num > 2)
            {
                base.OnException("received length exceeded");
                return false;
            }

            if (e.Buffer[0] != 5)
            {
                base.OnException("invalid protocol version");
                return false;
            }

            return true;
        }

        // Token: 0x0600011E RID: 286 RVA: 0x00004F7C File Offset: 0x0000317C
        protected override void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (!base.ValidateAsyncResult(e))
            {
                return;
            }

            Socks5Connector.SocksContext socksContext = (Socks5Connector.SocksContext)e.UserToken;
            if (socksContext.State == Socks5Connector.SocksState.NotAuthenticated)
            {
                if (!this.ProcessAuthenticationResponse(socksContext.Socket, e))
                {
                    return;
                }

                byte b = e.Buffer[1];
                if (b == 0)
                {
                    socksContext.State = Socks5Connector.SocksState.Authenticated;
                    this.SendHandshake(e);
                    return;
                }

                if (b == 2)
                {
                    socksContext.State = Socks5Connector.SocksState.Authenticating;
                    this.AutheticateWithUserNamePassword(e);
                    return;
                }

                if (b == 255)
                {
                    base.OnException("no acceptable methods were offered");
                    return;
                }

                base.OnException("protocol error");
                return;
            }
            else if (socksContext.State == Socks5Connector.SocksState.Authenticating)
            {
                if (!this.ProcessAuthenticationResponse(socksContext.Socket, e))
                {
                    return;
                }

                if (e.Buffer[1] == 0)
                {
                    socksContext.State = Socks5Connector.SocksState.Authenticated;
                    this.SendHandshake(e);
                    return;
                }

                base.OnException("authentication failure");
                return;
            }
            else
            {
                byte[] array = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, array, 0, e.BytesTransferred);
                socksContext.ReceivedData.AddRange(array);
                if (socksContext.ExpectedLength > socksContext.ReceivedData.Count)
                {
                    this.StartReceive(socksContext.Socket, e);
                    return;
                }

                if (socksContext.State != Socks5Connector.SocksState.FoundLength)
                {
                    byte b2 = socksContext.ReceivedData[3];
                    int num;
                    if (b2 == 1)
                    {
                        num = 10;
                    }
                    else if (b2 == 3)
                    {
                        num = (int)(7 + socksContext.ReceivedData[4]);
                    }
                    else
                    {
                        num = 22;
                    }

                    if (socksContext.ReceivedData.Count < num)
                    {
                        socksContext.ExpectedLength = num;
                        this.StartReceive(socksContext.Socket, e);
                        return;
                    }

                    if (socksContext.ReceivedData.Count > num)
                    {
                        base.OnException("response length exceeded");
                        return;
                    }

                    this.OnGetFullResponse(socksContext);
                    return;
                }
                else
                {
                    if (socksContext.ReceivedData.Count > socksContext.ExpectedLength)
                    {
                        base.OnException("response length exceeded");
                        return;
                    }

                    this.OnGetFullResponse(socksContext);
                    return;
                }
            }
        }

        // Token: 0x0600011F RID: 287 RVA: 0x00005140 File Offset: 0x00003340
        private void OnGetFullResponse(Socks5Connector.SocksContext context)
        {
            List<byte> receivedData = context.ReceivedData;
            if (receivedData[0] != 5)
            {
                base.OnException("invalid protocol version");
                return;
            }

            byte b = receivedData[1];
            if (b == 0)
            {
                base.OnCompleted(new ProxyEventArgs(context.Socket));
                return;
            }

            string exception = string.Empty;
            switch (b)
            {
                case 2:
                    exception = "connection not allowed by ruleset";
                    break;
                case 3:
                    exception = "network unreachable";
                    break;
                case 4:
                    exception = "host unreachable";
                    break;
                case 5:
                    exception = "connection refused by destination host";
                    break;
                case 6:
                    exception = "TTL expired";
                    break;
                case 7:
                    exception = "command not supported / protocol error";
                    break;
                case 8:
                    exception = "address type not supported";
                    break;
                default:
                    exception = "general failure";
                    break;
            }

            base.OnException(exception);
        }

        // Token: 0x06000120 RID: 288 RVA: 0x000051F8 File Offset: 0x000033F8
        private void SendHandshake(SocketAsyncEventArgs e)
        {
            Socks5Connector.SocksContext socksContext = e.UserToken as Socks5Connector.SocksContext;
            EndPoint targetEndPoint = socksContext.TargetEndPoint;
            int port;
            byte[] array;
            int num;
            if (targetEndPoint is IPEndPoint)
            {
                IPEndPoint ipendPoint = targetEndPoint as IPEndPoint;
                port = ipendPoint.Port;
                if (ipendPoint.AddressFamily == AddressFamily.InterNetwork)
                {
                    array = new byte[10];
                    array[3] = 1;
                    Buffer.BlockCopy(ipendPoint.Address.GetAddressBytes(), 0, array, 4, 4);
                }
                else
                {
                    if (ipendPoint.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        base.OnException("unknown address family");
                        return;
                    }

                    array = new byte[22];
                    array[3] = 4;
                    Buffer.BlockCopy(ipendPoint.Address.GetAddressBytes(), 0, array, 4, 16);
                }

                num = array.Length;
            }
            else
            {
                DnsEndPoint dnsEndPoint = targetEndPoint as DnsEndPoint;
                port = dnsEndPoint.Port;
                array = new byte[7 + ProxyConnectorBase.ASCIIEncoding.GetMaxByteCount(dnsEndPoint.Host.Length)];
                array[3] = 3;
                num = 5;
                num += ProxyConnectorBase.ASCIIEncoding.GetBytes(dnsEndPoint.Host, 0, dnsEndPoint.Host.Length, array, num);
                num += 2;
            }

            array[0] = 5;
            array[1] = 1;
            array[2] = 0;
            array[num - 2] = (byte)(port / 256);
            array[num - 1] = (byte)(port % 256);
            e.SetBuffer(array, 0, num);
            socksContext.ReceivedData = new List<byte>(num + 5);
            socksContext.ExpectedLength = 5;
            base.StartSend(socksContext.Socket, e);
        }

        // Token: 0x06000121 RID: 289 RVA: 0x00005354 File Offset: 0x00003554
        private void AutheticateWithUserNamePassword(SocketAsyncEventArgs e)
        {
            Socket socket = ((Socks5Connector.SocksContext)e.UserToken).Socket;
            e.SetBuffer(this.m_UserNameAuthenRequest.Array, this.m_UserNameAuthenRequest.Offset, this.m_UserNameAuthenRequest.Count);
            base.StartSend(socket, e);
        }

        // Token: 0x04000041 RID: 65
        private ArraySegment<byte> m_UserNameAuthenRequest;

        // Token: 0x04000042 RID: 66
        private static byte[] m_AuthenHandshake = new byte[]
        {
            5,
            2,
            0,
            2
        };

        // Token: 0x02000029 RID: 41
        private enum SocksState
        {
            // Token: 0x04000068 RID: 104
            NotAuthenticated,

            // Token: 0x04000069 RID: 105
            Authenticating,

            // Token: 0x0400006A RID: 106
            Authenticated,

            // Token: 0x0400006B RID: 107
            FoundLength,

            // Token: 0x0400006C RID: 108
            Connected
        }

        // Token: 0x0200002A RID: 42
        private class SocksContext
        {
            // Token: 0x1700003C RID: 60
            // (get) Token: 0x06000145 RID: 325 RVA: 0x00005997 File Offset: 0x00003B97
            // (set) Token: 0x06000146 RID: 326 RVA: 0x0000599F File Offset: 0x00003B9F
            public Socket Socket { get; set; }

            // Token: 0x1700003D RID: 61
            // (get) Token: 0x06000147 RID: 327 RVA: 0x000059A8 File Offset: 0x00003BA8
            // (set) Token: 0x06000148 RID: 328 RVA: 0x000059B0 File Offset: 0x00003BB0
            public Socks5Connector.SocksState State { get; set; }

            // Token: 0x1700003E RID: 62
            // (get) Token: 0x06000149 RID: 329 RVA: 0x000059B9 File Offset: 0x00003BB9
            // (set) Token: 0x0600014A RID: 330 RVA: 0x000059C1 File Offset: 0x00003BC1
            public EndPoint TargetEndPoint { get; set; }

            // Token: 0x1700003F RID: 63
            // (get) Token: 0x0600014B RID: 331 RVA: 0x000059CA File Offset: 0x00003BCA
            // (set) Token: 0x0600014C RID: 332 RVA: 0x000059D2 File Offset: 0x00003BD2
            public List<byte> ReceivedData { get; set; }

            // Token: 0x17000040 RID: 64
            // (get) Token: 0x0600014D RID: 333 RVA: 0x000059DB File Offset: 0x00003BDB
            // (set) Token: 0x0600014E RID: 334 RVA: 0x000059E3 File Offset: 0x00003BE3
            public int ExpectedLength { get; set; }
        }
    }
}