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
            OS_Init,
            OS_Order,
            OS_Subscription,
            OS_InStore,
            OS_ForeignSending,
            OS_InDealer,
            OS_InlandSending,
            OS_InClient,
            OS_Payment,
            OS_Complete
        }

        public enum ESendType
        {
            ST_Direct,          //直邮
            ST_Indirect         //转邮
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
        public int UID
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
    }
}