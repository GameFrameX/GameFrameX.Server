using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FastMapper.Generator
{
    [Generator]
    public class MapperGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MapperSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is MapperSyntaxReceiver receiver))
                return;

            var generator = new ManualMapperGeneratorAuto(); // 使用无参构造函数，表示是Source Generator模式
            var typeConverter = new CompileTimeTypeConverter(context);

            // 将编译时类型转换为运行时类型给生成器使用
            var mappings = receiver.Mappings.ToDictionary(
                kvp => (typeConverter.GetRuntimeType(kvp.Key.Source), 
                       typeConverter.GetRuntimeType(kvp.Key.Dest)),
                kvp => kvp.Value);

            // 生成代码
            generator.GenerateMappers(mappings);

            // 获取生成的代码并添加到编译中
            foreach (var generatedCode in generator.GetGeneratedCode())
            {
                context.AddSource($"{generatedCode.Key}.g.cs", 
                    SourceText.From(generatedCode.Value, Encoding.UTF8));
            }
        }
    }

    public class MapperSyntaxReceiver : ISyntaxContextReceiver
    {
        public Dictionary<(ITypeSymbol Source, ITypeSymbol Dest), bool> Mappings { get; } = 
            new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration)
            {
                var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
                if (typeSymbol == null) return;

                foreach (var attribute in typeSymbol.GetAttributes())
                {
                    if (attribute.AttributeClass?.Name != "ManualMapperAttribute")
                        continue;

                    var sourceTypeData = attribute.ConstructorArguments[0];
                    var destTypeData = attribute.ConstructorArguments[1];
                    var isBidirectional = false;
                    if (attribute.ConstructorArguments.Length > 2)
                    {
                        isBidirectional = (bool)attribute.ConstructorArguments[2].Value;
                    }

                    var sourceType = context.SemanticModel.GetTypeInfo(
                        (TypeOfExpressionSyntax)sourceTypeData.Value).Type;
                    var destType = context.SemanticModel.GetTypeInfo(
                        (TypeOfExpressionSyntax)destTypeData.Value).Type;

                    if (sourceType != null && destType != null)
                    {
                        Mappings[(sourceType, destType)] = isBidirectional;
                        if (isBidirectional)
                        {
                            Mappings[(destType, sourceType)] = true;
                        }
                    }
                }
            }
        }
    }

    internal class CompileTimeTypeConverter
    {
        private readonly GeneratorExecutionContext _context;

        public CompileTimeTypeConverter(GeneratorExecutionContext context)
        {
            _context = context;
        }

        public Type GetRuntimeType(ITypeSymbol typeSymbol)
        {
            return new RuntimeTypeInfo(typeSymbol);
        }
    }

internal class RuntimeTypeInfo : Type
{
    private readonly ITypeSymbol _symbol;

    public RuntimeTypeInfo(ITypeSymbol symbol)
    {
        _symbol = symbol;
    }

    public override string FullName => _symbol.ToDisplayString();
    public override string Name => _symbol.Name;
    public override string Namespace => _symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;

    public override bool IsGenericType => _symbol is INamedTypeSymbol { IsGenericType: true };
    
    public override Type[] GetGenericArguments()
    {
        if (_symbol is INamedTypeSymbol namedType)
        {
            return namedType.TypeArguments
                .Select(t => new RuntimeTypeInfo(t))
                .ToArray();
        }
        return Array.Empty<Type>();
    }

    public override PropertyInfo[] GetProperties(BindingFlags bindingFlags)
    {
        var properties = _symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Select(p => new RuntimePropertyInfo(p))
            .Cast<PropertyInfo>()
            .ToArray();
        return properties;
    }

    // 必需实现的基类成员
    public override Assembly Assembly => typeof(object).Assembly;
    public override string AssemblyQualifiedName => FullName;
    public override Type BaseType => typeof(object);
    public override bool IsEnum => _symbol.TypeKind == TypeKind.Enum;
    
