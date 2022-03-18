using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public interface ICustomData
    {
        public Dictionary<string, object?> customData { get; }
    }
}
