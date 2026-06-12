public class ReadWordContent
{
    //public static string Read(string path)
    //{
    //    string message = "";
    //    FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    //    XWPFDocument word = new XWPFDocument(file);
    //    foreach (XWPFParagraph paragraph in word.Paragraphs)
    //    {
    //        message = paragraph.ParagraphText;//获取段落内容
    //    }
    //    file.Close();
    //    return message;
    //}
//    public static void OpenWord(string path)
//    {
//        // Word应用对象
//        Microsoft.Office.Interop.Word.Application wdApp = null;
//        // Word文档对象
//        Microsoft.Office.Interop.Word.Document wdDoc = null;

//        // Word路径
//        object oWdPath = path;
//        // Word设定：缺损项
//        object oMissing = System.Reflection.Missing.Value;
//        // Word设定：不保存
//        object doNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;

//        try
//        {
//            // 新建Word应用
//            wdApp = new Microsoft.Office.Interop.Word.Application();
//            // 设置Word应用为可见
//            wdApp.Visible = true;
//            // 打开Word文档
//            wdDoc = wdApp.Documents.Open(ref oWdPath,
//                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
//               ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
//                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
//        }
//        finally
//        {

//#pragma warning disable
//            // 关闭Word文档
//            if (wdDoc != null) { wdDoc.Close(); wdDoc = null; }
//            // 关闭Word应用
//            if (wdApp != null) { wdApp.Quit(ref doNotSaveChanges, ref oMissing, ref oMissing); wdApp = null; }

//#pragma warning disable
//            //GC.Collect();
//        }
//    }
}
