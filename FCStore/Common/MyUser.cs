using System;
using System.Security.Principal;
using FCStore.Models;
using System.Collections.Generic;

namespace FCStore.Common
{
    public class MyUser : IPrincipal, IIdentity
    {
        public bool IsGuest;

        public int UID;

        public string UserName;

        public string RIDArrStr;

        public string RNameArrStr;

        public string Permission;

        public string SmallUserHead
        {
            get
            {
                string tmpStr = "00000000" + Convert.ToString(UID, 16);
                tmpStr = tmpStr.Substring(tmpStr.Length - 8);
                return "/picture/user/" + tmpStr + "_40_40.jpg";
            }
        }

        public string UserHead
        {
            get
            {
                string tmpStr = "00000000" + Convert.ToString(UID, 16);
                tmpStr = tmpStr.Substring(tmpStr.Length - 8);
                return "/picture/user/" + tmpStr + "_100_100.jpg";
            }
        }

        public MyUser(int uid,string UName, string RStr,string RNStr,string permission)
        {
            UID = uid;
            UserName = UName;
            IsGuest = string.IsNullOrEmpty(UserName);
            RIDArrStr = RStr;
            RNameArrStr = RNStr;
            Permission = permission;
        }

        #region IIdentity Members
        public string AuthenticationType
        {
            get
            {
                return "Froms";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return !IsGuest;
            }
        }

        string IIdentity.Name
        {
            get
            {
                return IsGuest ? "Guest" : UserName;
            }
        }
        #endregion

        #region IPrincipal Members
        public IIdentity Identity
        {
            get
            {
                return this;
            }
        }

        public bool IsInRole(string roleName)
        {
            return RNameArrStr.IndexOf("," + roleName + ",") > -1;
        }

        public bool IsInRoleID(int RoleID)
        {
            return RIDArrStr.IndexOf("," + RoleID.ToString() + ",") > -1;
        }
        #endregion

        public bool HavePermissionInAction(string controller, string action)
        {
            return Permission.IndexOf("," + controller + ",") > -1 || Permission.IndexOf("," + controller + "." + action + ",") > -1;
        }

        public bool HavePermission(string permissionStr)
        {
            return Permission.IndexOf("," + permissionStr + ",") > -1;
        }

        public bool HaveDeny(string controller, string action)
        {
            return Permission.IndexOf(",!" + controller + ",") > -1 || Permission.IndexOf(",!" + controller + "." + action + ",") > -1;
        }
    }
}