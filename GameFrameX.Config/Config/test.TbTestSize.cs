
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Config.Core;


namespace cfg.test
{
public partial class TbTestSize
{
    private readonly System.Collections.Generic.Dictionary<int, test.TestSize> _dataMap;
    private readonly System.Collections.Generic.List<test.TestSize> _dataList;
    
    public TbTestSize(JsonElement _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, test.TestSize>();
        _dataList = new System.Collections.Generic.List<test.TestSize>();
        
        foreach(JsonElement _ele in _buf.EnumerateArray())
        {
            test.TestSize _v;
            _v = test.TestSize.DeserializeTestSize(_ele);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, test.TestSize> DataMap => _dataMap;
    public System.Collections.Generic.List<test.TestSize> DataList => _dataList;

    public test.TestSize GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public test.TestSize Get(int key) => _dataMap[key];
    public test.TestSize this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}