using System.Collections.Generic;

namespace TestClasses
{
    // Reproduces issue #485: two methods with the same parameter signature in a generic abstract class
    // both get renamed to the same obfuscated name, causing CheckRequest to call itself recursively.
    internal abstract class AbstractReadService<H, REQ, RESP>
    {
        internal void CheckRequest(List<REQ> requests, out string message)
        {
            message = null;
            CheckRequestInner(requests, out message);
        }

        protected abstract void CheckRequestInner(List<REQ> requests, out string message);
    }

    internal class ConcreteReadService : AbstractReadService<object, string, int>
    {
        protected override void CheckRequestInner(List<string> requests, out string message)
        {
            message = "ok";
        }
    }

    public static class ReadServiceFactory
    {
        public static object Create() => new ConcreteReadService();
    }
}
