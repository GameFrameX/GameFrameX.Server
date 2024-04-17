using System;

namespace SuperSocket.ClientEngine
{
	// Token: 0x02000011 RID: 17
	public class ErrorEventArgs : EventArgs
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00003911 File Offset: 0x00001B11
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00003919 File Offset: 0x00001B19
		public Exception Exception { get; private set; }

		// Token: 0x0600009D RID: 157 RVA: 0x00003922 File Offset: 0x00001B22
		public ErrorEventArgs(Exception exception)
		{
			this.Exception = exception;
		}
	}
}
