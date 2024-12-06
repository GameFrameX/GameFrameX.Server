
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config.test
{
    public partial class TbTestGlobal : BaseDataTable<test.TestGlobal>
    {
    
        private test.TestGlobal _data;
        public test.TestGlobal Data => _data;

        public int UnlockEquip => _data.UnlockEquip;
        public int UnlockHero => _data.UnlockHero;
    
        public override async Task LoadAsync()
        {
            var jsonElement = await _loadFunc();

            int n = jsonElement.GetArrayLength();
            if (n != 1) throw new SerializationException("table mode=one, but size != 1");
            _data = test.TestGlobal.DeserializeTestGlobal(jsonElement[0]);
        }

        public void ResolveRef(TablesComponent tables)
        {
            _data.ResolveRef(tables);
        }
    
        partial void PostInit();

        public TbTestGlobal(Func<Task<JsonElement>> loadFunc) : base(loadFunc)
        {
        }
    }
}