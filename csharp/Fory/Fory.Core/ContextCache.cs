/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

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
