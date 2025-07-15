//--------------------------------------------------------------------------
// File:    GarbageCollectorEx.cs
// Content:	Implementation of class GarbageCollectorEx
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Garbage Collector (<see cref="GC"/>) extensions.
	/// </summary>
	/// <seealso cref="GC"/>
	public static class GarbageCollectorEx
    {
        /// <summary>
        /// Collects and waits for pending finalizers several times
        /// </summary>
        public static void AggressiveCollect()
        {
            AggressiveCollect(4);
        }

        /// <summary>
        /// Collects and waits for pending finalizers <param name="iterations" />
        /// times
        /// </summary>
        public static void AggressiveCollect(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

    }
}
