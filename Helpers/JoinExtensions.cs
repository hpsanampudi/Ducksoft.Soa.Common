using System.Linq;
namespace System.Collections.Generic.Extensions
{
    /// <summary>
    ///  Extension methods to join two lists using lamda expression.
    ///  http://www.codeproject.com/Articles/488643/LinQ-Extended-Joins
    /// </summary>
    public static class JoinExtensions
    {
        /// <summary>
        /// Lefts the join.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TInner">The type of the inner.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="pk">The pk.</param>
        /// <param name="fk">The fk.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> LeftJoin<TSource, TInner, TKey, TResult>(
            this IEnumerable<TSource> source, IEnumerable<TInner> inner,
            Func<TSource, TKey> pk, Func<TInner, TKey> fk, Func<TSource, TInner, TResult> result)
        {
            IEnumerable<TResult> myResult = Enumerable.Empty<TResult>();

            myResult = from s in source
                       join i in inner
                       on pk(s) equals fk(i) into joinData
                       from left in joinData.DefaultIfEmpty()
                       select result(s, left);

            return myResult;
        }


        /// <summary>
        /// Rights the join.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TInner">The type of the inner.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="pk">The pk.</param>
        /// <param name="fk">The fk.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> RightJoin<TSource, TInner, TKey, TResult>(
            this IEnumerable<TSource> source, IEnumerable<TInner> inner,
            Func<TSource, TKey> pk, Func<TInner, TKey> fk, Func<TSource, TInner, TResult> result)
        {
            IEnumerable<TResult> myResult = Enumerable.Empty<TResult>();

            myResult = from i in inner
                       join s in source
                       on fk(i) equals pk(s) into joinData
                       from right in joinData.DefaultIfEmpty()
                       select result(right, i);

            return myResult;
        }


        /// <summary>
        /// Fulls the outer join.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TInner">The type of the inner.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="pk">The pk.</param>
        /// <param name="fk">The fk.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> FullOuterJoin<TSource, TInner, TKey, TResult>(
            this IEnumerable<TSource> source, IEnumerable<TInner> inner,
            Func<TSource, TKey> pk, Func<TInner, TKey> fk, Func<TSource, TInner, TResult> result)
        {

            var left = source.LeftJoin(inner, pk, fk, result);
            var right = source.RightJoin(inner, pk, fk, result);

            return left.Union(right);
        }


        /// <summary>
        /// Lefts the excluding join.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TInner">The type of the inner.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="pk">The pk.</param>
        /// <param name="fk">The fk.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> LeftExcludingJoin<TSource, TInner, TKey, TResult>(
            this IEnumerable<TSource> source, IEnumerable<TInner> inner,
            Func<TSource, TKey> pk, Func<TInner, TKey> fk, Func<TSource, TInner, TResult> result)
        {
            IEnumerable<TResult> myResult = Enumerable.Empty<TResult>();

            myResult = from s in source
                       join i in inner
                       on pk(s) equals fk(i) into joinData
                       from left in joinData.DefaultIfEmpty()
                       where left == null
                       select result(s, left);

            return myResult;
        }

        /// <summary>
        /// Rights the excluding join.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TInner">The type of the inner.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="pk">The pk.</param>
        /// <param name="fk">The fk.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> RightExcludingJoin<TSource, TInner, TKey, TResult>(
            this IEnumerable<TSource> source, IEnumerable<TInner> inner,
            Func<TSource, TKey> pk, Func<TInner, TKey> fk, Func<TSource, TInner, TResult> result)
        {
            IEnumerable<TResult> myResult = Enumerable.Empty<TResult>();

            myResult = from i in inner
                       join s in source
                       on fk(i) equals pk(s) into joinData
                       from right in joinData.DefaultIfEmpty()
                       where right == null
                       select result(right, i);

            return myResult;
        }


        /// <summary>
        /// Fulls the outer excluding join.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TInner">The type of the inner.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="pk">The pk.</param>
        /// <param name="fk">The fk.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> FullOuterExcludingJoin<TSource, TInner, TKey, TResult>(
            this IEnumerable<TSource> source, IEnumerable<TInner> inner,
            Func<TSource, TKey> pk, Func<TInner, TKey> fk, Func<TSource, TInner, TResult> result)
        {
            var left = source.LeftExcludingJoin(inner, pk, fk, result);
            var right = source.RightExcludingJoin(inner, pk, fk, result);

            return left.Union(right);
        }

        /// <summary>
        /// Appends the item to given source.
        /// Note: Caller needs to re-assigned the return value to source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(new[] { item });
        }
    }
}
