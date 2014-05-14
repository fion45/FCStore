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

        public int OID
        {
            get;
            set;
        }

        public List<OrderPacket> Products
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

        public decimal Amount
        {
            get;
            set;
        }

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