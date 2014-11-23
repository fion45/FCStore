using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace FCStore.Models
{
    public static class ArrayExtension
    {
        public static string ToMyString<T>(this T[] Array)//扩建的方法必须是静态方法，参数里面必须含有this关键字，this关键字后面的类型为需要扩展的类型
        {
            StringBuilder result = new StringBuilder();
            foreach (T obj in Array)
            {
                result.Append(obj.ToString() + ",");
            }
            if (result.Length > 1)
                result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
    }
    public static class EnumerableExtension
    {
        public static string ToMyString<T>(this IEnumerable<T> List)//扩建的方法必须是静态方法，参数里面必须含有this关键字，this关键字后面的类型为需要扩展的类型
        {
            StringBuilder result = new StringBuilder();
            foreach (T obj in List)
            {
                result.Append(obj.ToString() + ",");
            }
            if(result.Length > 1)
                result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
    }
}