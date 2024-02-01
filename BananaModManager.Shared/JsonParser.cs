using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace BananaModManager.Shared;

// Based on https://github.com/zanders3/json

public static class JsonParser
{
    [ThreadStatic]
    private static Stack<List<string>> _splitArrayPool;
    [ThreadStatic]
    private static StringBuilder _stringBuilder;
    [ThreadStatic]
    private static Dictionary<Type, Dictionary<string, FieldInfo>> _fieldInfoCache;
    [ThreadStatic]
    private static Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyInfoCache;

    public static T Deserialize<T>(this string json)
    {
        // Initialize, if needed, the ThreadStatic variables
        _propertyInfoCache ??= new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        _fieldInfoCache ??= new Dictionary<Type, Dictionary<string, FieldInfo>>();
        _stringBuilder ??= new StringBuilder();
        _splitArrayPool ??= new Stack<List<string>>();

        // Remove all whitespace not within strings to make parsing simpler
        _stringBuilder.Length = 0;
        for (var i = 0; i < json.Length; i++)
        {
            var c = json[i];
            if (c == '"')
            {
                i = AppendUntilStringEnd(true, i, json);
                continue;
            }
            if (char.IsWhiteSpace(c))
                continue;

            _stringBuilder.Append(c);
        }

        // Parse the thing!
        return (T) ParseValue(typeof(T), _stringBuilder.ToString());
    }

    private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
    {
        _stringBuilder.Append(json[startIdx]);
        for (var i = startIdx + 1; i < json.Length; i++)
        {
            switch (json[i])
            {
                case '\\':
                {
                    if (appendEscapeCharacter)
                        _stringBuilder.Append(json[i]);
                    _stringBuilder.Append(json[i + 1]);
                    // Skip next character as it is escaped
                    i++;
                    break;
                }
                case '"':
                    _stringBuilder.Append(json[i]);
                    return i;
                default:
                    _stringBuilder.Append(json[i]);
                    break;
            }
        }
        return json.Length - 1;
    }

    //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
    private static List<string> Split(string json)
    {
        var splitArray = _splitArrayPool.Count > 0 ? _splitArrayPool.Pop() : new List<string>();
        splitArray.Clear();
        if (json.Length == 2)
            return splitArray;
        var parseDepth = 0;
        _stringBuilder.Length = 0;
        for (var i = 1; i < json.Length - 1; i++)
        {
            switch (json[i])
            {
                case '[':
                case '{':
                    parseDepth++;
                    break;
                case ']':
                case '}':
                    parseDepth--;
                    break;
                case '"':
                    i = AppendUntilStringEnd(true, i, json);
                    continue;
                case ',':
                case ':':
                    if (parseDepth == 0)
                    {
                        splitArray.Add(_stringBuilder.ToString());
                        _stringBuilder.Length = 0;
                        continue;
                    }
                    break;
            }

            _stringBuilder.Append(json[i]);
        }

        splitArray.Add(_stringBuilder.ToString());

        return splitArray;
    }

