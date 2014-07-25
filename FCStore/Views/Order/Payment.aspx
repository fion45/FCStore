<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@	 Import	 Namespace="System"	 %>
<%@	 Import	 Namespace="System.Collections.Generic"	 %>
<%@	 Import	 Namespace="System.Web.UI"	 %>
<%@	 Import	 Namespace="Com.Alipay"	 %>
<%@	 Import	 Namespace="FCStore.Common"	 %>
<%@	 Import	 Namespace="FCStore.Controllers"	 %>
<%@	 Import	 Namespace="FCStore.Models"	 %>
  
<script	 type="text/C#" runat="Server">
protected void Page_Load(object	sender, EventArgs e)
{
    MyUser tmpUser = null;
    if (User.Identity.IsAuthenticated && (tmpUser = User as MyUser) != null)
    {
        //从数据库中加载订单内容
        FCStoreDbContext db = new FCStoreDbContext();
        HttpCookie cookie = Request.Cookies["Order"];
        string tmpStr = Server.UrlDecode(cookie.Value);
        Regex cookieRgx = new Regex(ProductController.ORDERCOOKIERGX);
        Match tmpMatch = cookieRgx.Match(tmpStr);
        if (!string.IsNullOrEmpty(tmpMatch.Value))
        {
            Group gi = tmpMatch.Groups["ORDERID"];
            int  OrderID = int.Parse(gi.Value);
            Order tmpOrder = db.Orders.FirstOrDefault(r => r.OID == OrderID);
            if (tmpOrder != null)
            {
                List<OrderPacket> opLST = tmpOrder.Packets;
                if (opLST.Count > 0)
                {
                    //加载用户的地址
                    ////////////////////////////////////////////请求参数////////////////////////////////////////////

                    //支付类型
                    string payment_type = "1";
                    //必填，不能修改
                    //服务器异步通知页面路径
                    //string notify_url = "http://www.rightgo.cn/order/AlipayCB";
                    //需http://格式的完整路径，不能加?id=123这类自定义参数

                    //页面跳转同步通知页面路径
                    string return_url = "http://www.rightgo.cn/order/AlipayCB";
                    //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

                    //卖家支付宝帐户
                    string seller_email = "86945494@qq.com";
                    //必填

                    //商户订单号
                    string out_trade_no = tmpOrder.OIDStr;
                    //商户网站订单系统中唯一订单号，必填

                    //订单描述
                    string body = "";
                    foreach (OrderPacket op in opLST)
                    {
                        body += op.Product.Title + ";";
                    }
                    body = body.Trim(new char[] { ';' });
                    //商品展示地址
                    string show_url = "http://www.rightgo.cn" + opLST[0].Product.ImgPathArr[0];
                    //需以http://开头的完整路径，如：http://www.xxx.com/myorder.html

                    //订单名称
                    string subject = "RightGO网订单" + body;
                    //必填

                    decimal totalPrice = Math.Round(tmpOrder.Amount, 2);
                    //付款金额
                    string price = totalPrice.ToString();
                    //必填

                    //商品数量
                    string quantity = "1";
                    //必填，建议默认为1，不改变值，把一次交易看成是一次下订单而非购买一件商品

                    //加载几组物流价钱
                    string logistics_fee;
                    string logistics_type;
                    string logistics_payment;
                    if (tmpOrder.SendType == (int)Order.ESendType.ST_Indirect)
                    {
                        //转邮
                        //物流费用
                        logistics_fee = "0.00";
                        //必填，即运费
                        //物流类型
                        logistics_type = "EXPRESS";
                        //必填，三个值可选：EXPRESS（快递）、POST（平邮）、EMS（EMS）
                        //物流支付方式
                        logistics_payment = "SELLER_PAY";
                        //必填，两个值可选：SELLER_PAY（卖家承担运费）、BUYER_PAY（买家承担运费）
                    }
                    else
                    {
                        //直邮
                        decimal sendPay = Math.Round(totalPrice * (decimal)0.01, 2);
                        //物流费用
                        logistics_fee = sendPay.ToString();
                        //必填，即运费
                        //物流类型
                        logistics_type = "EXPRESS";
                        //必填，三个值可选：EXPRESS（快递）、POST（平邮）、EMS（EMS）
                        //物流支付方式
                        logistics_payment = "BUYER_PAY";
                        //必填，两个值可选：SELLER_PAY（卖家承担运费）、BUYER_PAY（买家承担运费）
                    }

                    //收货人姓名
                    string receive_name = tmpUser.UserName;
                    //如：张三

                    //收货人地址
                    string receive_address = tmpOrder.ReceiveAddress;
                    //如：XX省XXX市XXX区XXX路XXX小区XXX栋XXX单元XXX号

                    //收货人邮编
                    string receive_zip = "TEST";
                    //如：123456

                    //收货人电话号码
                    string receive_phone = "TEST";
                    //如：0571-88158090

                    //收货人手机号码
                    string receive_mobile = "TEST";
                    //如：13312341234


                    ////////////////////////////////////////////////////////////////////////////////////////////////

                    //把请求参数打包成数组
                    SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                    sParaTemp.Add("partner", Config.Partner);
                    sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
                    sParaTemp.Add("service", "trade_create_by_buyer");
                    sParaTemp.Add("payment_type", payment_type);
                    sParaTemp.Add("return_url", return_url);
                    sParaTemp.Add("seller_email", seller_email);
                    sParaTemp.Add("out_trade_no", out_trade_no);
                    sParaTemp.Add("subject", subject);
                    sParaTemp.Add("price", price);
                    sParaTemp.Add("quantity", quantity);
                    sParaTemp.Add("logistics_fee", logistics_fee);
                    sParaTemp.Add("logistics_type", logistics_type);
                    sParaTemp.Add("logistics_payment", logistics_payment);
                    sParaTemp.Add("body", body);
                    sParaTemp.Add("show_url", show_url);
                    sParaTemp.Add("receive_name", receive_name);
                    sParaTemp.Add("receive_address", receive_address);
                    sParaTemp.Add("receive_zip", receive_zip);
                    sParaTemp.Add("receive_phone", receive_phone);
                    sParaTemp.Add("receive_mobile", receive_mobile);

                    //建立请求
                    string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
                    Response.Write(sHtmlText);
                    tmpOrder.Status = (int)Order.EOrderStatus.OS_Order;
                    db.SaveChanges();
                }
                else
                {
                    Response.Write("订单没有商品");
                }
            }
            else
            {
                Response.Write("订单不存在");
            }
            
        }
        else
        {
            Response.Write("Cookie有问题");
        }
        db.Dispose();
    }
    else
    {
        Response.Write("用户不存在或验证失败");
    }
}
</script>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Payment</title>
</head>
<body>
    <div>
        
    </div>
</body>
</html>
