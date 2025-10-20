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

        #region 实例成员方法

        /// <summary>
        /// 安全获取实例字段值
        /// </summary>
        public static T GetInstanceFieldValue<T>(object obj, string fieldName, T defaultValue = default(T),
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 获取实例字段 '{fieldName}' 失败：目标对象为null");
                    return defaultValue;
                }

                Type type = obj.GetType();
                FieldInfo field = type.GetField(fieldName, bindingFlags);

                if (field == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到实例字段 '{fieldName}'");
                    return defaultValue;
                }

                if (field.IsStatic)
                {
                    Debug.LogWarning($"[{TAG}] 字段 '{type.Name}.{fieldName}' 是静态字段，请使用 GetStaticFieldValue 方法");
                    return defaultValue;
                }

                object value = field.GetValue(obj);

                if (value is T result)
                {
                    Debug.Log($"[{TAG}] 成功获取实例字段 '{type.Name}.{fieldName}' = {result}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"[{TAG}] 实例字段 '{type.Name}.{fieldName}' 类型不匹配。期望: {typeof(T).Name}, 实际: {value?.GetType().Name ?? "null"}");
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 获取实例字段 '{fieldName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全设置实例字段值
        /// </summary>
        public static bool SetInstanceFieldValue(object obj, string fieldName, object value,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 设置实例字段 '{fieldName}' 失败：目标对象为null");
                    return false;
                }

                Type type = obj.GetType();
                FieldInfo field = type.GetField(fieldName, bindingFlags);

                if (field == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到实例字段 '{fieldName}'");
                    return false;
                }

                if (field.IsStatic)
                {
                    Debug.LogWarning($"[{TAG}] 字段 '{type.Name}.{fieldName}' 是静态字段，请使用 SetStaticFieldValue 方法");
                    return false;
                }

                // 检查只读字段
                if (field.IsInitOnly || field.IsLiteral)
                {
                    Debug.LogWarning($"[{TAG}] 实例字段 '{type.Name}.{fieldName}' 是只读字段");
                    return false;
                }

                // 类型转换
                object convertedValue = ConvertValue(value, field.FieldType);
                field.SetValue(obj, convertedValue);

                Debug.Log($"[{TAG}] 成功设置实例字段 '{type.Name}.{fieldName}' = {convertedValue}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 设置实例字段 '{fieldName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 安全获取实例属性值
        /// </summary>
        public static T GetInstancePropertyValue<T>(object obj, string propertyName, T defaultValue = default(T),
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 获取实例属性 '{propertyName}' 失败：目标对象为null");
                    return defaultValue;
                }

                Type type = obj.GetType();
                PropertyInfo property = type.GetProperty(propertyName, bindingFlags);

                if (property == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到实例属性 '{propertyName}'");
                    return defaultValue;
                }

                if (!property.CanRead)
                {
                    Debug.LogWarning($"[{TAG}] 实例属性 '{type.Name}.{propertyName}' 不可读");
                    return defaultValue;
                }

                // 检查静态属性
                MethodInfo getMethod = property.GetGetMethod(true);
                if (getMethod?.IsStatic == true)
                {
                    Debug.LogWarning($"[{TAG}] 属性 '{type.Name}.{propertyName}' 是静态属性，请使用 GetStaticPropertyValue 方法");
                    return defaultValue;
                }

                object value = property.GetValue(obj);

                if (value is T result)
                {
                    Debug.Log($"[{TAG}] 成功获取实例属性 '{type.Name}.{propertyName}' = {result}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"[{TAG}] 实例属性 '{type.Name}.{propertyName}' 类型不匹配。期望: {typeof(T).Name}, 实际: {value?.GetType().Name ?? "null"}");
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 获取实例属性 '{propertyName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全调用实例方法
        /// </summary>
        public static object InvokeInstanceMethod(object obj, string methodName, object[] parameters = null,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                if (obj == null)
                {
                    Debug.LogWarning($"[{TAG}] 调用实例方法 '{methodName}' 失败：目标对象为null");
                    return null;
                }

                Type type = obj.GetType();
                MethodInfo method = type.GetMethod(methodName, bindingFlags);

                if (method == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到实例方法 '{methodName}'");
                    return null;
                }

                if (method.IsStatic)
                {
                    Debug.LogWarning($"[{TAG}] 方法 '{type.Name}.{methodName}' 是静态方法，请使用 InvokeStaticMethod 方法");
                    return null;
                }

                object result = method.Invoke(obj, parameters);
                Debug.Log($"[{TAG}] 成功调用实例方法 '{type.Name}.{methodName}'");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 调用实例方法 '{methodName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        #endregion

        #region 静态成员方法

        /// <summary>
        /// 安全获取静态字段值
        /// </summary>
        public static T GetStaticFieldValue<T>(Type type, string fieldName, T defaultValue = default(T),
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        {
            try
            {
                if (type == null)
                {
                    Debug.LogWarning($"[{TAG}] 获取静态字段 '{fieldName}' 失败：目标类型为null");
                    return defaultValue;
                }

                FieldInfo field = type.GetField(fieldName, bindingFlags);

                if (field == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到静态字段 '{fieldName}'");
                    return defaultValue;
                }

                if (!field.IsStatic)
                {
                    Debug.LogWarning($"[{TAG}] 字段 '{type.Name}.{fieldName}' 不是静态字段");
                    return defaultValue;
                }

                object value = field.GetValue(null);

                if (value is T result)
                {
                    Debug.Log($"[{TAG}] 成功获取静态字段 '{type.Name}.{fieldName}' = {result}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"[{TAG}] 静态字段 '{type.Name}.{fieldName}' 类型不匹配。期望: {typeof(T).Name}, 实际: {value?.GetType().Name ?? "null"}");
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 获取静态字段 '{fieldName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全设置静态字段值
        /// </summary>
        public static bool SetStaticFieldValue(Type type, string fieldName, object value,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        {
            try
            {
                if (type == null)
                {
                    Debug.LogWarning($"[{TAG}] 设置静态字段 '{fieldName}' 失败：目标类型为null");
                    return false;
                }

                FieldInfo field = type.GetField(fieldName, bindingFlags);

                if (field == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到静态字段 '{fieldName}'");
                    return false;
                }

                if (!field.IsStatic)
                {
                    Debug.LogWarning($"[{TAG}] 字段 '{type.Name}.{fieldName}' 不是静态字段");
                    return false;
                }

                // 检查只读字段
                if (field.IsInitOnly || field.IsLiteral)
                {
                    Debug.LogWarning($"[{TAG}] 静态字段 '{type.Name}.{fieldName}' 是只读字段");
                    return false;
                }

                // 类型转换
                object convertedValue = ConvertValue(value, field.FieldType);
                field.SetValue(null, convertedValue);

                Debug.Log($"[{TAG}] 成功设置静态字段 '{type.Name}.{fieldName}' = {convertedValue}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 设置静态字段 '{fieldName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 安全获取静态属性值
        /// </summary>
        public static T GetStaticPropertyValue<T>(Type type, string propertyName, T defaultValue = default(T),
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        {
            try
            {
                if (type == null)
                {
                    Debug.LogWarning($"[{TAG}] 获取静态属性 '{propertyName}' 失败：目标类型为null");
                    return defaultValue;
                }

                PropertyInfo property = type.GetProperty(propertyName, bindingFlags);

                if (property == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到静态属性 '{propertyName}'");
                    return defaultValue;
                }

                if (!property.CanRead)
                {
                    Debug.LogWarning($"[{TAG}] 静态属性 '{type.Name}.{propertyName}' 不可读");
                    return defaultValue;
                }

                // 检查静态属性
                MethodInfo getMethod = property.GetGetMethod(true);
                if (getMethod?.IsStatic != true)
                {
                    Debug.LogWarning($"[{TAG}] 属性 '{type.Name}.{propertyName}' 不是静态属性");
                    return defaultValue;
                }

                object value = property.GetValue(null);

                if (value is T result)
                {
                    Debug.Log($"[{TAG}] 成功获取静态属性 '{type.Name}.{propertyName}' = {result}");
                    return result;
                }
                else
                {
                    Debug.LogWarning($"[{TAG}] 静态属性 '{type.Name}.{propertyName}' 类型不匹配。期望: {typeof(T).Name}, 实际: {value?.GetType().Name ?? "null"}");
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 获取静态属性 '{propertyName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全调用静态方法
        /// </summary>
        public static object InvokeStaticMethod(Type type, string methodName, object[] parameters = null,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        {
            try
            {
                if (type == null)
                {
                    Debug.LogWarning($"[{TAG}] 调用静态方法 '{methodName}' 失败：目标类型为null");
                    return null;
                }

                MethodInfo method = type.GetMethod(methodName, bindingFlags);

                if (method == null)
                {
                    Debug.LogWarning($"[{TAG}] 在类型 '{type.Name}' 中未找到静态方法 '{methodName}'");
                    return null;
                }

                if (!method.IsStatic)
                {
                    Debug.LogWarning($"[{TAG}] 方法 '{type.Name}.{methodName}' 不是静态方法");
                    return null;
                }

                object result = method.Invoke(null, parameters);
                Debug.Log($"[{TAG}] 成功调用静态方法 '{type.Name}.{methodName}'");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{TAG}] 调用静态方法 '{methodName}' 时发生异常: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        #endregion

        #region 通用方法

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
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            return type.GetField(fieldName, bindingFlags) != null;
        }

        /// <summary>
        /// 检查类型是否包含指定属性
        /// </summary>
        public static bool HasProperty(Type type, string propertyName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            return type.GetProperty(propertyName, bindingFlags) != null;
        }

        /// <summary>
        /// 检查类型是否包含指定方法
        /// </summary>
        public static bool HasMethod(Type type, string methodName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            return type.GetMethod(methodName, bindingFlags) != null;
        }

        #endregion
    }
}