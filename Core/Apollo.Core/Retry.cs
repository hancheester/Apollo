using System;
using System.Collections.Generic;
using System.Threading;

namespace Apollo.Core
{
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int retryCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount);
        }

        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);
                    return action();
                }                
                catch(ApolloException ex1)
                {
                    exceptions.Add(ex1);
                    break;
                }
                catch (Exception ex2)
                {
                    exceptions.Add(ex2);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
