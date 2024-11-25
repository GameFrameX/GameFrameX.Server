using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GameFrameX.CodeGenerator.Utils;

public static class ResLoader
{
    public static string LoadTemplate(string resourceName)
    {
        var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        resourceName = $"GameFrameX.CodeGenerator.Template.{resourceName}";
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        {
            if (stream != null)
            {
                var assemblyData = new byte[stream.Length];
                _ = stream.Read(assemblyData, 0, assemblyData.Length);
                return Encoding.UTF8.GetString(assemblyData);
            }
        }

        return null;
    }

    public static void LoadDll()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
        {
            var name = new AssemblyName(args.Name);
            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == name.FullName);
            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            var resourceName = $"GameFrameX.CodeGenerator.{name.Name}.dll";
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                using (var memoryStream = new MemoryStream())
                {
                    resourceStream.CopyTo(memoryStream);
                    return Assembly.Load(memoryStream.ToArray());
                }
            }
        };
    }
}