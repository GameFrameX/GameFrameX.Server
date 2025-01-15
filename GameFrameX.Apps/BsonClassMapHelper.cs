using GameFrameX.Utility.Log;

namespace GameFrameX.Apps;

public class DictionaryRepresentationConvention : ConventionBase, IMemberMapConvention
{
    private readonly DictionaryRepresentation _dictionaryRepresentation;

    public DictionaryRepresentationConvention(DictionaryRepresentation dictionaryRepresentation = DictionaryRepresentation.ArrayOfDocuments)
    {
        _dictionaryRepresentation = dictionaryRepresentation;
    }

    public void Apply(BsonMemberMap memberMap)
    {
        var serializer = memberMap.GetSerializer();
        if (serializer is IDictionaryRepresentationConfigurable dictionaryRepresentationConfigurable)
        {
            var reconfiguredSerializer = dictionaryRepresentationConfigurable.WithDictionaryRepresentation(_dictionaryRepresentation);
            memberMap.SetSerializer(reconfiguredSerializer);
        }
    }
}

public class EmptyContainerSerializeMethodConvention : ConventionBase, IMemberMapConvention
{
    public void Apply(BsonMemberMap memberMap)
    {
        if (memberMap.MemberType.IsGenericType)
        {
            var genType = memberMap.MemberType.GetGenericTypeDefinition();
            if (genType == typeof(List<>))
            {
                memberMap.SetShouldSerializeMethod(o =>
                {
                    var value = memberMap.Getter(o);
                    if (value is IList list)
                    {
                        return list != null && list.Count > 0;
                    }

                    return true;
                });
            }

            else if (genType == typeof(ConcurrentDictionary<,>) || genType == typeof(Dictionary<,>))
            {
                memberMap.SetShouldSerializeMethod(o =>
                {
                    if (o != null)
                    {
                        var value = memberMap.Getter(o);
                        if (value != null)
                        {
                            var countProperty = value.GetType().GetProperty("Count");
                            if (countProperty != null)
                            {
                                var count = (int)countProperty.GetValue(value, null);
                                return count > 0;
                            }
                        }
                    }

                    return true;
                });
            }
        }
    }
}

public static class BsonClassMapHelper
{
    public static void SetConvention()
    {
        ConventionRegistry.Register(nameof(DictionaryRepresentationConvention),
                                    new ConventionPack { new DictionaryRepresentationConvention(), }, _ => true);

        ConventionRegistry.Register(nameof(EmptyContainerSerializeMethodConvention),
                                    new ConventionPack { new EmptyContainerSerializeMethodConvention(), }, _ => true);
    }

    /// <summary>
    /// 提前注册,简化多态类型处理
    /// </summary>
    /// <param name="assembly"></param>
    public static void RegisterAllClass(Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var t in types)
        {
            try
            {
                if (!BsonClassMap.IsClassMapRegistered(t))
                {
                    RegisterClass(t);
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
            }
        }
    }

    public static void RegisterClass(Type t)
    {
        var bsonClassMap = new BsonClassMap(t);
        bsonClassMap.AutoMap();
        bsonClassMap.SetIgnoreExtraElements(true);
        bsonClassMap.SetIgnoreExtraElementsIsInherited(true);
        BsonClassMap.RegisterClassMap(bsonClassMap);
    }
}