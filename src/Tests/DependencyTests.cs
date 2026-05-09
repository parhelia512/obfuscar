#region Copyright (c) 2007 Ryan Williams <drcforbin@gmail.com>

/// <copyright>
/// Copyright (c) 2007 Ryan Williams <drcforbin@gmail.com>
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
/// 
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// </copyright>

#endregion

using System.IO;
using Obfuscar;
using Xunit;

namespace ObfuscarTests
{
    public class DependencyTests
    {
        public DependencyTests()
        {
            TestHelper.CleanInput();
            TestHelper.BuildAssemblies(names: ["AssemblyA", "AssemblyB"]);
        }

        [Fact]
        public void CheckCrossAssemblyEnumDefaultParameterDoesNotCrash()
        {
            // Regression test for issue #235: obfuscating two assemblies where one references
            // the other's enum as a default parameter value must not crash with ResolutionException.
            // The enum type in the library is renamed; the referencing assembly's constant must
            // be updated to use the new name, otherwise writing fails.
            TestHelper.CleanInput();
            TestHelper.BuildAssemblies(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest, false,
                "AssemblyWithEnumLib", "AssemblyWithEnumUser");

            string inputPath = TestHelper.InputPath;
            string outputPath = TestHelper.OutputPath;
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='KeepPublicApi' value='false' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Module file='$(InPath){2}AssemblyWithEnumLib.dll' />" +
                @"<Module file='$(InPath){2}AssemblyWithEnumUser.dll' />" +
                @"</Obfuscator>", inputPath, outputPath, Path.DirectorySeparatorChar);

            // Must complete without throwing ResolutionException
            var obfuscator = TestHelper.Obfuscate(xml);
            var map = obfuscator.Mapping;

            // Verify the enum type was actually renamed (not just skipped)
            var inLib = AssemblyDefinition.ReadAssembly(Path.Combine(inputPath, "AssemblyWithEnumLib.dll"));
            var enumType = inLib.MainModule.GetType("TestClasses.LibraryMode");
            var enumEntry = map.GetClass(new TypeKey(enumType));
            Assert.True(enumEntry.Status == ObfuscationStatus.Renamed,
                $"LibraryMode enum should be renamed, got: {enumEntry.Status} ({enumEntry.StatusText})");

            // Verify the output assemblies exist
            Assert.True(File.Exists(Path.Combine(outputPath, "AssemblyWithEnumLib.dll")));
            Assert.True(File.Exists(Path.Combine(outputPath, "AssemblyWithEnumUser.dll")));
        }

        [Fact]
        public void CheckGoodDependency()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Module file='$(InPath){1}AssemblyB.dll' />" +
                @"</Obfuscator>", TestHelper.InputPath, Path.DirectorySeparatorChar);

            TestHelper.Obfuscate(xml);
        }

        [Fact]
        public void CheckDeletedDependency()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Module file='$(InPath){1}AssemblyB.dll' />" +
                @"</Obfuscator>", TestHelper.InputPath, Path.DirectorySeparatorChar);

            // explicitly delete AssemblyA
            File.Delete(Path.Combine(TestHelper.InputPath, "AssemblyA.dll"));
            var exception = Assert.Throws<ObfuscarException>(() => { TestHelper.Obfuscate(xml); });
            Assert.Equal("Unable to resolve dependency:  AssemblyA", exception.Message);
        }

        [Fact]
        public void CheckMissingDependency()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Module file='{0}{1}AssemblyD.dll' />" +
                @"</Obfuscator>", TestHelper.InputPath, Path.DirectorySeparatorChar);

            // InPath defaults to '.', which doesn't contain AssemblyA
            string destFileName = Path.Combine(TestHelper.InputPath, "AssemblyD.dll");
            if (!File.Exists(destFileName))
            {
                File.Copy(Path.Combine(TestHelper.InputPath, @"..", "AssemblyD.dll"),
                    destFileName, true);
            }

            var exception = Assert.Throws<ObfuscarException>(() => { TestHelper.Obfuscate(xml); });
            Assert.Equal("Unable to resolve dependency:  AssemblyC", exception.Message);
        }
    }
}
