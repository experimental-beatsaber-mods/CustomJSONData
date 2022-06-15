using System;

namespace CustomJSONData
{
    public class JsonNotDefinedException : Exception
    {
        public JsonNotDefinedException(string fieldName)
            : base($"[{fieldName}] required but was not defined.")
        {
        }
    }
}
