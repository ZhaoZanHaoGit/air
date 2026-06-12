using System;
using System.Net;

public class NetworkTimeFetcher 
{
    public static DateTime GetNetDateTime()
    {
        WebRequest request = null;
        WebResponse response = null;
        WebHeaderCollection headerCollection = null;
        string datetime = string.Empty;
        DateTime timeNow = DateTime.MinValue;
        try
        {
            request = WebRequest.Create("https://www.baidu.com");
            request.Timeout = 3000;
            request.Credentials = CredentialCache.DefaultCredentials;
            response = (WebResponse)request.GetResponse();
            headerCollection = response.Headers;
            foreach (var h in headerCollection.AllKeys)
            {
                //Debug.Log("h " + h);
                if (h == "Date")
                {
                    datetime = headerCollection[h];
                    timeNow = Convert.ToDateTime(datetime);
                }
            }
            return timeNow;
        }
        catch (Exception)
        {
            //Debug.LogError("获取网络时间错误");
            return DateTime.Now;
        }
        finally
        {
            if (request != null)
            { request.Abort(); }
            if (response != null)
            { response.Close(); }
            if (headerCollection != null)
            { headerCollection.Clear(); }
        }
    }
}
