
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.数据管理类
 *
 *  Author:				
 *       
 *  Date:              
 * 
 ******************************************************************************/

using System;
using System.Collections.Generic;

public class ModuleManager : Singleton<ModuleManager>
{
    /// <summary>
    /// 用于存储模块的字典，以模块名称为键，BaseModule 对象为值
    /// </summary>
    private Dictionary<string, BaseModule> dicModules = null;
    /// <summary>
    /// 重写 Singleton 类的 Init 方法，用于初始化 ModuleManager 的字典
    /// </summary>
    public override void Init() => dicModules = new Dictionary<string, BaseModule>();
    /// <summary>
    /// 根据模块名称获取对应的 BaseModule 对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public BaseModule Get(string key)
    {
        return dicModules.ContainsKey(key) ? dicModules[key] : null;
    }
    /// <summary>
    /// 根据泛型类型 T 获取对应的 BaseModule 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>() where T : BaseModule
    {
        var t = typeof(T);
        return dicModules.ContainsKey(t.ToString()) ? dicModules[t.ToString()] as T : null;
    }
    /// <summary>
    /// 注册所有的模块，这里只调用了 LoadModule 方法来加载 MessageBoxModule
    /// </summary>
    public void RegisterAllModules()
    {
        LoadModule(typeof(MessageBoxModule));

    }
    /// <summary>
    /// 根据传入的模块类型创建并加载相应的模块对象
    /// </summary>
    /// <param name="moduleType"></param>
    private void LoadModule(Type moduleType)
    {
        var bm = Activator.CreateInstance(moduleType) as BaseModule;
        bm?.Load();
    }
    /// <summary>
    /// 注册模块，将模块添加到字典中
    /// </summary>
    /// <param name="module"></param>
    public void Register(BaseModule module)
    {
        var t = module.GetType();
        Register(t.ToString(), module);
    }
    /// <summary>
    /// 注册模块，将模块添加到字典中，使用指定的键
    /// </summary>
    /// <param name="key"></param>
    /// <param name="module"></param>
    public void Register(string key, BaseModule module)
    {
        if (!dicModules.ContainsKey(key))
            dicModules.Add(key, module);
    }
    /// <summary>
    /// 取消注册所有模块，释放模块资源
    /// </summary>
    public void UnRegisterAll()
    {
        var keyList = new List<string>(dicModules.Keys);
        foreach (var t in keyList)
        {
            UnRegister(t);
        }
        dicModules.Clear();
    }
    /// <summary>
    /// 取消注册指定模块，释放模块资源
    /// </summary>
    /// <param name="module"></param>
    public void UnRegister(BaseModule module)
    {
        var t = module.GetType();
        UnRegister(t.ToString());
    }
    /// <summary>
    /// 取消注册指定键的模块，释放模块资源
    /// </summary>
    /// <param name="key"></param>
    public void UnRegister(string key)
    {
        if (!dicModules.ContainsKey(key)) return;
        var module = dicModules[key];
        module.Release();
        dicModules.Remove(key);
        module = null;
    }
}
