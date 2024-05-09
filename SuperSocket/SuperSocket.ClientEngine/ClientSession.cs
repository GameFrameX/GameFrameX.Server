using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SuperSocket.ClientEngine
{
    // Token: 0x0200000F RID: 15
    public abstract class ClientSession : IClientSession, IBufferSetter
    {
        // Token: 0x17000013 RID: 19
        // (get) Token: 0x0600006D RID: 109 RVA: 0x000035DE File Offset: 0x000017DE
        // (set) Token: 0x0600006E RID: 110 RVA: 0x000035E6 File Offset: 0x000017E6
        protected Socket Client { get; set; }

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600006F RID: 111 RVA: 0x000035EF File Offset: 0x000017EF
        Socket IClientSession.Socket
        {
            get { return this.Client; }
        }

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000070 RID: 112 RVA: 0x000035F7 File Offset: 0x000017F7
        // (set) Token: 0x06000071 RID: 113 RVA: 0x000035FF File Offset: 0x000017FF
        public virtual EndPoint LocalEndPoint { get; set; }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x06000072 RID: 114 RVA: 0x00003608 File Offset: 0x00001808
        // (set) Token: 0x06000073 RID: 115 RVA: 0x00003610 File Offset: 0x00001810
        public bool IsConnected { get; private set; }

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000074 RID: 116 RVA: 0x00003619 File Offset: 0x00001819
        // (set) Token: 0x06000075 RID: 117 RVA: 0x00003621 File Offset: 0x00001821
        public bool NoDelay { get; set; }

        // Token: 0x06000076 RID: 118 RVA: 0x0000362A File Offset: 0x0000182A
        public ClientSession()
        {
        }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000077 RID: 119 RVA: 0x0000363D File Offset: 0x0000183D
        // (set) Token: 0x06000078 RID: 120 RVA: 0x00003645 File Offset: 0x00001845
        public int SendingQueueSize { get; set; }

        // Token: 0x06000079 RID: 121
        public abstract void Connect(EndPoint remoteEndPoint);

        // Token: 0x0600007A RID: 122
        public abstract bool TrySend(ArraySegment<byte> segment);

        // Token: 0x0600007B RID: 123
        public abstract bool TrySend(IList<ArraySegment<byte>> segments);

        // Token: 0x0600007C RID: 124 RVA: 0x0000364E File Offset: 0x0000184E
        public void Send(byte[] data, int offset, int length)
        {
            this.Send(new ArraySegment<byte>(data, offset, length));
        }

        // Token: 0x0600007D RID: 125 RVA: 0x00003660 File Offset: 0x00001860
        public void Send(ArraySegment<byte> segment)
        {
            if (this.TrySend(segment))
            {
                return;
            }

            SpinWait spinWait = default(SpinWait);
            do
            {
                spinWait.SpinOnce();
            } while (!this.TrySend(segment));
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00003690 File Offset: 0x00001890
        public void Send(IList<ArraySegment<byte>> segments)
        {
            if (this.TrySend(segments))
            {
                return;
            }

            SpinWait spinWait = default(SpinWait);
            do
            {
                spinWait.SpinOnce();
            } while (!this.TrySend(segments));
        }

        // Token: 0x0600007F RID: 127
        public abstract void Close();

        // Token: 0x14000002 RID: 2
        // (add) Token: 0x06000080 RID: 128 RVA: 0x000036BF File Offset: 0x000018BF
        // (remove) Token: 0x06000081 RID: 129 RVA: 0x000036D8 File Offset: 0x000018D8
        public event EventHandler Closed
        {
            add { this.m_Closed = (EventHandler)Delegate.Combine(this.m_Closed, value); }
            remove { this.m_Closed = (EventHandler)Delegate.Remove(this.m_Closed, value); }
        }

        // Token: 0x06000082 RID: 130 RVA: 0x000036F4 File Offset: 0x000018F4
        protected virtual void OnClosed()
        {
            this.IsConnected = false;
            this.LocalEndPoint = null;
            EventHandler closed = this.m_Closed;
            if (closed != null)
            {
                closed(this, EventArgs.Empty);
            }
        }

        // Token: 0x14000003 RID: 3
        // (add) Token: 0x06000083 RID: 131 RVA: 0x00003725 File Offset: 0x00001925
        // (remove) Token: 0x06000084 RID: 132 RVA: 0x0000373E File Offset: 0x0000193E
        public event EventHandler<ErrorEventArgs> Error
        {
            add { this.m_Error = (EventHandler<ErrorEventArgs>)Delegate.Combine(this.m_Error, value); }
            remove { this.m_Error = (EventHandler<ErrorEventArgs>)Delegate.Remove(this.m_Error, value); }
        }

        // Token: 0x06000085 RID: 133 RVA: 0x00003758 File Offset: 0x00001958
        protected virtual void OnError(Exception e)
        {
            EventHandler<ErrorEventArgs> error = this.m_Error;
            if (error == null)
            {
                return;
            }

            error(this, new ErrorEventArgs(e));
        }

        // Token: 0x14000004 RID: 4
        // (add) Token: 0x06000086 RID: 134 RVA: 0x0000377D File Offset: 0x0000197D
        // (remove) Token: 0x06000087 RID: 135 RVA: 0x00003796 File Offset: 0x00001996
        public event EventHandler Connected
        {
            add { this.m_Connected = (EventHandler)Delegate.Combine(this.m_Connected, value); }
            remove { this.m_Connected = (EventHandler)Delegate.Remove(this.m_Connected, value); }
        }

        // Token: 0x06000088 RID: 136 RVA: 0x000037B0 File Offset: 0x000019B0
        protected virtual void OnConnected()
        {
            Socket client = this.Client;
            if (client != null)
            {
                try
                {
                    if (client.NoDelay != this.NoDelay)
                    {
                        client.NoDelay = this.NoDelay;
                    }
                }
                catch
                {
                }
            }

            this.IsConnected = true;
            EventHandler connected = this.m_Connected;
            if (connected == null)
            {
                return;
            }

            connected(this, EventArgs.Empty);
        }

        // Token: 0x14000005 RID: 5
        // (add) Token: 0x06000089 RID: 137 RVA: 0x00003814 File Offset: 0x00001A14
        // (remove) Token: 0x0600008A RID: 138 RVA: 0x0000382D File Offset: 0x00001A2D
        public event EventHandler<DataEventArgs> DataReceived
        {
            add { this.m_DataReceived = (EventHandler<DataEventArgs>)Delegate.Combine(this.m_DataReceived, value); }
            remove { this.m_DataReceived = (EventHandler<DataEventArgs>)Delegate.Remove(this.m_DataReceived, value); }
        }

        // Token: 0x0600008B RID: 139 RVA: 0x00003848 File Offset: 0x00001A48
        protected virtual void OnDataReceived(byte[] data, int offset, int length)
        {
            EventHandler<DataEventArgs> dataReceived = this.m_DataReceived;
            if (dataReceived == null)
            {
                return;
            }

            this.m_DataArgs.Data = data;
            this.m_DataArgs.Offset = offset;
            this.m_DataArgs.Length = length;
            dataReceived(this, this.m_DataArgs);
        }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x0600008C RID: 140 RVA: 0x00003891 File Offset: 0x00001A91
        // (set) Token: 0x0600008D RID: 141 RVA: 0x00003899 File Offset: 0x00001A99
        public virtual int ReceiveBufferSize { get; set; }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x0600008E RID: 142 RVA: 0x000038A2 File Offset: 0x00001AA2
        // (set) Token: 0x0600008F RID: 143 RVA: 0x000038AA File Offset: 0x00001AAA
        public IProxyConnector Proxy { get; set; }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000090 RID: 144 RVA: 0x000038B3 File Offset: 0x00001AB3
        // (set) Token: 0x06000091 RID: 145 RVA: 0x000038BB File Offset: 0x00001ABB
        protected ArraySegment<byte> Buffer { get; set; }

        // Token: 0x06000092 RID: 146 RVA: 0x000038C4 File Offset: 0x00001AC4
        void IBufferSetter.SetBuffer(ArraySegment<byte> bufferSegment)
        {
            this.SetBuffer(bufferSegment);
        }

        // Token: 0x06000093 RID: 147 RVA: 0x000038CD File Offset: 0x00001ACD
        protected virtual void SetBuffer(ArraySegment<byte> bufferSegment)
        {
            this.Buffer = bufferSegment;
        }

        // Token: 0x04000016 RID: 22
        public const int DefaultReceiveBufferSize = 4096;

        // Token: 0x0400001C RID: 28
        private EventHandler m_Closed;

        // Token: 0x0400001D RID: 29
        private EventHandler<ErrorEventArgs> m_Error;

        // Token: 0x0400001E RID: 30
        private EventHandler m_Connected;

        // Token: 0x0400001F RID: 31
        private EventHandler<DataEventArgs> m_DataReceived;

        // Token: 0x04000020 RID: 32
        private DataEventArgs m_DataArgs = new DataEventArgs();
    }
}