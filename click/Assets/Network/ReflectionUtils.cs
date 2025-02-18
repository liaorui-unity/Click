
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：反射工具类
// 创建时间：2019-07-30 15:50:41
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace fs
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// 主程序集
        /// </summary>
        public const string AssemblyCSharpName = "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        /// <summary>
        /// Plugins程序集
        /// </summary>
        public const string AssemblyCSharpFirstpassName = "Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        public static Assembly GetCSharpAssembly()
        {
            return Assembly.Load(AssemblyCSharpName);
        }
        public static Assembly GetCSharpFirstpassAssembly()
        {
            return Assembly.Load(AssemblyCSharpFirstpassName);
        }

        /// <summary>
        /// 获得类型
        /// </summary>
        /// <param name="assemblyName">程序集</param>
        /// <param name="fullName">命名空间.类型名</param>
        /// <returns></returns>
        public static System.Type GetType(string assemblyName, string fullName)
        {
            string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
            try
            {
                Type o = Type.GetType(path);//加载类型
                return o;
            }
            catch
            {
                //发生异常
                return null;
            }
        }

        public static T CreateInstance<T>(Type o)
        {
            try
            {
                object obj = Activator.CreateInstance(o, true);//根据类型创建实例
                return (T)obj;//类型转换并返回
            }
            catch
            {
                //发生异常，返回类型的默认值
                return default(T);
            }
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">程序集</param>
        /// <param name="fullName">命名空间.类型名</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string assemblyName, string fullName)
        {
            string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
            try
            {
                Type o = Type.GetType(path);//加载类型
                object obj = Activator.CreateInstance(o, true);//根据类型创建实例
                return (T)obj;//类型转换并返回
            }
            catch
            {
                //发生异常，返回类型的默认值
                return default(T);
            }
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T">要创建对象的类型</typeparam>
        /// <param name="assemblyName">类型所在程序集名称</param>
        /// <param name="nameSpace">类型所在命名空间</param>
        /// <param name="className">类型名</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string assemblyName, string nameSpace, string className)
        {
            string fullName = nameSpace + "." + className;//命名空间.类型名
            try
            {
                //此为第一种写法
                object ect = Assembly.Load(assemblyName).CreateInstance(fullName);//加载程序集，创建程序集里面的 命名空间.类型名 实例
                return (T)ect;//类型转换并返回
                //下面是第二种写法
                //string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
                //Type o = Type.GetType(path);//加载类型
                //object obj = Activator.CreateInstance(o, true);//根据类型创建实例
                //return (T)obj;//类型转换并返回
            }
            catch
            {
                //发生异常，返回类型的默认值
                return default(T);
            }
        }
    }
}
