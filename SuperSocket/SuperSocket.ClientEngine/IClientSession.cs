using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000013 RID: 19
    public interface IClientSession
    {
        // Token: 0x17000020 RID: 32
        // (get) Token: 0x0600009F RID: 159
        Socket Socket { get; }

        // Token: 0x17000021 RID: 33
        // (get) Token: 0x060000A0 RID: 160
        // (set) Token: 0x060000A1 RID: 161
        IProxyConnector Proxy { get; set; }

        // Token: 0x17000022 RID: 34
        // (get) Token: 0x060000A2 RID: 162
        // (set) Token: 0x060000A3 RID: 163
        int ReceiveBufferSize { get; set; }

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x060000A4 RID: 164
        // (set) Token: 0x060000A5 RID: 165
        int SendingQueueSize { get; set; }

        // Token: 0x17000024 RID: 36
        // (get) Token: 0x060000A6 RID: 166
        bool IsConnected { get; }

        // Token: 0x060000A7 RID: 167
        void Connect(EndPoint remoteEndPoint);

        // Token: 0x060000A8 RID: 168
        void Send(ArraySegment<byte> segment);

        // Token: 0x060000A9 RID: 169
        void Send(IList<ArraySegment<byte>> segments);

        // Token: 0x060000AA RID: 170
        void Send(byte[] data, int offset, int length);

        // Token: 0x060000AB RID: 171
        bool TrySend(ArraySegment<byte> segment);

        // Token: 0x060000AC RID: 172
        bool TrySend(IList<ArraySegment<byte>> segments);

        // Token: 0x060000AD RID: 173
        void Close();

        // Token: 0x14000006 RID: 6
        // (add) Token: 0x060000AE RID: 174
        // (remove) Token: 0x060000AF RID: 175
        event EventHandler Connected;

        // Token: 0x14000007 RID: 7
        // (add) Token: 0x060000B0 RID: 176
        // (remove) Token: 0x060000B1 RID: 177
        event EventHandler Closed;

        // Token: 0x14000008 RID: 8
        // (add) Token: 0x060000B2 RID: 178
        // (remove) Token: 0x060000B3 RID: 179
        event EventHandler<ErrorEventArgs> Error;

        // Token: 0x14000009 RID: 9
        // (add) Token: 0x060000B4 RID: 180
        // (remove) Token: 0x060000B5 RID: 181
        event EventHandler<DataEventArgs> DataReceived;
    }
}