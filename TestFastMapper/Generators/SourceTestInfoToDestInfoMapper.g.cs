
namespace FastMapper 
{
    public static class SourceTestInfoToDestInfoMapper 
    {
        public static FastMapper.Models.DestInfo Map(FastMapper.Models.SourceTestInfo source)
        {
            if (source == null) return null;
            var dest = new FastMapper.Models.DestInfo();
            dest.Id = source.Id;
            dest.Name = source.Name;
            dest.CreateTime = source.CreateTime;

            return dest;
        }
    }
}