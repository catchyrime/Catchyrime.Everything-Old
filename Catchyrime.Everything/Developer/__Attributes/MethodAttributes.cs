using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static Catchyrime.Everything.Constant;

namespace Catchyrime.Everything.Developer
{
    [Conditional(DUMMY_NO_CONDITION)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Delegate, AllowMultiple = true)]
    internal abstract class MethodAttribute : Attribute { }


    internal sealed class NoThrowExceptionAttribute : MethodAttribute { }
    internal sealed class ReturnDefaultOnFailureAttribute : MethodAttribute { }

    internal sealed class ThrowExceptionAttribute : MethodAttribute {
        public ThrowExceptionAttribute(string exceptionName, string why) { }
    }

    internal sealed class ThrowExceptionInnerAttribute : MethodAttribute {
        public ThrowExceptionInnerAttribute(string methodName) { }
    }
}
