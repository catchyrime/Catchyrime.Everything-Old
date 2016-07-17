using System;

namespace Catchyrime.Everything.Developer
{
    public static partial class Validations
    {
        public static ValidationInfo<T> ArgumentNotNull<T>(
            [In, Checked, NotNull] this ValidationInfo<T> info, 
            [In, Optional] string throwMsg = null
            )
        {
            if (info.Condition) {
                if (ReferenceEquals(info.Value, null)) {
                    throw new ArgumentNullException(info.Name, throwMsg ?? $"Parameter: \"{info.Name}\" is null.");
                }
            }
            return info;
        }

        public static ValidationInfo<T> IsNotNull<T>(
            [In, Checked, NotNull] this ValidationInfo<T> info,
            [In, Optional] string throwMsg = null
            )
        {
            if (info.Condition) {
                if (ReferenceEquals(info.Value, null)) {
                    throw new NullReferenceException(throwMsg ?? $"Reference: \"{info.Name}\" is null.");
                }
            }
            return info;
        }

    }
}
