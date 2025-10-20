using Duckov.Options;
using Pathfinding;
using SodaCraft.Localizations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

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
                    Type? oldType = ES3Helper.getType(key);
                    if (oldType != valueType)
                    {
                        Debug.LogWarning($"检测到{key}旧配置项类型{oldType}与当前类型{valueType}不同, 已重置为新的默认值");
                        ES3Helper.DeleteKey(this.key);
                    }

                    if (valueType == typeof(int))
                        return OptionsManager_Mod.Load<int>(this.key, (int)defaultValue);
                    else if (valueType == typeof(float))
                    {
                        Debug.Log("正在读取float配置默认值");
                        float defaultV = Convert.ToSingle(defaultValue);
                        Debug.Log("float配置默认值为" + defaultV.ToString("F1"));
                        float loaded = OptionsManager_Mod.Load<float>(this.key, defaultV);
                        Debug.Log("float配置数值已读取=" + loaded.ToString("F1"));

                        return loaded;
                    }
                    else if (valueType == typeof(string))
                    {
                        Debug.Log("正在读取string配置默认值");
                        return OptionsManager_Mod.Load<string>(this.key, (string)defaultValue);
                    }
                    else if (valueType == typeof(bool))
                    {
                        Debug.Log("正在读取bool配置默认值");
                        return OptionsManager_Mod.Load<bool>(this.key, (bool)defaultValue);
                    }
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
                    Debug.Log("正在设置配置值");
                    if (valueType == typeof(int))
                    {
                        int intValue = Convert.ToInt32(value);
                        OptionsManager_Mod.Save<int>(this.key, intValue);
                    }
                    else if (valueType == typeof(float))
                    {
                        float floatValue = Convert.ToSingle(value);
                        OptionsManager_Mod.Save<float>(this.key, floatValue);
                    }
                    else if (valueType == typeof(string))
                    {
                        OptionsManager_Mod.Save<string>(this.key, value?.ToString() ?? "");
                    }
                    else if (valueType == typeof(bool))
                    {
                        bool boolValue = Convert.ToBoolean(value);
                        OptionsManager_Mod.Save<bool>(this.key, boolValue);
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
            //LocalizationManager.OnSetLanguage += this.OnLanguageChanged;
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
            if (!initDone)
            {
                return;
            }
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
                        if (this.sliderRange != null)
                            intValue = Mathf.Clamp(intValue, (int)this.sliderRange.Value.x, (int)this.sliderRange.Value.y);
                        this.Value = intValue;
                    }
                    else
                    {
                        // 输入无效，恢复原值
                        Debug.LogWarning("请输入有效的整数");
                        this.RefreshValues();
                    }
                }
                else if (valueType == typeof(float))
                {
                    if (float.TryParse(arg0, out float floatValue))
                    {
                        if (this.sliderRange != null)
                            floatValue = Mathf.Clamp(floatValue, this.sliderRange.Value.x, this.sliderRange.Value.y);
                        this.Value = floatValue;
                    }
                    else
                    {
                        // 输入无效，恢复原值
                        Debug.LogWarning("请输入有效的数字");
                        this.RefreshValues();
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
                    else
                    {
                        // 尝试其他布尔表示形式
                        string lowerArg = arg0.ToLower();
                        if (lowerArg == "true" || lowerArg == "1" || lowerArg == "yes" || lowerArg == "on")
                        {
                            this.Value = true;
                        }
                        else if (lowerArg == "false" || lowerArg == "0" || lowerArg == "no" || lowerArg == "off")
                        {
                            this.Value = false;
                        }
                        else
                        {
                            Debug.LogWarning("请输入 true/false, 1/0, yes/no, on/off");
                            this.RefreshValues();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理输入框值时发生错误: {ex.Message}");
                this.RefreshValues(); // 出错时恢复原值
            }

            this.RefreshValues();
        }

        private void OnEnable()
        {
            if (!initDone)
            {
                return;
            }

            this.RefreshValues();
        }

        private void OnSliderValueChanged(float value)
        {
            if (!initDone)
            {
                return;
            }

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
            if (defaultValue == null)
            {
                Debug.LogError("配置项默认值不能为null: " + key);
                return;
            }

            if (valueField == null)
            {
                Debug.LogError("valueField为空!!");
                return;
            }
            else
            {
                valueField.contentType = TMP_InputField.ContentType.Standard;
                valueField.characterLimit = 1000;
            }

            if (this.slider == null)
            {
                Debug.LogError("slider为空!!");
                return;
            }

            this.key = key;
            this.defaultValue = defaultValue;
            this.valueType = valueType;
            this.sliderRange = sliderRange;

            if (this.label != null)
                this.label.SetText(description);

            // 设置 slider 的范围和可见性
            if (sliderRange.HasValue)
            {
                // 有范围时，设置 slider 范围并显示
                this.slider.minValue = sliderRange.Value.x;
                this.slider.maxValue = sliderRange.Value.y;

                // 根据类型设置整数模式
                if (valueType == typeof(int))
                {
                    this.slider.wholeNumbers = true;
                }
                else
                {
                    this.slider.wholeNumbers = false;
                }

                this.slider.gameObject.SetActive(true);

                // 验证默认值是否在范围内
                try
                {
                    if (valueType == typeof(float))
                    {
                        float defaultV = Convert.ToSingle(defaultValue);
                        if (defaultV < sliderRange.Value.x || defaultV > sliderRange.Value.y)
                        {
                            Debug.LogError($"配置项{key}的默认值{defaultV}超出设置的范围[{sliderRange.Value.x}, {sliderRange.Value.y}]");
                        }
                    }
                    else if (valueType == typeof(int))
                    {
                        int defaultV = Convert.ToInt32(defaultValue);
                        if (defaultV < sliderRange.Value.x || defaultV > sliderRange.Value.y)
                        {
                            Debug.LogError($"配置项{key}的默认值{defaultV}超出设置的范围[{sliderRange.Value.x}, {sliderRange.Value.y}]");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"验证默认值范围时出错: {ex.Message}");
                }
            }
            else
            {
                // 没有范围时，隐藏 slider
                // 同时去除数值范围限制
                if (valueType == typeof(int))
                {
                    // 对于整数，设置合理的范围
                    this.slider.minValue = int.MinValue;
                    this.slider.maxValue = int.MaxValue;
                    this.slider.wholeNumbers = true;
                }
                else if (valueType == typeof(float))
                {
                    // 对于浮点数，设置合理的范围（避免使用 float.MinValue/MaxValue）
                    this.slider.minValue = -100000f;  // 足够大的负值
                    this.slider.maxValue = 100000f;   // 足够大的正值
                    this.slider.wholeNumbers = false;
                }

                this.slider.gameObject.SetActive(true);
                Debug.Log($"已解除slider范围限制 - Key: {key}, 新范围: [{this.slider.minValue}, {this.slider.maxValue}]");
            }

            initDone = true;

            // 初始化显示
            RefreshValues();
        }
    }
}