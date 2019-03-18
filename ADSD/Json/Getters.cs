using System;
using System.Collections.Generic;
using System.Reflection;

namespace ADSD.Json
{
    internal class Getters
    {
        public string Name;
        public JsonTool.GenericGetter Getter;
        public Type PropertyType;
		public FieldInfo FieldInfo;
    }

    internal class DatasetSchema
    {
        public List<string> Info { get; set; }
        public string Name { get; set; }
    }
}
