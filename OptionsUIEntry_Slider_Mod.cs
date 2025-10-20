using Duckov.Options;
using SodaCraft.Localizations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModConfig
{
    class OptionsUIEntry_Slider_Mod : MonoBehaviour
    {
        public object Value
        {
            get
            {
                try
                {
                    if (valueType == typeof(int))
                        return OptionsManager.Load<int>(this.key, (int)defaultValue);
                    else if (valueType == typeof(float))
                    {
                        Debug.Log("正在读取float配置默认值");
                        float defaultV = Convert.ToSingle(defaultValue);
                        Debug.Log("float配置默认值为" + defaultV.ToString("F1"));
                        float loaded = OptionsManager.Load<float>(this.key, defaultV);
                        Debug.Log("float配置数值已读取=" + loaded.ToString("F1"));

                        return loaded;
                    }
                    else if (valueType == typeof(string))
                        return OptionsManager.Load<string>(this.key, (string)defaultValue);
                    else if (valueType == typeof(bool))
                        return OptionsManager.Load<bool>(this.key, (bool)defaultValue);
                    else
                    {
                        Debug.LogError($"不支持的配置值类型: {valueType}");
                        return defaultValue;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"获取配置值时发生错误: {ex.Message} valueType={valueType} key={key}");
                    return defaultValue;
                }
            }
            set
            {
                try
                {
                    if (valueType == typeof(int))
                    {
                        int intValue = Convert.ToInt32(value);
                        OptionsManager.Save<int>(this.key, intValue);
                    }
                    else if (valueType == typeof(float))
                    {
                        float floatValue = Convert.ToSingle(value);
                        OptionsManager.Save<float>(this.key, floatValue);
                    }
                    else if (valueType == typeof(string))
                    {
                        OptionsManager.Save<string>(this.key, value?.ToString() ?? "");
                    }
                    else if (valueType == typeof(bool))
                    {
                        bool boolValue = Convert.ToBoolean(value);
                        OptionsManager.Save<bool>(this.key, boolValue);
                    }
                    else
                    {
                        Debug.LogError($"不支持保存的配置值类型: {valueType}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"保存配置值时发生错误: {ex.Message}");
                }
            }
        }

        private void Awake()
        {
            // 确保组件已经初始化
            if (this.slider != null)
                this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));

            if (this.valueField != null)
                this.valueField.onEndEdit.AddListener(new UnityAction<string>(this.OnFieldEndEdit));

            this.RefreshLable();
            LocalizationManager.OnSetLanguage += this.OnLanguageChanged;
        }

        private void Start()
        {
            // 在Start中确保初始化完成后再刷新值
            this.RefreshValues();
        }

        private void OnDestroy()
        {
            LocalizationManager.OnSetLanguage -= this.OnLanguageChanged;
        }

        private void OnLanguageChanged(SystemLanguage language)
        {
            this.RefreshLable();
        }

        private void RefreshLable()
        {
            // 保持原有的标签刷新逻辑
        }

        private void OnFieldEndEdit(string arg0)
        {
            try
            {
                if (valueType == typeof(int))
                {
                    if (int.TryParse(arg0, out int intValue))
                    {
                        if (this.slider != null)
                            intValue = Mathf.Clamp(intValue, (int)this.slider.minValue, (int)this.slider.maxValue);
                        this.Value = intValue;
                    }
                }
                else if (valueType == typeof(float))
                {
                    if (float.TryParse(arg0, out float floatValue))
                    {
                        if (this.slider != null)
                            floatValue = Mathf.Clamp(floatValue, this.slider.minValue, this.slider.maxValue);
                        this.Value = floatValue;
                    }
                }
                else if (valueType == typeof(string))
                {
                    this.Value = arg0;
                }
                else if (valueType == typeof(bool))
                {
                    if (bool.TryParse(arg0, out bool boolValue))
                    {
                        this.Value = boolValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理输入框值时发生错误: {ex.Message}");
            }

            this.RefreshValues();
        }

        private void OnEnable()
        {
            this.RefreshValues();
        }

        private void OnSliderValueChanged(float value)
        {
            this.Value = value;
            this.RefreshValues();
        }

        private void RefreshValues()
        {
            if (!initDone)
            {
                return;
            }

            try
            {
                object currentValue = this.Value;

                if (valueType == typeof(int))
                {
                    int intValue = Convert.ToInt32(currentValue);
                    if (this.valueField != null)
                        this.valueField.SetTextWithoutNotify(intValue.ToString());
                    if (this.slider != null)
                        this.slider.SetValueWithoutNotify(intValue);
                }
                else if (valueType == typeof(float))
                {
                    float floatValue = Convert.ToSingle(currentValue);
                    if (this.valueField != null)
                        this.valueField.SetTextWithoutNotify(floatValue.ToString("F2"));
                    if (this.slider != null)
                        this.slider.SetValueWithoutNotify(floatValue);
                }
                else if (valueType == typeof(string))
                {
                    // 不需要slider
                    if (this.valueField != null)
                        this.valueField.SetTextWithoutNotify((string)currentValue);
                    if (this.slider != null)
                        this.slider.gameObject.SetActive(false);
                }
                else if (valueType == typeof(bool))
                {
                    // 不需要slider
                    bool boolValue = Convert.ToBoolean(currentValue);
                    if (this.valueField != null)
                        this.valueField.SetTextWithoutNotify(boolValue ? "True" : "False");
                    if (this.slider != null)
                        this.slider.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"不支持Refresh的配置值类型: {valueType}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"刷新UI值时发生错误: {ex.Message}");
            }
        }

        private bool initDone = false;

        private string key = "";

        [Space]
        private object? defaultValue = null;

        public TextMeshProUGUI? label;

        public Slider? slider;

        public TMP_InputField? valueField;

        private Type valueType = typeof(float);
        private Vector2? sliderRange = null;

        public void Init(string key, string description, Type valueType, object defaultValue, Vector2? sliderRange)
        {
            if(defaultValue == null)
            {
                Debug.LogError("配置项默认值不能为null: "+ key);
            }

            this.key = key;
            this.defaultValue = defaultValue;
            this.valueType = valueType;
            this.sliderRange = sliderRange;

            if (this.label != null)
                this.label.SetText(description);

            // 设置slider的范围
            if (sliderRange.HasValue && this.slider != null)
            {
                this.slider.minValue = sliderRange.Value.x;
                this.slider.maxValue = sliderRange.Value.y;

                // 根据类型设置slider的整数模式
                if (valueType == typeof(int))
                {
                    this.slider.wholeNumbers = true;
                }
                else
                {
                    this.slider.wholeNumbers = false;
                }

                this.slider.gameObject.SetActive(true);
            }
            else if (this.slider != null)
            {
                // 如果sliderRange是null就隐藏slider
                this.slider.gameObject.SetActive(false);
            }


             initDone = true;

            // 初始化显示
            RefreshValues();
        }
    }
}