#if !NO_RUNTIME
using System.Reflection;

namespace ProtoBuf.Meta;

internal abstract class AttributeMap
{
    public abstract Type AttributeType { get; }

    public abstract object Target { get; }
#if DEBUG
    [Obsolete("Please use AttributeType instead")]
    public new Type GetType()
    {
        return AttributeType;
    }
#endif
    public override string ToString()
    {
        return AttributeType?.FullName ?? "";
    }

    public abstract bool TryGet(string key, bool publicOnly, out object value);

    public bool TryGet(string key, out object value)
    {
        return TryGet(key, true, out value);
    }

    public static AttributeMap[] Create(TypeModel model, Type type, bool inherit)
    {
#if COREFX || PROFILE259
			Attribute[] all = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.OfType<Attribute>(type.GetTypeInfo().GetCustomAttributes(inherit)));
#else
        var all = type.GetCustomAttributes(inherit);
#endif
        var result = new AttributeMap[all.Length];
        for (var i = 0; i < all.Length; i++)
        {
            result[i] = new ReflectionAttributeMap((Attribute)all[i]);
        }

        return result;
    }

    public static AttributeMap[] Create(TypeModel model, MemberInfo member, bool inherit)
    {
#if COREFX || PROFILE259
			Attribute[] all = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.OfType<Attribute>(member.GetCustomAttributes(inherit)));
#else
        var all = member.GetCustomAttributes(inherit);
#endif
        var result = new AttributeMap[all.Length];
        for (var i = 0; i < all.Length; i++)
        {
            result[i] = new ReflectionAttributeMap((Attribute)all[i]);
        }

        return result;
    }

    public static AttributeMap[] Create(TypeModel model, Assembly assembly)
    {
#if COREFX || PROFILE259
			Attribute[] all = System.Linq.Enumerable.ToArray(assembly.GetCustomAttributes());
#else
        const bool inherit = false;
        var all = assembly.GetCustomAttributes(inherit);
#endif
        var result = new AttributeMap[all.Length];
        for (var i = 0; i < all.Length; i++)
        {
            result[i] = new ReflectionAttributeMap((Attribute)all[i]);
        }

        return result;
    }

    private sealed class ReflectionAttributeMap : AttributeMap
    {
        private readonly Attribute attribute;

        public ReflectionAttributeMap(Attribute attribute)
        {
            this.attribute = attribute;
        }

        public override object Target
        {
            get { return attribute; }
        }

        public override Type AttributeType
        {
            get { return attribute.GetType(); }
        }

        public override bool TryGet(string key, bool publicOnly, out object value)
        {
            var members = Helpers.GetInstanceFieldsAndProperties(attribute.GetType(), publicOnly);
            foreach (var member in members)
            {
                if (string.Equals(member.Name, key, StringComparison.OrdinalIgnoreCase))
                {
                    if (member is PropertyInfo prop)
                    {
                        value = prop.GetValue(attribute, null);
                        return true;
                    }

                    if (member is FieldInfo field)
                    {
                        value = field.GetValue(attribute);
                        return true;
                    }

                    throw new NotSupportedException(member.GetType().Name);
                }
            }

            value = null;
            return false;
        }
    }
}
#endif