    private static object ParseValue(Type type, string json)
    {
        if (type == typeof(string))
        {
            if (json.Length <= 2)
                return string.Empty;
            var parseStringBuilder = new StringBuilder(json.Length);
            for (var i = 1; i < json.Length - 1; ++i)
            {
                if (json[i] == '\\' && i + 1 < json.Length - 1)
                {
                    var j = "\"\\nrtbf/".IndexOf(json[i + 1]);
                    if (j >= 0)
                    {
                        parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                        ++i;
                        continue;
                    }
                    if (json[i + 1] == 'u' && i + 5 < json.Length - 1)
                    {
                        uint c = 0;
                        if (uint.TryParse(json.Substring(i + 2, 4), NumberStyles.AllowHexSpecifier, null, out c))
                        {
                            parseStringBuilder.Append((char) c);
                            i += 5;
                            continue;
                        }
                    }
                }
                parseStringBuilder.Append(json[i]);
            }
            return parseStringBuilder.ToString();
        }
        if (type.IsPrimitive)
        {
            var result = Convert.ChangeType(json, type, CultureInfo.InvariantCulture);
            return result;
        }
        if (type == typeof(decimal))
        {
            decimal.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            return result;
        }
        if (type == typeof(DateTime))
        {
            DateTime.TryParse(json.Replace("\"", ""), CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            return result;
        }
        if (json == "null")
        {
            return null;
        }
        if (type.IsEnum)
        {
            if (json[0] == '"')
                json = json.Substring(1, json.Length - 2);
            try
            {
                return Enum.Parse(type, json, false);
            }
            catch
            {
                return 0;
            }
        }
        if (type.IsArray)
        {
            var arrayType = type.GetElementType();
            if (json[0] != '[' || json[json.Length - 1] != ']')
                return null;

            var elems = Split(json);
            var newArray = Array.CreateInstance(arrayType, elems.Count);
            for (var i = 0; i < elems.Count; i++)
                newArray.SetValue(ParseValue(arrayType, elems[i]), i);
            _splitArrayPool.Push(elems);
            return newArray;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var listType = type.GetGenericArguments()[0];
            if (json[0] != '[' || json[json.Length - 1] != ']')
                return null;

            var elems = Split(json);
            var list = (IList) type.GetConstructor(new[]
            {
                typeof(int)
            }).Invoke(new object[]
            {
                elems.Count
            });
            for (var i = 0; i < elems.Count; i++)
            {
                list.Add(ParseValue(listType, elems[i]));
            }
            _splitArrayPool.Push(elems);
            return list;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            Type keyType, valueType;
            {
                var args = type.GetGenericArguments();
                keyType = args[0];
                valueType = args[1];
            }

            // Refuse to parse dictionary keys that aren't of type string
            if (keyType != typeof(string))
                return null;
            // Must be a valid dictionary element
            if (json[0] != '{' || json[json.Length - 1] != '}')
                return null;
            // The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            var elems = Split(json);
            if (elems.Count % 2 != 0)
                return null;

            var dictionary = (IDictionary) type.GetConstructor(new[]
            {
                typeof(int)
            }).Invoke(new object[]
            {
                elems.Count / 2
            });
            for (var i = 0; i < elems.Count; i += 2)
            {
                if (elems[i].Length <= 2)
                    continue;
                var keyValue = elems[i].Substring(1, elems[i].Length - 2);
                var val = ParseValue(valueType, elems[i + 1]);
                dictionary[keyValue] = val;
            }
            return dictionary;
        }
        if (type == typeof(object))
        {
            return ParseAnonymousValue(json);
        }
        if (json[0] == '{' && json[json.Length - 1] == '}')
        {
            return ParseObject(type, json);
        }

        return null;
    }

    private static object ParseAnonymousValue(string json)
    {
        if (json.Length == 0)
            return null;
        if (json[0] == '{' && json[json.Length - 1] == '}')
        {
            var elems = Split(json);
            if (elems.Count % 2 != 0)
                return null;
            var dict = new Dictionary<string, object>(elems.Count / 2);
            for (var i = 0; i < elems.Count; i += 2)
                dict[elems[i].Substring(1, elems[i].Length - 2)] = ParseAnonymousValue(elems[i + 1]);
            return dict;
        }
        if (json[0] == '[' && json[json.Length - 1] == ']')
        {
            var items = Split(json);
            var finalList = new List<object>(items.Count);
            for (var i = 0; i < items.Count; i++)
            {
                finalList.Add(ParseAnonymousValue(items[i]));
            }
            return finalList;
        }
        if (json[0] == '"' && json[json.Length - 1] == '"')
        {
            var str = json.Substring(1, json.Length - 2);
            return str.Replace("\\", string.Empty);
        }
        if (char.IsDigit(json[0]) || json[0] == '-')
        {
            if (json.Contains("."))
            {
                double.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
                return result;
            }
            else
            {
                int.TryParse(json, out var result);
                return result;
            }
        }
        return json switch
        {
            "true" => true,
            "false" => false,
            _ => null
        };
    }

    private static Dictionary<string, T> CreateMemberNameDictionary<T>(T[] members) where T : MemberInfo
    {
        var nameToMember = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        foreach (var member in members)
        {
            nameToMember.Add(member.Name, member);
        }

        return nameToMember;
    }

    private static object ParseObject(Type type, string json)
    {
        var instance = FormatterServices.GetUninitializedObject(type);

        // The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
        var elems = Split(json);
        if (elems.Count % 2 != 0)
            return instance;

        if (!_fieldInfoCache.TryGetValue(type, out var nameToField))
        {
            nameToField = CreateMemberNameDictionary(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
            _fieldInfoCache.Add(type, nameToField);
        }
        if (!_propertyInfoCache.TryGetValue(type, out var nameToProperty))
        {
            nameToProperty = CreateMemberNameDictionary(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
            _propertyInfoCache.Add(type, nameToProperty);
        }

        for (var i = 0; i < elems.Count; i += 2)
        {
            if (elems[i].Length <= 2)
                continue;
            var key = elems[i].Substring(1, elems[i].Length - 2);
            var value = elems[i + 1];

            if (nameToField.TryGetValue(key, out var fieldInfo))
                fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value));
            else if (nameToProperty.TryGetValue(key, out var propertyInfo))
                propertyInfo.SetValue(instance, ParseValue(propertyInfo.PropertyType, value), null);
        }

        return instance;
    }
}
