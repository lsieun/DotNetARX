using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace DotNetARX
{
    /// <summary>
    /// TypedValue 列表类，简化选择集过滤器的构造
    /// </summary>
    public class TypedValueList : List<TypedValue>
    {
        /// <summary>
        /// 接受可变参数的构造函数
        /// </summary>
        /// <param name="args">TypedValue 对象</param>
        public TypedValueList(params TypedValue[] args)
        {
            AddRange(args);
        }

        /// <summary>
        /// 添加 DXF 组码及对应的类型
        /// </summary>
        /// <param name="typecode">DXF 组码</param>
        /// <param name="value">类型</param>
        public void Add(int typecode, object value)
        {
            base.Add(new TypedValue(typecode, value));
        }

        /// <summary>
        /// 添加 DXF 组码
        /// </summary>
        /// <param name="typecode">DXF 组码</param>
        public void Add(int typecode)
        {
            base.Add(new TypedValue(typecode));
        }

        /// <summary>
        /// 添加 DXF 组码及对应的类型
        /// </summary>
        /// <param name="typecode">DXF 组码</param>
        /// <param name="value">类型</param>
        public void Add(DxfCode typecode, object value)
        {
            base.Add(new TypedValue((int)typecode, value));
        }

        /// <summary>
        /// 添加 DXF 组码
        /// </summary>
        /// <param name="typecode">DXF 组码</param>
        public void Add(DxfCode typecode)
        {
            base.Add(new TypedValue((int)typecode));
        }

        /// <summary>
        /// 添加图元类型,DXF 组码缺省为 0
        /// </summary>
        /// <param name="entityType">图元类型</param>
        public void Add(Type entityType)
        {
            base.Add(new TypedValue(0, RXClass.GetClass(entityType).DxfName));
        }

        /// <summary>
        /// TypedValueList 隐式转换为 SelectionFilter
        /// </summary>
        /// <param name="src">要转换的 TypedValueList 对象</param>
        /// <returns>返回对应的 SelectionFilter 类对象</returns>
        public static implicit operator SelectionFilter(TypedValueList src)
        {
            return src != null ? new SelectionFilter(src) : null;
        }

        /// <summary>
        /// TypedValueList 隐式转换为 ResultBuffer
        /// </summary>
        /// <param name="src">要转换的 TypedValueList 对象</param>
        /// <returns>返回对应的 ResultBuffer 对象</returns>
        public static implicit operator ResultBuffer(TypedValueList src)
        {
            return src != null ? new ResultBuffer(src) : null;
        }

        /// <summary>
        /// TypedValueList 隐式转换为 TypedValue 数组
        /// </summary>
        /// <param name="src">要转换的 TypedValueList 对象</param>
        /// <returns>返回对应的 TypedValue 数组</returns>
        public static implicit operator TypedValue[](TypedValueList src)
        {
            return src != null ? src.ToArray() : null;
        }

        /// <summary>
        /// TypedValue 数组隐式转换为 TypedValueList
        /// </summary>
        /// <param name="src">要转换的 TypedValue 数组</param>
        /// <returns>返回对应的 TypedValueList</returns>
        public static implicit operator TypedValueList(TypedValue[] src)
        {
            return src != null ? new TypedValueList(src) : null;
        }

        /// <summary>
        /// SelectionFilter 隐式转换为 TypedValueList
        /// </summary>
        /// <param name="src">要转换的 SelectionFilter</param>
        /// <returns>返回对应的 TypedValueList</returns>
        public static implicit operator TypedValueList(SelectionFilter src)
        {
            return src != null ? new TypedValueList(src.GetFilter()) : null;
        }

        /// <summary>
        /// ResultBuffer 隐式转换为 TypedValueList
        /// </summary>
        /// <param name="src">要转换的 ResultBuffer</param>
        /// <returns>返回对应的 TypedValueList</returns>
        public static implicit operator TypedValueList(ResultBuffer src)
        {
            return src != null ? new TypedValueList(src.AsArray()) : null;
        }
    }

    /// <summary>
    /// Point3d 列表类
    /// </summary>
    public class Point3dList : List<Point3d>
    {
        /// <summary>
        /// 接受可变参数的构造函数
        /// </summary>
        /// <param name="args">Point3d 类对象</param>
        public Point3dList(params Point3d[] args)
        {
            AddRange(args);
        }

        /// <summary>
        /// Point3dList 隐式转换为 Point3d 数组
        /// </summary>
        /// <param name="src">要转换的 Point3dList 对象</param>
        /// <returns>返回对应的 Point3d 数组</returns>
        public static implicit operator Point3d[](Point3dList src)
        {
            return src != null ? src.ToArray() : null;
        }

        /// <summary>
        /// Point3dList 隐式转换为 Point3dCollection
        /// </summary>
        /// <param name="src">要转换的 Point3dList 对象</param>
        /// <returns>返回对应的 Point3dCollection</returns>
        public static implicit operator Point3dCollection(Point3dList src)
        {
            return src != null ? new Point3dCollection(src) : null;
        }

        /// <summary>
        /// Point3d 数组隐式转换为 Point3dList
        /// </summary>
        /// <param name="src">要转换的 Point3d 数组</param>
        /// <returns>返回对应的 Point3dList</returns>
        public static implicit operator Point3dList(Point3d[] src)
        {
            return src != null ? new Point3dList(src) : null;
        }

        /// <summary>
        /// Point3dCollection 隐式转换为 Point3dList
        /// </summary>
        /// <param name="src">要转换的 Point3dCollection</param>
        /// <returns>返回对应的 Point3dList</returns>
        public static implicit operator Point3dList(Point3dCollection src)
        {
            if (src != null)
            {
                Point3d[] ids = new Point3d[src.Count];
                src.CopyTo(ids, 0);
                return new Point3dList(ids);
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// ObjectId 列表
    /// </summary>
    public class ObjectIdList : List<ObjectId>
    {
        /// <summary>
        /// 接受可变参数的构造函数
        /// </summary>
        /// <param name="args">ObjectId 对象</param>
        public ObjectIdList(params ObjectId[] args)
        {
            AddRange(args);
        }

        /// <summary>
        /// ObjectIdList 隐式转换为 ObjectId 数组
        /// </summary>
        /// <param name="src">要转换的 ObjectIdList 对象</param>
        /// <returns>返回对应的 ObjectId 数组</returns>
        public static implicit operator ObjectId[](ObjectIdList src)
        {
            return src != null ? src.ToArray() : null;
        }

        /// <summary>
        /// ObjectIdList 隐式转换为 ObjectIdCollection
        /// </summary>
        /// <param name="src">要转换的 ObjectIdList 对象</param>
        /// <returns>返回对应的 ObjectIdCollection</returns>
        public static implicit operator ObjectIdCollection(ObjectIdList src)
        {
            return src != null ? new ObjectIdCollection(src) : null;
        }

        /// <summary>
        /// ObjectId 数组隐式转换为 ObjectIdList
        /// </summary>
        /// <param name="src">要转换的 ObjectId 数组</param>
        /// <returns>返回对应的 ObjectIdList</returns>
        public static implicit operator ObjectIdList(ObjectId[] src)
        {
            return src != null ? new ObjectIdList(src) : null;
        }

        /// <summary>
        /// ObjectIdCollection 隐式转换为 ObjectIdList
        /// </summary>
        /// <param name="src">要转换的 ObjectIdCollection</param>
        /// <returns>返回对应的 ObjectIdList</returns>
        public static implicit operator ObjectIdList(ObjectIdCollection src)
        {
            if (src != null)
            {
                ObjectId[] ids = new ObjectId[src.Count];
                src.CopyTo(ids, 0);
                return new ObjectIdList(ids);
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Entity 列表
    /// </summary>
    public class EntityList : List<Entity>
    {
        /// <summary>
        /// 接受可变参数的构造函数
        /// </summary>
        /// <param name="args">实体对象</param>
        public EntityList(params Entity[] args)
        {
            AddRange(args);
        }

        /// <summary>
        /// EntityList 隐式转换为 Entity 数组
        /// </summary>
        /// <param name="src">要转换的 EntityList</param>
        /// <returns>返回对应的 Entity 数组</returns>
        public static implicit operator Entity[](EntityList src)
        {
            return src != null ? src.ToArray() : null;
        }

        /// <summary>
        /// Entity 数组隐式转换为 EntityList
        /// </summary>
        /// <param name="src">要转换的 Entity 数组</param>
        /// <returns>返回对应的 EntityList</returns>
        public static implicit operator EntityList(Entity[] src)
        {
            return src != null ? new EntityList(src) : null;
        }
    }
}