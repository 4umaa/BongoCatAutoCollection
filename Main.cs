using Godot;
using System;

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;


public partial class Main : Node
{
    [Export] Timer timer;
    public override void _Ready()
    {
        timer.Timeout += StartCollection;
        StartCollection();

    }

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    private static string GetActiveWindowTitle()
    {
        const int nChars = 256;
        StringBuilder Buff = new StringBuilder(nChars);
        IntPtr handle = GetForegroundWindow();

        if (GetWindowText(handle, Buff, nChars) > 0)
        {
            return Buff.ToString();
        }
        return null;
    }
    static void SwitchWindoowToOld(string windowName)
    {
        var prc = Process.GetProcessesByName(windowName);
        if (prc.Length > 0)
        {
            SetForegroundWindow(prc[0].MainWindowHandle);
        }
    }

    static void SwitchToBongo()
    {
        var prc = Process.GetProcessesByName("BongoCat");
        if (prc.Length > 0)
        {
            SetForegroundWindow(prc[0].MainWindowHandle);
        }
    }
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);


    [DllImport("user32.dll")]
    static extern bool GetWindowRect(int handle, out Rect rect);

    struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    public void StartCollection()
    {
        var oldWindowName = GetActiveWindowTitle();
        SwitchToBongo();
        GetTree().CreateTimer(0.1).Timeout += () =>
        {
            var handle = GetForegroundWindow();
            var rectangle = new Rect();
            GetWindowRect((int)handle, out rectangle);
            var oldMousePos = MouseOperations.GetCursorPosition();
            MouseOperations.SetCursorPosition(rectangle.Left + 145, rectangle.Bottom - 70);
            GetTree().CreateTimer(0.05).Timeout += () =>
            {
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                GetTree().CreateTimer(0.05).Timeout += () =>
                {
                    MouseOperations.SetCursorPosition(oldMousePos);
                    SwitchWindoowToOld(oldWindowName);
                };
            };
        };
    }
}
