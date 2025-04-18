
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config.Local
{
    public partial class TbLocalization : BaseDataTable<Local.Localization>
    {
        //private readonly System.Collections.Generic.Dictionary<string, Local.Localization> _dataMap;
        //private readonly System.Collections.Generic.List<Local.Localization> _dataList;
    
        //public System.Collections.Generic.Dictionary<string, Local.Localization> DataMap => _dataMap;
        //public System.Collections.Generic.List<Local.Localization> DataList => _dataList;
        //public Local.Localization GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
        //public Local.Localization Get(string key) => _dataMap[key];
        //public Local.Localization this[string key] => _dataMap[key];
    
        public override async System.Threading.Tasks.Task LoadAsync()
        {
            var jsonElement = await _loadFunc();
            DataList.Clear();
            LongDataMaps.Clear();
            StringDataMaps.Clear();
            foreach(var element in jsonElement.EnumerateArray())
            {
                Local.Localization _v;
                _v = Local.Localization.DeserializeLocalization(element);
                DataList.Add(_v);
                StringDataMaps.Add(_v.Key.ToString(), _v);
            }
            PostInit();
        }

        public void ResolveRef(TablesComponent tables)
        {
            foreach(var element in DataList)
            {
                element.ResolveRef(tables);
            }
        }
    
    
        partial void PostInit();

        public TbLocalization(Func<Task<JsonElement>> loadFunc) : base(loadFunc)
        {
        }
    }
}
