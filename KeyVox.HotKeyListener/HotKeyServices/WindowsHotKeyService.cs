using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;

namespace KeyVox.HotKeyListener.HotKeyServices;

public class WindowsHotKeyService : IHotKeyService
{
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID = 9000;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public event EventHandler HotKeyPressed;

    public bool TryRegisterHotKey()
    {
        bool result = RegisterHotKey(IntPtr.Zero, HOTKEY_ID, (uint)Keys.Shift, (uint)Keys.Z);
        if (result)
            Application.AddMessageFilter(new MessageFilter(WM_HOTKEY, HOTKEY_ID, () => HotKeyPressed?.Invoke(this, EventArgs.Empty)));
        return result;
    }

    public void UnregisterHotKey()
    {
        UnregisterHotKey(IntPtr.Zero, HOTKEY_ID);
    }

    private class MessageFilter : IMessageFilter
    {
        private readonly int message;
        private readonly int id;
        private readonly Action callback;

        public MessageFilter(int message, int id, Action callback)
        {
            this.message = message;
            this.id = id;
            this.callback = callback;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == message && m.WParam.ToInt32() == id)
            {
                callback();
                return true;
            }
            return false;
        }
    }
}
