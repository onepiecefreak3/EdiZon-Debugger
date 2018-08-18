using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json.Linq;

namespace EdiZonDebugger.Models
{
    public class EdiZonConfig
    {
        private bool _shouldSerializeTitleVersion = true;
        public bool ShouldSerializetitleVersion() => _shouldSerializeTitleVersion;
        public void SetShouldSerializeTitleVersion(bool value) => _shouldSerializeTitleVersion = value;

        public List<string> saveFilePaths { get; set; }
        public string files { get; set; }
        public string filetype { get; set; }
        public string titleVersion { get; set; }

        public List<Item> items { get; set; }

        public class Item
        {
            private bool _shouldSerializeCategory = true;
            public bool ShouldSerializecategory() => _shouldSerializeCategory;
            public void SetShouldSerializeCategory(bool value) => _shouldSerializeCategory = value;

            public string category { get; set; }
            public string name { get; set; }

            public List<int> intArgs { get; set; }
            public List<string> strArgs { get; set; }

            public Widget widget { get; set; }

            public override string ToString() => Path.Combine(category, name);

            public class Widget
            {
                private bool _shouldSerializeStepSize = true;
                public bool ShouldSerializestepSize() => _shouldSerializeStepSize;
                public void SetShouldSerializeStepSize(bool value) => _shouldSerializeStepSize = value;

                private bool _shouldSerializeMinValue = true;
                public bool ShouldSerializeminValue() => _shouldSerializeMinValue;
                public void SetShouldSerializeMinValue(bool value) => _shouldSerializeMinValue = value;
                private bool _shouldSerializeMaxValue = true;
                public bool ShouldSerializemaxValue() => _shouldSerializeMaxValue;
                public void SetShouldSerializeMaxValue(bool value) => _shouldSerializeMaxValue = value;

                private bool _shouldSerializeOnValue = true;
                public bool ShouldSerializeonValue() => _shouldSerializeOnValue;
                public void SetShouldSerializeOnValue(bool value) => _shouldSerializeOnValue = value;
                private bool _shouldSerializeOffValue = true;
                public bool ShouldSerializeoffValue() => _shouldSerializeOffValue;
                public void SetShouldSerializeOffValue(bool value) => _shouldSerializeOffValue = value;

                private bool _shouldSerializeListItemNames = true;
                public bool ShouldSerializelistItemNames() => _shouldSerializeListItemNames;
                public void SetShouldSerializeListItemNames(bool value) => _shouldSerializeListItemNames = value;
                private bool _shouldSerializeListItemValues = true;
                public bool ShouldSerializelistItemValues() => _shouldSerializeListItemValues;
                public void SetShouldSerializeListItemValues(bool value) => _shouldSerializeListItemValues = value;

                public string type { get; set; }
                public int? stepSize { get; set; }

                public int minValue { get; set; }
                public int maxValue { get; set; }

                public int onValue { get; set; }
                public int offValue { get; set; }

                public List<string> listItemNames { get; set; }
                public List<uint> listItemValues { get; set; }
            }
        }
    }
}
