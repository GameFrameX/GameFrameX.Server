using System;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine
{
	// Token: 0x0200000B RID: 11
	public class ProxyEventArgs : EventArgs
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00002F4C File Offset: 0x0000114C
		public ProxyEventArgs(Socket socket) : this(true, socket, null, null)
		{
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002F58 File Offset: 0x00001158
		public ProxyEventArgs(Socket socket, string targetHostHame) : this(true, socket, targetHostHame, null)
		{
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002F64 File Offset: 0x00001164
		public ProxyEventArgs(Exception exception) : this(false, null, null, exception)
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002F70 File Offset: 0x00001170
		public ProxyEventArgs(bool connected, Socket socket, string targetHostName, Exception exception)
		{
			this.Connected = connected;
			this.Socket = socket;
			this.TargetHostName = targetHostName;
			this.Exception = exception;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002F95 File Offset: 0x00001195
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002F9D File Offset: 0x0000119D
		public bool Connected { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002FA6 File Offset: 0x000011A6
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002FAE File Offset: 0x000011AE
		public Socket Socket { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002FB7 File Offset: 0x000011B7
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002FBF File Offset: 0x000011BF
		public Exception Exception { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002FC8 File Offset: 0x000011C8
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002FD0 File Offset: 0x000011D0
		public string TargetHostName { get; private set; }
	}
}
