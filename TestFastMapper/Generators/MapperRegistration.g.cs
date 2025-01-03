
namespace FastMapper 
{
    public static partial class Mapper 
    {
        static partial void RegisterGeneratedMappers()
        {
            
            _mappers[(typeof(FastMapper.Models.SourceClass), typeof(FastMapper.Models.DestClass))] = 
                (SourceClassToDestClassMapper.Map);

        }
    }
}