using System;

namespace LanguageExt.Bson.Serialization
{
    public static class TypeHelper
    {
        public static bool IsSubclassOfGeneric(this Type type, Type generic, out Type found) {
            while (type != null && type != typeof(object)) {
                var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == current)
                {
                    found = type;
                    return true;
                }
                type = type.BaseType;
            }
            found = null;
            return false;
        }
    }
}