// ========================================================
// 描述：SourceClass.cs
// 作者：Bambomtan 
// 创建时间：2024-12-31 16:57:24 星期二 
// Email：837659628@qq.com
// 版 本：1.0
// ========================================================

using System;
using System.Collections.Generic;

namespace FastMapper.Models;

public class SourceTestInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
    
}
public class DestInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
    
}
public class SourceClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
    public List<SourceTestInfo> Tags { get; set; }
    public Dictionary<int,SourceTestInfo> TagsMap { get; set; }
    public List<int> TagsInt { get; set; }
    public List<string> TagsStr { get; set; }
    public List<uint> TagsUInt { get; set; }
}
    
public class DestClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
    public List<DestInfo> Tags { get; set; }
    public Dictionary<int,DestInfo> TagsMap { get; set; }
    public List<int> TagsInt { get; set; }
    public List<string> TagsStr { get; set; }
    public List<uint> TagsUInt { get; set; }
    
    public List<uint> ahahha { get; set; }
}
