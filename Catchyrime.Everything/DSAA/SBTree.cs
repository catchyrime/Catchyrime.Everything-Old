using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Catchyrime.Everything.Developer;

namespace Catchyrime.Everything.DSAA 
{
    internal class SBTNode<T> 
    {
        public static readonly SBTNode<T> Null = new SBTNode<T>();

        public SBTNode<T> Left, Right;
        public int Size;
        public T Value;

        public bool IsNullNode {
            get {
                if (this.Size == 0) {
                    Debug.Assert(this.Left.Size == 0, "this.Left.Size == 0");
                    Debug.Assert(this.Right.Size == 0, "this.Right.Size == 0");
                    Debug.Assert(object.Equals(this.Value, default(T)), "object.Equals(this.Value, default(T))");
                    return true;
                }
                return false;
            }
        }

        public bool IsLeafNode {
            get {
                if (this.Size == 1) {
                    Debug.Assert(this.Left.IsNullNode);
                    Debug.Assert(this.Right.IsNullNode);
                    return true;
                }
                return false;
            }
        }


        private SBTNode()
        {
            this.Left = this.Right = this;
            this.Size = 0;
            this.Value = default(T);
        }


        public SBTNode(
            [In, CanBeNull] T value
            )
        {
            this.Left = this.Right = SBTNode<T>.Null;
            this.Size = 1;
            this.Value = value;
        }

        public SBTNode(
            [In, CanBeNull] T value,
            [In, Checked("size >= 1")] int size,
            [In, Checked, NotNull] SBTNode<T> left,
            [In, Checked, NotNull] SBTNode<T> right
            )
        {
            this.Left = left;
            this.Right = right;
            this.Size = size;
            this.Value = value;
        }


        public bool DeepEquals(
            [In, Checked, NotNull] SBTNode<T> other,
            [In, Checked, NotNull] IEqualityComparer<T> comparer
            )
        {
            Debug.Assert(other != null, "other != null");
            Debug.Assert(comparer != null, "comparer != null");

            if (this.IsNullNode) {
                return other.IsNullNode;
            }
            if (this.Size != other.Size) {
                return false;
            }
            if (!comparer.Equals(this.Value, other.Value)) {
                return false;
            }
            return this.Left.DeepEquals(other.Left, comparer) &&
                   this.Right.DeepEquals(other.Right, comparer);
        }

    }



    internal static class SBTHelper {
        private const int SBTREE_MAX_DEPTH = 36; // It is an upper bound


        public static SBTNode<T> FromArray<T>(
            [In, Checked, NotNull] T[] array,
            [In, Checked("start >= 0 && start < array.Length")] int start,
            [In, Checked("length >= 0 && start + length <= array.Length")] int length
            )
        {
            Debug.Assert(array != null, "array != null");
            Debug.Assert(start >= 0, "start >= 0");
            Debug.Assert(length >= 0 && start + length <= array.Length, "length >= 0 && start + length <= array.Length");

            switch (length) {
            case 0:
                return SBTNode<T>.Null;
            case 1:
                return new SBTNode<T>(array[start]);
            default: // length >= 2
                int mid = start + (length >> 1);
                return new SBTNode<T>(array[mid],
                                      length,
                                      FromArray(array, start, length >> 1),
                                      FromArray(array, mid + 1, (length - 1) >> 1));
            }
        }

        internal static void CopyToArray<T>(
            [In, Checked, NotNull] SBTNode<T> root,
            [Out, Checked("startIndex + root.Size <= array.Length"), NotNull] T[] array,
            [InOut, Checked("startIndex >= 0")] ref int startIndex
            )
        {
            Debug.Assert(root != null, "root != null");
            Debug.Assert(startIndex >= 0, "startIndex >= 0");
            Debug.Assert(startIndex + root.Size <= array.Length, "startIndex + root.Size <= array.Length");

            while (true) {
                if (root.IsNullNode)
                    return;

                CopyToArray(root.Left, array, ref startIndex);
                array[startIndex++] = root.Value;
                root = root.Right;
            }
        }


