using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Catchyrime.Everything.Developer;
using static Catchyrime.Everything.Constant;

namespace Catchyrime.Everything.DSAA
{
    public class DSArray<T> : IList<T>, IEquatable<DSArray<T>>
    {
        private SBTNode<T> m_Root = SBTNode<T>.Null;

        public int Count => this.m_Root.Size;
        public bool IsReadOnly => false;
        public bool IsEmpty => (this.m_Root.IsNullNode);
        public IEqualityComparer<T> Comparer { get; }


        public DSArray(
            [In, Optional, CanBeNull] IEqualityComparer<T> comparer = null
            )
        {
            this.Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public DSArray(
            [In, NotChecked, NotNull] IEnumerable<T> collection,
            [In, Optional, CanBeNull] IEqualityComparer<T> comparer = null
            ) : this(comparer)
        {
            Debug.Assert(collection != null); // Checked later in "collection.ToArray()"

            T[] array = collection.ToArray();
            this.m_Root = SBTHelper.FromArray(array, 0, array.Length);
        }

        public DSArray(
            [In, NotChecked, NotNull] T[] array,
            [In, Optional, CanBeNull] IEqualityComparer<T> comparer = null
            ) : this(comparer)
        {
            Validations.Requires(array, nameof(array)).ArgumentNotNull();

            this.m_Root = SBTHelper.FromArray(array, 0, array.Length);
        }


        [Pure]
        public T[] ToArray()
        {
            // This also works when (this.Count == 0);
            T[] array = new T[this.Count];
            int startIndex = 0;
            SBTHelper.CopyToArray(this.m_Root, array, ref startIndex);
            return array;
        }

        public static explicit operator DSArray<T>(
            [In, NotChecked, NotNull] T[] array
            )
        {
            return new DSArray<T>(array);
        }

        public static explicit operator T[](
            [In, NotChecked, NotNull] DSArray<T> darray
            )
        {
            return darray.ToArray();
        }


        public IEnumerator<T> GetEnumerator() => SBTHelper.Enumerate(this.m_Root);
        public IEnumerator<T> GetReverseEnumerator() => SBTHelper.ReverseEnumerate(this.m_Root);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        [Obsolete(PERFORMACE_SLOW_O_N)] // Give out a warning for its slow performance
        [Pure]
        public int IndexOf(
            [In, CanBeNull] T item
            )
        {
            return this.IndexOf(item, null);
        }

        [Obsolete(PERFORMACE_SLOW_O_N)] // Give out a warning for its slow performance
        [Pure]
        public int IndexOf(
            [In, CanBeNull] T item,
            [In, Optional, CanBeNull] IEqualityComparer<T> comparer
            )
        {
            if (comparer == null) {
                comparer = this.Comparer;
            }

            int index = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (T t in (IEnumerable<T>)this) {
                if (comparer.Equals(item, t)) {
                    break;
                }
                ++index;
            }

            if (index == this.Count) {
                index = ILIST_INVALID_INDEX;
            }
            return index;
        }

        [Obsolete(PERFORMACE_SLOW_O_N)] // Give out a warning for its slow performance
        [Pure]
        public bool Contains(T item)
        {
            return (IndexOf(item) != ILIST_INVALID_INDEX);
        }


        [Obsolete(PERFORMACE_SLOW_O_N)] // Give out a warning for its slow performance
        public bool Remove(
            [In, CanBeNull] T item
            )
        {
            return Remove(item, null);
        }

        [Obsolete(PERFORMACE_SLOW_O_N)] // Give out a warning for its slow performance
        public bool Remove(
            [In, CanBeNull] T item,
            [In, Optional, CanBeNull] IEqualityComparer<T> comparer
            )
        {
            int index = IndexOf(item, comparer);
            if (index == ILIST_INVALID_INDEX) {
                return false;
            }
            else {
                Debug.Assert(index >= 0 && index < this.Count, "index >= 0 && index < this.Count");

                T dummy;
                SBTHelper.RemoveAt(ref this.m_Root, index, out dummy);
                return true;
            }
        }

        public void RemoveAt(
            [In, NotChecked("index >= 0 && index < this.Count")] int index
            )
        {
            Validations.Requires(index, nameof(index)).ArgumentInRange(0, this.Count - 1);
            T dummy;
            SBTHelper.RemoveAt(ref this.m_Root, index, out dummy);

            // TODO: Comment this to disable O(n) balance check
            // Debug.Assert(SBTHelper.IsBalanced(this.m_Root));
        }

        public void RemoveAt(
            [In, NotChecked("index >= 0 && index < this.Count")] int index,
            [Out] out T item
            )
        {
            Validations.Requires(index, nameof(index)).ArgumentInRange(0, this.Count - 1);
            SBTHelper.RemoveAt(ref this.m_Root, index, out item);

            // TODO: Comment this to disable O(n) balance check
            // Debug.Assert(SBTHelper.IsBalanced(this.m_Root));
        }


        public T this[
            [In, NotChecked("index >= 0 && index <=(set)|<(get) this.Count")] int index
            ] {
            [Pure] get {
                Validations.Requires(index, nameof(index)).IndexInRange(0, this.Count - 1);

                SBTNode<T> node = SBTHelper.ElementAt(this.m_Root, index);
                return node.Value;
            }
            set {
                Validations.Requires(index, nameof(index)).IndexInRange(0, this.Count);

                if (index == this.Count) {
                    Add(value);
                }
                else {
                    SBTNode<T> node = SBTHelper.ElementAt(this.m_Root, index);
                    node.Value = value;
                }
            }
        }


        public void Add(
            [In, CanBeNull] T item
            )
        {
            SBTHelper.InsertAtEnd(ref this.m_Root, item);
        }


        public void Insert(
            [In, NotChecked("index >= 0 && index <= this.Count")] int index,
            [In, CanBeNull] T item
            )
        {
            Validations.Requires(index, nameof(index)).ArgumentInRange(0, this.Count);

            if (index == this.Count) {
                SBTHelper.InsertAtEnd(ref this.m_Root, item);
            }
            else {
                SBTHelper.InsertAt(ref this.m_Root, index, item);
            }
        }


        public void Clear()
        {
            this.m_Root = SBTNode<T>.Null;
        }


        public void CopyTo(
            [Out, NotChecked("arrayIndex + root.Size <= array.Length"), NotNull] T[] array,
            [InOut, NotChecked("arrayIndex >= 0")] int arrayIndex
            )
        {
            Validations.Requires(array, nameof(array)).ArgumentNotNull();
            Validations.Requires(arrayIndex, nameof(arrayIndex)).ArgumentInRange(0, array.Length - 1);
            Validations.Requires(array.Length, "array.Length").GreaterOrEqual(arrayIndex + this.Count);

            SBTHelper.CopyToArray(this.m_Root, array, ref arrayIndex);
        }


        public bool Equals(
            [In, NotChecked, CanBeNull] DSArray<T> other,
            [In, Optional, CanBeNull] IEqualityComparer<T> comparer
            )
        {
            if (ReferenceEquals(other, null)) {
                return false;
            }
            if (this.Count != other.Count) {
                return false;
            }
            if (comparer == null) {
                comparer = this.Comparer;
            }

            IEnumerator<T> en1 = this.GetEnumerator();
            IEnumerator<T> en2 = other.GetEnumerator();
            while (en1.MoveNext()) {
                en2.MoveNext();
                if (!comparer.Equals(en1.Current, en2.Current)) {
                    return false;
                }
            }
            return true;
        }

        public bool Equals(
            [In, NotChecked, CanBeNull] DSArray<T> other
            )
        {
            return this.Equals(other, null);
        }

        public override bool Equals(
            [In, NotChecked, CanBeNull] object obj
            )
        {
            return this.Equals(obj as DSArray<T>, null);
        }

        public override int GetHashCode()
        {
            if (this.Count == 0) {
                return 0;
            }
            else {
                unchecked {
                    // TODO: Develop a better hash algorithm
                    int hash1 = SBTHelper.ElementAt(this.m_Root, 0).Value?.GetHashCode() ?? 0;
                    int hash2 = SBTHelper.ElementAt(this.m_Root, this.Count - 1).Value?.GetHashCode() ?? 0;
                    return hash1 * 65541 + hash2 * 397 + this.Count;
                }
            }
        }
    }
}
