using System;
using System.Collections.Generic;

namespace SpaMetadata
{
    public class MetadataToken
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class MetadataMiddlewareOptions
    {
        public string FilterPath { get; set; }
        public Func<string, IEnumerable<MetadataToken>> GetTokens { get; set; }
    }
}
