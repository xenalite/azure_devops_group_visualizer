using System;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using DevOps.VSTS.Cmdlets.Utilities;

namespace DevOps.VSTS.Cmdlets.Implementation
{
    public abstract class CmdletBase : Cmdlet
    {
        protected CmdletBase()
        {
            // We need to manually resolve binding redirects, because PowerShell ignores our app.config
            RedirectAssembly("System.Net.Http", Version.Parse("4.1.0.0"), "b03f5f7f11d50a3a");
            RedirectAssembly("Newtonsoft.Json", Version.Parse("8.0.0.0"), "30ad4fe6b2a6aeed");
            RedirectAssembly("System.Security.Cryptography.X509Certificates", Version.Parse("4.1.0.0"), "b03f5f7f11d50a3a");
            RedirectAssembly("Microsoft.Win32.Primitives", Version.Parse("4.0.1.0"), "b03f5f7f11d50a3a");
            RedirectAssembly("System.Runtime.Serialization.Primitives", Version.Parse("4.1.1.0"), "b03f5f7f11d50a3a");
        }

        private static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;

                var alreadyLoadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == requestedAssembly.Name);

                if (alreadyLoadedAssembly != null)
                    return alreadyLoadedAssembly;

                var token = new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken();

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(token);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                return Assembly.Load(requestedAssembly);
            };
        }

        protected override void ProcessRecord()
        {
            using (PowerShellLogger.Attach(this))
            {
                Execute();
                base.ProcessRecord();
            }
        }

        protected abstract void Execute();
    }
}