using System;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine
{
    // Token: 0x0200000D RID: 13
    public class AsyncTcpSession : TcpClientSession
    {
        // Token: 0x06000057 RID: 87 RVA: 0x00003012 File Offset: 0x00001212
        protected override void SocketEventArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Connect)
            {
                base.ProcessConnect(sender as Socket, null, e, null);
                return;
            }

            this.ProcessReceive(e);
        }

        // Token: 0x06000058 RID: 88 RVA: 0x00003034 File Offset: 0x00001234
        protected override void SetBuffer(ArraySegment<byte> bufferSegment)
        {
            base.SetBuffer(bufferSegment);
            if (this.m_SocketEventArgs != null)
            {
                this.m_SocketEventArgs.SetBuffer(bufferSegment.Array, bufferSegment.Offset, bufferSegment.Count);
            }
        }

        // Token: 0x06000059 RID: 89 RVA: 0x00003068 File Offset: 0x00001268
        protected override void OnGetSocket(SocketAsyncEventArgs e)
        {
            if (base.Buffer.Array == null)
            {
                int num = this.ReceiveBufferSize;
                if (num <= 0)
                {
                    num = 4096;
                }

                this.ReceiveBufferSize = num;
                base.Buffer = new ArraySegment<byte>(new byte[num]);
            }

            e.SetBuffer(base.Buffer.Array, base.Buffer.Offset, base.Buffer.Count);
            this.m_SocketEventArgs = e;
            this.OnConnected();
            this.StartReceive();
        }

        // Token: 0x0600005A RID: 90 RVA: 0x000030F1 File Offset: 0x000012F1
        private void BeginReceive()
        {
            if (!base.Client.ReceiveAsync(this.m_SocketEventArgs))
            {
                this.ProcessReceive(this.m_SocketEventArgs);
            }
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00003114 File Offset: 0x00001314
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                if (base.EnsureSocketClosed())
                {
                    this.OnClosed();
                }

                if (!base.IsIgnorableSocketError((int)e.SocketError))
                {
                    this.OnError(new SocketException((int)e.SocketError));
                }

                return;
            }

            if (e.BytesTransferred == 0)
            {
                if (base.EnsureSocketClosed())
                {
                    this.OnClosed();
                }

                return;
            }

            this.OnDataReceived(e.Buffer, e.Offset, e.BytesTransferred);
            this.StartReceive();
        }

        // Token: 0x0600005C RID: 92 RVA: 0x0000318C File Offset: 0x0000138C
        private void StartReceive()
        {
            Socket client = base.Client;
            if (client == null)
            {
                return;
            }

            bool flag;
            try
            {
                flag = client.ReceiveAsync(this.m_SocketEventArgs);
            }
            catch (SocketException ex)
            {
                int socketErrorCode = (int)ex.SocketErrorCode;
                if (!base.IsIgnorableSocketError(socketErrorCode))
                {
                    this.OnError(ex);
                }

                if (base.EnsureSocketClosed(client))
                {
                    this.OnClosed();
                }

                return;
            }
            catch (Exception e)
            {
                if (!this.IsIgnorableException(e))
                {
                    this.OnError(e);
                }

                if (base.EnsureSocketClosed(client))
                {
                    this.OnClosed();
                }

                return;
            }

            if (!flag)
            {
                this.ProcessReceive(this.m_SocketEventArgs);
            }
        }

        // Token: 0x0600005D RID: 93 RVA: 0x00003230 File Offset: 0x00001430
        protected override void SendInternal(PosList<ArraySegment<byte>> items)
        {
            if (this.m_SocketEventArgsSend == null)
            {
                this.m_SocketEventArgsSend = new SocketAsyncEventArgs();
                this.m_SocketEventArgsSend.Completed += this.Sending_Completed;
            }

            bool flag;
            try
            {
                if (items.Count > 1)
                {
                    if (this.m_SocketEventArgsSend.Buffer != null)
                    {
                        this.m_SocketEventArgsSend.SetBuffer(null, 0, 0);
                    }

                    this.m_SocketEventArgsSend.BufferList = items;
                }
                else
                {
                    ArraySegment<byte> arraySegment = items[0];
                    try
                    {
                        if (this.m_SocketEventArgsSend.BufferList != null)
                        {
                            this.m_SocketEventArgsSend.BufferList = null;
                        }
                    }
                    catch
                    {
                    }

                    this.m_SocketEventArgsSend.SetBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
                }

                flag = base.Client.SendAsync(this.m_SocketEventArgsSend);
            }
            catch (SocketException ex)
            {
                int socketErrorCode = (int)ex.SocketErrorCode;
                if (base.EnsureSocketClosed() && !base.IsIgnorableSocketError(socketErrorCode))
                {
                    this.OnError(ex);
                }

                return;
            }
            catch (Exception e)
            {
                if (base.EnsureSocketClosed() && this.IsIgnorableException(e))
                {
                    this.OnError(e);
                }

                return;
            }

            if (!flag)
            {
                this.Sending_Completed(base.Client, this.m_SocketEventArgsSend);
            }
        }

        // Token: 0x0600005E RID: 94 RVA: 0x00003374 File Offset: 0x00001574
        private void Sending_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success || e.BytesTransferred == 0)
            {
                if (base.EnsureSocketClosed())
                {
                    this.OnClosed();
                }

                if (e.SocketError != SocketError.Success && !base.IsIgnorableSocketError((int)e.SocketError))
                {
                    this.OnError(new SocketException((int)e.SocketError));
                }

                return;
            }

            base.OnSendingCompleted();
        }

        // Token: 0x0600005F RID: 95 RVA: 0x000033CD File Offset: 0x000015CD
        protected override void OnClosed()
        {
            if (this.m_SocketEventArgsSend != null)
            {
                this.m_SocketEventArgsSend.Dispose();
                this.m_SocketEventArgsSend = null;
            }

            if (this.m_SocketEventArgs != null)
            {
                this.m_SocketEventArgs.Dispose();
                this.m_SocketEventArgs = null;
            }

            base.OnClosed();
        }

        // Token: 0x04000012 RID: 18
        private SocketAsyncEventArgs m_SocketEventArgs;

        // Token: 0x04000013 RID: 19
        private SocketAsyncEventArgs m_SocketEventArgsSend;
    }
}