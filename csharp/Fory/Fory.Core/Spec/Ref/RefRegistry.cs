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

using System.Collections.Generic;

namespace Fory.Core.Spec.Ref;

internal class RefWriterRegistry
{
    private readonly Dictionary<object, uint> _registry = new();

    public uint GetOrRegister(object key)
    {
        if (!_registry.TryGetValue(key, out var id))
        {
            id = (uint) _registry.Count;
            _registry.Add(key, id);
        }

        return id;
    }
}

internal class RefReaderRegistry
{
    private readonly Dictionary<uint, object> _registry = new();

    public uint Register(object value)
    {
        var id = (uint) _registry.Count;
        _registry.Add(id, value);

        return id;
    }

    public TValue Get<TValue>(uint key)
    {
        return (TValue) _registry[key];
    }
}
