namespace TestClasses
{
    // Reproduces issue #217: method parameters in a public class.
    // With KeepPublicApi=true + HidePrivateApi=true:
    //   - public method parameters should be preserved (public API contract)
    //   - private method parameters should be renamed
    //   - protected method parameters: IsPublic() returns true for protected (IsFamily),
    //     so they are treated as public API and parameters are also preserved.
    //     This is a known limitation — protected members are kept when KeepPublicApi=true
    //     because subclasses in other assemblies may rely on named parameters.
    public class PublicClassWithParams
    {
        public string PublicMethod(string firstName, int age) => firstName;

        protected string ProtectedMethod(string familyData, int familyId) => familyData;

        private string PrivateMethod(string secretData, int internalId) => secretData;
    }
}
