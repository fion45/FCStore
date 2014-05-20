using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public int OID
        {
            get;
            set;
        }

        public List<OrderPacket> Packets
        {
            get;
            set;
        }

        public int UID
        {
            get;
            set;
        }

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

        public EOrderStatus Status
        {
            get;
            set;
        }

        public ESendType SendType
        {
            get;
            set;
        }

        public EPayType PayType
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
    }
}