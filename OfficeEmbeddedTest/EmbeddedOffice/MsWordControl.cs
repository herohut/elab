using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LingoesThief;
using Microsoft.Office.Interop.Word;

namespace EmbeddedOffice
{
    public partial class MsWordControl : EmbeddedControl
    {
        public MsWordControl()
        {
            InitializeComponent();
            this.Disposed += new EventHandler(MsWordControl_Disposed);    
        }

        void MsWordControl_Disposed(object sender, EventArgs e)
        {
            UnloadWindow();
        }


        public void New()
        {
            PrepareApplication();
            currentDoct = _application.Documents.Add();
            PositionHandle();
        }

        private Document currentDoct;
        private const string WordApplicationTitle = "MsWordControl";

        void PrepareApplication()
        {
            if (_application == null)
            {
                _application = new Microsoft.Office.Interop.Word.Application { Caption = WordApplicationTitle, Visible = true };



                ((ApplicationEvents4_Event)_application).Quit += _application_ApplicationEvents4_Event_Quit;

                var processId = Win32Helper.GetProcessIdByWindowTitle(WordApplicationTitle);
                //var processId = Win32Helper.GetEmtyWordProcess();

                while (processId < 0)
                {
                    Thread.Sleep(5);
                    processId = Win32Helper.GetProcessIdByWindowTitle(WordApplicationTitle);
                    //processId = Win32Helper.GetEmtyWordProcess();
                }

                _application.Visible = false;

                HostedHandle = Win32Helper.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, WordApplicationTitle).ToInt32();

                _application.Visible = true;
            }
        }

        void _application_ApplicationEvents4_Event_Quit()
        {
            _application = null;
        }


        private Microsoft.Office.Interop.Word.Application _application;

        internal void CloseDocument()
        {
            currentDoct.Close(false);
            if (_application.Documents.Count == 0)
                _application.Quit();
        }

        internal void OpenFile(string file)
        {
            PrepareApplication();
            currentDoct = _application.Documents.Open(file);
        }
    }
}
