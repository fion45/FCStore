using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Models
{
    public class Procurement
    {
        public int PID
        {
            get;
            set;
        }

        public int BuyerID
        {
            get;
            set;
        }

        public User Buyer
        {
            get;
            set;
        }

        public List<ProcurementPacket> Packets
        {
            get;
            set;
        }

        public string Date
        {
            get;
            set;
        }

    }
}