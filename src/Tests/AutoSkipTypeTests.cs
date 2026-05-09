using Obfuscar;
using System.IO;
using Xunit;

namespace ObfuscarTests
{
    public class AutoSkipTypeTests
    {
        MethodDefinition FindByName(TypeDefinition typeDef, string name)
        {
            foreach (MethodDefinition method in typeDef.Methods)
                if (method.Name == name)
                    return method;

            Assert.Fail(string.Format("Expected to find method: {0}", name));
            return null; // never here
        }

        [Fact]
        public void CheckHidePrivateApiFalse()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='KeepPublicApi' value='false' />" +
                @"<Var name='HidePrivateApi' value='false' />" +
                @"<Module file='$(InPath){2}AssemblyWithTypes.dll'>" +
                @"</Module>" +
                @"</Obfuscator>", TestHelper.InputPath, TestHelper.OutputPath, Path.DirectorySeparatorChar);

            var obfuscator = TestHelper.BuildAndObfuscate("AssemblyWithTypes", string.Empty, xml);
            var map = obfuscator.Mapping;

            string assmName = "AssemblyWithTypes.dll";

            AssemblyDefinition inAssmDef = AssemblyDefinition.ReadAssembly(
                Path.Combine(TestHelper.InputPath, assmName));

            var classBType = inAssmDef.MainModule.GetType("TestClasses.InternalClass");
            var classB = map.GetClass(new TypeKey(classBType));

            Assert.True(classB.Status == ObfuscationStatus.Skipped, "Internal class is obfuscated");

            var enumType = inAssmDef.MainModule.GetType("TestClasses.TestEnum");
            var enum1 = map.GetClass(new TypeKey(enumType));
            Assert.True(enum1.Status == ObfuscationStatus.Skipped, "Internal enum is obfuscated");

            var classAType = inAssmDef.MainModule.GetType("TestClasses.PublicClass");
            var classA = map.GetClass(new TypeKey(classAType));
            var classAmethod1 = FindByName(classAType, "PrivateMethod");
            var classAmethod2 = FindByName(classAType, "PublicMethod");

            var classAMethod1 = map.GetMethod(new MethodKey(classAmethod1));
            var classAMethod2 = map.GetMethod(new MethodKey(classAmethod2));

