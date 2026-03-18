#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
检查 Keys 类中定义的所有 key 是否在 resx 资源文件中存在
如果不存在则自动补充
"""

import os
import re
import xml.etree.ElementTree as ET
from xml.dom import minidom
from pathlib import Path

# 项目路径
SCRIPT_DIR = Path(__file__).parent
KEYS_DIR = SCRIPT_DIR
RESX_DIR = SCRIPT_DIR / "Localization" / "Messages"

# resx 文件路径
RESX_FILES = {
    "default": RESX_DIR / "Resources.resx",      # 英文默认
    "zh-CN": RESX_DIR / "Resources.zh-CN.resx",  # 中文
}


def extract_keys_from_cs_files():
    """从所有 Keys.*.cs 文件中提取 key 定义"""
    keys = set()

    # 查找所有 Keys.*.cs 文件
    cs_files = list(KEYS_DIR.glob("Keys.*.cs"))
    cs_files.append(KEYS_DIR / "Keys.cs")

    # 正则匹配 const string 定义
    # 格式: public const string Name = "Key.Value";
    pattern = re.compile(r'public\s+const\s+string\s+\w+\s*=\s*"([^"]+)"')

    for cs_file in cs_files:
        if not cs_file.exists():
            continue

        with open(cs_file, 'r', encoding='utf-8') as f:
            content = f.read()
            matches = pattern.findall(content)
            keys.update(matches)

    return keys


def parse_resx_keys(resx_path):
    """解析 resx 文件中已存在的 key"""
    keys = set()

    if not resx_path.exists():
        return keys

    tree = ET.parse(resx_path)
    root = tree.getroot()

    # resx 文件的命名空间
    ns = {'resx': ''}

    for data in root.findall('data'):
        name = data.get('name')
        if name:
            keys.add(name)

    return keys


def prettify_xml(elem):
    """将 XML 元素格式化为漂亮的字符串"""
    rough_string = ET.tostring(elem, encoding='unicode')
    reparsed = minidom.parseString(rough_string)
    return reparsed.toprettyxml(indent="  ", encoding=None)


def add_keys_to_resx(resx_path, missing_keys, is_chinese=False):
    """将缺失的 key 添加到 resx 文件中"""
    if not missing_keys:
        return 0

    if not resx_path.exists():
        print(f"  文件不存在: {resx_path}")
        return 0

    # 读取原始文件内容
    with open(resx_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # 解析 XML
    tree = ET.parse(resx_path)
    root = tree.getroot()

    # 找到 </root> 前的位置，添加新的 data 元素
    added_count = 0

    for key in sorted(missing_keys):
        # 创建新的 data 元素
        data_elem = ET.SubElement(root, 'data')
        data_elem.set('name', key)
        data_elem.set('xml:space', 'preserve')

        # 添加 value 子元素
        value_elem = ET.SubElement(data_elem, 'value')

        # 根据是否是中文设置不同的占位值
        if is_chinese:
            value_elem.text = f"[待翻译] {key}"
        else:
            value_elem.text = f"[TODO] {key}"

        added_count += 1

    # 保存文件 - 手动格式化以保持原有格式
    # 使用更简单的方式：直接在 </root> 前插入
    lines = content.split('\n')

    # 找到 </root> 的位置
    root_end_index = -1
    for i, line in enumerate(lines):
        if line.strip() == '</root>':
            root_end_index = i
            break

    if root_end_index == -1:
        print(f"  无法找到 </root> 标签")
        return 0

    # 插入新的 data 元素
    new_lines = []
    for key in sorted(missing_keys):
        if is_chinese:
            value = f"[待翻译] {key}"
        else:
            value = f"[TODO] {key}"

        new_lines.append(f'  <data name="{key}" xml:space="preserve">')
        new_lines.append(f'    <value>{value}</value>')
        new_lines.append(f'  </data>')
        new_lines.append('')

    # 在 </root> 前插入新行
    lines = lines[:root_end_index] + new_lines + lines[root_end_index:]

    # 写回文件
    with open(resx_path, 'w', encoding='utf-8') as f:
        f.write('\n'.join(lines))

    return added_count


def main():
    print("=" * 60)
    print("Keys 检查脚本 - 检查 resx 资源文件中缺失的 key")
    print("=" * 60)
    print()

    # 1. 从 C# 文件中提取所有 key
    print("1. 从 Keys.*.cs 文件中提取 key...")
    cs_keys = extract_keys_from_cs_files()
    print(f"   找到 {len(cs_keys)} 个 key 定义")

    # 2. 检查每个 resx 文件
    for lang, resx_path in RESX_FILES.items():
        print()
        print(f"2. 检查 {lang} 资源文件: {resx_path.name}")

        # 获取 resx 中已存在的 key
        resx_keys = parse_resx_keys(resx_path)
        print(f"   资源文件中有 {len(resx_keys)} 个 key")

        # 找出缺失的 key
        missing_keys = cs_keys - resx_keys
        extra_keys = resx_keys - cs_keys

        if missing_keys:
            print(f"   缺失 {len(missing_keys)} 个 key:")
            for key in sorted(missing_keys)[:10]:  # 只显示前10个
                print(f"     - {key}")
            if len(missing_keys) > 10:
                print(f"     ... 还有 {len(missing_keys) - 10} 个")

            # 添加缺失的 key
            is_chinese = lang == "zh-CN"
            added = add_keys_to_resx(resx_path, missing_keys, is_chinese)
            print(f"   已添加 {added} 个缺失的 key 到 {resx_path.name}")
        else:
            print(f"   所有 key 都存在，无需补充")

        if extra_keys:
            print(f"   资源文件中有 {len(extra_keys)} 个多余的 key (在 C# 中未定义)")
            for key in sorted(extra_keys)[:10]:  # 只显示前10个
                print(f"     - {key}")
            if len(extra_keys) > 10:
                print(f"     ... 还有 {len(extra_keys) - 10} 个")

    print()
    print("=" * 60)
    print("检查完成!")
    print("=" * 60)


if __name__ == "__main__":
    main()
