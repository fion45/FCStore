using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FCStore.Models
{
    public class Order
    {
        public enum EOrderStatus
        {
            OS_Init                 = 0,            //初始化
            OS_Order                = 1,            //已下单
            OS_Subscription         = 2,            //已落订
            OS_Payment              = 3,            //已付款
            OS_InStore              = 4,            //国外已购买
            OS_ForeignSending       = 5,            //国外邮递中
            OS_InDealer             = 6,            //在经销商手上
            OS_InlandSending        = 7,            //国内邮递中
            OS_InClient             = 8,            //到客人手上
            OS_Complete             = 9,            //已完成
            OS_RTN_Apply            = 10,           //申请退货中
            OS_RTN_InlandSending    = 11,           //退货在国内邮递中
            OS_RTN_InStore          = 12,           //已回仓库
            OS_RTN_Complete         = 13            //退货完成
        }

        public enum ESendType
        {
            ST_Direct = 0,          //直邮
            ST_Indirect = 1         //转邮
        }

        public enum EPayType
        {
            PT_Alipay
        }

        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OID
        {
            get;
            set;
        }

        public virtual List<OrderPacket> Packets
        {
            get;
            set;
        }

        [ForeignKey("User")]
        public int? UID
        {
            get;
            set;
        }

        [JsonIgnore]
        public User User
        {
            get;
            set;
        }

        //订单的总价
        public decimal Amount
        {
            get
            {
                decimal result = 0;
                foreach(OrderPacket packet in Packets) 
                {
                    result += packet.Amount;
                }
                return result;
            }
        }

        //实际应付
        public decimal PayAmount
        {
            get
            {
                decimal result = 0;
                foreach (OrderPacket packet in Packets)
                {
                    result += packet.PayAmount;
                }
                return result;
            }
        }

        //邮费
        public decimal Postage
        {
            get;
            set;
        }

        //实际应付的总价加上邮费
        public decimal PayAmountAndPostage
        {
            get
            {
                return Postage + PayAmount;
            }
        }

        //订金
        public decimal Subscription
        {
            get;
            set;
        }

        public decimal Payment
        {
            get
            {
                return Amount - Subscription;
            }
        }

        public int Status
        {
            get;
            set;
        }

        public int SendType
        {
            get;
            set;
        }

        public int PayType
        {
            get;
            set;
        }

        public string OrderDate
        {
            get;
            set;
        }

        public string ReceivedDate
        {
            get;
            set;
        }

        public string CompleteDate
        {
            get;
            set;
        }

        public string GetCoookieStr()
        {
            string result = "";
            foreach(OrderPacket op in Packets)
            {
                result += OID.ToString() + "," + op.Count + "," + op.Product.Title.Substring(0, Math.Min(20, op.Product.Title.Length)) + "," + op.Product.ImgPathArr[0] + ",";
            }
            return result;
        }

        //收货信息
        public string Contacts
        {
            get;
            set;
        }

        [ForeignKey("BelongTown")]
        public int? TownID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Town BelongTown
        {
            get;
            set;
        }

        public string AddressName
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public string PostCode
        {
            get;
            set;
        }
        
        [JsonIgnore]
        public string ReceiveAddress
        {
            get
            {
                return BelongTown.FullName + " " + AddressName;
            }
        }
    }
}