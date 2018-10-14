using System;
using System.Collections.Generic;
using DevOps.VSTS.Cmdlets.Contracts;

namespace DevOps.VSTS.Cmdlets.Utilities
{
    public static class Log
    {
        public static ICollection<ILogger> Loggers { get; } = new List<ILogger>();

        public static void Attach(ILogger logger)
        {
            if (!Loggers.Contains(logger))
                Loggers.Add(logger);
        }

        public static void Detach(ILogger logger)
        {
            if (Loggers.Contains(logger))
                Loggers.Remove(logger);
        }

        public static void Output(object output)
        {
            foreach (var logger in Loggers)
                logger.Output(output);
        }

        public static void Error(Exception exception)
        {
            foreach (var logger in Loggers)
                logger.Error(exception);
        }

        public static void Error(string message)
        {
            foreach (var logger in Loggers)
                logger.Error(message);
        }

        public static void Warning(string message)
        {
            foreach (var logger in Loggers)
                logger.Warning(message);
        }

        public static void Verbose(string message)
        {
            foreach (var logger in Loggers)
                logger.Verbose(message);
        }
    }
}