        public static IEnumerator<T> Enumerate<T>(
            [In, Checked, NotNull] SBTNode<T> root
            )
        {
            Debug.Assert(root != null, "root != null");

            var stack = new SBTNode<T>[SBTREE_MAX_DEPTH]; // Actually, 32 is enough
            int stackSize = 0;
            while (!root.IsNullNode || stackSize > 0) {
                while (!root.IsNullNode) {
                    stack[stackSize++] = root;
                    root = root.Left;
                }
                root = stack[--stackSize];
                yield return root.Value;
                root = root.Right;
            }
        }

        public static IEnumerator<T> ReverseEnumerate<T>(
            [In, Checked, NotNull] SBTNode<T> root
            )
        {
            Debug.Assert(root != null, "root != null");

            SBTNode<T>[] stack = new SBTNode<T>[SBTREE_MAX_DEPTH]; // Actually, 32 is enough
            int stackSize = 0;
            while (!root.IsNullNode || stackSize > 0) {
                while (!root.IsNullNode) {
                    stack[stackSize++] = root;
                    root = root.Right;
                }
                root = stack[--stackSize];
                yield return root.Value;
                root = root.Left;
            }
        }


        private static void LeftRotate<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root
            )
        {
            var tmp = root.Right;
            root.Right = tmp.Left;
            tmp.Left = root;
            tmp.Size = root.Size;
            root.Size = root.Left.Size + root.Right.Size + 1;
            root = tmp;
        }

