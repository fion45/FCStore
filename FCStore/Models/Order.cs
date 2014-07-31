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
            OS_Payment              = 3,            //已付款，等待卖家发货
            OS_InStore              = 4,            //国外已购买
            OS_ForeignSending       = 5,            //国外邮递中
            OS_InDealer             = 6,            //在经销商手上
            OS_InlandSending        = 7,            //国内邮递中
            OS_InClient             = 8,            //到客人手上
            OS_Complete             = 9,            //已完成
            OS_RTN_Apply            = 10,           //申请退货中
            OS_RTN_AGREE            = 11,           //淘宝退货协议上的，退款协议等待卖家确认中
            OS_RTN_REFUSE           = 12,           //淘宝退货协议上的，卖家不同意协议，等待买家修改
            OS_RTN_BUYER_GOODS      = 13,           //淘宝退货协议上的，退款协议达成，等待买家退货
            OS_RTN_SELLER_GOODS     = 14,           //淘宝退货协议上的，等待卖家收货      
            OS_RTN_InlandSending    = 15,           //退货在国内邮递中
            OS_RTN_InStore          = 16,           //已回仓库
            OS_RTN_SUCCESS          = 17,           //淘宝退货协议上的，退款成功
            OS_RTN_CLOSED           = 18,           //淘宝退货协议上的，退款关闭
            OS_RTN_Complete         = 19,           //退货完成
            OS_ERR_PAYMENT          = 20,           //支付宝支付信息有异常
            OS_ERR_Complete         = 21            //支付宝交易过程关闭
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

        public string OIDStr
        {
            get
            {
                string tmpStr = "00000000" + Convert.ToString(OID,16);
                tmpStr = tmpStr.Substring(tmpStr.Length - 8);
                DateTime tmpDT = DateTime.Parse(OrderDate);
                return tmpDT.ToString("yyyyMMdd") + tmpStr;
            }
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

        public string StatusStr
        {
            get
            {
                string result = "未定义";
                switch(Status)
                {
                    case (int)EOrderStatus.OS_Init :
                        {
                            result = "新订单";
                            break;
                        }
                    case (int)EOrderStatus.OS_Order:
                        {
                            result = "已下单";
                            break;
                        }
                    case (int)EOrderStatus.OS_Subscription:
                        {
                            result = "已落订";
                            break;
                        }
                    case (int)EOrderStatus.OS_Payment:
                        {
                            result = "已付款，等待发货";
                            break;
                        }
                    case (int)EOrderStatus.OS_InStore:
                        {
                            result = "已进货";
                            break;
                        }
                    case (int)EOrderStatus.OS_ForeignSending:
                        {
                            result = "国外邮递中";
                            break;
                        }
                    case (int)EOrderStatus.OS_InDealer:
                        {
                            result = "在经销商手上";
                            break;
                        }
                    case (int)EOrderStatus.OS_InlandSending:
                        {
                            result = "国内邮递中";
                            break;
                        }
                    case (int)EOrderStatus.OS_InClient:
                        {
                            result = "已收货";
                            break;
                        }
                    case (int)EOrderStatus.OS_Complete:
                        {
                            result = "成功完成订单";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_Apply:
                        {
                            result = "申请退货中";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_AGREE:
                        {
                            result = "退款协议等待卖家确认中";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_REFUSE:
                        {
                            result = "卖家不同意协议，等待买家修改";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_BUYER_GOODS:
                        {
                            result = "退款协议达成，等待买家退货";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_SELLER_GOODS:
                        {
                            result = "等待卖家收货";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_InlandSending:
                        {
                            result = "退货在国内邮递中";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_InStore:
                        {
                            result = "已回仓库";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_SUCCESS:
                        {
                            result = "退款成功";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_CLOSED:
                        {
                            result = "退款关闭";
                            break;
                        }
                    case (int)EOrderStatus.OS_RTN_Complete:
                        {
                            result = "退货完成";
                            break;
                        }
                    case (int)EOrderStatus.OS_ERR_PAYMENT:
                        {
                            result = "支付宝支付信息有异常";
                            break;
                        }
                    case (int)EOrderStatus.OS_ERR_Complete:
                        {
                            result = "支付宝交易过程关闭";
                            break;
                        }
                }
                return result;
            }
        }

        public int SendType
        {
            get;
            set;
        }

        public virtual List<Post> Posts
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

        public string PayDate
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

        [NotMapped]
        public string ReceiveAddress
        {
            get
            {
                return BelongTown.FullName + " " + AddressName;
            }
        }

        public string AP_TradeNO
        {
            get;
            set;
        }
    }
}