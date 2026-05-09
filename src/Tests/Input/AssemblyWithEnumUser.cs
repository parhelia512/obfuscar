namespace TestClasses
{
    // References the enum from AssemblyWithEnumLib as a default parameter value.
    // This is the pattern that triggered the ResolutionException in issue #235.
    public class EnumConsumer
    {
        public void Method(LibraryMode mode = LibraryMode.Read)
        {
        }
    }
}
