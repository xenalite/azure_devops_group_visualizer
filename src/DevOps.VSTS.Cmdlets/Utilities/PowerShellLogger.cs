using System;
using System.Management.Automation;
using DevOps.VSTS.Cmdlets.Contracts;

namespace DevOps.VSTS.Cmdlets.Utilities
{
    public class PowerShellLogger : ILogger
    {
        private readonly Cmdlet _cmdlet;

        public static IDisposable Attach(Cmdlet cmdlet)
        {
            var logger = new PowerShellLogger(cmdlet);
            Log.Attach(logger);
            return logger;
        }

        private PowerShellLogger(Cmdlet cmdlet)
        {
            _cmdlet = cmdlet;
        }

        public void Output(object output)
        {
            _cmdlet.WriteObject(output);
        }

        public void Error(Exception exception)
        {
            // TODO: Fix logging from different thread
            _cmdlet.WriteError(new ErrorRecord(exception, "", ErrorCategory.FromStdErr, null));
        }

        public void Error(string message)
        {
            _cmdlet.WriteError(new ErrorRecord(new Exception(message), "", ErrorCategory.FromStdErr, null));
        }

        public void Warning(string message)
        {
            _cmdlet.WriteWarning(message);
        }

        public void Verbose(string message)
        {
            _cmdlet.WriteVerbose(message);
        }

        public void Dispose()
        {
            Log.Detach(this);
        }
    }
}