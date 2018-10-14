using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace DevOps.Cmdlets.Common.Utilities
{
    public static class PipelineUtils
    {
        public static IEnumerable<PSObject> Execute(Command command)
        {
            using (var pipeline = Runspace.DefaultRunspace.CreateNestedPipeline())
            {
                Log.Verbose($"Invoking nested execution: {command}");
                pipeline.Commands.Add(command);
                var pipelineResults = pipeline.Invoke();
                return pipelineResults;
            }
        }
    }
}