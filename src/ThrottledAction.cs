namespace Philiprehberger.Debounce;

/// <summary>
/// Limits execution of an action to at most once per configured interval.
/// Uses <see cref="SemaphoreSlim"/> for thread safety.
/// </summary>
public sealed class ThrottledAction : IDisposable
{
    private readonly Action? _action;
    private readonly Func<Task>? _asyncAction;
    private readonly TimeSpan _interval;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTimeOffset _lastExecution = DateTimeOffset.MinValue;
    private bool _disposed;

    internal ThrottledAction(Action? action, Func<Task>? asyncAction, TimeSpan interval)
    {
        _action = action;
        _asyncAction = asyncAction;
        _interval = interval;
    }

    /// <summary>
    /// Executes the action if the configured interval has elapsed since the last execution.
    /// If the interval has not elapsed, the call is silently ignored.
    /// </summary>
    /// <returns><c>true</c> if the action was executed; <c>false</c> if throttled.</returns>
    public bool Invoke()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _semaphore.Wait();
        try
        {
            var now = DateTimeOffset.UtcNow;
            if (now - _lastExecution < _interval)
            {
                return false;
            }

            _lastExecution = now;

            if (_action is not null)
            {
                _action();
            }
            else if (_asyncAction is not null)
            {
                _asyncAction().GetAwaiter().GetResult();
            }

            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Executes the async action if the configured interval has elapsed since the last execution.
    /// If the interval has not elapsed, the call is silently ignored.
    /// </summary>
    /// <returns><c>true</c> if the action was executed; <c>false</c> if throttled.</returns>
    public async Task<bool> InvokeAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            var now = DateTimeOffset.UtcNow;
            if (now - _lastExecution < _interval)
            {
                return false;
            }

            _lastExecution = now;

            if (_asyncAction is not null)
            {
                await _asyncAction().ConfigureAwait(false);
            }
            else if (_action is not null)
            {
                _action();
            }

            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Resets the throttle window, allowing the next <see cref="Invoke"/> or <see cref="InvokeAsync"/> call to execute immediately.
    /// </summary>
    public void Reset()
    {
        _semaphore.Wait();
        try
        {
            _lastExecution = DateTimeOffset.MinValue;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _semaphore.Dispose();
    }
}
