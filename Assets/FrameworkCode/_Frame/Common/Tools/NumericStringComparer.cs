using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NumericStringComparer : IComparer<User>
{
    //public int Compare(User x, User y)
    //{
    //    // 在这里编写你的比较逻辑
    //    // 这里假设 SortKey 是包含数字的字符串，我们可以根据这个数字部分进行排序
    //    int xNum = int.Parse(System.Text.RegularExpressions.Regex.Replace(x.Account, "[^0-9]", ""));
    //    int yNum = int.Parse(System.Text.RegularExpressions.Regex.Replace(y.Account, "[^0-9]", ""));

    //    return xNum.CompareTo(yNum);
    //}
    public int Compare(User x, User y)
    {
        // 提取数字部分并转换为长整型
        long xNum = ExtractNumber(x.Account);
        long yNum = ExtractNumber(y.Account);

        // 如果x和y都全是数字，直接比较数字部分
        if (xNum != 0 && yNum != 0)
        {
            return xNum.CompareTo(yNum);
        }
        // 如果x是全数字，而y不是，则x排在前面
        else if (xNum != 0 && yNum == 0)
        {
            return -1;
        }
        // 如果x不是全数字，而y是数字，则y排在前面
        else if (xNum == 0 && yNum != 0)
        {
            return 1;
        }
        // 如果都不包含数字，按照字符串比较
        else
        {
            return x.Account.CompareTo(y.Account);
        }
    }

    private long ExtractNumber(string input)
    {
        // 使用正则表达式提取数字部分
        Match match = Regex.Match(input, @"\d+");
        if (match.Success)
        {
            return long.Parse(match.Value);
        }
        else
        {
            // 如果没有找到数字部分，则返回0
            return 0;
        }
    }
}

