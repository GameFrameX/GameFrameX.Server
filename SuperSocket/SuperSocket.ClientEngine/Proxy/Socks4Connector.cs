using System;
using System.Net;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine.Proxy
{
    // Token: 0x0200001C RID: 28
    public class Socks4Connector : ProxyConnectorBase
    {
        // Token: 0x17000033 RID: 51
        // (get) Token: 0x0600010F RID: 271 RVA: 0x000049B4 File Offset: 0x00002BB4
        // (set) Token: 0x06000110 RID: 272 RVA: 0x000049BC File Offset: 0x00002BBC
        public string UserID { get; private set; }

        // Token: 0x06000111 RID: 273 RVA: 0x000049C5 File Offset: 0x00002BC5
        public Socks4Connector(EndPoint proxyEndPoint, string userID) : base(proxyEndPoint)
        {
            this.UserID = userID;
        }

        // Token: 0x06000112 RID: 274 RVA: 0x000049D8 File Offset: 0x00002BD8
        public override void Connect(EndPoint remoteEndPoint)
        {
            IPEndPoint ipendPoint = remoteEndPoint as IPEndPoint;
            if (ipendPoint == null)
            {
                base.OnCompleted(new ProxyEventArgs(new Exception("The argument 'remoteEndPoint' must be a IPEndPoint")));
                return;
            }

            try
            {
                base.ProxyEndPoint.ConnectAsync(null, new ConnectedCallback(this.ProcessConnect), ipendPoint);
            }
            catch (Exception innerException)
            {
                base.OnException(new Exception("Failed to connect proxy server", innerException));
            }
        }

        // Token: 0x06000113 RID: 275 RVA: 0x00004A48 File Offset: 0x00002C48
        protected virtual byte[] GetSendingBuffer(EndPoint targetEndPoint, out int actualLength)
        {
            IPEndPoint ipendPoint = targetEndPoint as IPEndPoint;
            byte[] addressBytes = ipendPoint.Address.GetAddressBytes();
            byte[] array = new byte[Math.Max(8, (string.IsNullOrEmpty(this.UserID) ? 0 : ProxyConnectorBase.ASCIIEncoding.GetMaxByteCount(this.UserID.Length)) + 5 + addressBytes.Length)];
            array[0] = 4;
            array[1] = 1;
            array[2] = (byte)(ipendPoint.Port / 256);
            array[3] = (byte)(ipendPoint.Port % 256);
            Buffer.BlockCopy(addressBytes, 0, array, 4, addressBytes.Length);
            actualLength = 4 + addressBytes.Length;
            if (!string.IsNullOrEmpty(this.UserID))
            {
                actualLength += ProxyConnectorBase.ASCIIEncoding.GetBytes(this.UserID, 0, this.UserID.Length, array, actualLength);
            }

            byte[] array2 = array;
            int num = actualLength;
            actualLength = num + 1;
            array2[num] = 0;
            return array;
        }

        // Token: 0x06000114 RID: 276 RVA: 0x00004B18 File Offset: 0x00002D18
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

            int num;
            byte[] sendingBuffer = this.GetSendingBuffer((EndPoint)targetEndPoint, out num);
            e.SetBuffer(sendingBuffer, 0, num);
            e.UserToken = socket;
            e.Completed += base.AsyncEventArgsCompleted;
            base.StartSend(socket, e);
        }

        // Token: 0x06000115 RID: 277 RVA: 0x00004B96 File Offset: 0x00002D96
        protected override void ProcessSend(SocketAsyncEventArgs e)
        {
            if (!base.ValidateAsyncResult(e))
            {
                return;
            }

            e.SetBuffer(0, 8);
            this.StartReceive((Socket)e.UserToken, e);
        }

        // Token: 0x06000116 RID: 278 RVA: 0x00004BBC File Offset: 0x00002DBC
        protected override void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (!base.ValidateAsyncResult(e))
            {
                return;
            }

            int num = e.Offset + e.BytesTransferred;
            if (num < 8)
            {
                e.SetBuffer(num, 8 - num);
                this.StartReceive((Socket)e.UserToken, e);
                return;
            }

            if (num != 8)
            {
                base.OnException("socks protocol error: size of response cannot be larger than 8");
                return;
            }

            byte b = e.Buffer[1];
            if (b == 90)
            {
                base.OnCompleted(new ProxyEventArgs((Socket)e.UserToken));
                return;
            }

            this.HandleFaultStatus(b);
        }

        // Token: 0x06000117 RID: 279 RVA: 0x00004C40 File Offset: 0x00002E40
        protected virtual void HandleFaultStatus(byte status)
        {
            string exception = string.Empty;
            switch (status)
            {
                case 91:
                    exception = "request rejected or failed";
                    break;
                case 92:
                    exception = "request failed because client is not running identd (or not reachable from the server)";
                    break;
                case 93:
                    exception = "request failed because client's identd could not confirm the user ID string in the reques";
                    break;
                default:
                    exception = "request rejected for unknown error";
                    break;
            }

            base.OnException(exception);
        }

        // Token: 0x04000040 RID: 64
        private const int m_ValidResponseSize = 8;
    }
}