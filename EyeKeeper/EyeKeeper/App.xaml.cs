﻿using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using EyeKeeper.ViewModel;

namespace EyeKeeper
{
    public partial class App : Application
    {
        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
        }

        internal string ApplicationID = "{BACD025E-123C-4729-A291-1FDC89A11C44}";
        internal string AcqusitionToken = "{E2F9BD0A-C351-4313-928C-7AF8D111A2D4}";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RootVisual = new Main();
            DispatcherHelper.Initialize();
            App.Current.MainWindow.TopMost = true;
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            ViewModelLocator.Cleanup();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
