using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DevOps.VSTS.Cmdlets.Dtos;

namespace DevOps.VSTS.Cmdlets.Services
{
    public class VstsMembershipsDirectedGraph
    {
        private static readonly XNamespace DgmlNamespace = "http://schemas.microsoft.com/vs/2009/dgml";
        private static readonly XName NodesXName = DgmlNamespace + "Nodes";
        private static readonly XName LinksXName = DgmlNamespace + "Links";
        private static readonly XName CategoriesXName = DgmlNamespace + "Categories";
        private static readonly XName StylesXName = DgmlNamespace + "Styles";

        private readonly IDictionary<TFIdentityCategory, string> _nodeColours
            = new Dictionary<TFIdentityCategory, string>
            {
                {TFIdentityCategory.ProjectCollection, "#ffffff"}, // white
                {TFIdentityCategory.ResourceGroup, "#0dd80d"}, // light green
                {TFIdentityCategory.RoleGroup, "#0dd80d"}, // light green
                {TFIdentityCategory.OtherGroup, "#fa7800"}, // dark orange
                {TFIdentityCategory.TFGroup, "#c6c6f4"}, // light blue
                {TFIdentityCategory.TFAccount, "#ffff00"}, // yellow
                {TFIdentityCategory.UserAccount, "#ffffff"}, // white
                {TFIdentityCategory.DirectUserAccount, "#ff0000"} // red
            };

        private readonly XElement _nodes;
        private readonly XElement _links;
        private readonly XElement _styles;
        private readonly XElement _categories;

        private readonly IdentityMemberships _rootIdentity;
        private readonly string[] _identitiesToCollapse;
        private readonly XDocument _graph;

        public VstsMembershipsDirectedGraph(IdentityMemberships rootIdentity, string[] identitiesToCollapse)
        {
            _rootIdentity = rootIdentity;
            _identitiesToCollapse = identitiesToCollapse;

            _graph = new XDocument(
                new XElement(DgmlNamespace + "DirectedGraph",
                    new XAttribute("GraphDirection", "TopToBottom"),
                    new XAttribute("Layout", "ForceDirected"),
                    new XElement(NodesXName),
                    new XElement(LinksXName),
                    new XElement(CategoriesXName),
                    new XElement(StylesXName)));

            if (_graph.Root == null)
                throw new InvalidOperationException("Root of the XDocument is null.");

            _nodes = _graph.Root.Element(NodesXName);
            _links = _graph.Root.Element(LinksXName);
            _categories = _graph.Root.Element(CategoriesXName);
            _styles = _graph.Root.Element(StylesXName);
        }

        public XDocument CreateGraph()
        {
            AddCategories();
            AddIdentityNodes(_rootIdentity);

            return _graph;
        }

        private Guid AddIdentityNodes(IdentityMemberships identity)
        {
            var parentNodeId = AddNode(identity);
            foreach (var childIdentity in identity.NestedIdentities)
            {
                var childNodeId = AddIdentityNodes(childIdentity);
                AddLink(parentNodeId, childNodeId);
            }
            return parentNodeId;
        }

        private Guid AddNode(IdentityMemberships identity)
        {
            var id = Guid.NewGuid();
            var label = identity.DisplayName;
            var seenBefore = _nodes.Elements().Any(e => e.Attributes("Label").Any(a => a.Value == label));
            var shouldCollapse = seenBefore 
                || identity.NestedIdentities.Length == 0 
                || _identitiesToCollapse.Contains(identity.DisplayName);

            var node = new XElement(DgmlNamespace + "Node",
                new XAttribute("Id", id),
                new XAttribute("Label", label),
                new XAttribute("Category", identity.Category),
                new XAttribute("Group", shouldCollapse ? "Collapsed" : "Expanded"));

            _nodes.Add(node);
            return id;
        }

        private void AddLink(Guid sourceNodeId, Guid targetNodeId)
        {
            var link = new XElement(DgmlNamespace + "Link",
                new XAttribute("Source", sourceNodeId),
                new XAttribute("Target", targetNodeId),
                new XAttribute("Category", "Contains"));

            _links.Add(link);
        }

        private void AddCategories()
        {
            foreach (var nodeCategory in _nodeColours.Keys)
            {
                var category = $"{nodeCategory}";
                AddCategory(category);

                var style = AddStyle(category);
                style.Add(AddStyleCondition($"HasCategory('{category}')"));
                style.Add(NewStyleSetterProperty("Background", _nodeColours[nodeCategory]));
            }
        }

        private void AddCategory(string name)
        {
            var category = new XElement(DgmlNamespace + "Category",
                new XAttribute("Id", name),
                new XAttribute("Label", name));

            _categories.Add(category);
        }

        private XElement AddStyle(string groupLabel)
        {
            var style = new XElement(DgmlNamespace + "Style",
                new XAttribute("TargetType", "Node"),
                new XAttribute("GroupLabel", groupLabel),
                new XAttribute("ValueLabel", "Has category"));

            _styles.Add(style);
            return style;
        }

        private static XElement AddStyleCondition(string expression)
        {
            var element = new XElement(DgmlNamespace + "Condition",
                new XAttribute("Expression", expression));

            return element;
        }

        private static XElement NewStyleSetterProperty(string propertyName, string propertyValue)
        {
            var element = new XElement(DgmlNamespace + "Setter",
                new XAttribute("Property", propertyName),
                new XAttribute("Value", propertyValue));

            return element;
        }
    }
}