using System.Collections.Generic;
using System.Linq;

namespace Navvy.SampleApp.Console.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<ICollection<TSource>> Batch<TSource>(
            this IEnumerable<TSource> source,
            int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (true)
                {
                    var batch = new List<TSource>(batchSize);

                    for (var i = 0; i < batchSize; i++)
                    {
                        if (enumerator.MoveNext())
                        {
                            batch.Add(enumerator.Current);
                        }
                        else
                        {
                            if (batch.Any())
                            {
                                yield return batch;
                            }

                            yield break;
                        }
                    }

                    yield return batch;
                }
            }
        }
    }
}