            Assert.True(classA.Status == ObfuscationStatus.Renamed, "Public class is not obfuscated");
            Assert.True(classAMethod1.Status == ObfuscationStatus.Skipped, "private method is obfuscated.");
            Assert.True(classAMethod2.Status == ObfuscationStatus.Renamed, "pubilc method is not obfuscated.");
        }

        [Fact]
        public void CheckHidePrivateApiTrue()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Module file='$(InPath){2}AssemblyWithTypes.dll'>" +
                @"</Module>" +
                @"</Obfuscator>", TestHelper.InputPath, TestHelper.OutputPath, Path.DirectorySeparatorChar);

            var obfuscator = TestHelper.BuildAndObfuscate("AssemblyWithTypes", string.Empty, xml);
            var map = obfuscator.Mapping;

            string assmName = "AssemblyWithTypes.dll";
            AssemblyDefinition inAssmDef = AssemblyDefinition.ReadAssembly(
                Path.Combine(TestHelper.InputPath, assmName));

            var classBType = inAssmDef.MainModule.GetType("TestClasses.InternalClass");
            var classB = map.GetClass(new TypeKey(classBType));

            Assert.True(classB.Status == ObfuscationStatus.Renamed, "Internal class should have been obfuscated");

            var enumType = inAssmDef.MainModule.GetType("TestClasses.TestEnum");
            var enum1 = map.GetClass(new TypeKey(enumType));
            Assert.True(enum1.Status == ObfuscationStatus.Renamed, "Internal enum should have been obfuscated");
        }

        [Fact]
        public void CheckKeepPublicApiFalse()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Var name='KeepPublicApi' value='false' />" +
                @"<Module file='$(InPath){2}AssemblyWithTypes.dll'>" +
                @"</Module>" +
                @"</Obfuscator>", TestHelper.InputPath, TestHelper.OutputPath, Path.DirectorySeparatorChar);

            var obfuscator = TestHelper.BuildAndObfuscate("AssemblyWithTypes", string.Empty, xml);
            var map = obfuscator.Mapping;

            string assmName = "AssemblyWithTypes.dll";

            AssemblyDefinition inAssmDef = AssemblyDefinition.ReadAssembly(
                Path.Combine(TestHelper.InputPath, assmName));

            TypeDefinition classAType = inAssmDef.MainModule.GetType("TestClasses.PublicClass");
            MethodDefinition classAmethod1 = FindByName(classAType, "PrivateMethod");
            MethodDefinition classAmethod2 = FindByName(classAType, "PublicMethod");

            ObfuscatedThing classAMethod1 = map.GetMethod(new MethodKey(classAmethod1));
            ObfuscatedThing classAMethod2 = map.GetMethod(new MethodKey(classAmethod2));

            var classA = map.GetClass(new TypeKey(classAType));
            Assert.True(classA.Status == ObfuscationStatus.Renamed, "Public class should have been obfuscated");
            Assert.True(classAMethod1.Status == ObfuscationStatus.Renamed, "private method is not obfuscated.");
            Assert.True(classAMethod2.Status == ObfuscationStatus.Renamed, "public method is not obfuscated.");

            var protectedMethod = FindByName(classAType, "ProtectedMethod");
            var protectedAfter = map.GetMethod(new MethodKey(protectedMethod));
            Assert.True(protectedAfter.Status == ObfuscationStatus.Renamed, "protected method is not obfuscated.");
        }

        [Fact]
        public void CheckKeepPublicApiTrue()
        {
            string output = TestHelper.OutputPath;
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='KeepPublicApi' value='true' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Var name='HideStrings' value='false' />" +
                @"<Module file='$(InPath){2}AssemblyWithTypes.dll'>" +
                @"</Module>" +
                @"</Obfuscator>", TestHelper.InputPath, output, Path.DirectorySeparatorChar);

            Obfuscar.Obfuscator obfuscator = TestHelper.BuildAndObfuscate("AssemblyWithTypes", string.Empty, xml);
            var map = obfuscator.Mapping;

            string assmName = "AssemblyWithTypes.dll";

            AssemblyDefinition inAssmDef = AssemblyDefinition.ReadAssembly(
                Path.Combine(TestHelper.InputPath, assmName));

            TypeDefinition classAType = inAssmDef.MainModule.GetType("TestClasses.PublicClass");
            MethodDefinition classAmethod1 = FindByName(classAType, "PrivateMethod");
            MethodDefinition classAmethod2 = FindByName(classAType, "PublicMethod");
            MethodDefinition classAmethod3 = FindByName(classAType, "InternalProtectedMethod");

            ObfuscatedThing classAMethod1 = map.GetMethod(new MethodKey(classAmethod1));
            ObfuscatedThing classAMethod2 = map.GetMethod(new MethodKey(classAmethod2));
            ObfuscatedThing classAMethod3 = map.GetMethod(new MethodKey(classAmethod3));
            var classA = map.GetClass(new TypeKey(classAType));
            Assert.True(classA.Status == ObfuscationStatus.Skipped, "Public class shouldn't have been obfuscated");
            Assert.True(classAMethod1.Status == ObfuscationStatus.Renamed, "private method is not obfuscated.");
            var newName = classAMethod1.StatusText;
            Assert.True(classAMethod2.Status == ObfuscationStatus.Skipped, "public method is obfuscated.");
            Assert.True(classAMethod3.Status == ObfuscationStatus.Skipped, "internal protected method is obfuscated.");

            var protectedMethod = FindByName(classAType, "ProtectedMethod");
            var protectedAfter = map.GetMethod(new MethodKey(protectedMethod));
            Assert.True(protectedAfter.Status == ObfuscationStatus.Skipped, "protected method is obfuscated.");

            var outAssmDef = AssemblyDefinition.ReadAssembly(
                Path.Combine(output, assmName));

            var outClassAType = outAssmDef.MainModule.GetType("TestClasses.PublicClass");
            Assert.NotNull(outClassAType);

            string[] newNameParts = newName.Split(']');
            string correctMethodName = newNameParts.Length > 1 ? newNameParts[1] : newName;
            var outClassAmethod1 = FindByName(outClassAType, correctMethodName);
            var outClassAmethod2 = FindByName(outClassAType, "PublicMethod");
            var outClassAmethod3 = FindByName(outClassAType, "InternalProtectedMethod");

            Assert.NotEqual(classAmethod1.Name, outClassAmethod1.Name);
            Assert.Equal(string.Empty, outClassAmethod1.Parameters[0].Name); // obfuscation removed the parameter name
            Assert.Equal(classAmethod2.Name, outClassAmethod2.Name);
            Assert.Equal("yourName", outClassAmethod2.Parameters[0].Name); // skipped method keeps parameter names
            Assert.Equal(classAmethod3.Name, outClassAmethod3.Name);
            Assert.Equal("theirNames", outClassAmethod3.Parameters[0].Name);

            var outProtectedMethod = FindByName(outClassAType, "ProtectedMethod");
            Assert.Equal(protectedMethod.Name, outProtectedMethod.Name);
            Assert.Equal("itsName", protectedMethod.Parameters[0].Name);
        }

        [Fact]
        public void CheckSkipNamespace()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='KeepPublicApi' value='false' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Module file='$(InPath){2}AssemblyWithTypes.dll'>" +
                @"<SkipNamespace name='TestClasses1' />" +
                @"</Module>" +
                @"</Obfuscator>", TestHelper.InputPath, TestHelper.OutputPath, Path.DirectorySeparatorChar);

            Obfuscar.Obfuscator obfuscator = TestHelper.BuildAndObfuscate("AssemblyWithTypes", string.Empty, xml);
            var map = obfuscator.Mapping;

            string assmName = "AssemblyWithTypes.dll";

            AssemblyDefinition inAssmDef = AssemblyDefinition.ReadAssembly(
                Path.Combine(TestHelper.InputPath, assmName));

            TypeDefinition classAType = inAssmDef.MainModule.GetType("TestClasses1.PublicClass");
            var classA = map.GetClass(new TypeKey(classAType));
            Assert.True(classA.Status == ObfuscationStatus.Skipped, "Public class shouldn't have been obfuscated");
        }

        [Fact]
        public void CheckSkipEnum()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='KeepPublicApi' value='false' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Module file='$(InPath){2}AssemblyWithTypes.dll'>" +
                @"<SkipType name='TestClasses.TestEnum' />" +
                @"</Module>" +
                @"</Obfuscator>", TestHelper.InputPath, TestHelper.OutputPath, Path.DirectorySeparatorChar);

            Obfuscar.Obfuscator obfuscator = TestHelper.BuildAndObfuscate("AssemblyWithTypes", string.Empty, xml);
            var map = obfuscator.Mapping;

            string assmName = "AssemblyWithTypes.dll";

            AssemblyDefinition inAssmDef = AssemblyDefinition.ReadAssembly(
                Path.Combine(TestHelper.InputPath, assmName));

            var enumType = inAssmDef.MainModule.GetType("TestClasses.TestEnum");
            var enum1 = map.GetClass(new TypeKey(enumType));
            Assert.True(enum1.Status == ObfuscationStatus.Skipped, "Internal enum is obfuscated");
        }

        [Fact]
        public void CheckSkipAllEnums()
        {
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='KeepPublicApi' value='false' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Module file='$(InPath){2}AssemblyWithTypes.dll'>" +
                @"<SkipEnums value='true' />" +
                @"</Module>" +
                @"</Obfuscator>", TestHelper.InputPath, TestHelper.OutputPath, Path.DirectorySeparatorChar);

            Obfuscator obfuscator = TestHelper.BuildAndObfuscate("AssemblyWithTypes", string.Empty, xml);
            var map = obfuscator.Mapping;

            string assmName = "AssemblyWithTypes.dll";

            var inAssmDef = AssemblyDefinition.ReadAssembly(Path.Combine(TestHelper.InputPath, assmName));

            var enumType = inAssmDef.MainModule.GetType("TestClasses.TestEnum");
            var enum1 = map.GetClass(new TypeKey(enumType));
            Assert.True(enum1.Status == ObfuscationStatus.Skipped, "Internal enum is obfuscated");
        }

        [Fact]
        public void CheckMethodParamRenaming()
        {
            // Regression test for issue #217: with KeepPublicApi=true + HidePrivateApi=true,
            // public method parameters in a public class must be preserved (public API contract),
            // while private method parameters must be renamed (set to null by obfuscator).
            // Protected methods are also treated as public API (IsPublic() returns true for
            // IsFamily/IsFamilyOrAssembly), so their parameters are also preserved — this is the
            // root cause of the behavior the reporter observed.
            string inputPath = TestHelper.InputPath;
            string outputPath = TestHelper.OutputPath;
            string xml = string.Format(
                @"<?xml version='1.0'?>" +
                @"<Obfuscator>" +
                @"<Var name='InPath' value='{0}' />" +
                @"<Var name='OutPath' value='{1}' />" +
                @"<Var name='KeepPublicApi' value='true' />" +
                @"<Var name='HidePrivateApi' value='true' />" +
                @"<Module file='$(InPath){2}AssemblyWithMethodParams.dll' />" +
                @"</Obfuscator>", inputPath, outputPath, Path.DirectorySeparatorChar);

            TestHelper.BuildAndObfuscate("AssemblyWithMethodParams", string.Empty, xml);

            var outAssmDef = AssemblyDefinition.ReadAssembly(Path.Combine(outputPath, "AssemblyWithMethodParams.dll"));
            var type = outAssmDef.MainModule.GetType("TestClasses.PublicClassWithParams");
            Assert.NotNull(type);

            MethodDefinition publicMethod = null, protectedMethod = null, privateMethod = null;
            foreach (var m in type.Methods)
            {
                if (m.IsPublic && !m.IsSpecialName) publicMethod = m;
                else if (m.IsFamily && !m.IsSpecialName) protectedMethod = m;
                else if (m.IsPrivate && !m.IsSpecialName) privateMethod = m;
            }

            Assert.NotNull(publicMethod);
            Assert.NotNull(protectedMethod);
            Assert.NotNull(privateMethod);

            // Public method parameters must retain their original names (KeepPublicApi)
            Assert.Equal("firstName", publicMethod.Parameters[0].Name);
            Assert.Equal("age", publicMethod.Parameters[1].Name);

            // Protected method parameters are also treated as public API (IsPublic() covers IsFamily),
            // so they are also preserved when KeepPublicApi=true
            Assert.Equal("familyData", protectedMethod.Parameters[0].Name);
            Assert.Equal("familyId", protectedMethod.Parameters[1].Name);

            // Private method parameters must be renamed — obfuscator clears their names
            Assert.True(string.IsNullOrEmpty(privateMethod.Parameters[0].Name),
                $"Expected private param name to be cleared, got: '{privateMethod.Parameters[0].Name}'");
            Assert.True(string.IsNullOrEmpty(privateMethod.Parameters[1].Name),
                $"Expected private param name to be cleared, got: '{privateMethod.Parameters[1].Name}'");
        }
    }
}
