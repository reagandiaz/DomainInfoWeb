using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IntegrationTools
{
    public class QueueHelper
    {
        public static List<List<T>> CreateQueue<T>(List<T> data, int worker)
        {
            var batch = new List<List<T>>();
            if (data.Count > 0)
            {
                var set = data.Count / worker;
                int pos = 0;

                if (set == 0)
                {
                    data.ForEach(s =>
                    {
                        batch.Add(new List<T>() { s });
                    });
                }
                else
                {
                    int iter = worker;
                    while (iter > 0)
                    {
                        batch.Add(data.Skip(pos).Take(set).ToList());
                        pos += set;
                        iter--;
                    }

                    //handle remainder
                    var rem = data.Count % worker;
                    if (rem > 0)
                    {
                        for (int i = 0; i < rem; i++)
                            batch[i].Add(data[pos + i]);
                    }
                }
            }
            return batch;
        }
    }
}
