using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Models
{
    public struct PacketObj
    {
        public int PacketID;
        public int Count;
    }
    public struct SubmitObj
    {
        public int OrderID
        {
            get;
            set;
        }

        public List<PacketObj> Packets
        {
            get;
            set;
        }
    }
}