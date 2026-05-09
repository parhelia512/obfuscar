namespace TestClasses
{
    // Library assembly exposing an enum used as a default parameter value cross-assembly.
    // Reproduces issue #235: obfuscating both assemblies crashes when the enum is renamed
    // and the referencing assembly still holds the old type name in its constant metadata.
    public enum LibraryMode
    {
        None = 0,
        Read = 1,
        Write = 2,
    }
}
