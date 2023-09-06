using System;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace DotNetARX
{
    /// <summary>
    /// UCS 操作类
    /// </summary>
    public static class UCSTools
    {
        [DllImport("acad.exe", CallingConvention = CallingConvention.Cdecl, EntryPoint = "acedTrans")]
        private static extern int acedTrans(double[] point, IntPtr fromResbuf, IntPtr toResbuf, int displacement,
            double[] result);

        /// <summary>
        /// 坐标系的类型
        /// </summary>
        public enum CoordSystem
        {
            /// <summary>
            /// WCS
            /// </summary>
            WCS = 0,

            /// <summary>
            /// UCS
            /// </summary>
            UCS,

            /// <summary>
            /// DCS
            /// </summary>
            DCS,

            /// <summary>
            /// PSDCS
            /// </summary>
            PSDCS
        }

        /// <summary>
        /// 将点或矢量从一个坐标系转换到另一个坐标系
        /// </summary>
        /// <param name="pt">坐标，表示点或位移矢量</param>
        /// <param name="from">源坐标系</param>
        /// <param name="to">目标坐标系</param>
        /// <param name="disp">位移矢量标记，1 表示位移矢量，0 表示点</param>
        /// <returns>返回转化后的坐标</returns>
        public static Point3d TranslateCoordinates(this Point3d pt, CoordSystem from, CoordSystem to, int disp)
        {
            double[] result = new double[3];
            acedTrans(pt.ToArray(),
                new ResultBuffer(new TypedValue(5003, from)).UnmanagedObject,
                new ResultBuffer(new TypedValue(5003, to)).UnmanagedObject,
                disp, result);
            return new Point3d(result);
        }

        /// <summary>
        /// 将点从一个坐标系转换到另一个坐标系
        /// </summary>
        /// <param name="pt">点</param>
        /// <param name="from">源坐标系</param>
        /// <param name="to">目标坐标系</param>
        /// <returns>返回转化后的坐标</returns>
        public static Point3d TranslateCoordinates(this Point3d pt, CoordSystem from, CoordSystem to)
        {
            return pt.TranslateCoordinates(from, to, 0);
        }

        /// <summary>
        /// 创建一个新的 UCS
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="UCSName">要创建的 UCS 名称</param>
        /// <returns>返回创建的 UCS 的 Id</returns>
        public static ObjectId AddUCS(this Database db, string UCSName)
        {
            var trans = db.TransactionManager;
            // 打开 UCS 表
            UcsTable table = (UcsTable)trans.GetObject(db.UcsTableId, OpenMode.ForRead);
            if (!table.Has(UCSName)) // 如果不存在名为 UCSName 的 UCS，则新建一个 UCS
            {
                // 定义一个新的 UCS
                UcsTableRecord record = new UcsTableRecord();
                record.Name = UCSName; // 设置 UCS 名

                table.UpgradeOpen(); // 切换 UCS 表的状态为写以添加新的 UCS
                // 将 UCS 的信息添加到 UCS 表中
                table.Add(record);

                // 把 UCS 添加到事务处理中
                trans.AddNewlyCreatedDBObject(record, true);
                table.DowngradeOpen(); // 为了安全，将 UCS 表的状态切换为读
            }

            return table[UCSName]; // 返回新添加的 UCS 的 ObjectId
        }

        /// <summary>
        /// 将 UCS 表中已经存在的一个 UCS 设置为当前 UCS
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="UCSName">UCS 名称</param>
        /// <returns>如果设置成功返回 true，否则返回 false</returns>
        public static bool SetCurrentUCS(this Database db, string UCSName)
        {
            var trans = db.TransactionManager;
            Editor ed = db.GetEditor();
            // 打开 UCS 表
            UcsTable table = (UcsTable)trans.GetObject(db.UcsTableId, OpenMode.ForRead);
            // 如果不存在名为 UCSName 的 UCS，则返回
            if (!table.Has(UCSName))
            {
                return false;
            }

            // 打开当前活动的视口为写的状态
            ViewportTableRecord record =
                (ViewportTableRecord)trans.GetObject(db.CurrentViewportTableRecordId(), OpenMode.ForWrite);

            // 对于 2009 以上的版本
            // ViewportTableRecord record = (ViewportTableRecord)trans.GetObject(ed.ActiveViewportId, OpenMode.ForWrite);

            // 设置当前 UCS
            record.SetUcs(table[UCSName]);
            record.DowngradeOpen();

            // 更新视口
            ed.UpdateTiledViewportsFromDatabase();
            return true;
        }

        /// <summary>
        /// 获取当前 UCS
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <returns>返回当前 UCS 的 Id</returns>
        public static ObjectId GetCurrentUCS(this Database db)
        {
            var trans = db.TransactionManager;
            Editor ed = db.GetEditor();

            // 打开 UCS 表
            UcsTable table = (UcsTable)trans.GetObject(db.UcsTableId, OpenMode.ForRead);
            // 打开当前活动的视口
            ViewportTableRecord record =
                (ViewportTableRecord)trans.GetObject(db.CurrentViewportTableRecordId(), OpenMode.ForRead);

            // 返回当前 UCS 的 ObjectId
            return record.UcsName;
        }

        /// <summary>
        /// 设置 UCS 的原点
        /// </summary>
        /// <param name="ucsId">UCS 的 Id</param>
        /// <param name="pt">要设置的 UCS 原点坐标</param>
        public static void SetUCSOrigin(this ObjectId ucsId, Point3d pt)
        {
            Database db = ucsId.Database;
            var trans = db.TransactionManager;

            // 打开 UCS
            UcsTableRecord record = trans.GetObject(ucsId, OpenMode.ForRead) as UcsTableRecord;
            if (record == null)
            {
                // 若 UCS 不存在，则返回
                return;
            }

            record.UpgradeOpen(); // 切换 UCS 为写的状态
            record.Origin = pt; // 设置 UCS 的原点
            record.DowngradeOpen(); // 为了安全，切换 UCS 为读的状态
        }

        /// <summary>
        /// 旋转 UCS
        /// </summary>
        /// <param name="ucsId">UCS 的 Id</param>
        /// <param name="rotateAngle">旋转角度</param>
        /// <param name="rotateAxis">旋转轴</param>
        public static void RotateUCS(this ObjectId ucsId, double rotateAngle, Vector3d rotateAxis)
        {
            Database db = ucsId.Database;
            var trans = db.TransactionManager;

            // 打开 UCS
            UcsTableRecord record = trans.GetObject(ucsId, OpenMode.ForRead) as UcsTableRecord;
            if (record == null)
            {
                // 若 UCS 不存在，则返回
                return;
            }

            record.UpgradeOpen(); // 切换 UCS 为写的状态
            Vector3d xAxis = record.XAxis; // UCS 的 X 轴方向
            Vector3d yAxis = record.YAxis; // UCS 的 Y 轴方向

            // 旋转 UCS
            record.XAxis = xAxis.RotateBy(rotateAngle * Math.PI / 180, rotateAxis);
            record.YAxis = yAxis.RotateBy(rotateAngle * Math.PI / 180, rotateAxis);
            record.DowngradeOpen(); // 为了安全，切换 UCS 为读的状态
        }
    }
}