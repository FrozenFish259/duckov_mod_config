using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModConfig
{
    class DropDownOptionsProvider : OptionsProviderBase
    {
        private SortedDictionary<string, object>? options = null;
        private string key = "";
        private string description = "";
        private Type valueType;
        private object defaultValue;

        public override string Key => this.key;

        public override string GetCurrentOption()
        {
            throw new NotImplementedException();
        }

        public override string[] GetOptions()
        {
            return options?.Keys.ToArray<string>() ?? Array.Empty<string>();
        }

        public override void Set(int index)
        {
            throw new NotImplementedException();
        }

        public DropDownOptionsProvider(string key, string description, 
            SortedDictionary<string, object> options, Type valueType, object defaultValue) {
            this.key = key;
            this.description = description;
            this.options = options;
            this.valueType = valueType;
            this.defaultValue = defaultValue;
        }
    }
}
