namespace KeyVox.HotKeyListener.HotKeyServices;

public interface IHotKeyService
{
    bool TryRegisterHotKey();
    void UnregisterHotKey();
    event EventHandler HotKeyPressed;
}