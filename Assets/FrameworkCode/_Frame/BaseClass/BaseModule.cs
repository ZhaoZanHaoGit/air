/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.数据基类
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/

public abstract class BaseModule
{
     /// <summary>
     ///   枚举类型，表示对象的状态
     /// </summary>
    public enum EnumRegisterMode
    {
        NotRegister,
        AutoRegister,
        AlreadyRegister,
    }

    private EnumObjectState _state = EnumObjectState.Initial;
    public event StateChangeEvent StateChanged;
    public EnumObjectState State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            var oldState = _state;
            _state = value;
            StateChanged?.Invoke(this, _state, oldState);
            OnStateChanged(_state, oldState);
        }
    }
    /// <summary>
    /// 虚方法，用于在子类中处理状态变化时的额外逻辑
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="oldState"></param>
    protected virtual void OnStateChanged(EnumObjectState newState, EnumObjectState oldState) { }
    /// <summary>
    /// 枚举类型，表示模块的注册模式
    /// </summary>
    private EnumRegisterMode registerMode = EnumRegisterMode.NotRegister;
    /// <summary>
    /// 公共属性，用于获取和设置模块的自动注册属性
    /// </summary>
    public bool AutoRegister
    {
        get => registerMode != EnumRegisterMode.NotRegister;
        set
        {
            if (registerMode == EnumRegisterMode.NotRegister || registerMode == EnumRegisterMode.AutoRegister)
                registerMode = value ? EnumRegisterMode.AutoRegister : EnumRegisterMode.NotRegister;
        }
    }

    public bool HasRegistered => registerMode == EnumRegisterMode.AlreadyRegister;
    /// <summary>
    /// 公共方法，用于加载模块
    /// </summary>
    public void Load()
    {
        if (State != EnumObjectState.Initial) return;

        State = EnumObjectState.Loading;
        if (registerMode == EnumRegisterMode.AutoRegister)
        {
            ModuleManager.Instance.Register(this);
            registerMode = EnumRegisterMode.AlreadyRegister;
        }

        OnLoad();
        State = EnumObjectState.Ready;
    }
    /// <summary>
    /// 虚方法，用于在子类中处理加载时的额外逻辑
    /// </summary>
    protected virtual void OnLoad() { }
    /// <summary>
    /// 公共方法，用于释放模块
    /// </summary>
    public void Release()
    {
        if (State != EnumObjectState.Disabled)
        {
            State = EnumObjectState.Disabled;
            if (registerMode == EnumRegisterMode.AlreadyRegister)
            {
                ModuleManager.Instance.UnRegister(this);
                registerMode = EnumRegisterMode.AutoRegister;
            }
            OnRelease();
        }
    }
    /// <summary>
    /// 虚方法，用于在子类中处理释放时的额外逻辑
    /// </summary>
    protected virtual void OnRelease() { }


}
