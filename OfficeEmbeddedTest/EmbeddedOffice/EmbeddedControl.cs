using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LingoesThief
{
    public partial class EmbeddedControl : UserControl
    {
        #region "API usage declarations"

        [DllImport("user32.dll")]
        public static extern int FindWindow(string strclassName, string strWindowName);

        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal position
            int Y,                  // vertical position
            int cx,                 // width
            int cy,                 // height
            uint uFlags             // window-positioning options
            );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(
            int hWnd,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            bool bRepaint
            );

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(int hWnd, int nCmdShow);


        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string lpszClass, string lpszWindow);



        //        HWND WINAPI FindWindowEx(
        //  __in_opt  HWND hwndParent,
        //  __in_opt  HWND hwndChildAfter,
        //  __in_opt  LPCTSTR lpszClass,
        //  __in_opt  LPCTSTR lpszWindow
        //);


        const int SW_SHOWDEFAULT = 10;
        private const int SW_SHOWNORMAL = 1;

        const int SWP_DRAWFRAME = 0x20;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOZORDER = 0x4;

        #endregion

        public EmbeddedControl()
        {
            InitializeComponent();
            Resize += OnResize;
        }

        private int _hostedHandle;

        /// <summary>
        /// Public variable to access Document opened in this control
        /// </summary>
        protected int HostedHandle
        {
            get { return _hostedHandle; }
            set
            {
                _hostedHandle = value;
                if (_hostedHandle > 0)
                    LoadWindow(_hostedHandle);

            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            PositionHandle();
        }

        protected void PositionHandle()
        {
            if (!loaded)
                return;
            var borderWidth = SystemInformation.Border3DSize.Width;
            var borderHeight = SystemInformation.Border3DSize.Height;
            var captionHeight = SystemInformation.CaptionHeight;
            var statusHeight = SystemInformation.ToolWindowCaptionHeight;

            MoveWindow(
                HostedHandle,
                0,
                0,
                Bounds.Width,
                Bounds.Height,
                true);
        }


        /// <summary>
        /// Just load the word control
        /// </summary>
        public void LoadWindow(string className)
        {
            HostedHandle = FindWindow(className, null);
        }

        public void LoadWindow(int handle)
        {
            if (HostedHandle > 0)
            {
                SetParent(handle, Handle.ToInt32());
                //SetWindowPos(handle, Handle.ToInt32(), 0, 0, Bounds.Width, Bounds.Height, SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME | SWP_NOSIZE);
                PositionHandle();
                loaded = true;
            }
        }

        private bool loaded;
        public void UnloadWindow()
        {
            if (!loaded)
                return;
            SetParent(HostedHandle, 0);

            loaded = false;
        }
    }
    public enum DocType
    {
        docx,
        doc,
        xml2007,
        xml2003,
        PDF
    }
}