using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string ProcessToX0(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        string[] parts = input.Split('_');
        // 匹配 X_Y 或 X_Y_Z（Z 为任意数字）
        if (parts.Length >= 2 && int.TryParse(parts[0], out _) && int.TryParse(parts[1], out _))
        {
            return parts[0] + "_" + parts[1]; // 始终返回 X_Y
        }

        return input; // 不匹配则返回原字符串
    }
}