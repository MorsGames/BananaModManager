using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace BananaModManager.Shared;

// Based on https://github.com/zanders3/json

public static class JsonWriter
{
    public static string Serialize(this object item)
    {
        var stringBuilder = new StringBuilder();
        AppendValue(stringBuilder, item);
        return stringBuilder.ToString();
    }

    private static void AppendValue(StringBuilder stringBuilder, object item)
    {
        if (item == null)
        {
            stringBuilder.Append("null");
            return;
        }

        var type = item.GetType();
        if (type == typeof(string) || type == typeof(char))
        {
            stringBuilder.Append('"');
            var str = item.ToString();
            foreach (var character in str)
            {
                if (character is < ' ' or '"' or '\\')
                {
                    stringBuilder.Append('\\');
                    var j = "\"\\\n\r\t\b\f".IndexOf(character);
                    if (j >= 0)
                        stringBuilder.Append("\"\\nrtbf"[j]);
                    else
                        stringBuilder.AppendFormat("u{0:X4}", (uint) character);
                }
                else
                    stringBuilder.Append(character);
            }
            stringBuilder.Append('"');
        }
        else if (type == typeof(byte) || type == typeof(sbyte))
        {
            stringBuilder.Append(item);
        }
        else if (type == typeof(short) || type == typeof(ushort))
        {
            stringBuilder.Append(item);
        }
        else if (type == typeof(int) || type == typeof(uint))
        {
            stringBuilder.Append(item);
        }
        else if (type == typeof(long) || type == typeof(ulong))
        {
            stringBuilder.Append(item);
        }
        else if (type == typeof(float))
        {
            stringBuilder.Append(((float) item).ToString(CultureInfo.InvariantCulture));
        }
        else if (type == typeof(double))
        {
            stringBuilder.Append(((double) item).ToString(CultureInfo.InvariantCulture));
        }
        else if (type == typeof(decimal))
        {
            stringBuilder.Append(((decimal) item).ToString(CultureInfo.InvariantCulture));
        }
        else if (type == typeof(bool))
        {
            stringBuilder.Append((bool) item ? "true" : "false");
        }
        else if (type == typeof(DateTime))
        {
            stringBuilder.Append('"');
            stringBuilder.Append(((DateTime) item).ToString(CultureInfo.InvariantCulture));
            stringBuilder.Append('"');
        }
        else if (type.IsEnum)
        {
            stringBuilder.Append('"');
            stringBuilder.Append(item);
            stringBuilder.Append('"');
        }
        else if (item is IList list)
        {
            stringBuilder.Append('[');
            var isFirst = true;
            for (var i = 0; i < list.Count; i++)
            {
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(',');
                AppendValue(stringBuilder, list[i]);
            }
            stringBuilder.Append(']');
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var keyType = type.GetGenericArguments()[0];

            // Refuse to output dictionary keys that aren't of type string
            if (keyType != typeof(string))
            {
                stringBuilder.Append("{}");
                return;
            }

            stringBuilder.Append('{');
            var dict = item as IDictionary;
            var isFirst = true;
            foreach (var key in dict.Keys)
            {
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(',');
                stringBuilder.Append('\"');
                stringBuilder.Append((string) key);
                stringBuilder.Append("\":");
                AppendValue(stringBuilder, dict[key]);
            }
            stringBuilder.Append('}');
        }
        else
        {
            stringBuilder.Append('{');

            var isFirst = true;
            var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var fieldInfo in fieldInfos)
            {
                var value = fieldInfo.GetValue(item);
                if (value == null)
                    continue;
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(',');
                stringBuilder.Append('\"');
                stringBuilder.Append(fieldInfo.Name);
                stringBuilder.Append("\":");
                AppendValue(stringBuilder, value);
            }
            var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.CanRead)
                    continue;

                var value = propertyInfo.GetValue(item, null);
                if (value == null)
                    continue;
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(',');
                stringBuilder.Append('\"');
                stringBuilder.Append(propertyInfo.Name);
                stringBuilder.Append("\":");
                AppendValue(stringBuilder, value);
            }

            stringBuilder.Append('}');
        }
    }
}
