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
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = true)]
    internal abstract class ParameterAttribute : Attribute { }

    internal sealed class InAttribute : ParameterAttribute { }
    internal sealed class OutAttribute : ParameterAttribute { }
    internal sealed class InOutAttribute : ParameterAttribute { }

    internal sealed class SetDefaultOnFailureAttribute : ParameterAttribute { }
    internal sealed class OptionalAttribute : ParameterAttribute
    {
        public OptionalAttribute(string condition = null) { }
    }


    internal sealed class CanBeNullAttribute : ParameterAttribute { }
    internal sealed class NotNullAttribute : ParameterAttribute { }
    internal sealed class NotNullOrEmptyAttribute : ParameterAttribute { }


    internal sealed class CheckedAttribute : ParameterAttribute
    {
        public CheckedAttribute() { }
        public CheckedAttribute(string check) { }
    }

    internal sealed class NotCheckedAttribute : ParameterAttribute
    {
        public NotCheckedAttribute() { }
        public NotCheckedAttribute(string check) { }
    }
}
