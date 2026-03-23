namespace Philiprehberger.Debounce;

/// <summary>
/// Provides factory methods for creating throttled actions.
/// </summary>
public static class Throttle
{
    /// <summary>
    /// Creates a throttled wrapper around the specified action.
    /// The action will execute at most once per the specified interval.
    /// </summary>
    /// <param name="action">The action to throttle.</param>
    /// <param name="interval">The minimum interval between executions.</param>
    /// <returns>A <see cref="ThrottledAction"/> that wraps the action.</returns>
    public static ThrottledAction Create(Action action, TimeSpan interval)
    {
        ArgumentNullException.ThrowIfNull(action);
        return new ThrottledAction(action, null, interval);
    }

    /// <summary>
    /// Creates a throttled wrapper around the specified async function.
    /// The function will execute at most once per the specified interval.
    /// </summary>
    /// <param name="asyncAction">The async function to throttle.</param>
    /// <param name="interval">The minimum interval between executions.</param>
    /// <returns>A <see cref="ThrottledAction"/> that wraps the async function.</returns>
    public static ThrottledAction Create(Func<Task> asyncAction, TimeSpan interval)
    {
        ArgumentNullException.ThrowIfNull(asyncAction);
        return new ThrottledAction(null, asyncAction, interval);
    }
}
