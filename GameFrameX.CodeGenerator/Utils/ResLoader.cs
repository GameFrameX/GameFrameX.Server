using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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
        AssemblyLoadContext.Default.Resolving += (_, assemblyName) =>
        {
            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == assemblyName.FullName);
            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            var resourceName = $"GameFrameX.CodeGenerator.{assemblyName.Name}.dll";
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                var memoryStream = new MemoryStream();
                resourceStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return AssemblyLoadContext.Default.LoadFromStream(memoryStream);
            }
        };
    }
}