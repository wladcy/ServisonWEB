using NLog;
using System;

namespace Admin.Services
{
    public class LoggerController
    {
        public static void Initialize()
        {
            LogManager.Configuration.Variables["AppName"] = SettingsController.AppName.Name;
        }
        public static void AddBeginMethodLog(string type, string methodName)
        {
            Logger log = LogManager.GetLogger(type);
            log.Info("Wywołanie metody " + methodName + "...");
        }

        public static void AddErrorLog(string type, string error)
        {
            Logger log = LogManager.GetLogger(type);
            log.Error(error);
        }

        public static void AddExceptionLog(string type, Exception exception)
        {
            Logger log = LogManager.GetLogger(type);
            log.Error("Błąd: " + exception.Message);
            log.Error(exception.StackTrace);
            log.Error(exception.InnerException);
            log.Error(exception.Message);
        }

        public static void AddEndMethodLog(string type, string methodName, long miliseconds)
        {
            Logger log = LogManager.GetLogger(type);
            log.Info("Wykonywanie metody " + methodName + " zakończone po czasi" +
                "e: " + miliseconds + " ms.");
        }
    }
}
