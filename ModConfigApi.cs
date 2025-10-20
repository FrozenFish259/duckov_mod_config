using System;
using System.Reflection;
using UnityEngine;


/// <summary>
/// ModConfig 安全接口封装类 - 提供不抛异常的静态接口
/// </summary>
public static class ModConfigAPI
{
    private const string TAG = "ModConfigAPI";

    private static Type modBehaviourType;
    private static bool isInitialized = false;

    /// <summary>
    /// 初始化 ModConfigAPI，检查必要的函数是否存在
    /// </summary>
    public static bool Initialize()
    {
        try
        {
            if (isInitialized)
                return true;

            // 获取 ModBehaviour 类型
            modBehaviourType = Type.GetType("ModConfig.ModBehaviour");
            if (modBehaviourType == null)
            {
                Debug.LogWarning($"[{TAG}] ModConfig.ModBehaviour 类型未找到，ModConfig 可能未加载");
                return false;
            }

            // 检查必要的静态方法是否存在
            string[] requiredMethods = {
                "AddDropdownList",
                "AddInputWithSlider",
                "AddBoolDropdownList"
            };

            foreach (string methodName in requiredMethods)
            {
                MethodInfo method = modBehaviourType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                {
                    Debug.LogError($"[{TAG}] 必要方法 {methodName} 未找到");
                    return false;
                }
            }

            isInitialized = true;
            Debug.Log($"[{TAG}] ModConfigAPI 初始化成功");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{TAG}] 初始化失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 安全地添加下拉列表配置项
    /// </summary>
    public static bool SafeAddDropdownList(string modName, string key, string description, System.Collections.Generic.SortedDictionary<string, object> options, Type valueType, object defaultValue)
    {
        if (!Initialize())
            return false;

        try
        {
            MethodInfo method = modBehaviourType.GetMethod("AddDropdownList", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] { modName, key, description, options, valueType, defaultValue });

            Debug.Log($"[{TAG}] 成功添加下拉列表: {modName}.{key}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{TAG}] 添加下拉列表失败 {modName}.{key}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 安全地添加带滑条的输入框配置项
    /// </summary>
    public static bool SafeAddInputWithSlider(string modName, string key, string description, Type valueType, object defaultValue, UnityEngine.Vector2? sliderRange = null)
    {
        if (!Initialize())
            return false;

        try
        {
            MethodInfo method = modBehaviourType.GetMethod("AddInputWithSlider", BindingFlags.Public | BindingFlags.Static);

            // 处理可空参数
            object[] parameters = sliderRange.HasValue ?
                new object[] { modName, key, description, valueType, defaultValue, sliderRange.Value } :
                new object[] { modName, key, description, valueType, defaultValue, null };

            method.Invoke(null, parameters);

            Debug.Log($"[{TAG}] 成功添加滑条输入框: {modName}.{key}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{TAG}] 添加滑条输入框失败 {modName}.{key}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 安全地添加布尔下拉列表配置项
    /// </summary>
    public static bool SafeAddBoolDropdownList(string modName, string key, string description, bool defaultValue)
    {
        if (!Initialize())
            return false;

        try
        {
            MethodInfo method = modBehaviourType.GetMethod("AddBoolDropdownList", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] { modName, key, description, defaultValue });

            Debug.Log($"[{TAG}] 成功添加布尔下拉列表: {modName}.{key}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{TAG}] 添加布尔下拉列表失败 {modName}.{key}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 检查 ModConfig 是否可用
    /// </summary>
    public static bool IsAvailable()
    {
        return Initialize();
    }

    /// <summary>
    /// 获取 ModConfig 版本信息（如果存在）
    /// </summary>
    public static string GetVersionInfo()
    {
        if (!Initialize())
            return "ModConfig 未加载";

        try
        {
            // 尝试获取版本信息（如果 ModBehaviour 有相关字段或属性）
            FieldInfo versionField = modBehaviourType.GetField("Version", BindingFlags.Public | BindingFlags.Static);
            if (versionField != null)
            {
                return versionField.GetValue(null)?.ToString() ?? "未知版本";
            }

            PropertyInfo versionProperty = modBehaviourType.GetProperty("Version", BindingFlags.Public | BindingFlags.Static);
            if (versionProperty != null)
            {
                return versionProperty.GetValue(null)?.ToString() ?? "未知版本";
            }

            return "ModConfig 已加载（版本信息不可用）";
        }
        catch
        {
            return "ModConfig 已加载（版本检查失败）";
        }
    }
}

