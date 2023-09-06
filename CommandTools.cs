using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace DotNetARX
{
    /// <summary>
    /// 用于 ADSRX 库函数（如 acedCmd）输入参数的类型
    /// </summary>
    public enum ResBufCode
    {
        /// <summary>
        /// 用户输入值有效
        /// </summary>
        Norm = 5100,

        /// <summary>
        /// 没有结果
        /// </summary>
        None = 5000,

        /// <summary>
        /// 实数
        /// </summary>
        Real = 5001,

        /// <summary>
        /// 2 维点
        /// </summary>
        Point2d = 5002,

        /// <summary>
        /// 短整型
        /// </summary>
        Short = 5003,

        /// <summary>
        /// 角度
        /// </summary>
        Angle = 5004,

        /// <summary>
        /// 字符串
        /// </summary>
        String = 5005,

        /// <summary>
        /// 实体 Id
        /// </summary>
        ObjectId = 5006,

        /// <summary>
        /// 选择集名
        /// </summary>
        PickSet = 5007,

        /// <summary>
        /// 方向
        /// </summary>
        Orientation = 5008,

        /// <summary>
        /// 三维点
        /// </summary>
        Point3d = 5009,

        /// <summary>
        /// 长整数
        /// </summary>
        Long = 5010,

        /// <summary>
        /// 空白符号
        /// </summary>
        Void = 5014,

        /// <summary>
        /// 列表开始
        /// </summary>
        ListBegin = 5016,

        /// <summary>
        /// 列表结束
        /// </summary>
        ListEnd = 5017,

        /// <summary>
        /// 点对
        /// </summary>
        DottedPair = 5018,

        /// <summary>
        /// 空
        /// </summary>
        Nil = 5019,

        /// <summary>
        /// DXF 为的 0 的组码
        /// </summary>
        DXF0 = 5020,

        /// <summary>
        /// T atom 
        /// </summary>
        TAtom = 5021,

        /// <summary>
        /// resbuf
        /// </summary>
        Resbuf = 5023,

        /// <summary>
        /// 无模式对话框
        /// </summary>
        Modeless = 5027,
    }

    public static class CommandTools
    {
        /// <summary>
        /// 调用 COM 的 SendCommand 函数
        /// </summary>
        /// <param name="doc">文档对象</param>
        /// <param name="args">命令参数列表</param>
        public static void SendCommand(this Document doc, params string[] args)
        {
            Type AcadDocument = Type.GetTypeFromHandle(Type.GetTypeHandle(doc));
            try
            {
                // 通过后期绑定的方式调用 SendCommand 命令
                AcadDocument.InvokeMember("SendCommand", BindingFlags.InvokeMethod, null, doc, args);
            }
            catch // 捕获异常
            {
                return;
            }
        }

        [DllImport("acad.exe", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "?acedPostCommand@@YAHPB_W@Z")]
        extern static private int acedPostCommand(string strExpr);

        /// <summary>
        /// 调用 C++ 的 acedPostCommand 函数
        /// </summary>
        /// <param name="ed">无意义，只是为了定义扩展函数</param>
        /// <param name="expression">要执行的命令字符串</param>
        public static void PostCommand(this Editor ed, string expression)
        {
            acedPostCommand(expression);
        }

        [DllImport("acad.exe", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        extern static private int ads_queueexpr(string strExpr);

        /// <summary>
        /// 调用 C++ 的 ads_queueexpr 函数
        /// </summary>
        /// <param name="ed">无意义，只是为了定义扩展函数</param>
        /// <param name="expression">要执行的命令字符串</param>
        public static void QueueExpression(this Editor ed, string expression)
        {
            ads_queueexpr(expression);
        }

        // 调用 AutoCAD 命令，ARX 原型：int acedCmd(const struct resbuf * rbp);
        [DllImport("acad.exe", EntryPoint = "acedCmd", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl)]
        private extern static int acedCmd(IntPtr rbp);

        /// <summary>
        /// 调用 C++ 的 acedCmd 函数
        /// </summary>
        /// <param name="ed">无意义，只是为了定义扩展函数</param>
        /// <param name="args">命令参数列表</param>
        /// <returns>返回命令执行的状态</returns>
        public static int AcedCmd(this Editor ed, ResultBuffer args)
        {
            // 由于 acedCmd 只能在程序环境下运行，因此需调用此语句
            if (!Application.DocumentManager.IsApplicationContext)
                return acedCmd(args.UnmanagedObject);
            else
                return 0;
        }
    }
}