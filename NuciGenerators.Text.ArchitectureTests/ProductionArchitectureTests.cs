using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ArchUnitNet;
using ArchUnitNet.Common.Assertion;
using ArchUnitNet.Common.FluentApi;
using ArchUnitNet.Files.FluentApi;

using NUnit.Framework;

namespace NuciGenerators.Text.ArchitectureTests
{
    [TestFixture]
    public sealed class ProductionArchitectureTests
    {
        private static readonly string ProjectPath = ResolveProjectPath();

        [Test]
        public async Task GivenProductionProject_WhenCheckingSourceFiles_ThenProjectHasNoCycles()
        {
            IReadOnlyList<Violation> violations = await ArchUnit.ProjectFiles(ProjectPath)
                .InPath("**/*.cs")
                .Should()
                .HaveNoCycles()
                .CheckAsync(new CheckOptions());

            Assert.That(violations, Is.Empty, BuildFailureMessage(violations));
        }

        [Test]
        public async Task GivenProductionProject_WhenCheckingSourceFiles_ThenProductionFilesAreInProductionFolder()
        {
            IReadOnlyList<Violation> violations = await ArchUnit.ProjectFiles(ProjectPath)
                .InPath("**/*.cs")
                .Should()
                .BeInPath("**/*.cs")
                .CheckAsync(new CheckOptions());

            Assert.That(violations, Is.Empty, BuildFailureMessage(violations));
        }

        [Test]
        public void GivenProductionAssembly_WhenInspectingTypes_ThenAssemblyIsLoadedExplicitly()
        {
            Assembly assembly = typeof(NuciGenerators.Text.NameGenerator).Assembly;

            Assert.That(assembly.GetName().Name, Is.EqualTo("NuciGenerators.Text"));
            Assert.That(assembly.GetTypes(), Is.Not.Empty);
            Assert.That(assembly.GetTypes().All(type => type.Assembly == assembly), Is.True);
        }

        private static string BuildFailureMessage(IEnumerable<Violation> violations) =>
            string.Join(Environment.NewLine, violations.Select(violation => violation.ToString()));

        private static string ResolveProjectPath()
        {
            string projectDirectory = System.IO.Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string projectPath = System.IO.Path.Combine(projectDirectory, "NuciGenerators.Text", "NuciGenerators.Text.csproj");

            return $"/{projectPath}";
        }
    }
}
