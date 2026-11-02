@REM 游戏服务器（零依赖快速启动：SQLite 嵌入式，数据落运行目录 data/ 子目录）
start dotnet GameFrameX.Launcher.dll --AllInOne --DatabaseProvider=Sqlite --DataBaseUrl="Data Source=data/gameframex.db"