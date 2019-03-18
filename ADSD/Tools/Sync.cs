using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ADSD.Tools
{
    /// <summary>
    /// Helper class to properly wait for async tasks
    /// </summary>
    internal static class Sync  
    {
        [NotNull]private static readonly TaskFactory _taskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        /// <summary>
        /// Run an async function synchronously and return the result
        /// </summary>
        public static TResult Run<TResult>([NotNull]Func<Task<TResult>> func) {
            var raw = _taskFactory.StartNew(func).Unwrap();
            if (raw == null) throw new Exception("Failed to access task in Sync.Run");
            return raw.GetAwaiter().GetResult();
        }
    }
}