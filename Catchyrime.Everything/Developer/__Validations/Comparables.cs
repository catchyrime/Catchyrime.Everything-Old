using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable BuiltInTypeReferenceStyle

namespace Catchyrime.Everything.Developer
{
    public static partial class Validations
    {
        /* ArgumentInRange
        public static ValidationInfo<T> ArgumentInRange<T>(
            [In, NotNull, Checked] this ValidationInfo<T> info,
            [In, CanBeNull] T lower,
            [In, CanBeNull] T upper,
            [In, Optional, CanBeNull] string throwMsg = null
            ) where T: IComparable<T>
        {
            if (info.Condition) {
                if (!(info.Value.CompareTo(lower) >= 0) &&
                    (info.Value.CompareTo(upper) <= 0)) {
                    throw new ArgumentOutOfRangeException(
                        info.Name,
                        info.Value,
                        throwMsg ?? $"Parameter: \"{info.Name}\" should be in range from {lower} to {upper}.");
                }
            }
            return info;
        }
        */
    }


    //!==== Int32 ====
    public static partial class Validations
    {
        public static ValidationInfo<Int32> ArgumentInRange(
            [In, NotNull, Checked] this ValidationInfo<Int32> info,
            [In, CanBeNull] Int32 lower,
            [In, CanBeNull] Int32 upper,
            [In, Optional, CanBeNull] string throwMsg = null
            )
        {
            if (info.Condition) {
                if (!(info.Value >= lower &&
                      info.Value <= upper)) {
                    throw new ArgumentOutOfRangeException(
                        info.Name,
                        info.Value,
                        throwMsg ?? $"Parameter: \"{info.Name}\" should be in range from {lower} to {upper}.");
                }
            }
            return info;
        }

        public static ValidationInfo<Int32> IndexInRange(
            [In, NotNull, Checked] this ValidationInfo<Int32> info,
            [In, CanBeNull] Int32 lower,
            [In, CanBeNull] Int32 upper,
            [In, Optional, CanBeNull] string throwMsg = null
            )
        {
            if (info.Condition) {
                if (!(info.Value >= lower &&
                      info.Value <= upper)) {
                    throw new IndexOutOfRangeException(
                        throwMsg ?? $"Index: \"{info.Name}\" = {info.Value} should be in range from {lower} to {upper}.");
                }
            }
            return info;
        }

        public static ValidationInfo<Int32> GreaterOrEqual(
            [In, NotNull, Checked] this ValidationInfo<Int32> info,
            [In, CanBeNull] Int32 lower,
            [In, Optional, CanBeNull] string throwMsg = null
            )
        {
            if (info.Condition) {
                if (!(info.Value >= lower)) {
                    throw new ArgumentOutOfRangeException(
                        info.Name,
                        info.Value,
                        throwMsg ?? $"Parameter: \"{info.Name}\" = {info.Value} should be greater than or equal to {lower}.");
                }
            }
            return info;
        }

    }
}
