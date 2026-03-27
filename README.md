# Philiprehberger.Debounce

[![CI](https://github.com/philiprehberger/dotnet-debounce/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-debounce/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.Debounce.svg)](https://www.nuget.org/packages/Philiprehberger.Debounce)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-debounce)](LICENSE)
[![Sponsor](https://img.shields.io/badge/sponsor-GitHub%20Sponsors-ec6cb9)](https://github.com/sponsors/philiprehberger)

Debounce and throttle functions for .NET with configurable delay, leading/trailing edge, and async support.

## Installation

```bash
dotnet add package Philiprehberger.Debounce
```

## Usage

### Trailing Edge Debounce

```csharp
using Philiprehberger.Debounce;

var debounced = Debounce.Create(() =>
{
    Console.WriteLine("Saving...");
}, TimeSpan.FromMilliseconds(300));

debounced.Invoke(); // resets timer
debounced.Invoke(); // resets timer again
// "Saving..." fires 300ms after the last Invoke()
```

### Leading Edge Debounce

```csharp
using Philiprehberger.Debounce;

var leading = Debounce.Create(() =>
{
    Console.WriteLine("Clicked!");
}, TimeSpan.FromMilliseconds(500), leading: true);

leading.Invoke(); // fires immediately
leading.Invoke(); // ignored (within 500ms window)
```

### Async Debounce with Manual Control

```csharp
using Philiprehberger.Debounce;

var debounced = Debounce.Create(async () =>
{
    await SaveToDatabase();
}, TimeSpan.FromSeconds(1));

await debounced.InvokeAsync();

debounced.Cancel(); // cancel pending execution
await debounced.FlushAsync(); // execute immediately if pending
Console.WriteLine($"Pending: {debounced.IsPending}");
```

### Throttle

```csharp
using Philiprehberger.Debounce;

var throttled = Throttle.Create(() =>
{
    Console.WriteLine("Processing...");
}, TimeSpan.FromSeconds(1));

throttled.Invoke(); // fires
throttled.Invoke(); // ignored (within 1s)
throttled.Reset();  // reset the throttle window
throttled.Invoke(); // fires again
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

[MIT](LICENSE)
