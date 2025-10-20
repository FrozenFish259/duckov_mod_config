using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Options;
using UnityEngine;

using Utilities;

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

        //要显示的当前选项字符串
        public override string GetCurrentOption()
        {
            object currentValue = GetCurrentValue();

            // 在选项中查找对应的显示文本
            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.Value.Equals(currentValue))
                    {
                        return option.Key;  // 返回显示文本，如"选项1"
                    }
                }
            }

            // 如果没有找到匹配项，返回第一个选项的显示文本
            return options?.First().Key ?? currentValue?.ToString() ?? "null";
        }

        public override string[] GetOptions()
        {
            return options?.Keys.ToArray() ?? Array.Empty<string>();
        }

        public override void Set(int index)
        {
            if (options == null || index < 0 || index >= options.Count)
                return;

            // 获取选中的键值对
            var selectedOption = options.ElementAt(index);
            object selectedValue = selectedOption.Value;

            // 根据值类型保存到 OptionsManager
            SaveValue(selectedValue);

            // 可以在这里添加额外的逻辑，比如应用设置等
            ApplySetting(selectedValue);
        }

        /// <summary>
        /// 根据值类型获取当前存储的值
        /// </summary>
        private object GetCurrentValue()
        {
            if (valueType == typeof(int))
                return OptionsManager.Load<int>(this.Key, (int)defaultValue);
            else if (valueType == typeof(float))
                return OptionsManager.Load<float>(this.Key, (float)defaultValue);
            else if (valueType == typeof(double))
                return OptionsManager.Load<double>(this.key, (double)defaultValue);
            else if (valueType == typeof(string))
                return OptionsManager.Load<string>(this.Key, (string)defaultValue);
            else if (valueType == typeof(bool))
                return OptionsManager.Load<bool>(this.Key, (bool)defaultValue);
            else
            {
                Debug.LogError($"不支持的配置值类型: {valueType}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 根据值类型保存值
        /// </summary>
        private void SaveValue(object value)
        {
            if (valueType == typeof(int))
                OptionsManager.Save<int>(this.Key, (int)value);
            else if (valueType == typeof(float))
                OptionsManager.Save<float>(this.Key, (float)value);
            else if (valueType == typeof(double))
                OptionsManager.Save<double>(this.Key, (double)value);
            else if (valueType == typeof(string))
                OptionsManager.Save<string>(this.Key, (string)value);
            else if (valueType == typeof(bool))
                OptionsManager.Save<bool>(this.Key, (bool)value);
            else
                Debug.LogError($"不支持的配置值类型: {valueType}");
        }

        /// <summary>
        /// 应用设置（可以根据需要扩展）
        /// </summary>
        private void ApplySetting(object value)
        {
            // 这里可以根据不同的key和value执行不同的逻辑
            // 例如：
            // if (this.Key == "QualitySetting")
            // {
            //     QualitySettings.SetQualityLevel((int)value);
            // }
            // else if (this.Key == "Resolution")
            // {
            //     // 设置分辨率逻辑
            // }

            Debug.Log($"应用设置: {this.Key} = {value}");
        }

        /// <summary>
        /// 获取当前选中的索引
        /// </summary>
        public int GetCurrentIndex()
        {
            if (options == null || options.Count == 0)
                return 0;

            object currentValue = GetCurrentValue();

            for (int i = 0; i < options.Count; i++)
            {
                var option = options.ElementAt(i);
                if (option.Value.Equals(currentValue))
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// 根据值获取对应的显示文本
        /// </summary>
        public string GetOptionByValue(object value)
        {
            if (options == null)
                return string.Empty;

            foreach (var option in options)
            {
                if (option.Value.Equals(value))
                    return option.Key;
            }

            return options.First().Key;
        }

        public void init(string key, string description,
            SortedDictionary<string, object> options, Type valueType, object defaultValue)
        {
            try
            {
                Debug.Log($"=== DropDownOptionsProvider 构造函数开始 ===");
                Debug.Log($"参数 - key: {key}, description: {description}");
                Debug.Log($"参数 - options: {options != null}, valueType: {valueType}, defaultValue: {defaultValue}");

                this.key = key;
                this.description = description;
                this.options = options;
                this.valueType = valueType;
                this.defaultValue = defaultValue;

                Debug.Log($"字段设置完成 - Key: {this.key}, OptionsCount: {this.options?.Count ?? 0}");
                Debug.Log($"this == null: {this == null}");
                Debug.Log($"=== DropDownOptionsProvider 构造函数结束 ===");
            }
            catch (Exception ex)
            {
                Debug.LogError($"DropDownOptionsProvider 构造函数异常: {ex.Message}\n{ex.StackTrace}");
                // 这里不要重新抛出异常，否则会看到真正的错误
            }
        }
    }
}