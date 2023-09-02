using Autodesk.AutoCAD.DatabaseServices;

namespace DotNetARX
{
    public static class LineTypeTools
    {
        /// <summary>
        /// 用于形定义文件的图案
        /// </summary>
        public enum Shape
        {
            /// <summary>
            /// 竖线
            /// </summary>
            Track = 130,

            /// <summary>
            /// Z字型
            /// </summary>
            Zig,

            /// <summary>
            /// 矩形
            /// </summary>
            Box,

            /// <summary>
            /// 圆
            /// </summary>
            Circle,

            /// <summary>
            /// S型
            /// </summary>
            Bat
        }

        /// <summary>
        /// 创建一个新的线型
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="typeName">线型名</param>
        /// <returns>返回新建线型的Id</returns>
        public static ObjectId AddLineType(this Database db, string typeName)
        {
            // 打开线型表
            LinetypeTable table = (LinetypeTable)db.LinetypeTableId.GetObject(OpenMode.ForRead);
            if (!table.Has(typeName)) // 如果存在名为typeName的线型表记录
            {
                table.UpgradeOpen(); // 切换线型表为写

                // 新建一个线型表记录
                LinetypeTableRecord record = new LinetypeTableRecord();
                record.Name = typeName; // 设置线型表记录的名称

                table.Add(record); // 将新的线型表记录的信息添加到的线型表中
                db.TransactionManager.AddNewlyCreatedDBObject(record, true);

                // 为了安全，将线型表的状态切换为读
                table.DowngradeOpen();
            }

            return table[typeName]; // 返回新添加的线型表记录的ObjectId
        }

        /// <summary>
        /// 从“acadiso.lin”线型文件中装载指定线型
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="typeName">线型名</param>
        /// <returns>返回装载的线型的Id</returns>
        public static ObjectId LoadLineType(this Database db, string typeName)
        {
            // 打开线型表
            LinetypeTable table = (LinetypeTable)db.LinetypeTableId.GetObject(OpenMode.ForRead);
            if (!table.Has(typeName)) // 如果不存在名为typeName的线型
            {
                // 加载 typeName 线型
                db.LoadLineTypeFile(typeName, "acad.lin");
            }

            return table[typeName]; // 返回加载的线型的ObjectId
        }

        /// <summary>
        /// 设置当前线型
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="typeName">要设置的线型名</param>
        public static void SetCurrentLineType(this Database db, string typeName)
        {
            var trans = db.TransactionManager;
            // 打开线型表
            LinetypeTable table = (LinetypeTable)trans.GetObject(db.LinetypeTableId, OpenMode.ForRead);
            if (table.Has(typeName)) // 如果存在名为 typeName 的线型
            {
                db.Celtype = table[typeName]; // 设置当前线型
            }
        }
    }
}