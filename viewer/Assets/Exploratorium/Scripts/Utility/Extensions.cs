using System;
using System.Collections.Generic;
using System.Linq;

namespace Exploratorium.Utility
{
    public static class Extensions
    {
        private static readonly Random Rng = new Random();

        /// <summary>
        /// Fisher-Yates shuffle
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> source) => source.OrderBy(it => Rng.Next());

        /// <summary>
        /// Get all descendant nodes in a tree. 
        /// </summary>
        public static IEnumerable<T> Descendants<T>(
            this T root, Func<T, IEnumerable<T>> children, TraversalMode mode = TraversalMode.DepthFirst
        )
        {
            switch (mode)
            {
                case TraversalMode.DepthFirst:
                {
                    Stack<T> nodes = new Stack<T>(new[] { root });
                    while (nodes.Any())
                    {
                        T node = nodes.Pop();
                        yield return node;
                        foreach (var n in children(node))
                            nodes.Push(n);
                    }

                    break;
                }
                case TraversalMode.BreadthFirst:
                {
                    Queue<T> nodes = new Queue<T>(new[] { root });
                    while (nodes.Any())
                    {
                        T node = nodes.Dequeue();
                        yield return node;
                        foreach (var n in children(node))
                            nodes.Enqueue(n);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Get all nodes in a potentially cyclic graph via depth first traversal.
        /// </summary>
        public static IEnumerable<T> Closure<T>(
            this T root, Func<T, IEnumerable<T>> children, TraversalMode mode = TraversalMode.DepthFirst
        )
        {
            var seen = new HashSet<T>();
            switch (mode)
            {
                case TraversalMode.DepthFirst:
                    var stack = new Stack<T>();
                    stack.Push(root);

                    while (stack.Count != 0)
                    {
                        T item = stack.Pop();
                        if (seen.Contains(item))
                            continue;
                        seen.Add(item);
                        yield return item;
                        foreach (var child in children(item))
                            stack.Push(child);
                    }

                    break;
                case TraversalMode.BreadthFirst:
                    var queue = new Queue<T>();
                    queue.Enqueue(root);

                    while (queue.Count != 0)
                    {
                        T item = queue.Dequeue();
                        if (seen.Contains(item))
                            continue;
                        seen.Add(item);
                        yield return item;
                        foreach (var child in children(item))
                            queue.Enqueue(child);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum TraversalMode
        {
            DepthFirst,
            BreadthFirst
        }
    }
}