using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Collections;

namespace FCStore.Common
{

    public delegate string MemberToStringDG(object obj);

    public static class MTSHelper
    {
        public static string ListToString(object obj)
        {
            IEnumerable list = obj as IEnumerable;
            StringBuilder tmpSB = new StringBuilder();
            foreach (object item in list)
            {
                tmpSB.Append(item.ToString() + ",");
            }
            string result = tmpSB.ToString();
            result.TrimEnd(new char[] { ',' });
            return result;
        }
    }
}