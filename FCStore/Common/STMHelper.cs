using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Common
{

    public delegate object StringToMemberDG(string str);
    public static class STMHelper
    {
        //public static object StringToList(string str)
        //{
        //    IEnumerable list = obj as IEnumerable;
        //    StringBuilder tmpSB = new StringBuilder();
        //    foreach (object item in list)
        //    {
        //        tmpSB.Append(item.ToString() + ",");
        //    }
        //    string result = tmpSB.ToString();
        //    result.TrimEnd(new char[] { ',' });
        //    return result;
        //}
    }
}