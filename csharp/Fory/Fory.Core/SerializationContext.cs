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

using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Fory.Core.Spec.DataType;
using Fory.Core.Spec.Meta;

namespace Fory.Core;

public sealed class SerializationContext
{
    private readonly ForyOptions _options;
    private readonly Pipe _pipe;

    private PipeWriter? _writer;

    internal SerializationContext(ForyOptions options, TypeSpecificationRegistry typeSpecificationRegistry)
    {
        _pipe = new Pipe();
        TypeMetaRegistry = new TypeMetaRegistry(typeSpecificationRegistry);
        TypeMetaStringRegistry = new TypeMetaStringRegistry();

        _options = options;
        TypeSpecificationRegistry = typeSpecificationRegistry;
    }

    public PipeWriter Writer => _writer ?? _pipe.Writer;

    public bool IsXlang => _options.Xlang;

    public bool ShareMeta => _options.Compatible;

    internal TypeSpecificationRegistry TypeSpecificationRegistry { get; private set; }
    internal TypeMetaRegistry TypeMetaRegistry { get; private set; }
    internal TypeMetaStringRegistry TypeMetaStringRegistry { get; private set; }

    internal void Initialize()
    {
        _writer = null;
    }

    internal void Initialize(Stream stream)
    {
        _writer = PipeWriter.Create(stream, new StreamPipeWriterOptions(leaveOpen: true));
    }

    internal async ValueTask<Stream> CompleteAsync(CancellationToken cancellationToken = default)
    {
        await Writer.CompleteAsync().ConfigureAwait(false);
        return _pipe.Reader.AsStream();
    }
}
