
namespace FastMapper 
{
    public static class SourceClassToDestClassMapper 
    {
        public static FastMapper.Models.DestClass Map(FastMapper.Models.SourceClass source)
        {
            if (source == null) return null;
            var dest = new FastMapper.Models.DestClass();
            dest.Id = source.Id;
            dest.Name = source.Name;
            dest.CreateTime = source.CreateTime;
            dest.Tags = source.Tags != null ? 
                source.Tags.Select(item => 
                    SourceTestInfoToDestInfoMapper.Map(item))
                    .ToList() : null;
            dest.TagsMap = source.TagsMap != null ?
                    source.TagsMap.ToDictionary(
                        kvp => kvp.Key,
                        kvp => SourceTestInfoToDestInfoMapper.Map(kvp.Value)) : null;
            dest.TagsInt = source.TagsInt != null ? 
                    new List<System.Int32>(source.TagsInt) : null;
            dest.TagsStr = source.TagsStr != null ? 
                    new List<System.String>(source.TagsStr) : null;
            dest.TagsUInt = source.TagsUInt != null ? 
                    new List<System.UInt32>(source.TagsUInt) : null;

            return dest;
        }
    }
}