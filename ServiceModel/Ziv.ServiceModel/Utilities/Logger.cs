using System;

namespace Ziv.ServiceModel.Utilities
{
    public class Logger
    {
        public static event EventHandler<LogRegisteredEventArgs> LogRegisterd;

        public static void LogEvent(string eventDescription, object sender, ImportanceLevels importnceLevel)
        {
            if (LogRegisterd != null)
            {
                var logEventArgs = new LogRegisteredEventArgs()
                                             {
                                                 EventDescription = eventDescription,
                                                 Sender = sender,
                                                 ImportanceLevels = importnceLevel
                                             };
                foreach (EventHandler<LogRegisteredEventArgs> method in LogRegisterd.GetInvocationList())
                {
                    try
                    {
                        method.Invoke(sender, logEventArgs);
                    }
                    catch 
                    {
                        // Don't fail on log error.
                    }
                }
            }
        }
    }

    public enum ImportanceLevels
    {
        aMostImportant = 1,
        bVeryHigh,
        cHigh,
        dMedium,
        eLow,
        fVeryLow,
        gUnimportant,
        xUnspecified,
    }

    public class LogRegisteredEventArgs : EventArgs
    {
        public string EventDescription { get; set; }
        public object Sender { get; set; }
        public ImportanceLevels ImportanceLevels { get; set; }
    }
}
