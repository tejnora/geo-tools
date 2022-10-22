using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace GeoBase.Utils
{
    public class CrashReporter
    {
        public CrashReporter(Application application)
        {
            application.DispatcherUnhandledException += DispatcherUnhandledException;
        }
        void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var regName = string.Format("{0}.crash",
                                           Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
            var crashFilePath = appDir + regName;
            using (TextWriter writer = File.CreateText(crashFilePath))
            {
                writer.WriteLine("HelpLink:{0}", e.Exception.HelpLink);
                writer.WriteLine("InnerException:{0}", e.Exception.InnerException);
                writer.WriteLine("Message:{0}", e.Exception.Message);
                writer.WriteLine("Source:{0}", e.Exception.Source);
                writer.WriteLine("StackTrace:{0}", e.Exception.StackTrace);
                writer.WriteLine("TargetSite:{0}", e.Exception.TargetSite);
            }
        }
    }
}
