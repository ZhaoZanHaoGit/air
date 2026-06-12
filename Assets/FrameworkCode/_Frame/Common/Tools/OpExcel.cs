using Excel;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

public class OpExcel 
{
    public static DataSet GetDataSet(string path)
    {
        try
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataSet = excelReader.AsDataSet();
                excelReader.Close();
                return dataSet;
            }
        }
        catch (IOException ex)
        {
            if (IsFileLocked(ex))
            {               
                UIManager.Instance.OpenMessageBoxUI("提示信息", "请关闭Excel再重试。", 0, EnumMessageBoxType.OK);
            }
            else
            {             
                UIManager.Instance.OpenMessageBoxUI("提示信息", ex.Message, 0, EnumMessageBoxType.OK);
            }
            return null; 
        }
    }

    private static bool IsFileLocked(IOException exception)
    {
        int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
        return errorCode == 32 || errorCode == 33; // 32 表示文件被占用，33 表示文件正在被使用
    }
}