    public  bool IsPrimitive => _symbol.SpecialType switch
    {
        SpecialType.System_Boolean or
        SpecialType.System_Byte or
        SpecialType.System_SByte or
        SpecialType.System_Int16 or
        SpecialType.System_UInt16 or
        SpecialType.System_Int32 or
        SpecialType.System_UInt32 or
        SpecialType.System_Int64 or
        SpecialType.System_UInt64 or
        SpecialType.System_Char or
        SpecialType.System_Double or
        SpecialType.System_Single => true,
        _ => false
    };

    // 根据需要实现的其他Type成员
    public override Type GetElementType() => throw new NotImplementedException();
    protected override TypeAttributes GetAttributeFlagsImpl() => TypeAttributes.Public;
    protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder,
        CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => 
        throw new NotImplementedException();
    protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder,
        CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => 
        throw new NotImplementedException();
    public override Type UnderlyingSystemType => this;
    protected override bool HasElementTypeImpl() => false;
    protected override bool IsArrayImpl() => false;
    protected override bool IsByRefImpl() => false;
    protected override bool IsCOMObjectImpl() => false;
    protected override bool IsPointerImpl() => false;
    protected override bool IsPrimitiveImpl() => IsPrimitive;

    public override object[] GetCustomAttributes(bool inherit) => Array.Empty<object>();
    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => Array.Empty<object>();
    public override bool IsDefined(Type attributeType, bool inherit) => false;
    
    public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => 
        GetProperties(bindingAttr).Cast<MemberInfo>().ToArray();

    public override Type MakeGenericType(params Type[] typeArguments) => 
        throw new NotImplementedException();

    // 实现缺失的抽象成员
    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => throw new NotImplementedException();
    public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
    public override EventInfo[] GetEvents(BindingFlags bindingAttr) => throw new NotImplementedException();
    public override FieldInfo GetField(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
    public override FieldInfo[] GetFields(BindingFlags bindingAttr) => throw new NotImplementedException();
    public override Type GetInterface(string name, bool ignoreCase) => throw new NotImplementedException();
    public override Type[] GetInterfaces() => throw new NotImplementedException();
    public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => throw new NotImplementedException();
    public override Type GetNestedType(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
    public override Type[] GetNestedTypes(BindingFlags bindingAttr) => throw new NotImplementedException();
    protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();
    public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) => throw new NotImplementedException();
    public override Guid GUID => throw new NotImplementedException();
    public override Module Module => throw new NotImplementedException();
}


    internal class RuntimePropertyInfo : PropertyInfo
    {
        private readonly IPropertySymbol _symbol;

        public RuntimePropertyInfo(IPropertySymbol symbol)
        {
            _symbol = symbol;
        }

        public override string Name => _symbol.Name;
        public override Type PropertyType => new RuntimeTypeInfo(_symbol.Type);
        public override Type DeclaringType => new RuntimeTypeInfo(_symbol.ContainingType);
        public override Type ReflectedType => DeclaringType;

        public override bool CanRead => _symbol.GetMethod != null;
        public override bool CanWrite => _symbol.SetMethod != null;

        public override PropertyAttributes Attributes => PropertyAttributes.None;

        // 必需实现的其他成员
        public override MethodInfo[] GetAccessors(bool nonPublic) => Array.Empty<MethodInfo>();
        public override MethodInfo GetGetMethod(bool nonPublic) => throw new NotImplementedException();
        public override MethodInfo GetSetMethod(bool nonPublic) => throw new NotImplementedException();
        public override ParameterInfo[] GetIndexParameters() => Array.Empty<ParameterInfo>();
        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, 
            object[] index, System.Globalization.CultureInfo culture) => 
            throw new NotImplementedException();
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder,
            object[] index, System.Globalization.CultureInfo culture) => 
            throw new NotImplementedException();
        public override object[] GetCustomAttributes(bool inherit) => Array.Empty<object>();
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => 
            Array.Empty<object>();
        public override bool IsDefined(Type attributeType, bool inherit) => false;
    }
}