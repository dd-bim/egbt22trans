//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace egbt22lib
//{
//    public static class LoggingInitializer
//    {
//        private static ILoggerFactory? _loggerFactory;

//        public static void InitializeLogging(ILoggerFactory loggerFactory)
//        {
//            _loggerFactory = loggerFactory;

//            // Konfigurieren Sie die Logger für die verschiedenen Klassen
//            Convert.InitializeLogger(_loggerFactory.CreateLogger("Convert"));

//            var logger = _loggerFactory.CreateLogger("LoggingInitializer");
//            logger.LogInformation("Logging initialized for egbt22lib.");
//        }

//        public static void ShutdownLogging()
//        {
//            if (_loggerFactory != null)
//            {
//                _loggerFactory.Dispose();
//                _loggerFactory = null;
//            }
//        }
//    }}
