using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class JsonNameDeduplicator
{
    /// <summary>
    /// 处理 JSON 数据中的重名现象，如果名称有重复则在名称后依次加 1。
    /// </summary>
    /// <param name="jsonData">原始的 JSON 字符串</param>
    /// <param name="nameField">要检查重名的字段名称（默认为 "Name"）</param>
    /// <returns>处理后的 JSON 字符串</returns>
    public static string DeduplicateNames(string jsonData, string nameField = "Name")
    {
        // 解析 JSON 数据为 JArray
        JArray jsonArray = JArray.Parse(jsonData);

        // 字典存储文件名和其出现次数
        Dictionary<string, int> nameCounts = new Dictionary<string, int>();

        // 遍历 JSON 对象并检查重复的名称
        foreach (JObject obj in jsonArray)
        {
            string originalName = (string)obj[nameField];
            string newName = originalName;

            // 如果名称已存在，递增计数器并生成新的名称
            if (nameCounts.ContainsKey(originalName))
            {
                nameCounts[originalName]++;
                string nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(originalName);
                string extension = System.IO.Path.GetExtension(originalName);
                newName = $"{nameWithoutExtension}{nameCounts[originalName]}{extension}";
            }
            else
            {
                nameCounts[originalName] = 1;
            }

            // 更新 JSON 对象中的名称
            obj[nameField] = newName;
        }

        // 返回修改后的 JSON 字符串
        return jsonArray.ToString(Formatting.Indented);
    }
}
