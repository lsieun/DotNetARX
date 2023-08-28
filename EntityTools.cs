using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace DotNetARX
{
    public static class EntityTools
    {
        /// <summary>
        /// 移动实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="sourcePoint">移动的源点</param>
        /// <param name="targetPoint">移动的目标点</param>
        public static void Move(this ObjectId id, Point3d sourcePoint, Point3d targetPoint)
        {
            //构建用于移动实体的矩阵
            Vector3d vector = targetPoint.GetVectorTo(sourcePoint);
            Matrix3d transform = Matrix3d.Displacement(vector);

            //以写的方式打开id表示的实体对象
            Entity entity = (Entity)id.GetObject(OpenMode.ForWrite);
            entity.TransformBy(transform); //对实体实施移动
            entity.DowngradeOpen(); //为防止错误，切换实体为读的状态
        }

        /// <summary>
        /// 移动实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="sourcePoint">移动的源点</param>
        /// <param name="targetPoint">移动的目标点</param>
        public static void Move(this Entity entity, Point3d sourcePoint, Point3d targetPoint)
        {
            if (entity.IsNewObject) // 如果是还未被添加到数据库中的新实体
            {
                // 构建用于移动实体的矩阵
                Vector3d vector = targetPoint.GetVectorTo(sourcePoint);
                Matrix3d transform = Matrix3d.Displacement(vector);
                entity.TransformBy(transform); //对实体实施移动
            }
            else // 如果是已经添加到数据库中的实体
            {
                entity.ObjectId.Move(sourcePoint, targetPoint);
            }
        }


        /// <summary>
        /// 复制实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="sourcePoint">复制的源点</param>
        /// <param name="targetPoint">复制的目标点</param>
        /// <returns>返回复制实体的ObjectId</returns>
        public static ObjectId Copy(this ObjectId id, Point3d sourcePoint, Point3d targetPoint)
        {
            //构建用于复制实体的矩阵
            Vector3d vector = targetPoint.GetVectorTo(sourcePoint);
            Matrix3d transform = Matrix3d.Displacement(vector);

            //获取id表示的实体对象
            Entity entity = (Entity)id.GetObject(OpenMode.ForRead);
            //获取实体的拷贝
            Entity entityCopy = entity.GetTransformedCopy(transform);

            //将复制的实体对象添加到模型空间
            ObjectId copyId = id.Database.AddToModelSpace(entityCopy);
            return copyId; //返回复制实体的ObjectId
        }

        /// <summary>
        /// 复制实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="sourcePoint">复制的源点</param>
        /// <param name="targetPoint">复制的目标点</param>
        /// <returns>返回复制实体的ObjectId</returns>
        public static ObjectId Copy(this Entity entity, Point3d sourcePoint, Point3d targetPoint)
        {
            ObjectId copyId;
            if (entity.IsNewObject) // 如果是还未被添加到数据库中的新实体
            {
                //构建用于复制实体的矩阵
                Vector3d vector = targetPoint.GetVectorTo(sourcePoint);
                Matrix3d transform = Matrix3d.Displacement(vector);

                //获取实体的拷贝
                Entity entCopy = entity.GetTransformedCopy(transform);
                //将复制的实体对象添加到模型空间
                copyId = entity.Database.AddToModelSpace(entCopy);
            }
            else
            {
                copyId = entity.ObjectId.Copy(sourcePoint, targetPoint);
            }

            return copyId;
        }

        /// <summary>
        /// 旋转实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="basePoint">旋转基点</param>
        /// <param name="angle">旋转角度</param>
        public static void Rotate(this ObjectId id, Point3d basePoint, double angle)
        {
            Matrix3d transform = Matrix3d.Rotation(angle, Vector3d.ZAxis, basePoint);
            Entity entity = (Entity)id.GetObject(OpenMode.ForWrite);
            entity.TransformBy(transform);
            entity.DowngradeOpen();
        }

        /// <summary>
        /// 旋转实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="basePoint">旋转基点</param>
        /// <param name="angle">旋转角度</param>
        public static void Rotate(this Entity entity, Point3d basePoint, double angle)
        {
            if (entity.IsNewObject)
            {
                Matrix3d transform = Matrix3d.Rotation(angle, Vector3d.ZAxis, basePoint);
                entity.TransformBy(transform);
            }
            else
            {
                entity.ObjectId.Rotate(basePoint, angle);
            }
        }

        /// <summary>
        /// 缩放实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="basePoint">缩放基点</param>
        /// <param name="scaleFactor">缩放比例</param>
        public static void Scale(this ObjectId id, Point3d basePoint, double scaleFactor)
        {
            Matrix3d transform = Matrix3d.Scaling(scaleFactor, basePoint);
            Entity entity = (Entity)id.GetObject(OpenMode.ForWrite);
            entity.TransformBy(transform);
            entity.DowngradeOpen();
        }

        /// <summary>
        /// 缩放实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="basePoint">缩放基点</param>
        /// <param name="scaleFactor">缩放比例</param>
        public static void Scale(this Entity entity, Point3d basePoint, double scaleFactor)
        {
            if (entity.IsNewObject)
            {
                Matrix3d transform = Matrix3d.Scaling(scaleFactor, basePoint);
                entity.TransformBy(transform);
            }
            else
            {
                entity.ObjectId.Scale(basePoint, scaleFactor);
            }
        }

        /// <summary>
        /// 镜像实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="mirrorPoint1">镜像轴的第一点</param>
        /// <param name="mirrorPoint2">镜像轴的第二点</param>
        /// <param name="eraseSourceObject">是否删除源对象</param>
        /// <returns>返回镜像实体的ObjectId</returns>
        public static ObjectId Mirror(this ObjectId id,
            Point3d mirrorPoint1, Point3d mirrorPoint2,
            bool eraseSourceObject)
        {
            Line3d mirrorLine = new Line3d(mirrorPoint1, mirrorPoint2); //镜像线
            Matrix3d transform = Matrix3d.Mirroring(mirrorLine); //镜像矩阵

            ObjectId mirrorId = id;
            Entity entity = (Entity)id.GetObject(OpenMode.ForWrite);

            //如果删除源对象，则直接对源对象实行镜像变换
            if (eraseSourceObject)
            {
                entity.TransformBy(transform);
            }
            else //如果不删除源对象，则镜像复制源对象
            {
                Entity entityCopy = entity.GetTransformedCopy(transform);
                mirrorId = id.Database.AddToModelSpace(entityCopy);
            }

            return mirrorId;
        }

        /// <summary>
        /// 镜像实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="mirrorPoint1">镜像轴的第一点</param>
        /// <param name="mirrorPoint2">镜像轴的第二点</param>
        /// <param name="eraseSourceObject">是否删除源对象</param>
        /// <returns>返回镜像实体的ObjectId</returns>
        public static ObjectId Mirror(this Entity entity,
            Point3d mirrorPoint1, Point3d mirrorPoint2,
            bool eraseSourceObject)
        {
            Line3d mirrorLine = new Line3d(mirrorPoint1, mirrorPoint2); //镜像线
            Matrix3d transform = Matrix3d.Mirroring(mirrorLine); //镜像矩阵

            ObjectId mirrorId = ObjectId.Null;
            if (entity.IsNewObject)
            {
                //如果删除源对象，则直接对源对象实行镜像变换
                if (eraseSourceObject)
                {
                    entity.TransformBy(transform);
                }
                else //如果不删除源对象，则镜像复制源对象
                {
                    Entity entCopy = entity.GetTransformedCopy(transform);
                    mirrorId = entity.Database.AddToModelSpace(entCopy);
                }
            }
            else
            {
                mirrorId = entity.ObjectId.Mirror(mirrorPoint1, mirrorPoint2, eraseSourceObject);
            }

            return mirrorId;
        }

        /// <summary>
        /// 偏移实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="distance">偏移距离</param>
        /// <returns>返回偏移后的实体Id集合</returns>
        public static ObjectIdCollection Offset(this ObjectId id, double distance)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            Curve cur = id.GetObject(OpenMode.ForWrite) as Curve;
            if (cur != null)
            {
                try
                {
                    //获取偏移的对象集合
                    DBObjectCollection offsetCurves = cur.GetOffsetCurves(distance);
                    //将对象集合类型转换为实体类的数组，以方便加入实体的操作
                    Entity[] entities = new Entity[offsetCurves.Count];
                    offsetCurves.CopyTo(entities, 0);
                    //将偏移的对象加入到数据库
                    ids = id.Database.AddToModelSpace(entities);
                }
                catch
                {
                    Application.ShowAlertDialog("无法偏移！");
                }
            }
            else
            {
                Application.ShowAlertDialog("无法偏移！");
            }

            return ids; //返回偏移后的实体Id集合
        }

        /// <summary>
        /// 偏移实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="distance">偏移距离</param>
        /// <returns>返回偏移后的实体集合</returns>
        public static DBObjectCollection Offset(this Entity entity, double distance)
        {
            DBObjectCollection offsetCurves = new DBObjectCollection();
            Curve cur = entity as Curve;
            if (cur != null)
            {
                try
                {
                    offsetCurves = cur.GetOffsetCurves(distance);
                    Entity[] offsetEnts = new Entity[offsetCurves.Count];
                    offsetCurves.CopyTo(offsetEnts, 0);
                }
                catch
                {
                    Application.ShowAlertDialog("无法偏移！");
                }
            }
            else
                Application.ShowAlertDialog("无法偏移！");

            return offsetCurves;
        }

        /// <summary>
        /// 矩形阵列实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="numRows">矩形阵列的行数,该值必须为正数</param>
        /// <param name="numCols">矩形阵列的列数,该值必须为正数</param>
        /// <param name="disRows">行间的距离</param>
        /// <param name="disCols">列间的距离</param>
        /// <returns>返回阵列后的实体集合的ObjectId</returns>
        public static ObjectIdCollection ArrayRectang(this ObjectId id,
            int numRows, int numCols,
            double disRows, double disCols)
        {
            // 用于返回阵列后的实体集合的ObjectId
            ObjectIdCollection ids = new ObjectIdCollection();
            // 以读的方式打开实体
            Entity ent = (Entity)id.GetObject(OpenMode.ForRead);
            for (int m = 0; m < numRows; m++)
            {
                for (int n = 0; n < numCols; n++)
                {
                    // 获取平移矩阵
                    Matrix3d mt = Matrix3d.Displacement(new Vector3d(n * disCols, m * disRows, 0));
                    Entity entCopy = ent.GetTransformedCopy(mt); // 复制实体
                    // 将复制的实体添加到模型空间
                    ObjectId entCopyId = id.Database.AddToModelSpace(entCopy);
                    ids.Add(entCopyId); // 将复制实体的ObjectId添加到集合中
                }
            }

            ent.UpgradeOpen(); // 切换实体为写的状态
            ent.Erase(); // 删除实体
            return ids; // 返回阵列后的实体集合的ObjectId
        }

        /// <summary>
        /// 环形阵列实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        /// <param name="centerPoint">环形阵列的中心点</param>
        /// <param name="numObj">在环形阵列中所要创建的对象数量</param>
        /// <param name="angle">以弧度表示的填充角度，正值表示逆时针方向旋转，负值表示顺时针方向旋转，如果角度为0则出错</param>
        /// <returns>返回阵列后的实体集合的ObjectId</returns>
        public static ObjectIdCollection ArrayPolar(this ObjectId id, Point3d centerPoint, int numObj, double angle)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            Entity entity = (Entity)id.GetObject(OpenMode.ForRead);
            for (int i = 0; i < numObj - 1; i++)
            {
                Matrix3d mt = Matrix3d.Rotation(angle * (i + 1) / numObj, Vector3d.ZAxis, centerPoint);
                Entity entityCopy = entity.GetTransformedCopy(mt);
                ObjectId entityCopyId = id.Database.AddToModelSpace(entityCopy);
                ids.Add(entityCopyId);
            }

            return ids;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体的ObjectId</param>
        public static void Erase(this ObjectId id)
        {
            DBObject entity = id.GetObject(OpenMode.ForWrite);
            entity.Erase();
        }

        /// <summary>
        /// 计算图形数据库模型空间中所有实体的包围框
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <returns>返回模型空间中所有实体的包围框</returns>
        public static Extents3d GetAllEntsExtent(this Database db)
        {
            Extents3d totalExt = new Extents3d();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btRcd =
                    (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                foreach (ObjectId entId in btRcd)
                {
                    Entity ent = trans.GetObject(entId, OpenMode.ForRead) as Entity;
                    totalExt.AddExtents(ent.GeometricExtents);
                }
            }

            return totalExt;
        }
    }
}