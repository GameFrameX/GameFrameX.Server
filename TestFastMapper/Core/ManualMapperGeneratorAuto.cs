using System.Reflection;
using System.Text;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MapperAttribute : Attribute
{
    public Type SourceType { get; }
    public Type DestType { get; }
    public bool IsBidirectional { get; }

    public MapperAttribute(Type sourceType, Type destType, bool isBidirectional = false)
    {
        SourceType = sourceType;
        DestType = destType;
        IsBidirectional = isBidirectional;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ManualMapperAttribute : Attribute
{
    public Type SourceType { get; }
    public Type DestType { get; }
    public bool IsBidirectional { get; }

    public ManualMapperAttribute(Type sourceType, Type destType, bool isBidirectional = false)
    {
        SourceType = sourceType;
        DestType = destType;
        IsBidirectional = isBidirectional;
    }
}
public class ManualMapperGeneratorAuto
{
    private readonly string? _outputPath;
    private readonly HashSet<string> _generatedMappers = new();
    private readonly HashSet<(Type Source, Type Dest)> _processingTypes = new();
    private readonly Dictionary<string, string> _generatedCode = new();
    private readonly bool _isSourceGenerator;

    public ManualMapperGeneratorAuto(string outputPath)
    {
        _outputPath = outputPath;
        _isSourceGenerator = false;
    }

    public ManualMapperGeneratorAuto()
    {
        _isSourceGenerator = true;
    }

    public void GenerateMappers(Dictionary<(Type Source, Type Dest), bool> mappings)
    {
        foreach (var mapping in mappings)
        {
            GenerateMapper(mapping.Key.Source, mapping.Key.Dest);
        }

        GenerateMapperRegistration(mappings);
    }

    public void GenerateMappers(params Assembly[] assemblies)
    {
        var mappings = new Dictionary<(Type Source, Type Dest), bool>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes<ManualMapperAttribute>().Any());

            foreach (var type in types)
            {
                foreach (var attr in type.GetCustomAttributes<ManualMapperAttribute>())
                {
                    if (!mappings.ContainsKey((attr.SourceType, attr.DestType)))
                    {
                        mappings[(attr.SourceType, attr.DestType)] = attr.IsBidirectional;
                    }
                    
                    if (attr.IsBidirectional && !mappings.ContainsKey((attr.DestType, attr.SourceType)))
                    {
                        mappings[(attr.DestType, attr.SourceType)] = true;
                    }
                }
            }
        }

        GenerateMappers(mappings);
    }

    public Dictionary<string, string> GetGeneratedCode()
    {
        return _generatedCode;
    }

    private void GenerateMapper(Type sourceType, Type destType)
    {
        var mapperKey = $"{sourceType.Name}To{destType.Name}";
        if (_generatedMappers.Contains(mapperKey))
        {
            return;
        }

        if (_processingTypes.Contains((sourceType, destType)))
        {
            return;
        }
        _processingTypes.Add((sourceType, destType));

        var mappings = new StringBuilder();

        // 处理属性映射
        var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name);

        var destProps = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite);

        foreach (var destProp in destProps)
        {
            if (sourceProps.TryGetValue(destProp.Name, out var sourceProp))
            {
                var mapping = GenerateMapping(sourceProp.PropertyType, destProp.PropertyType, 
                    $"source.{sourceProp.Name}", $"dest.{destProp.Name}");
                if (!string.IsNullOrEmpty(mapping))
                {
                    mappings.AppendLine(mapping);
                }
            }
        }

        // 处理字段映射
        var sourceFields = sourceType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(f => f.Name);

        var destFields = destType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destField in destFields)
        {
            if (sourceFields.TryGetValue(destField.Name, out var sourceField))
            {
                var mapping = GenerateMapping(sourceField.FieldType, destField.FieldType,
                    $"source.{sourceField.Name}", $"dest.{destField.Name}");
                if (!string.IsNullOrEmpty(mapping))
                {
                    mappings.AppendLine(mapping);
                }
            }
        }

        var code = $@"
