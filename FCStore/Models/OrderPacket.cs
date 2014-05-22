using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FCStore.Models
{
    public class OrderPacket
    {

        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PacketID
        {
            get;
            set;
        }

        [ForeignKey("Product")]
        public int PID
        {
            get;
            set;
        }

        public virtual Product Product
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public decimal Discount
        {
            get;
            set;
        }

        //订单单价
        public decimal Univalence
        {
            get;
            set;
        }

        public decimal Amount
        {
            get
            {
                return Count * Univalence;
            }
        }

        public decimal PayUnivalence
        {
            get
            {
                return Univalence * Discount;
            }
        }

        public decimal PayAmount
        {
            get
            {
                return Amount * Discount;
            }
        }
    }
}