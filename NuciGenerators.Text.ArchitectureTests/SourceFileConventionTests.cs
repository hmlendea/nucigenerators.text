using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciGenerators.Text.ArchitectureTests
{
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenInspectingDeclarations_ThenEveryFileHasOneTypeMatchingItsName()
        {
            string sourceDirectory = ResolveSourceDirectory();
            string[] sourceFiles = Directory.GetFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(file => !file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal) &&
                    !file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .OrderBy(file => file, StringComparer.Ordinal)
                .ToArray();

            Assert.That(sourceFiles, Is.Not.Empty, "Production source files were not found.");

            List<string> violations = [];

            foreach (string sourceFile in sourceFiles)
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFile), path: sourceFile);
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
                List<string> declarationNames = root.DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Select(GetDeclarationName)
                    .ToList();
                declarationNames.AddRange(root.DescendantNodes().OfType<DelegateDeclarationSyntax>().Select(GetDeclarationName));

                if (declarationNames.Count != 1)
                {
                    violations.Add($"{sourceFile}: expected exactly one declared type, found {declarationNames.Count}.");
                    continue;
                }

                string expectedName = Path.GetFileNameWithoutExtension(sourceFile);
                string actualName = declarationNames[0];

                if (!string.Equals(expectedName, actualName, StringComparison.Ordinal))
                {
                    violations.Add($"{sourceFile}: expected type '{expectedName}', found '{actualName}'.");
                }
            }

            Assert.That(violations, Is.Empty, string.Join(Environment.NewLine, violations));
        }

        [Test]
        public void GivenProductionAssembly_WhenInspectingTypes_ThenAllPublicTypesUseTheProductionNamespace()
        {
            Assembly assembly = typeof(NuciGenerators.Text.NameGenerator).Assembly;
            Type[] types = assembly.GetTypes();
            List<string> violations = types
                .Where(type => type.IsPublic && !type.Namespace!.StartsWith("NuciGenerators.Text", StringComparison.Ordinal))
                .Select(type => $"{type.FullName}: public type is outside the production namespace.")
                .ToList();

            Assert.That(types, Is.Not.Empty);
            Assert.That(violations, Is.Empty, string.Join(Environment.NewLine, violations));
        }

        private static string GetDeclarationName(BaseTypeDeclarationSyntax declaration)
        {
            return declaration.Identifier.ValueText;
        }

        private static string GetDeclarationName(DelegateDeclarationSyntax declaration)
        {
            return declaration.Identifier.ValueText;
        }

        private static string ResolveSourceDirectory()
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

            return Path.Combine(projectDirectory, "NuciGenerators.Text");
        }
    }
}
