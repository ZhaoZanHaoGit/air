#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TJGenerators.Utils
{
    /// <summary>
    /// Object 查找 API 兼容层。
    /// Unity 2022.2+ 使用 <see cref="Object.FindObjectsByType{T}"/> / <see cref="Object.FindFirstObjectByType{T}"/>；
    /// Unity 2020.1–2022.1 使用 <see cref="Object.FindObjectsOfType{T}"/> / <see cref="Object.FindObjectOfType{T}"/>；
    /// Unity 2019 无 includeInactive 重载，含 inactive 查找回退到 Resources.FindObjectsOfTypeAll 并过滤场景对象。
    /// </summary>
    public static class UnityObjectCompat
    {
        public static T FindObjectOfType<T>() where T : Object
        {
#if UNITY_2022_2_OR_NEWER
            return Object.FindFirstObjectByType<T>(FindObjectsInactive.Exclude);
#else
            return Object.FindObjectOfType<T>();
#endif
        }

        public static T[] FindObjectsOfType<T>() where T : Object
        {
#if UNITY_2022_2_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }

        public static T[] FindObjectsOfTypeIncludingInactive<T>() where T : Object
        {
#if UNITY_2022_2_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#elif UNITY_2020_1_OR_NEWER
            return Object.FindObjectsOfType<T>(true);
#else
            return FindFromResources<T>(includeInactive: true);
#endif
        }

#if !UNITY_2020_1_OR_NEWER
        private static T[] FindFromResources<T>(bool includeInactive) where T : Object
        {
            var results = new List<T>();
            foreach (var obj in Resources.FindObjectsOfTypeAll<T>())
            {
                if (obj == null)
                    continue;

                var component = obj as Component;
                if (component == null || !component.gameObject.scene.IsValid())
                    continue;

                if (!includeInactive && !component.gameObject.activeInHierarchy)
                    continue;

                results.Add(obj);
            }
            return results.ToArray();
        }
#endif
    }
}
#endif
