using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Actors;
using GameFrameX.Setting;

namespace GameFrameX.Core.Utility
{
    /// <summary>
    /// ActorId
    ///     14   +   7  + 30 +  12   = 63
    ///     服务器id 类型 时间戳 自增
    ///         玩家
    ///         公会
    ///     服务器id * 1000 + 全局功能id
    ///         全局玩法
    /// </summary>
    public static class IdGenerator
    {
        private static long _genSecond = 0L;
        private static long _incrNum   = 0L;

        //此时间决定可用id年限(最晚有效年限=34年+此时间)(可调整,早于开服时间就行)
        private static readonly DateTime UtcTimeStart = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly object LockObj = new();

        /// <summary>
        /// 根据ActorId获取服务器id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetServerId(long id)
        {
            return (int)(id < GlobalConst.MaxGlobalId ? id / 1000 : id >> GlobalConst.ServerIdOrModuleIdMask);
        }

        /// <summary>
        /// 根据ActorId获取生成时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="utc"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DateTime GetGenerateTime(long id, bool utc = false)
        {
            if (id < GlobalConst.MaxGlobalId)
            {
                throw new ArgumentException($"input is a global id:{id}");
            }

            var  serverId = GetServerId(id);
            long seconds;
            if (serverId < GlobalConst.MinServerId)
            {
                // IDModule unique_id
                seconds = (id >> GlobalConst.ModuleIdTimestampMask) & GlobalConst.SecondMask;
            }
            else
            {
                seconds = (id >> GlobalConst.TimestampMask) & GlobalConst.SecondMask;
            }

            var date = UtcTimeStart.AddSeconds(seconds);
            return utc ? date : date.ToLocalTime();
        }

        /// <summary>
        /// 根据ActorId获取ActorType
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ActorType GetActorType(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException($"input id error:{id}");
            }

            if (id < GlobalConst.MaxGlobalId)
            {
                // 全局actor
                return (ActorType)(id % 1000);
            }

            return (ActorType)((id >> GlobalConst.ActorTypeMask) & 0xF);
        }


        /// <summary>
        /// 根据ActorType获取ActorId
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static long GetActorId(ActorType type, int serverId = 0)
        {
            if (type == ActorType.Separator)
            {
                throw new ArgumentException($"input actor type error: {type}");
            }

            if (serverId < 0)
            {
                throw new ArgumentException($"serverId negtive when generate id {serverId}");
            }
            else if (serverId == 0)
            {
                serverId = GlobalSettings.ServerId;
            }

            if (type < ActorType.Separator)
            {
                return GetMultiActorId(type, serverId);
            }
            else
            {
                return GetGlobalActorId(type, serverId);
            }
        }

        /// <summary>
        /// 根据ActorType获取ActorId开始值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static long GetMultiActorIdBegin(ActorType type)
        {
            if (type >= ActorType.Separator)
            {
                throw new ArgumentException($"input actor type error: {type}");
            }

            var id = (long)GlobalSettings.ServerId << GlobalConst.ServerIdOrModuleIdMask;
            id |= (long)type << GlobalConst.ActorTypeMask;
            return id;
        }

        private static long GetGlobalActorId(ActorType type, int serverId)
        {
            return (long)(serverId * 1000 + type);
        }


        private static long GetMultiActorId(ActorType type, int serverId)
        {
            long second = (long)(DateTime.UtcNow - UtcTimeStart).TotalSeconds;
            lock (LockObj)
            {
                if (second > _genSecond)
                {
                    _genSecond = second;
                    _incrNum   = 0L;
                }
                else if (_incrNum >= GlobalConst.MaxActorIncrease)
                {
                    ++_genSecond;
                    _incrNum = 0L;
                }
                else
                {
                    ++_incrNum;
                }
            }

            var id = (long)serverId << GlobalConst.ServerIdOrModuleIdMask; // serverId-14位, 支持1000~9999
            id |= (long)type << GlobalConst.ActorTypeMask; // 多actor类型-7位, 支持0~127
            id |= _genSecond << GlobalConst.TimestampMask; // 时间戳-30位, 支持34年
            id |= _incrNum; // 自增-12位, 每秒4096个
            return id;
        }

        /// <summary>
        /// 根据模块获取唯一ID
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static long GetUniqueId(IdModule module)
        {
            long second = (long)(DateTime.UtcNow - UtcTimeStart).TotalSeconds;
            lock (LockObj)
            {
                if (second > _genSecond)
                {
                    _genSecond = second;
                    _incrNum   = 0L;
                }
                else if (_incrNum >= GlobalConst.MaxUniqueIncrease)
                {
                    ++_genSecond;
                    _incrNum = 0L;
                }
                else
                {
                    ++_incrNum;
                }
            }

            var id = (long)module << GlobalConst.ServerIdOrModuleIdMask; // 模块id 14位 支持 0~9999
            lock (LockObj)
            {
                id |= _genSecond << GlobalConst.ModuleIdTimestampMask; // 时间戳 30位
            }

            id |= _incrNum; // 自增 19位
            return id;
        }
    }
}