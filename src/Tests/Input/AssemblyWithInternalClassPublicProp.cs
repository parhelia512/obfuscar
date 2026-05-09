namespace TestClasses
{
    // Reproduces issue #559: public property on an internal class.
    // The class is internal, so none of its members are accessible from other assemblies.
    // With KeepPublicApi=true + HidePrivateApi=true, the property should be renamed because
    // it is not part of the externally-visible API surface.
    internal class InternalClassWithPublicProp
    {
        public string PublicProp { get; set; }
        public int AnotherProp { get; set; }
    }

    public class PublicClassWithPublicProp
    {
        public string PublicProp { get; set; }
    }
}
