using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BananaModManager.Shared;

namespace BananaModManager
{
    internal class ConfigPropertyGridAdapter : ICustomTypeDescriptor
    {
        private readonly Dictionary<string, ConfigItem> _currentConfig;
        private readonly Dictionary<string, ConfigItem> _defaultConfig;

        public ConfigPropertyGridAdapter(Dictionary<string, ConfigItem> currentConfig,
            Dictionary<string, ConfigItem> defaultConfig)
        {   
            _currentConfig = currentConfig;
            _defaultConfig = defaultConfig;
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _currentConfig;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor) this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = new ArrayList();
            foreach (var e in _currentConfig)
            {
                var name = Regex.Replace(e.Key, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
                var item = e.Value;
                properties.Add(new ConfigPropertyDescriptor(_currentConfig, _defaultConfig, e.Key, name,
                    item.Description, item.Category));
            }

            var props =
                (PropertyDescriptor[]) properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }
    }
}