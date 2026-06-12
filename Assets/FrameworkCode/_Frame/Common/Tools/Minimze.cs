using System;
using System.Runtime.InteropServices;

public class Minimze 
{
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdshow);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    private const int SW_SHOWMINIMIZED = 2;//最小化，激活
    private const int SW_SHOWMAXIMIZED = 3;//最大化
    private const int SW_SHOWRESTORE = 1;//还原

    public static void Minmized()
    {
        ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED);
    }
}
