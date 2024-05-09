using System;

namespace SuperSocket.ClientEngine.Proxy
{
    // Token: 0x0200001B RID: 27
    internal class ReceiveState
    {
        // Token: 0x0600010A RID: 266 RVA: 0x00004983 File Offset: 0x00002B83
        public ReceiveState(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        // Token: 0x17000031 RID: 49
        // (get) Token: 0x0600010B RID: 267 RVA: 0x00004992 File Offset: 0x00002B92
        // (set) Token: 0x0600010C RID: 268 RVA: 0x0000499A File Offset: 0x00002B9A
        public byte[] Buffer { get; private set; }

        // Token: 0x17000032 RID: 50
        // (get) Token: 0x0600010D RID: 269 RVA: 0x000049A3 File Offset: 0x00002BA3
        // (set) Token: 0x0600010E RID: 270 RVA: 0x000049AB File Offset: 0x00002BAB
        public int Length { get; set; }
    }
}