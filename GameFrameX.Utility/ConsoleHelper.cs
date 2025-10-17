// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

namespace GameFrameX.Utility;

/// <summary>
/// 控制台辅助器
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// 打印控制台logo
    /// </summary>
    public static void ConsoleLogo()
    {
        Console.WriteLine();
        Console.WriteLine(@"        _____                         ______                               __   __     ");
        Console.WriteLine(@"       |  __ \                        |  ___|                              \ \ / /     ");
        Console.WriteLine(@"       | |  \/  __ _  _ __ ___    ___ | |_    _ __   __ _  _ __ ___    ___  \ V /      ");
        Console.WriteLine(@"       | | __  / _` || '_ ` _ \  / _ \|  _|  | '__| / _` || '_ ` _ \  / _ \ /   \      ");
        Console.WriteLine(@"       | |_\ \| (_| || | | | | ||  __/| |    | |   | (_| || | | | | ||  __// /^\ \     ");
        Console.WriteLine(@"        \____/ \__,_||_| |_| |_| \___|\_|    |_|    \__,_||_| |_| |_| \___|\/   \/     ");
        Console.WriteLine();
        Console.WriteLine(@"        GitHub Repository[项目主页]:https://github.com/GameFrameX/GameFrameX            ");
        Console.WriteLine(@"        Gitee Repository[项目主页]:https://gitee.com/GameFrameX/GameFrameX              ");
        Console.WriteLine(@"        Official Documentation[官方文档]:https://gameframex.doc.alianblank.com          ");
        Console.WriteLine();
        Console.WriteLine();
    }
}