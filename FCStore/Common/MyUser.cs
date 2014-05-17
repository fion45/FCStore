using System.Security.Principal;
using FCStore.Models;
using System.Collections.Generic;

namespace FCStore.Common
{
    public class MyUser : IPrincipal, IIdentity
    {
        private bool IsGuest;

        private int UID;

        private string UserName;

        private string RIDArrStr;

        private string RNameArrStr;

        private string Permission;

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


    }
}