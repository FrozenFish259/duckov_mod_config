using System;
using System.Reflection;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// 反射工具类 - 提供安全的反射获取和修改方法
    /// </summary>
    public static class ReflectionHelper
    {
        private const string TAG = "ReflectionHelper";

        /// <summary>
        /// 安全获取字段值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="obj">目标对象</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>字段值或默认值</returns>
        public static T GetFieldValue<T>(object obj, string fieldName, T defaultValue = default(T),
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 获取字段 '{fieldName}' 失败：目标对象为null");
                    return defaultValue;
                }

                Type type = obj.GetType();
                FieldInfo field = type.GetField(fieldName, bindingFlags);

                if (field == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到字段 '{fieldName}'");
                    return defaultValue;
                }

                object value = field.GetValue(obj);

                if (value is T result)
                {
                    Debug.Log($"[{TAG}] 成功获取字段 '{type.Name}.{fieldName}' = {result}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"[{TAG}] 字段 '{type.Name}.{fieldName}' 类型不匹配。期望: {typeof(T).Name}, 实际: {value?.GetType().Name ?? "null"}");
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 获取字段 '{fieldName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全设置字段值
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">要设置的值</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>是否设置成功</returns>
        public static bool SetFieldValue(object obj, string fieldName, object value,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 设置字段 '{fieldName}' 失败：目标对象为null");
                    return false;
                }

                Type type = obj.GetType();
                FieldInfo field = type.GetField(fieldName, bindingFlags);

                if (field == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到字段 '{fieldName}'");
                    return false;
                }

                // 类型转换
                object convertedValue = ConvertValue(value, field.FieldType);
                field.SetValue(obj, convertedValue);

                Debug.Log($"[{TAG}] 成功设置字段 '{type.Name}.{fieldName}' = {convertedValue}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 设置字段 '{fieldName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 安全获取属性值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="obj">目标对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>属性值或默认值</returns>
        public static T GetPropertyValue<T>(object obj, string propertyName, T defaultValue = default(T),
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 获取属性 '{propertyName}' 失败：目标对象为null");
                    return defaultValue;
                }

                Type type = obj.GetType();
                PropertyInfo property = type.GetProperty(propertyName, bindingFlags);

                if (property == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到属性 '{propertyName}'");
                    return defaultValue;
                }

                if (!property.CanRead)
                {
                    Debug.LogWarning($"[{TAG}] 属性 '{type.Name}.{propertyName}' 不可读");
                    return defaultValue;
                }

                object value = property.GetValue(obj);

                if (value is T result)
                {
                    Debug.Log($"[{TAG}] 成功获取属性 '{type.Name}.{propertyName}' = {result}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"[{TAG}] 属性 '{type.Name}.{propertyName}' 类型不匹配。期望: {typeof(T).Name}, 实际: {value?.GetType().Name ?? "null"}");
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 获取属性 '{propertyName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全设置属性值
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">要设置的值</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>是否设置成功</returns>
        public static bool SetPropertyValue(object obj, string propertyName, object value,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 设置属性 '{propertyName}' 失败：目标对象为null");
                    return false;
                }

                Type type = obj.GetType();
                PropertyInfo property = type.GetProperty(propertyName, bindingFlags);

                if (property == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到属性 '{propertyName}'");
                    return false;
                }

                if (!property.CanWrite)
                {
                    Debug.LogWarning($"[{TAG}] 属性 '{type.Name}.{propertyName}' 不可写");
                    return false;
                }

                // 类型转换
                object convertedValue = ConvertValue(value, property.PropertyType);
                property.SetValue(obj, convertedValue);

                Debug.Log($"[{TAG}] 成功设置属性 '{type.Name}.{propertyName}' = {convertedValue}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 设置属性 '{propertyName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 安全调用方法
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">参数数组</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>方法返回值，失败返回null</returns>
        public static object InvokeMethod(object obj, string methodName, object[] parameters = null,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 调用方法 '{methodName}' 失败：目标对象为null");
                    return null;
                }

                Type type = obj.GetType();
                MethodInfo method = type.GetMethod(methodName, bindingFlags);

                if (method == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到方法 '{methodName}'");
                    return null;
                }

                object result = method.Invoke(obj, parameters);
                Debug.Log($"[{TAG}] 成功调用方法 '{type.Name}.{methodName}'");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 调用方法 '{methodName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// 类型转换辅助方法
        /// </summary>
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            if (targetType.IsInstanceOfType(value))
                return value;

            try
            {
                // 处理可空类型
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type underlyingType = Nullable.GetUnderlyingType(targetType);
                    return Convert.ChangeType(value, underlyingType);
                }

                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 类型转换失败: 从 {value.GetType().Name} 到 {targetType.Name}, 值: {value}. 异常: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 检查类型是否包含指定字段
        /// </summary>
        public static bool HasField(Type type, string fieldName,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return type.GetField(fieldName, bindingFlags) != null;
        }

        /// <summary>
        /// 检查类型是否包含指定属性
        /// </summary>
        public static bool HasProperty(Type type, string propertyName,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return type.GetProperty(propertyName, bindingFlags) != null;
        }

        /// <summary>
        /// 检查类型是否包含指定方法
        /// </summary>
        public static bool HasMethod(Type type, string methodName,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return type.GetMethod(methodName, bindingFlags) != null;
        }
    }
}