using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// Static class which is used to extend excption related information.
    /// </summary>
    public static partial class ExceptionExtensions
    {
        /// <summary>
        /// Returns a list of all the exception messages from the top-level
        /// exception down through all the inner exceptions. Useful for making
        /// logs and error pages easier to read when dealing with exceptions.
        /// </summary>
        /// <remarks>
        /// Example: Exception.Messages()
        /// </remarks>
        public static IEnumerable<string> Messages(this Exception ex)
        {
            // Hp --> Logic: Return an empty sequence if the provided exception is null
            if (null == ex)
            {
                yield break;
            }

            // Hp --> Logic: first return this exception's message at the beginning of the list
            yield return ex.Message;

            // Hp --> Logic: Then get all the lower-level exception messages recursively (if any)
            var innerExceptions = Enumerable.Empty<Exception>();
            if (ex is AggregateException && (ex as AggregateException).InnerExceptions.Any())
            {
                innerExceptions = (ex as AggregateException).InnerExceptions;
            }
            else if (null != ex.InnerException)
            {
                innerExceptions = new Exception[] { ex.InnerException };
            }

            foreach (var innerEx in innerExceptions)
            {
                foreach (string msg in innerEx.Messages())
                {
                    yield return msg;
                }
            }
        }
    }
}
