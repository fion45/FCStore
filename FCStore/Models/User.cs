using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FCStore.Models
{
    [Serializable]
    public class User
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UID
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string LoginID
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [JsonIgnore]
        public string LoginPSW
        {
            get;
            set;
        }


        [JsonIgnore]
        public List<ReUserRole> ReUserRoleLST
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public bool Sex             //false:女,true:男
        {
            get;
            set;
        }


        [ForeignKey("DefaultAddress")]
        public int? DefaultAddrID
        {
            get;
            set;
        }

        public virtual Address DefaultAddress
        {
            get;
            set;
        }

        public virtual List<Address> Addresses
        {
            get;
            set;
        }

        public string Permission
        {
            get;
            set;
        }

        public int Gift
        {
            get;
            set;
        }

        [NotMapped]
        public string Phone
        {
            get
            {
                return (Addresses != null && Addresses.Count > 0) ? Addresses[0].Phone : "";
            }
        }

        [NotMapped]
        public string[] PermissionTag
        {
            get
            {
                return null;
            }
        }

        [NotMapped]
        public string HeadPictureFilePath_S
        {
            get
            {
                string tmpStr = "00000000" + Convert.ToString(UID, 16);
                tmpStr = tmpStr.Substring(tmpStr.Length - 8);
                return "/picture/user/" + tmpStr + "_40_40.jpg";
            }
        }

        [NotMapped]
        public string HeadPictureFilePath
        {
            get
            {
                string tmpStr = "00000000" + Convert.ToString(UID, 16);
                tmpStr = tmpStr.Substring(tmpStr.Length - 8);
                return "/picture/user/" + tmpStr + "_100_100.jpg";
            }
        }

        public string QQOpenID
        {
            get;
            set;
        }

        public string QQAccessToken
        {
            get;
            set;
        }

        public string WBID
        {
            get;
            set;
        }

        public string UserID
        {
            get;
            set;
        }

        public string LastLoginDT
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual List<PushInfo> PushInfos
        {
            get;
            set;
        }
    }
}