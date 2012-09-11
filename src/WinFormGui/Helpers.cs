namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Windows.Forms;

    public static class Helpers
    {
         public static void ShowError (this Exception ex, Logging.LoggingInterfaces.ITracer logger, string message)
         {
             logger.LogError("{0}: {1}", message, ex);
             MessageBox.Show(ex.Message, message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
    }
}