namespace FastMapper 
{{
    public static class {mapperKey}Mapper 
    {{
        public static {destType.FullName} Map({sourceType.FullName} source)
        {{
            if (source == null) return null;
            var dest = new {destType.FullName}();
{mappings}
            return dest;
        }}
    }}
}}";

        if (_isSourceGenerator)
        {
            _generatedCode[mapperKey] = code;
        }
        else
        {
            var filePath = Path.Combine(_outputPath!, $"{mapperKey}Mapper.g.cs");
            File.WriteAllText(filePath, code);
            Console.WriteLine($"Generated mapper: {filePath}");
        }

        _generatedMappers.Add(mapperKey);
        _processingTypes.Remove((sourceType, destType));
    }

    private string GenerateMapping(Type sourceType, Type destType, string sourcePath, string destPath)
    {
        // 处理基本类型或相同类型的直接映射
        if ((/*sourceType == destType ||*/ IsBasicType(sourceType)) && !IsListType(sourceType))
        {
            return $"            {destPath} = {sourcePath};";
        }

        // 处理List类型
        if (IsListType(sourceType) && IsListType(destType))
        {
            var sourceElementType = sourceType.GetGenericArguments()[0];
            var destElementType = destType.GetGenericArguments()[0];

            // 如果列表元素是基本类型，总是创建新的列表
            if (sourceElementType == destElementType &&  IsBasicType(sourceElementType))
            {
                return $@"            {destPath} = {sourcePath} != null ? 
                    new List<{destElementType.FullName}>({sourcePath}) : null;";
            }
            
            // 为复杂元素类型生成mapper
            GenerateMapper(sourceElementType, destElementType);

            return $@"            {destPath} = {sourcePath} != null ? 
                {sourcePath}.Select(item => 
                    {sourceElementType.Name}To{destElementType.Name}Mapper.Map(item))
                    .ToList() : null;";
        }

        // 处理Dictionary类型
        if (IsDictionaryType(sourceType) && IsDictionaryType(destType))
        {
            var sourceTypes = sourceType.GetGenericArguments();
            var destTypes = destType.GetGenericArguments();

            if (sourceTypes[0] == destTypes[0])
            {
                GenerateMapper(sourceTypes[1], destTypes[1]);

                return $@"            {destPath} = {sourcePath} != null ?
                    {sourcePath}.ToDictionary(
                        kvp => kvp.Key,
                        kvp => {sourceTypes[1].Name}To{destTypes[1].Name}Mapper.Map(kvp.Value)) : null;";
            }
        }

        // 处理复杂对象
        if (!IsBasicType(sourceType) && !IsBasicType(destType))
        {
            GenerateMapper(sourceType, destType);
            return $"            {destPath} = {sourceType.Name}To{destType.Name}Mapper.Map({sourcePath});";
        }

        return string.Empty;
    }

    private bool IsBasicType(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || 
               type == typeof(DateTime) || type == typeof(Guid) || 
               Nullable.GetUnderlyingType(type) != null;
    }

    private bool IsListType(Type type)
    {
        return type.IsGenericType && 
               (type.GetGenericTypeDefinition() == typeof(List<>) ||
                type.GetGenericTypeDefinition() == typeof(IList<>) ||
                type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    private bool IsDictionaryType(Type type)
    {
        return type.IsGenericType && 
               (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
    }

    private void GenerateMapperRegistration(Dictionary<(Type Source, Type Dest), bool> mappings)
    {
        var registrations = new StringBuilder();
        var processedPairs = new HashSet<string>();

        foreach (var mapping in mappings)
        {
            var sourceType = mapping.Key.Source;
            var destType = mapping.Key.Dest;
            var mapperKey = $"{sourceType.Name}To{destType.Name}";

            if (!processedPairs.Contains(mapperKey))
            {
                registrations.AppendLine($@"
            _mappers[(typeof({sourceType.FullName}), typeof({destType.FullName}))] = 
                ({sourceType.Name}To{destType.Name}Mapper.Map);");

                processedPairs.Add(mapperKey);
            }
        }

        var code = $@"
namespace FastMapper 
{{
    public static partial class Mapper 
    {{
        static partial void RegisterGeneratedMappers()
        {{
            {registrations}
        }}
    }}
}}";

        if (_isSourceGenerator)
        {
            _generatedCode["MapperRegistration"] = code;
        }
        else
        {
            var filePath = Path.Combine(_outputPath!, "MapperRegistration.g.cs");
            File.WriteAllText(filePath, code);
            Console.WriteLine($"Generated registration file: {filePath}");
        }
    }
}