#pragma warning disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace Twisted.Editor
{
    internal sealed class ProjectGeneration : AssetPostprocessor
    {
        private static string OnGeneratedCSProject(string path, string content)
        {
            var document = XDocument.Parse(content);

            var root = document.Root;
            if (root is null)
                throw new ArgumentNullException(nameof(root));

            var ns = root.GetDefaultNamespace();

            var propertyGroupName = ns + "PropertyGroup";

            var propertyGroup = root.Descendants(propertyGroupName).FirstOrDefault();

            if (propertyGroup == null)
            {
                root.Add(propertyGroup = new XElement(propertyGroupName));
            }

            var settings = ProjectGenerationSettings.instance;

            if (settings.Nullable)
            {
                var nullableName = ns + "Nullable";

                var nullable = root.Descendants(nullableName).FirstOrDefault();

                if (nullable == null)
                {
                    propertyGroup.Add(nullable = new XElement(nullableName));
                }

                nullable.Value = "enable";
            }

            if (settings.NullableWarningsDisabled)
            {
                var noWarnName = ns + "NoWarn";

                var noWarns = root.Descendants(propertyGroupName).Descendants(noWarnName);

                foreach (var noWarn in noWarns)
                {
                    var set = new HashSet<string>(noWarn.Value.Split(';'))
                    {
                        "8632" // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context
                    };

                    noWarn.Value = string.Join(";", set);
                }
            }

            if (settings.PreserveProjectStructure)
            {
                var asmDefPath = root.Descendants(ns + "None")
                    .Attributes("Include")
                    .FirstOrDefault(s => s.Value.EndsWith(".asmdef", StringComparison.OrdinalIgnoreCase))?
                    .Value;

                if (asmDefPath != null)
                {
                    asmDefPath = Path.GetFullPath(asmDefPath);

                    var dataPath = Path.GetDirectoryName(Application.dataPath)!;

                    if (asmDefPath.IndexOf(dataPath, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        var directoryName = Path.GetDirectoryName(asmDefPath);

                        foreach (var element in root.Descendants().Where(s => s.Name.LocalName is "Compile" or "None"))
                        {
                            var include = element.Attribute("Include");
                            if (include is null) 
                                continue;

                            var substring = include.Value[(directoryName!.Length + 1)..];

                            element.Add(new XElement(ns + "Link", substring));
                        }
                    }
                }
            }

            if (settings.RemoveEmptyRootNamespace)
            {
                root.Descendants(ns + "RootNamespace").ToList().ForEach(s =>
                {
                    if (string.IsNullOrEmpty(s.Value))
                    {
                        s.Remove();
                    }
                });
            }

            // ReSharper disable once ConvertToUsingDeclaration
            using (var writer = new Utf8StringWriter())
            {
                document.Save(writer);

                content = writer.ToString();
            }

            return content;
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}