using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// 属性转换类，将一个类的属性值转换给另外一个类的同名属性，注意该类使用的是浅表复制。
    /// <example>
    ///        下面几种用法一样:
    ///        ModuleCast.GetCast(typeof(CarInfo), typeof(ImplCarInfo)).Cast(info, ic);
    ///        ModuleCast.CastObject《CarInfo, ImplCarInfo》(info, ic);
    ///        ModuleCast.CastObject(info, ic);
    ///
    ///        ImplCarInfo icResult= info.CopyTo《ImplCarInfo》(null);
    ///
    ///        ImplCarInfo icResult2 = new ImplCarInfo();
    ///        info.CopyTo《ImplCarInfo》(icResult2);
    /// 
    /// </example>
    /// </summary>
    public class ModuleCast
    {
        private readonly List<CastProperty> mProperties = new List<CastProperty>();

        static readonly Dictionary<Type, Dictionary<Type, ModuleCast>> mCasters = new Dictionary<Type, Dictionary<Type, ModuleCast>>(256);

        private static Dictionary<Type, ModuleCast> GetModuleCast(Type sourceType)
        {
            Dictionary<Type, ModuleCast> result;
            lock (mCasters)
            {
                if (!mCasters.TryGetValue(sourceType, out result))
                {
                    result = new Dictionary<Type, ModuleCast>(8);
                    mCasters.Add(sourceType, result);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取要转换的当前转换类实例
        /// </summary>
        /// <param name="sourceType">要转换的源类型</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static ModuleCast GetCast(Type sourceType, Type targetType)
        {
            Dictionary<Type, ModuleCast> casts = GetModuleCast(sourceType);
            ModuleCast result;
            lock (casts)
            {
                if (!casts.TryGetValue(targetType, out result))
                {
                    result = new ModuleCast(sourceType, targetType);
                    casts.Add(targetType, result);
                }
            }
            return result;
        }

        /// <summary>
        /// 以两个要转换的类型作为构造函数，构造一个对应的转换类
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        public ModuleCast(Type sourceType, Type targetType)
        {
            PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo sp in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (PropertyInfo tp in targetProperties)
                {
                    if (sp.Name.Equals(tp.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //if (sp.PropertyType == tp.PropertyType)
                        //{
                        CastProperty cp = new CastProperty();
                        cp.SourceProperty = new PropertyAccessorHandler(sp);
                        cp.TargetProperty = new PropertyAccessorHandler(tp);
                        mProperties.Add(cp);
                        break;
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// 将源类型的属性值转换给目标类型同名的属性
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Cast(object source, object target,bool isSkipNull=false)
        {
            Cast(source, target, null,isSkipNull);
        }

        /// <summary>
        /// 将源类型的属性值转换给目标类型同名的属性，排除要过滤的属性名称
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="filter">要过滤的属性名称</param>
        /// <param name="isSkipNull">是否跳过源类型中null值的赋值</param>
        public void Cast(object source, object target, string[] filter,bool isSkipNull=false)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (target == null)
                throw new ArgumentNullException("target");

            for (int i = 0; i < mProperties.Count; i++)
            {
                CastProperty cp = mProperties[i];
                if (cp.SourceProperty.Getter != null)
                {
                    object Value = cp.SourceProperty.Getter(source, null); //PropertyInfo.GetValue(source,null);
                    if (isSkipNull && Value == null)
                    {
                        continue;
                    }
                    if (cp.TargetProperty.Setter != null)
                    {
                        if (filter == null || !filter.Contains(cp.TargetProperty.PropertyName))
                        {
                            if (cp.SourceProperty.PInfo.PropertyType != cp.TargetProperty.PInfo.PropertyType)
                            {
                                if (cp.SourceProperty.PInfo.PropertyType == typeof(DateTime))
                                {
                                    var temp = DateTime.Parse(Value.ToString());
                                    cp.TargetProperty.Setter(target, temp.ToString("yyyy-MM-dd HH:mm:ss"), null);
                                }
                                else if (Value != null && cp.SourceProperty.PInfo.PropertyType == typeof(DateTime?))
                                {
                                    var temp = DateTime.Parse(Value.ToString());
                                    cp.TargetProperty.Setter(target, temp.ToString("yyyy-MM-dd HH:mm:ss"), null);
                                }
                                else if (Value != null && (cp.SourceProperty.PInfo.PropertyType == typeof(decimal) || cp.SourceProperty.PInfo.PropertyType == typeof(decimal?)))
                                {
                                    var temp = decimal.Parse(Value.ToString());
                                    cp.TargetProperty.Setter(target, temp.ToString(), null);
                                }
                                else if (Value != null && (cp.SourceProperty.PInfo.PropertyType == typeof(double) || cp.SourceProperty.PInfo.PropertyType == typeof(double?)))
                                {
                                    var temp = double.Parse(Value.ToString());
                                    cp.TargetProperty.Setter(target, temp.ToString(), null);
                                }
                                else if (Value != null && (cp.SourceProperty.PInfo.PropertyType == typeof(int) || cp.SourceProperty.PInfo.PropertyType == typeof(int?)))
                                {
                                    var temp = int.Parse(Value.ToString());
                                    cp.TargetProperty.Setter(target, temp.ToString(), null);
                                }
                                else if (Value != null && (cp.TargetProperty.PInfo.PropertyType == typeof(DateTime) || cp.TargetProperty.PInfo.PropertyType == typeof(DateTime?)))
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(Value.ToString(), out temp))
                                    {
                                        cp.TargetProperty.Setter(target, temp, null);
                                    }
                                }
                                else if (Value != null && (cp.TargetProperty.PInfo.PropertyType == typeof(decimal) || cp.TargetProperty.PInfo.PropertyType == typeof(decimal?)))
                                {
                                    decimal temp;
                                    if (decimal.TryParse(Value.ToString(), out temp))
                                    {
                                        cp.TargetProperty.Setter(target, temp, null);
                                    }
                                }
                                else if (Value != null && (cp.TargetProperty.PInfo.PropertyType == typeof(double) || cp.TargetProperty.PInfo.PropertyType == typeof(double?)))
                                {
                                    double temp;
                                    if (double.TryParse(Value.ToString(), out temp))
                                    {
                                        cp.TargetProperty.Setter(target, temp, null);
                                    }
                                }
                                else if (Value != null && (cp.TargetProperty.PInfo.PropertyType == typeof(float) || cp.TargetProperty.PInfo.PropertyType == typeof(float?)))
                                {
                                    float temp;
                                    if (float.TryParse(Value.ToString(), out temp))
                                    {
                                        cp.TargetProperty.Setter(target, temp, null);
                                    }
                                }
                                else if (Value != null && (cp.TargetProperty.PInfo.PropertyType == typeof(int) || cp.TargetProperty.PInfo.PropertyType == typeof(int?)))
                                {
                                    int temp;
                                    if (int.TryParse(Value.ToString(), out temp))
                                    {
                                        cp.TargetProperty.Setter(target, temp, null);
                                    }
                                }
                            }
                            else
                            {
                                cp.TargetProperty.Setter(target, Value, null);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        public static void CastObject<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            ModuleCast.GetCast(typeof(TSource), typeof(TTarget)).Cast(source, target);
        }

        public static List<TTarget> CastObjectList<TSource, TTarget>(List<TSource> source)
            where TSource : class
            where TTarget : class,new() 
        {
            List<TTarget> targetList = new List<TTarget>();
            foreach (var itemsource in source)
            {
                TTarget target=new TTarget();
                GetCast(typeof(TSource), typeof(TTarget)).Cast(itemsource, target);
                targetList.Add(target);
            }
            return targetList;
        }

        /// <summary>
        /// 转换对象
        /// 从源对象转到目标对象时,会跳过源对象中null的值
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="isSkipNull"></param>
        public static void CastObjectSkipNull<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            ModuleCast.GetCast(typeof(TSource), typeof(TTarget)).Cast(source, target,true);
        }

        /// <summary>
        /// 转换属性对象
        /// </summary>
        public class CastProperty
        {
            public PropertyAccessorHandler SourceProperty
            {
                get;
                set;
            }

            public PropertyAccessorHandler TargetProperty
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 属性访问器
        /// </summary>
        public class PropertyAccessorHandler
        {
            public PropertyAccessorHandler(PropertyInfo propInfo)
            {
                this.PInfo = propInfo;
                this.PropertyName = propInfo.Name;
                //var obj = Activator.CreateInstance(classType);
                //var getterType = typeof(FastPropertyAccessor.GetPropertyValue<>).MakeGenericType(propInfo.PropertyType);
                //var setterType = typeof(FastPropertyAccessor.SetPropertyValue<>).MakeGenericType(propInfo.PropertyType);

                //this.Getter = Delegate.CreateDelegate(getterType, null, propInfo.GetGetMethod());
                //this.Setter = Delegate.CreateDelegate(setterType, null, propInfo.GetSetMethod());
                if (propInfo.CanRead)
                    this.Getter = propInfo.GetValue;

                if (propInfo.CanWrite)
                    this.Setter = propInfo.SetValue;

            }
            public PropertyInfo PInfo { get; set; }
            public string PropertyName { get; set; }
            public Func<object, object[], object> Getter { get; private set; }
            public Action<object, object, object[]> Setter { get; private set; }
        }
    }
}
