using ProtoBuf;
using System.ComponentModel;
using XExten.Common;
using XExten.Office.Enums;

namespace XExten.Test.TestModel
{
    public class TestA
    {
        [Description("标识")]
        public int Id { get; set; }

        [Description("姓名")]
        public string Name { get; set; }

        [Description("密码")]
        public string PassWord { get; set; }
    }

    [ProtoContract]
    public class TestB
    {
        [Description("Test")]
        [ProtoMember(1)]
        public int Id { get; set; }

        [Description("Test")]
        [ProtoMember(2)]
        public string Name { get; set; }

        [Description("Test")]
        [ProtoMember(3)]
        public string Account { get; set; }
    }
    public enum TestC
    {
        [Description("A等级")]
        A,
        [Description("B等级")]
        B
    }

    public class TestD
    {
        [Office(MapperField = "标志", IngoreField = true)]
        public int Id { get; set; }
        [Office(MapperField = "是否男", BoolEnum = typeof(ExcelBoolType))]
        public bool IsMan { get; set; }
        [Office(MapperField = "名称")]
        public string Name { get; set; }
        [Office(MapperField = "等级", Enum = true)]
        public TestC Tc { get; set; }
    }
}