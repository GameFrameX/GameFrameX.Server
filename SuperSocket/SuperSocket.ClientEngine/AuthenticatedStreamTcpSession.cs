using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

namespace SuperSocket.ClientEngine
{
	// Token: 0x0200000E RID: 14
	public abstract class AuthenticatedStreamTcpSession : TcpClientSession
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00003409 File Offset: 0x00001609
		public AuthenticatedStreamTcpSession()
		{
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00003411 File Offset: 0x00001611
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00003419 File Offset: 0x00001619
		public SecurityOption Security { get; set; }

		// Token: 0x06000063 RID: 99 RVA: 0x00003422 File Offset: 0x00001622
		protected override void SocketEventArgsCompleted(object sender, SocketAsyncEventArgs e)
		{
			base.ProcessConnect(sender as Socket, null, e, null);
		}

		// Token: 0x06000064 RID: 100
		protected abstract void StartAuthenticatedStream(Socket client);

		// Token: 0x06000065 RID: 101 RVA: 0x00003434 File Offset: 0x00001634
		protected override void OnGetSocket(SocketAsyncEventArgs e)
		{
			try
			{
				this.StartAuthenticatedStream(base.Client);
			}
			catch (Exception e2)
			{
				if (!this.IsIgnorableException(e2))
				{
					this.OnError(e2);
				}
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003474 File Offset: 0x00001674
		protected void OnAuthenticatedStreamConnected(AuthenticatedStream stream)
		{
			this.m_Stream = stream;
			this.OnConnected();
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
			this.BeginRead();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000034CD File Offset: 0x000016CD
		private void BeginRead()
		{
			this.ReadAsync();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000034D8 File Offset: 0x000016D8
		private async void ReadAsync()
		{
			while (base.IsConnected)
			{
				if (base.Client != null && this.m_Stream != null)
				{
					ArraySegment<byte> buffer = base.Buffer;
					int length = 0;
					try
					{
						int num = await this.m_Stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count, CancellationToken.None);
						length = num;
					}
					catch (Exception e)
					{
						if (!this.IsIgnorableException(e))
						{
							this.OnError(e);
						}
						if (base.EnsureSocketClosed(base.Client))
						{
							this.OnClosed();
						}
						break;
					}
					if (length != 0)
					{
						this.OnDataReceived(buffer.Array, buffer.Offset, length);
						buffer = default(ArraySegment<byte>);
						continue;
					}
					if (base.EnsureSocketClosed(base.Client))
					{
						this.OnClosed();
					}
				}
				return;
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003514 File Offset: 0x00001714
		protected override bool IsIgnorableException(Exception e)
		{
			if (base.IsIgnorableException(e))
			{
				return true;
			}
			if (e is IOException)
			{
				if (e.InnerException is ObjectDisposedException)
				{
					return true;
				}
				if (e.InnerException is IOException && e.InnerException.InnerException is ObjectDisposedException)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003565 File Offset: 0x00001765
		protected override void SendInternal(PosList<ArraySegment<byte>> items)
		{
			this.SendInternalAsync(items);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003570 File Offset: 0x00001770
		private async void SendInternalAsync(PosList<ArraySegment<byte>> items)
		{
			try
			{
				for (int i = items.Position; i < items.Count; i++)
				{
					ArraySegment<byte> arraySegment = items[i];
					await this.m_Stream.WriteAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count, CancellationToken.None);
				}
				this.m_Stream.Flush();
			}
			catch (Exception e)
			{
				if (!this.IsIgnorableException(e))
				{
					this.OnError(e);
				}
				if (base.EnsureSocketClosed(base.Client))
				{
					this.OnClosed();
				}
				return;
			}
			base.OnSendingCompleted();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000035B4 File Offset: 0x000017B4
		public override void Close()
		{
			AuthenticatedStream stream = this.m_Stream;
			if (stream != null)
			{
				stream.Dispose();
				this.m_Stream = null;
			}
			base.Close();
		}

		// Token: 0x04000014 RID: 20
		private AuthenticatedStream m_Stream;

		// Token: 0x02000022 RID: 34
		private class StreamAsyncState
		{
			// Token: 0x17000037 RID: 55
			// (get) Token: 0x0600012E RID: 302 RVA: 0x0000541B File Offset: 0x0000361B
			// (set) Token: 0x0600012F RID: 303 RVA: 0x00005423 File Offset: 0x00003623
			public AuthenticatedStream Stream { get; set; }

			// Token: 0x17000038 RID: 56
			// (get) Token: 0x06000130 RID: 304 RVA: 0x0000542C File Offset: 0x0000362C
			// (set) Token: 0x06000131 RID: 305 RVA: 0x00005434 File Offset: 0x00003634
			public Socket Client { get; set; }

			// Token: 0x17000039 RID: 57
			// (get) Token: 0x06000132 RID: 306 RVA: 0x0000543D File Offset: 0x0000363D
			// (set) Token: 0x06000133 RID: 307 RVA: 0x00005445 File Offset: 0x00003645
			public PosList<ArraySegment<byte>> SendingItems { get; set; }
		}
	}
}
