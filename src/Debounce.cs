namespace Philiprehberger.Debounce;

/// <summary>
/// Provides factory methods for creating debounced actions.
/// </summary>
public static class Debounce
{
    /// <summary>
    /// Creates a debounced wrapper around the specified action.
    /// The action will only execute after the specified delay has elapsed since the last invocation.
    /// </summary>
    /// <param name="action">The action to debounce.</param>
    /// <param name="delay">The debounce delay.</param>
    /// <param name="leading">
    /// If <c>true</c>, the action fires on the leading edge (immediately on first call)
    /// and subsequent calls within the delay window are ignored.
    /// If <c>false</c> (default), the action fires on the trailing edge (after the delay has elapsed).
    /// </param>
    /// <returns>A <see cref="DebouncedAction"/> that wraps the action.</returns>
    public static DebouncedAction Create(Action action, TimeSpan delay, bool leading = false)
    {
        ArgumentNullException.ThrowIfNull(action);
        return new DebouncedAction(action, null, delay, leading);
    }

    /// <summary>
    /// Creates a debounced wrapper around the specified async function.
    /// The function will only execute after the specified delay has elapsed since the last invocation.
    /// </summary>
    /// <param name="asyncAction">The async function to debounce.</param>
    /// <param name="delay">The debounce delay.</param>
    /// <param name="leading">
    /// If <c>true</c>, the function fires on the leading edge.
    /// If <c>false</c> (default), the function fires on the trailing edge.
    /// </param>
    /// <returns>A <see cref="DebouncedAction"/> that wraps the async function.</returns>
    public static DebouncedAction Create(Func<Task> asyncAction, TimeSpan delay, bool leading = false)
    {
        ArgumentNullException.ThrowIfNull(asyncAction);
        return new DebouncedAction(null, asyncAction, delay, leading);
    }
}
