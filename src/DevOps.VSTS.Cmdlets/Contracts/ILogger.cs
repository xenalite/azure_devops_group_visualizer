using System;

namespace DevOps.VSTS.Cmdlets.Contracts
{
    public interface ILogger : IDisposable
    {
        void Output(object output);
        void Error(Exception exception);
        void Error(string message);
        void Warning(string message);
        void Verbose(string message);
    }
}