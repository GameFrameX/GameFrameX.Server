using System;
using System.Net;

namespace SuperSocket.ClientEngine.Proxy
{
    // Token: 0x0200001A RID: 26
    public class Socks4aConnector : Socks4Connector
    {
        // Token: 0x06000105 RID: 261 RVA: 0x00004790 File Offset: 0x00002990
        public Socks4aConnector(EndPoint proxyEndPoint, string userID) : base(proxyEndPoint, userID)
        {
        }

        // Token: 0x06000106 RID: 262 RVA: 0x0000479C File Offset: 0x0000299C
        public override void Connect(EndPoint remoteEndPoint)
        {
            DnsEndPoint dnsEndPoint = remoteEndPoint as DnsEndPoint;
            if (dnsEndPoint == null)
            {
                base.OnCompleted(new ProxyEventArgs(new Exception("The argument 'remoteEndPoint' must be a DnsEndPoint")));
                return;
            }

            try
            {
                base.ProxyEndPoint.ConnectAsync(null, new ConnectedCallback(this.ProcessConnect), dnsEndPoint);
            }
            catch (Exception innerException)
            {
                base.OnException(new Exception("Failed to connect proxy server", innerException));
            }
        }

        // Token: 0x06000107 RID: 263 RVA: 0x0000480C File Offset: 0x00002A0C
        protected override byte[] GetSendingBuffer(EndPoint targetEndPoint, out int actualLength)
        {
            DnsEndPoint dnsEndPoint = targetEndPoint as DnsEndPoint;
            byte[] array = new byte[Math.Max(8, (string.IsNullOrEmpty(base.UserID) ? 0 : ProxyConnectorBase.ASCIIEncoding.GetMaxByteCount(base.UserID.Length)) + 5 + 4 + ProxyConnectorBase.ASCIIEncoding.GetMaxByteCount(dnsEndPoint.Host.Length) + 1)];
            array[0] = 4;
            array[1] = 1;
            array[2] = (byte)(dnsEndPoint.Port / 256);
            array[3] = (byte)(dnsEndPoint.Port % 256);
            array[4] = 0;
            array[5] = 0;
            array[6] = 0;
            array[7] = (byte)Socks4aConnector.m_Random.Next(1, 255);
            actualLength = 8;
            if (!string.IsNullOrEmpty(base.UserID))
            {
                actualLength += ProxyConnectorBase.ASCIIEncoding.GetBytes(base.UserID, 0, base.UserID.Length, array, actualLength);
            }

            byte[] array2 = array;
            int num = actualLength;
            actualLength = num + 1;
            array2[num] = 0;
            actualLength += ProxyConnectorBase.ASCIIEncoding.GetBytes(dnsEndPoint.Host, 0, dnsEndPoint.Host.Length, array, actualLength);
            byte[] array3 = array;
            num = actualLength;
            actualLength = num + 1;
            array3[num] = 0;
            return array;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00004928 File Offset: 0x00002B28
        protected override void HandleFaultStatus(byte status)
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

        // Token: 0x0400003C RID: 60
        private static Random m_Random = new Random();
    }
}