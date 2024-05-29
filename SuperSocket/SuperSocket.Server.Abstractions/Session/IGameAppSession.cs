using System;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSocket.Server.Abstractions.Session;

public interface IGameAppSession
{
    /// <summary>
    /// Session unique Id   
    /// </summary>
    string SessionID { get; }

    /// <summary>
    /// Send data to client
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask SendAsync(byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send data to client
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}