        private static void RightRotate<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root
            )
        {
            var tmp = root.Left;
            root.Left = tmp.Right;
            tmp.Right = root;
            tmp.Size = root.Size;
            root.Size = root.Left.Size + root.Right.Size + 1;
            root = tmp;
        }

        private static void MaintainRightWeighter<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root
            ) // flag = true
        {
            if (root.Left.Size < root.Right.Left.Size) {
                RightRotate(ref root.Right);
                LeftRotate(ref root);
            }
            else if (root.Left.Size < root.Right.Right.Size) {
                LeftRotate(ref root);
            }
            else {
                return;
            }

            MaintainLeftWeighter(ref root.Left);
            MaintainRightWeighter(ref root.Right);
            MaintainLeftWeighter(ref root);
            MaintainRightWeighter(ref root);
        }

        private static void MaintainLeftWeighter<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> node
            ) // bool flag = false
        {
            if (node.Right.Size < node.Left.Right.Size) {
                LeftRotate(ref node.Left);
                RightRotate(ref node);
            }
            else if (node.Right.Size < node.Left.Left.Size) {
                RightRotate(ref node);
            }
            else {
                return;
            }

            MaintainLeftWeighter(ref node.Left);
            MaintainRightWeighter(ref node.Right);
            MaintainLeftWeighter(ref node);
            MaintainRightWeighter(ref node);
        }



        internal static SBTNode<T> ElementAt<T>(
            [In, Checked, NotNull] SBTNode<T> root,
            [In, Checked("index >= 0 && index < root.Size")] int index
            )
        {
            Debug.Assert(root != null, "root != null");
            Debug.Assert(index >= 0 && index < root.Size, "index >= 0 && index < root.Size");

            while (true) {
                int leftSize = root.Left.Size;
                if (index == leftSize) {
                    return root;
                }

                if (index < leftSize) {
                    root = root.Left;
                }
                else { // index > leftSize
                    root = root.Right;
                    index -= (leftSize + 1);
                }
            }
        }

        internal static void InsertAt<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root,
            [In, Checked("index >= 0 && index <= root.Size")] int index,
            [In, CanBeNull] T value
            )
        {
            Debug.Assert(root != null, "root != null");
            Debug.Assert(index >= 0 && index <= root.Size, "index >= 0 && index <= root.Size");

            if (root.IsNullNode) {
                Debug.Assert(index == 0);
                root = new SBTNode<T>(value);
            }
            else {
                ++root.Size;
                int leftSize = root.Left.Size;
                if (index <= leftSize) {
                    InsertAt(ref root.Left, index, value);
                    MaintainLeftWeighter(ref root);
                }
                else { // index > leftSize
                    InsertAt(ref root.Right, index - (leftSize + 1), value);
                    MaintainRightWeighter(ref root);
                }
            }
        }

        internal static void InsertAtEnd<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root,
            [In, CanBeNull] T value
            )
        {
            Debug.Assert(root != null, "root != null");

            if (root.IsNullNode) {
                root = new SBTNode<T>(value);
            }
            else {
                InsertAtEnd(ref root.Right, value);
                ++root.Size;
                MaintainRightWeighter(ref root);
            }
        }


        public static void RemoveAt<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root,
            [In, Checked("index >= 0 && index < root.Size")] int index,
            [Out] out T value
            )
        {
            Debug.Assert(root != null, "root != null");
            Debug.Assert(index >= 0 && index < root.Size, "index >= 0 && index < root.Size");

            int leftSize = root.Left.Size;
            if (index < leftSize) {
                RemoveAt(ref root.Left, index, out value);
                --root.Size;
                MaintainRightWeighter(ref root);
            }
            else if (index > leftSize) {
                RemoveAt(ref root.Right, index - (leftSize + 1), out value);
                --root.Size;
                MaintainLeftWeighter(ref root);
            }
            else { // (index == leftSize)
                // root.Value is to be removed
                value = root.Value;

                if (!root.Left.IsNullNode) {
                    RemoveMax(ref root.Left, out root.Value);
                    --root.Size;
                    MaintainRightWeighter(ref root);
                }
                else if (!root.Right.IsNullNode) {
                    RemoveMin(ref root.Right, out root.Value);
                    --root.Size;
                    MaintainLeftWeighter(ref root);
                }
                else {
                    Debug.Assert(root.IsLeafNode);
                    root = SBTNode<T>.Null;
                }
            }
        }


        private static void RemoveMax<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root,
            [Out] out T value
            )
        {
            if (root.IsLeafNode) {
                value = root.Value;
                root = SBTNode<T>.Null;
                return;
            }

            if (root.Right.IsNullNode) { // This is the max
                // Now: (!root.Left.IsNullNode)
                Debug.Assert(root.Left.IsLeafNode, "root.Left.IsLeafNode");
                value = root.Value;
                root = root.Left;
            }
            else {
                RemoveMax(ref root.Right, out value);
                --root.Size;
                MaintainLeftWeighter(ref root);
            }
        }

        private static void RemoveMin<T>(
            [InOut, Checked, NotNull] ref SBTNode<T> root,
            [Out] out T value
            )
        {
            if (root.IsLeafNode) {
                value = root.Value;
                root = SBTNode<T>.Null;
                return;
            }

            if (root.Left.IsNullNode) { // This is the min
                // Now: (!root.Left.IsNullNode)
                Debug.Assert(root.Right.IsLeafNode, "root.Right.IsLeafNode");
                value = root.Value;
                root = root.Right;
            }
            else {
                RemoveMin(ref root.Left, out value);
                --root.Size;
                MaintainRightWeighter(ref root);
            }
        }



        [Conditional("DEBUG")]
        internal static void Display<T>(
            [In, Checked, NotNull] SBTNode<T> root,
            [In, Checked("tab >= 0")] int tab = 0
            )
        {
            Debug.Assert(root != null, "root != null");
            Debug.Assert(tab >= 0, "tab >= 0");

            if (root.IsNullNode) {
                Console.WriteLine(new string('-', tab * 4) + " <Dummy>");
                return;
            }
            Display(root.Left, tab + 1);
            Console.WriteLine(new string('-', tab * 4) + " " + root.Value);
            Display(root.Right, tab + 1);
        }


        internal static bool IsBalanced<T>(
            [In, Checked, NotNull] SBTNode<T> root
            )
        {
            if (root.IsNullNode) {
                return true;
            }

            if (!(root.Left.Size >= root.Right.Left.Size)) return false;
            if (!(root.Left.Size >= root.Right.Right.Size)) return false;
            if (!(root.Right.Size >= root.Left.Left.Size)) return false;
            if (!(root.Right.Size >= root.Left.Right.Size)) return false;

            return IsBalanced(root.Left) && IsBalanced(root.Right);
        }

    }

}
