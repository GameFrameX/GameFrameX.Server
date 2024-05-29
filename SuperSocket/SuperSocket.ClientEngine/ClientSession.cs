using System.Net;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine
{
    public abstract class ClientSession : IClientSession, IBufferSetter
    {
        protected Socket Client { get; set; }

        public string SessionID { get; private set; }

        public ValueTask SendAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            return SendAsync(new ArraySegment<byte>(data), cancellationToken);
        }

        public ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            Send(data.ToArray(), 0, data.Length);
            return ValueTask.CompletedTask;
        }


        Socket IClientSession.Socket
        {
            get { return this.Client; }
        }

        public virtual EndPoint LocalEndPoint { get; set; }


        public bool IsConnected { get; private set; }


        public bool NoDelay { get; set; }

        public ClientSession()
        {
            SessionID = Guid.NewGuid().ToString();
        }


        public int SendingQueueSize { get; set; }


        public abstract void Connect(EndPoint remoteEndPoint);


        public abstract bool TrySend(ArraySegment<byte> segment);


        public abstract bool TrySend(IList<ArraySegment<byte>> segments);

        public void Send(byte[] data, int offset, int length)
        {
            this.Send(new ArraySegment<byte>(data, offset, length));
        }

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

        public abstract void Close();

        public event EventHandler Closed
        {
            add { this.m_Closed = (EventHandler)Delegate.Combine(this.m_Closed, value); }
            remove { this.m_Closed = (EventHandler)Delegate.Remove(this.m_Closed, value); }
        }

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

        public event EventHandler<ErrorEventArgs> Error
        {
            add { this.m_Error = (EventHandler<ErrorEventArgs>)Delegate.Combine(this.m_Error, value); }
            remove { this.m_Error = (EventHandler<ErrorEventArgs>)Delegate.Remove(this.m_Error, value); }
        }

        protected virtual void OnError(Exception e)
        {
            EventHandler<ErrorEventArgs> error = this.m_Error;
            if (error == null)
            {
                return;
            }

            error(this, new ErrorEventArgs(e));
        }

        public event EventHandler Connected
        {
            add { this.m_Connected = (EventHandler)Delegate.Combine(this.m_Connected, value); }
            remove { this.m_Connected = (EventHandler)Delegate.Remove(this.m_Connected, value); }
        }

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

        public event EventHandler<DataEventArgs> DataReceived
        {
            add { this.m_DataReceived = (EventHandler<DataEventArgs>)Delegate.Combine(this.m_DataReceived, value); }
            remove { this.m_DataReceived = (EventHandler<DataEventArgs>)Delegate.Remove(this.m_DataReceived, value); }
        }

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

        public virtual int ReceiveBufferSize { get; set; }

        public IProxyConnector Proxy { get; set; }

        protected ArraySegment<byte> Buffer { get; set; }

        void IBufferSetter.SetBuffer(ArraySegment<byte> bufferSegment)
        {
            this.SetBuffer(bufferSegment);
        }

        protected virtual void SetBuffer(ArraySegment<byte> bufferSegment)
        {
            this.Buffer = bufferSegment;
        }

        public const int DefaultReceiveBufferSize = 4096;

        private EventHandler m_Closed;

        private EventHandler<ErrorEventArgs> m_Error;


        private EventHandler m_Connected;


        private EventHandler<DataEventArgs> m_DataReceived;


        private DataEventArgs m_DataArgs = new DataEventArgs();
    }
}