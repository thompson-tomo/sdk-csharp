﻿// Copyright 2021 Cloud Native Foundation. 
// Licensed under the Apache 2.0 license.
// See LICENSE file in the project root for full license information.

using CloudNative.CloudEvents.NewtonsoftJson;
using Xunit;
using static CloudNative.CloudEvents.UnitTests.CloudEventFormatterExtensions;

namespace CloudNative.CloudEvents.Extensions.UnitTests
{
    public class PartitioningTest
    {
        private static readonly string sampleJson = @"
           {
               'specversion' : '1.0',
               'type' : 'com.github.pull.create',
               'id' : 'A234-1234-1234',
               'source' : '//event-source',
               'partitionkey' : 'abc',
           }".Replace('\'', '"');


        [Fact]
        public void ParseJson()
        {
            var jsonFormatter = new JsonEventFormatter();
            var cloudEvent = jsonFormatter.DecodeStructuredModeText(sampleJson);
            Assert.Equal("abc", cloudEvent["partitionkey"]);
            Assert.Equal("abc", cloudEvent[Partitioning.PartitionKeyAttribute]);
            Assert.Equal("abc", cloudEvent.GetPartitionKey());
        }

        [Fact]
        public void Transcode()
        {
            var jsonFormatter = new JsonEventFormatter();
            var cloudEvent1 = jsonFormatter.DecodeStructuredModeText(sampleJson);
            var jsonData = jsonFormatter.EncodeStructuredModeMessage(cloudEvent1, out var contentType);
            var cloudEvent = jsonFormatter.DecodeStructuredModeMessage(jsonData, contentType, null);

            Assert.Equal("abc", cloudEvent["partitionkey"]);
            Assert.Equal("abc", cloudEvent[Partitioning.PartitionKeyAttribute]);
            Assert.Equal("abc", cloudEvent.GetPartitionKey());
        }

        [Fact]
        public void SetPartitionKey()
        {
            var cloudEvent = new CloudEvent();
            cloudEvent.SetPartitionKey("xyz");
            Assert.Equal("xyz", cloudEvent["partitionkey"]);
            Assert.Equal("xyz", cloudEvent[Partitioning.PartitionKeyAttribute]);

            cloudEvent.SetPartitionKey(null);
            Assert.Null(cloudEvent["partitionkey"]);
            Assert.Null(cloudEvent[Partitioning.PartitionKeyAttribute]);
        }

        [Fact]
        public void GetPartitionKey()
        {
            var cloudEvent = new CloudEvent
            {
                ["partitionkey"] = "xyz"
            };
            Assert.Equal("xyz", cloudEvent.GetPartitionKey());

            cloudEvent["partitionkey"] = null;
            Assert.Null(cloudEvent.GetPartitionKey());
        }
    }
}
