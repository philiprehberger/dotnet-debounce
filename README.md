# Philiprehberger.Debounce

[![CI](https://github.com/philiprehberger/dotnet-debounce/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-debounce/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.Debounce.svg)](https://www.nuget.org/packages/Philiprehberger.Debounce)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-debounce)](LICENSE)

Debounce and throttle functions for .NET with configurable delay, leading/trailing edge, and async support.

## Installation

```bash
dotnet add package Philiprehberger.Debounce
```

## Usage

```csharp
using Philiprehberger.Debounce;

// Trailing edge debounce (default) — fires after 300ms of quiet
var debounced = Debounce.Create(() =>
{
    Console.WriteLine("Saving...");
}, TimeSpan.FromMilliseconds(300));

debounced.Invoke(); // resets timer
debounced.Invoke(); // resets timer again
// "Saving..." fires 300ms after the last Invoke()

// Leading edge debounce — fires immediately, ignores subsequent calls within window
var leading = Debounce.Create(() =>
{
    Console.WriteLine("Clicked!");
}, TimeSpan.FromMilliseconds(500), leading: true);

leading.Invoke(); // fires immediately
leading.Invoke(); // ignored (within 500ms window)

// Async debounce
var asyncDebounced = Debounce.Create(async () =>
{
    await SaveToDatabase();
}, TimeSpan.FromSeconds(1));

await asyncDebounced.InvokeAsync();

// Manual control
debounced.Cancel(); // cancel pending execution
debounced.Flush();  // execute immediately if pending

// Throttle — max once per interval
var throttled = Throttle.Create(() =>
{
    Console.WriteLine("Processing...");
}, TimeSpan.FromSeconds(1));

throttled.Invoke(); // fires
throttled.Invoke(); // ignored (within 1s)
// After 1s, next Invoke() will fire again
```

## API

### `Debounce`

| Member | Description |
|--------|-------------|
| `Create(Action, TimeSpan, bool leading)` | Create a debounced action (trailing edge by default) |
| `Create(Func<Task>, TimeSpan, bool leading)` | Create a debounced async action |

### `DebouncedAction`

| Member | Description |
|--------|-------------|
| `Invoke()` | Trigger the debounced action (resets timer) |
| `InvokeAsync()` | Trigger the debounced async action |
| `Cancel()` | Cancel any pending execution |
| `Flush()` | Execute immediately if a call is pending |
| `FlushAsync()` | Execute async immediately if a call is pending |
| `IsPending` | Whether an execution is pending |

### `Throttle`

| Member | Description |
|--------|-------------|
| `Create(Action, TimeSpan)` | Create a throttled action |
| `Create(Func<Task>, TimeSpan)` | Create a throttled async action |

### `ThrottledAction`

| Member | Description |
|--------|-------------|
| `Invoke()` | Execute if the interval has elapsed since last execution |
| `InvokeAsync()` | Execute async if the interval has elapsed |
| `Reset()` | Reset the throttle window |

## Development

```bash
dotnet build src/Philiprehberger.Debounce.csproj --configuration Release
```

## License

MIT
