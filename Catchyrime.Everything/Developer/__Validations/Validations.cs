using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Catchyrime.Everything.Developer
{
    public class ValidationInfo<T>
    {
        public readonly T Value;
        public readonly string Name;
        public bool Condition;

        public ValidationInfo(
            [In, CanBeNull] T value, 
            [In, NotChecked, CanBeNull] string name
            )
        {
            this.Name = name;
            this.Value = value;
            this.Condition = true;
        }
    }

    public static partial class Validations
    {
        public static ValidationInfo<T> Requires<T>(
            [In, CanBeNull] T value, 
            [In, NotChecked, CanBeNull] string name
            ) 
            => new ValidationInfo<T>(value, name);

        public static ValidationInfo<T> When<T>(
            [In, Checked, NotNull] this ValidationInfo<T> info, 
            [In] bool condition
            )
        {
            info.Condition &= condition;
            return info;
        }

    }
}
