using System;
using System.Threading;
using Grpc.Core;
using Grpc.Core.Testing;
using Grpc.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MusicServices.Test.TestingHelpers
{
    class gRPC
    {
        /// <summary>
        /// Methods that creates a fake context for testing a service call
        /// </summary>
        /// <returns></returns>
        public static ServerCallContext CreateTestContext()
        {
            return TestServerCallContext.Create("fooMethod",
                null,
                DateTime.UtcNow.AddHours(1),
                new Metadata(),
                CancellationToken.None,
                "127.0.0.1",
                null,
                null,
                (metadata) => TaskUtils.CompletedTask,
                () => new WriteOptions(),
                (writeOptions) => { });
        }
    }
}
