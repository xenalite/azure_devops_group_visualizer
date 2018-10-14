using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using DevOps.Cmdlets.Common.Implementation;
using DevOps.Cmdlets.Common.Utilities;
using DevOps.VSTS.Cmdlets.Dtos;
using DevOps.VSTS.Cmdlets.Services;
using Newtonsoft.Json;

namespace DevOps.VSTS.Cmdlets.Implementation
{
    [Cmdlet(VerbsCommon.Get, "IxsVstsUserMembershipsReport")]
    public class GetVstsUserMembershipsReport : CmdletBase
    {
        [Parameter(Mandatory = true)]
        public string JsonInputFilePath { get; set; }

        [Parameter(Mandatory = false)]
        public string DiagramOutputDir { get; set; }

        [Parameter(Mandatory = false)]
        public string[] IdentityNamesToCollapse { get; set; } = new string[0];

        protected override void Execute()
        {
            var jsonContent = File.ReadAllText(JsonInputFilePath);
            var projectCollectionMemberships = JsonConvert.DeserializeObject<IdentityMemberships>(jsonContent);
            if (projectCollectionMemberships == null)
            {
                Log.Warning("Project Collection report is invalid.");
                return;
            }

            var projectCollectionOutputFile = Path.Combine(DiagramOutputDir, "ProjectCollection.dgml");
            SaveGraph(projectCollectionMemberships, projectCollectionOutputFile);
            SaveProjectsGraph(projectCollectionMemberships);
        }

        private void SaveProjectsGraph(IdentityMemberships projectCollectionMemberships)
        {
            var projectFilterName = @"\Project Valid Users";
            var teamProjectMemberships = GetTeamProjectsMemberships(projectCollectionMemberships, projectFilterName);
            foreach (var memberships in teamProjectMemberships)
            {
                var projectName = memberships.DisplayName
                    .Replace(projectFilterName, string.Empty)
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                var projectOutputFile = Path.Combine(DiagramOutputDir, $"{projectName}.dgml");
                SaveGraph(memberships, projectOutputFile);
            }
        }

        private static IEnumerable<IdentityMemberships> GetTeamProjectsMemberships(
            IdentityMemberships root, string filter)
        {
            if (root.DisplayName.EndsWith(filter))
                return new[] { root };

            return root
                .NestedIdentities
                .SelectMany(e => GetTeamProjectsMemberships(e, filter));
        }

        private void SaveGraph(IdentityMemberships identityMemberships, string outputFilePath)
        {
            var graph = new VstsMembershipsDirectedGraph(identityMemberships, IdentityNamesToCollapse);
            var document = graph.CreateGraph();
            document.Save(outputFilePath);
        }
    }
}