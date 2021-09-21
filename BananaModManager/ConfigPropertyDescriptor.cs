using System;
using System.Collections.Generic;
using System.ComponentModel;
using BananaModManager.Shared;

namespace BananaModManager
{
    internal class ConfigPropertyDescriptor : PropertyDescriptor
    {
        private readonly Dictionary<string, ConfigItem> _currentConfig;
        private readonly Dictionary<string, ConfigItem> _defaultConfig;
        private readonly string _key;

        internal ConfigPropertyDescriptor(Dictionary<string, ConfigItem> currentConfig,
            Dictionary<string, ConfigItem> defaultConfig, string key, string displayName, string description,
            string category)
            : base(displayName ?? string.Empty,
                new Attribute[] {new DescriptionAttribute(description), new CategoryAttribute(category)})
        {
            _currentConfig = currentConfig;
            _defaultConfig = defaultConfig;
            _key = key;
        }

        public override Type ComponentType => null;

        public override bool IsReadOnly => false;

        public override Type PropertyType => _currentConfig[_key].Value.GetType();


        public override void SetValue(object component, object value)
        {
            _currentConfig[_key].Value = value;
        }

        public override object GetValue(object component)
        {
            return _currentConfig[_key].Value;
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override void ResetValue(object component)
        {
            _currentConfig[_key].Value = _defaultConfig[_key].Value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return _currentConfig[_key].Value.ToString() != _defaultConfig[_key].Value.ToString();
        }
    }
}