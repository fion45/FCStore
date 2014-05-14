using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Models
{
    public class OrderPacket
    {
        public int PacketID
        {
            get;
            set;
        }

        public int PID
        {
            get;
            set;
        }

        public Product Product
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

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
    }
}