using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Launcher.Common.Session
{
    public class Session
    {
        /// <summary>
        /// 全局会话ID
        /// </summary>
        public string Id { get; }

        public long ActorId
        {
            get { return WorkChannel.GetData<long>(GlobalConst.ActorIdKey); }
            private set { WorkChannel.SetData(GlobalConst.ActorIdKey, value); }
        }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; private set; }

        /// <summary>
        /// 设置角色ID
        /// </summary>
        /// <param name="roleId"></param>
        public void SetRoleId(long roleId)
        {
            RoleId = roleId;
        }

        /// <summary>
        /// 设置ActorId
        /// </summary>
        /// <param name="actorId"></param>
        public void SetActorId(long actorId)
        {
            ActorId = actorId;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sessionId">连接会话ID</param>
        /// <param name="netWorkChannel">网络渠道对象</param>
        public Session(string sessionId, INetWorkChannel netWorkChannel)
        {
            WorkChannel = netWorkChannel;
            Id = sessionId;
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime CreateTime { get; }

        /// <summary>
        /// 连接上下文
        /// </summary>
        public INetWorkChannel WorkChannel { get; }

        /// <summary>
        /// 连接标示，避免自己顶自己的号,客户端每次启动游戏生成一次/或者每个设备一个
        /// </summary>
        public string Sign { get; private set; }

        /// <summary>
        /// 设置签名
        /// </summary>
        /// <param name="sign">签名</param>
        public void SetSign(string sign)
        {
            Sign = sign;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageObject"></param>
        public void WriteAsync(MessageObject messageObject)
        {
            WorkChannel?.Write(messageObject);
        }
    }
}