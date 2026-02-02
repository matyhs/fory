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

using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.Serialization;
using Fory.Core.Spec.DataType;

namespace Fory.Core;

public sealed class DeserializationContext
{
    private readonly ForyOptions _options;
    private PipeReader? _reader;

    internal DeserializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _options = options;
        TypeSpecificationRegistry = typeSpecificationRegistry;
    }

    public PipeReader Reader => _reader ?? throw new SerializationException("Stream reader not initialized");

    public bool IsXlang => _options.Xlang;

    public bool IsCompatible => _options.Compatible;

    internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }

    internal void Initialize(Stream stream)
    {
        _reader = PipeReader.Create(stream);
    }
}
