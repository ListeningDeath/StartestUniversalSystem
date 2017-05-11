using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterMachineSystem.Module
{
    abstract public class TestObject  // 抽象类测试对象
    {
        public Guid _GUID { set; get; }  // GUID
        public string _Name { set; get; }  // 名称
        public ushort _Type { set; get; }  // 类型（目前仅一种）
        protected DateTime _DateTime;  // 项目建立日期
        protected List<object> _ItemList;  // 子对象列表
        protected TestObject(string Name, ushort Type = 0)  // 构造函数
        {
            _GUID = Guid.NewGuid();
            _Name = Name;
            _Type = Type;
            _DateTime = DateTime.Now;
            _ItemList = new List<object>();
        }
        public void AddItem(object _object)  // 添加子对象
        {
            _ItemList.Add(_object);
        }
        public void RemoveItem(object _object)  // 删除子对象
        {
            _ItemList.Remove(_object);
        }
        public int ItemCount()  // 子对象个数
        {
            return _ItemList.Count;
        }
        abstract public bool Import(string FilePath);  // 导入
        abstract public bool Export(string FilePath);  // 导出
    }
    public class TestProject : TestObject  // 测试项目
    {
        public Comm _Comm { set; get; }  // 通信接口
        public TestProject(string ProjectName, ushort ProjectType = 0) : base(ProjectName, ProjectType)  // 初始化
        {
            
        }
        public bool Save(string FilePath)  // 保存
        {
            return true;
        }
        public bool Load(string FilePath)  // 载入
        {
            return true;
        }
        public override bool Import(string FilePath)  // 导入
        {
            return true;
        }
        public override bool Export(string FilePath)  // 导出
        {
            return true;
        }
    }


    public class TestPlan : TestObject  // 测试计划
    {
        public TestPlan(string PlanName) : base(PlanName)  // 初始化
        {
            _Name = PlanName;
            _DateTime = DateTime.Now;
        }
        public override bool Import(string FilePath)  // 导入
        {
            return true;
        }
        public override bool Export(string FilePath)  // 导出
        {
            return true;
        }
    }
}
