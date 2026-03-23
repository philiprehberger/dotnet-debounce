namespace Philiprehberger.Debounce;

/// <summary>
/// Wraps an action or async function with timer-based debouncing.
/// Supports both leading edge (fire immediately, ignore subsequent within window)
/// and trailing edge (fire after quiet period) modes.
/// </summary>
public sealed class DebouncedAction : IDisposable
{
    private readonly Action? _action;
    private readonly Func<Task>? _asyncAction;
    private readonly TimeSpan _delay;
    private readonly bool _leading;
    private readonly object _lock = new();
    private Timer? _timer;
    private bool _hasPendingCall;
    private bool _leadingFired;
    private bool _disposed;

    internal DebouncedAction(Action? action, Func<Task>? asyncAction, TimeSpan delay, bool leading)
    {
        _action = action;
        _asyncAction = asyncAction;
        _delay = delay;
        _leading = leading;
    }

    /// <summary>
    /// Gets a value indicating whether an execution is currently pending.
    /// </summary>
    public bool IsPending
    {
        get
        {
            lock (_lock)
            {
                return _hasPendingCall;
            }
        }
    }

    /// <summary>
    /// Triggers the debounced action. In trailing edge mode, resets the timer.
    /// In leading edge mode, fires immediately on the first call and ignores subsequent calls within the delay window.
    /// </summary>
    public void Invoke()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_lock)
        {
            if (_leading)
            {
                if (!_leadingFired)
                {
                    _leadingFired = true;
                    ExecuteSync();
                }

                // Reset the cooldown timer
                ResetTimer();
                _hasPendingCall = true;
            }
            else
            {
                _hasPendingCall = true;
                ResetTimer();
            }
        }
    }

    /// <summary>
    /// Triggers the debounced async action. In trailing edge mode, resets the timer.
    /// In leading edge mode, fires immediately on the first call.
    /// </summary>
    /// <returns>A task representing the operation.</returns>
    public async Task InvokeAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        bool shouldExecuteNow = false;

        lock (_lock)
        {
            if (_leading)
            {
                if (!_leadingFired)
                {
                    _leadingFired = true;
                    shouldExecuteNow = true;
                }

                ResetTimer();
                _hasPendingCall = true;
            }
            else
            {
                _hasPendingCall = true;
                ResetTimer();
            }
        }

        if (shouldExecuteNow)
        {
            await ExecuteAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Cancels any pending execution.
    /// </summary>
    public void Cancel()
    {
        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;
            _hasPendingCall = false;
            _leadingFired = false;
        }
    }

    /// <summary>
    /// Executes the action immediately if a call is pending, then cancels the timer.
    /// </summary>
    public void Flush()
    {
        bool shouldExecute;

        lock (_lock)
        {
            shouldExecute = _hasPendingCall && !_leading;
            _timer?.Dispose();
            _timer = null;
            _hasPendingCall = false;
            _leadingFired = false;
        }

        if (shouldExecute)
        {
            ExecuteSync();
        }
    }

    /// <summary>
    /// Executes the async action immediately if a call is pending, then cancels the timer.
    /// </summary>
    /// <returns>A task representing the operation.</returns>
    public async Task FlushAsync()
    {
        bool shouldExecute;

        lock (_lock)
        {
            shouldExecute = _hasPendingCall && !_leading;
            _timer?.Dispose();
            _timer = null;
            _hasPendingCall = false;
            _leadingFired = false;
        }

        if (shouldExecute)
        {
            await ExecuteAsync().ConfigureAwait(false);
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

        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }

    private void ResetTimer()
    {
        _timer?.Dispose();
        _timer = new Timer(OnTimerElapsed, null, _delay, Timeout.InfiniteTimeSpan);
    }

    private void OnTimerElapsed(object? state)
    {
        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;

            if (_leading)
            {
                // Leading edge: just reset so next Invoke fires again
                _hasPendingCall = false;
                _leadingFired = false;
            }
            else
            {
                // Trailing edge: fire the action now
                if (_hasPendingCall)
                {
                    _hasPendingCall = false;
                    ExecuteSync();
                }
            }
        }
    }

    private void ExecuteSync()
    {
        if (_action is not null)
        {
            _action();
        }
        else if (_asyncAction is not null)
        {
            _asyncAction().GetAwaiter().GetResult();
        }
    }

    private async Task ExecuteAsync()
    {
        if (_asyncAction is not null)
        {
            await _asyncAction().ConfigureAwait(false);
        }
        else if (_action is not null)
        {
            _action();
        }
    }
}
