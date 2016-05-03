namespace Web.Domain.Tests
{
    using System;
    using NUnitLite;
    using System.Reflection;
    using NUnit.Common;

    public class Program
    {
        public static int Main(string[] args)
        {
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly)
                .Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
        }
    }
}
