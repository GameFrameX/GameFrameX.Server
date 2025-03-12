using System.Linq;
using System.Threading.Tasks;
using GameFrameX.Apps.Common.Event;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Apps.Common.Session;

/// <summary>
/// 管理玩家session，一个玩家一个，下线之后移除，顶号之后释放之前的channel，替换channel
/// </summary>
public static class SessionManager
{
    private static readonly ConcurrentDictionary<string, Session> SessionMap = new();

    /// <summary>
    /// 获取当前在线玩家的数量。
    /// </summary>
    /// <returns>当前在线玩家的数量。</returns>
    public static int Count()
    {
        return SessionMap.Count;
    }

    /// <summary>
    /// 获取分页的玩家列表。
    /// </summary>
    /// <param name="pageSize">每页的玩家数量。</param>
    /// <param name="pageIndex">当前页的索引，从0开始。</param>
    /// <returns>指定页的玩家会话列表。</returns>
    public static List<Session> GetPageList(int pageSize, int pageIndex)
    {
        var result = SessionMap.Values.OrderBy(m => m.CreateTime)
            .Where(m => ActorManager.HasActor(m.RoleId))
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToList();
        return result;
    }

    /// <summary>
    /// 踢掉指定角色ID的玩家，移除其会话。
    /// </summary>
    /// <param name="roleId">要踢掉的玩家的角色ID。</param>
    public static void KickOffLineByUserId(long roleId)
    {
        var roleSession = Get(m => m.RoleId == roleId);
        if (roleSession != null)
        {
            if (SessionMap.TryRemove(roleSession.Id, out var value) && ActorManager.HasActor(roleSession.RoleId))
            {
                EventDispatcher.Dispatch(roleSession.RoleId, (int)EventId.SessionRemove);
            }
        }
    }

    /// <summary>
    /// 根据角色ID获取对应的会话对象。
    /// 会话对象必须已经存在才会返回。
    /// </summary>
    /// <param name="roleId">角色ID。</param>
    /// <returns>对应的会话对象，如果不存在则返回null。</returns>
    public static Session GetByRoleId(long roleId)
    {
        var roleSession = Get(m => m.RoleId == roleId);
        if (roleSession != null && ActorManager.HasActor(roleSession.RoleId))
        {
            return roleSession;
        }

        return roleSession;
    }

    /// <summary>
    /// 根据会话ID获取连接的会话对象。
    /// </summary>
    /// <param name="sessionId">会话ID。</param>
    /// <returns>对应的会话对象，如果不存在则返回null。</returns>
    public static Session Get(string sessionId)
    {
        SessionMap.TryGetValue(sessionId, out var value);
        return value;
    }

    /// <summary>
    /// 根据指定的查询条件获取会话对象。
    /// </summary>
    /// <param name="predicate">查询条件的委托。</param>
    /// <returns>符合条件的会话对象，如果不存在则返回null。</returns>
    public static Session Get(Func<Session, bool> predicate)
    {
        return SessionMap.Values.FirstOrDefault(predicate);
    }

    /// <summary>
    /// 根据指定的查询条件获取会话对象列表。
    /// </summary>
    /// <param name="predicate">查询条件的委托。</param>
    /// <returns>符合条件的会话对象列表。</returns>
    public static List<Session> GetList(Func<Session, bool> predicate)
    {
        return SessionMap.Values.Where(predicate).ToList();
    }

    /// <summary>
    /// 移除指定会话ID的玩家。
    /// </summary>
    /// <param name="sessionId">要移除的会话ID。</param>
    /// <returns>被移除的会话对象，如果不存在则返回null。</returns>
    public static Session Remove(string sessionId)
    {
        if (SessionMap.TryRemove(sessionId, out var value) && ActorManager.HasActor(value.RoleId))
        {
            EventDispatcher.Dispatch(value.RoleId, (int)EventId.SessionRemove);
        }

        return value;
    }

    /// <summary>
    /// 移除所有在线玩家的会话。
    /// </summary>
    /// <returns>一个表示异步操作的任务。</returns>
    public static Task RemoveAll()
    {
        foreach (var session in SessionMap.Values)
        {
            if (ActorManager.HasActor(session.RoleId))
            {
                EventDispatcher.Dispatch(session.RoleId, (int)EventId.SessionRemove);
            }
        }

        SessionMap.Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取指定会话ID的网络连接通道。
    /// </summary>
    /// <param name="sessionId">会话ID。</param>
    /// <returns>对应的网络连接通道，如果不存在则返回null。</returns>
    public static INetWorkChannel GetChannel(string sessionId)
    {
        SessionMap.TryGetValue(sessionId, out var session);
        return session?.WorkChannel;
    }

    /// <summary>
    /// 添加新的连接会话。
    /// </summary>
    /// <param name="session">要添加的会话对象。</param>
    public static void Add(Session session)
    {
        session.WorkChannel.SetData(GlobalConst.SessionIdKey, session.Id);
        SessionMap[session.Id] = session;
    }

    /// <summary>
    /// 更新会话，处理角色ID和签名的更新。
    /// 如果角色ID已在其他设备上登录，则会通知旧会话并关闭其连接。
    /// </summary>
    /// <param name="sessionId">会话ID，用于标识当前会话</param>
    /// <param name="roleId">角色ID，表示当前会话所关联的角色</param>
    /// <param name="sign">签名，用于验证会话的唯一性</param>
    public static async void UpdateSession(string sessionId, long roleId, string sign)
    {
        // 获取与角色ID关联的旧会话
        var oldSession = GetByRoleId(roleId);
        if (oldSession != null)
        {
            // 创建提示消息，通知用户其账号已在其他设备上登录
            var msg = new RespPrompt
            {
                Type = 5,
                Content = "你的账号已在其他设备上登陆",
            };
            // 发送消息给旧会话
            await oldSession.WriteAsync(msg);
            // 清除旧会话的连接数据并关闭连接
            oldSession.WorkChannel.ClearData();
            oldSession.WorkChannel.Close();

            // 这里先移除，等待Disconnected回调断开太慢了
            Remove(oldSession.Id);
        }

        // 获取当前会话并更新角色ID和签名
        var session = Get(sessionId);
        session.SetRoleId(roleId);
        session.SetSign(sign);
    }
}