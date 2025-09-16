using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UniRmmz
{
    public interface IMetadataContainer
    {
        public string Note { get; }
        public RmmzMetadata Meta { get; set; }
    }
}