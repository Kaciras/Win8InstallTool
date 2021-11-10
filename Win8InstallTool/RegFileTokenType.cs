namespace Win8InstallTool;

public enum RegFileTokenType : byte
{
    None,           // 未读取到有效数据
    Comment,        // 注释
    Version,        // 文件版本
    CreateKey,      // 创建注册表键
    DeleteKey,      // 删除注册表键
    ValueName,      // 注册表值的名字
    Value,          // 注册表值
    Kind,           // 注册表值的类型
    DeleteValue,    // 删除注册表值
}
