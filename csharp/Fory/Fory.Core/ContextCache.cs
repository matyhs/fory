using System;
using System.Collections.Concurrent;

namespace Fory.Core;

internal sealed class ContextCache<TContext>
{
    private readonly ConcurrentDictionary<long, TContext> _cache = new();
    private long _foryInstanceId = long.MinValue;
    private TContext? _currentContext;

    public TContext? GetOrCreate(long foryInstanceId, Func<TContext?> factory)
    {
        if (foryInstanceId == _foryInstanceId)
            return _currentContext;

        if (_currentContext is not null)
            _cache.TryAdd(_foryInstanceId, _currentContext);

        _foryInstanceId = foryInstanceId;
        _cache.TryRemove(foryInstanceId, out _currentContext);
        _currentContext ??= factory();

        return _currentContext;
    }
}
