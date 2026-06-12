using System;

namespace SFB
{
    // 用于文件扩展名过滤的结构体
    public struct ExtensionFilter
    {
        public string Name;           // 过滤器的名称（例如 "图像文件"）
        public string[] Extensions;   // 扩展名数组（例如 ["jpg", "png"]）

        // 构造函数，初始化过滤器名称和扩展名
        public ExtensionFilter(string filterName, params string[] filterExtensions)
        {
            Name = filterName;
            Extensions = filterExtensions;
        }
    }

    // 跨平台文件/文件夹对话框管理类
    public class StandaloneFileBrowser
    {
        // 定义一个平台相关的文件浏览器接口，存储不同平台的实现
        private static IStandaloneFileBrowser _platformWrapper = null;

        // 静态构造函数，根据不同的平台初始化 _platformWrapper
        static StandaloneFileBrowser()
        {
#if UNITY_STANDALONE_OSX
            _platformWrapper = new StandaloneFileBrowserMac();        // macOS 平台
#elif UNITY_STANDALONE_WIN
            _platformWrapper = new StandaloneFileBrowserWindows();    // Windows 平台
#elif UNITY_STANDALONE_LINUX
            _platformWrapper = new StandaloneFileBrowserLinux();      // Linux 平台
#elif UNITY_EDITOR
            _platformWrapper = new StandaloneFileBrowserEditor();     // Unity 编辑器平台
#endif
        }

        /// <summary>
        /// 打开本地文件对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认打开的目录</param>
        /// <param name="extension">允许的文件扩展名</param>
        /// <param name="multiselect">是否允许多选文件</param>
        /// <returns>返回所选文件的路径数组，取消时返回长度为 0 的数组</returns>
        public static string[] OpenFilePanel(string title, string directory, string extension, bool multiselect)
        {
            // 构造扩展名过滤器，如果没有指定扩展名则为 null
            var extensions = string.IsNullOrEmpty(extension) ? null : new[] { new ExtensionFilter("", extension) };
            // 调用重载方法，传递过滤器数组
            return OpenFilePanel(title, directory, extensions, multiselect);
        }

        /// <summary>
        /// 打开本地文件对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认打开的目录</param>
        /// <param name="extensions">扩展名过滤器列表。示例：new ExtensionFilter("图像文件", "jpg", "png")</param>
        /// <param name="multiselect">是否允许多选文件</param>
        /// <returns>返回所选文件的路径数组，取消时返回长度为 0 的数组</returns>
        public static string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            // 调用平台相关的文件对话框接口
            return _platformWrapper.OpenFilePanel(title, directory, extensions, multiselect);
        }

        /// <summary>
        /// 异步打开本地文件对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认打开的目录</param>
        /// <param name="extension">允许的文件扩展名</param>
        /// <param name="multiselect">是否允许多选文件</param>
        /// <param name="cb">回调函数，在文件选择完成后调用</param>
        public static void OpenFilePanelAsync(string title, string directory, string extension, bool multiselect, Action<string[]> cb)
        {
            // 构造扩展名过滤器，如果没有指定扩展名则为 null
            var extensions = string.IsNullOrEmpty(extension) ? null : new[] { new ExtensionFilter("", extension) };
            // 调用异步版本的文件对话框，传递回调函数
            OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
        }

        /// <summary>
        /// 异步打开本地文件对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认打开的目录</param>
        /// <param name="extensions">扩展名过滤器列表。示例：new ExtensionFilter("图像文件", "jpg", "png")</param>
        /// <param name="multiselect">是否允许多选文件</param>
        /// <param name="cb">回调函数，在文件选择完成后调用</param>
        public static void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb)
        {
            // 调用平台相关的异步文件对话框接口
            _platformWrapper.OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
        }

        /// <summary>
        /// 打开本地文件夹选择对话框
        /// 注意：Windows 平台不支持多选文件夹
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认打开的目录</param>
        /// <param name="multiselect">是否允许多选文件夹</param>
        /// <returns>返回所选文件夹的路径数组，取消时返回长度为 0 的数组</returns>
        public static string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            // 调用平台相关的文件夹选择对话框接口
            return _platformWrapper.OpenFolderPanel(title, directory, multiselect);
        }

        /// <summary>
        /// 异步打开本地文件夹选择对话框
        /// 注意：Windows 平台不支持多选文件夹
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认打开的目录</param>
        /// <param name="multiselect">是否允许多选文件夹</param>
        /// <param name="cb">回调函数，在文件夹选择完成后调用</param>
        public static void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
        {
            // 调用平台相关的异步文件夹选择对话框接口
            _platformWrapper.OpenFolderPanelAsync(title, directory, multiselect, cb);
        }

        /// <summary>
        /// 打开本地文件保存对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认保存的目录</param>
        /// <param name="defaultName">默认文件名</param>
        /// <param name="extension">允许的文件扩展名</param>
        /// <returns>返回保存文件的路径，取消时返回空字符串</returns>
        public static string SaveFilePanel(string title, string directory, string defaultName, string extension)
        {
            // 构造扩展名过滤器，如果没有指定扩展名则为 null
            var extensions = string.IsNullOrEmpty(extension) ? null : new[] { new ExtensionFilter("", extension) };
            // 调用重载方法，传递过滤器数组
            return SaveFilePanel(title, directory, defaultName, extensions);
        }

        /// <summary>
        /// 打开本地文件保存对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认保存的目录</param>
        /// <param name="defaultName">默认文件名</param>
        /// <param name="extensions">扩展名过滤器列表。示例：new ExtensionFilter("图像文件", "jpg", "png")</param>
        /// <returns>返回保存文件的路径，取消时返回空字符串</returns>
        public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            // 调用平台相关的文件保存对话框接口
            return _platformWrapper.SaveFilePanel(title, directory, defaultName, extensions);
        }

        /// <summary>
        /// 异步打开本地文件保存对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认保存的目录</param>
        /// <param name="defaultName">默认文件名</param>
        /// <param name="extension">允许的文件扩展名</param>
        /// <param name="cb">回调函数，在文件保存路径选择完成后调用</param>
        public static void SaveFilePanelAsync(string title, string directory, string defaultName, string extension, Action<string> cb)
        {
            // 构造扩展名过滤器，如果没有指定扩展名则为 null
            var extensions = string.IsNullOrEmpty(extension) ? null : new[] { new ExtensionFilter("", extension) };
            // 调用异步版本的文件保存对话框，传递回调函数
            SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
        }

        /// <summary>
        /// 异步打开本地文件保存对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">默认保存的目录</param>
        /// <param name="defaultName">默认文件名</param>
        /// <param name="extensions">扩展名过滤器列表。示例：new ExtensionFilter("图像文件", "jpg", "png")</param>
        /// <param name="cb">回调函数，在文件保存路径选择完成后调用</param>
        public static void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
        {
            // 调用平台相关的异步文件保存对话框接口
            _platformWrapper.SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
        }
    }
}
