using ManagedShell.Interop;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RetroBar.Utilities
{
    public class ExplorerMonitor : IDisposable
    {
        private bool _ExplorerMonitorIsMonitoring;
        private MonitorWindow _explorerMonitor;

        public void ExplorerMonitorStart(WindowManager _windowManagerReference)
        {
            if(_ExplorerMonitorIsMonitoring) // Prevent multiple monitors.
            {
                return;
            }
            else
            {
                _ExplorerMonitorIsMonitoring = true; // We will set flag to true to prevent multiple monitors.
                _explorerMonitor = new MonitorWindow(_windowManagerReference); // Start monitoring.
            }
        }

        public void Dispose()
        {
            if (_explorerMonitor != null){_explorerMonitor?.Dispose();}
        }

        // NativeWindow implementation for monitoring
        private class MonitorWindow : NativeWindow, IDisposable
        {
            private WindowManager _windowManager;
            private static readonly int TaskbarCreatedMessage = NativeMethods.RegisterWindowMessage("TaskbarCreated");

            public MonitorWindow(WindowManager _windowManagerReference)
            {
                _windowManager = _windowManagerReference;
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == TaskbarCreatedMessage)
                {
                    try
                    {
                        _windowManager.ReopenTaskbars(); // Reopen taskbars if explorer.exe is restarted.
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error handling TaskbarCreated message on ExplorerMonitor: {ex.Message}");
                    }
                }

                // Call the base class to process other messages so we dont accidentally cause crashes or bugs.
                base.WndProc(ref m);
            }

            public void Dispose()
            {
                DestroyHandle();
            }
        }
    }
}
