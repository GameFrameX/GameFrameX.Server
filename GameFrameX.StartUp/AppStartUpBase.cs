using System.Net;
using System.Timers;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.Setting;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using Timer = System.Timers.Timer;

namespace GameFrameX.StartUp
{
    /// <summary>
    /// 程序启动器基类
    /// </summary>
    public abstract class AppStartUpBase : IAppStartUp
    {
        /// <summary>
        /// 服务器类型
        /// </summary>
        public ServerType ServerType { get; private set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public AppSetting Setting { get; protected set; }

        /// <summary>
        /// 重连间隔时间。单位毫秒,默认为5秒
        /// </summary>
        protected virtual int ReconnectInterval { get; } = 5000;

        /// <summary>
        /// 心跳间隔时间。单位毫秒,默认为15秒
        /// </summary>
        protected virtual int HeartBeatInterval { get; } = 15000;

        /// <summary>
        /// 应用退出
        /// </summary>
        protected readonly TaskCompletionSource<string> AppExitSource = new TaskCompletionSource<string>();

        /// <summary>
        /// 重连定时器
        /// </summary>
        protected Timer ReconnectionTimer;

        /// <summary>
        /// 是否启用重连
        /// </summary>
        protected virtual bool IsEnableReconnection { get; } = true;

        /// <summary>
        /// 心跳定时器
        /// </summary>
        protected Timer HeartBeatTimer { get; set; }

        /// <summary>
        /// 是否启用心跳
        /// </summary>
        protected virtual bool IsEnableHeartBeat { get; } = true;

        /// <summary>
        /// 应用退出
        /// </summary>
        public Task<string> AppExitToken
        {
            get { return AppExitSource.Task; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serverType">服务器类型</param>
        /// <param name="setting">配置信息对象</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public bool Init(ServerType serverType, BaseSetting setting, string[] args = null)
        {
            ServerType = serverType;
            Setting    = (AppSetting)setting;
            Init();
            Setting.CheckNotNull(nameof(Setting));
            GlobalSettings.ServerId = Setting.ServerId;
            if (IsEnableHeartBeat)
            {
                HeartBeatTimer         =  new Timer(HeartBeatInterval);
                HeartBeatTimer.Elapsed += HeartBeatTimerOnElapsed;
            }

            if (IsEnableReconnection)
            {
                ReconnectionTimer         =  new Timer(ReconnectInterval);
                ReconnectionTimer.Elapsed += ReconnectionTimerOnElapsed;
            }

            return true;
        }

        /// <summary>
        /// 心跳定时器回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
        }

        /// <summary>
        /// 重连定时器回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
        }

        /// <summary>
        /// 配置启动,当InnerIP为空时.将使用Any
        /// </summary>
        /// <param name="options"></param>
        protected virtual void ConfigureSuperSocket(ServerOptions options)
        {
            options.AddListener(new ListenOptions { Ip = string.IsNullOrEmpty(Setting.InnerIp) ? IPAddress.Any.ToString() : Setting.InnerIp, Port = Setting.InnerPort });
        }


        /// <summary>
        /// 启动
        /// </summary>
        public abstract Task StartAsync();

        /// <summary>
        /// 终止服务器
        /// </summary>
        /// <param name="message">终止原因</param>
        public virtual async Task StopAsync(string message = "")
        {
            ReconnectionTimer?.Stop();
            HeartBeatTimer?.Stop();
            LogHelper.Error(message);
            AppExitSource?.TrySetResult(message);
            await Task.CompletedTask;
        }
    }
}