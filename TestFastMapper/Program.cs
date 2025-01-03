using FastMapper.Models;

namespace FastMapper
{
    [ManualMapper(typeof(SourceClass), typeof(DestClass),false)]
    
    public class Program
    {
        static void Main(string[] args)
        {
            var source = new SourceClass
            {
                Name = "Test",
                Id = 25
            };

          
           if (true)
           {
               // 生成映射代码
               var pathHotfix = @"D:\Work\GameFrameX.Server\TestFastMapper\Generators";
               var generator = new ManualMapperGeneratorAuto(pathHotfix);
                generator.GenerateMappers(typeof(SourceClass).Assembly);
           }
           else
           {
               //测试使用
               var dest = Mapper.Map<SourceClass, DestClass>(source);
               Console.WriteLine($"Name: {dest.Name}, Age: {dest.Id}");
           }
          
        }
    }
    
}