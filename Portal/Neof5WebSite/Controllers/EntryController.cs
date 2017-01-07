using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace Neof5WebSite.Controllers
{
    public class EntryController : Controller
    {
        //
        // GET: /Entry/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostEntry(Models.EntryFormModel inputData)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

			string mailbody = string.Format(@"
{0} さん

ありがとうございます。

「ネオ富豪」への申し込みを受け付けました。

===== 記入内容 =====
ニックネーム：{0}
メールアドレス：{1}
参加予定クラス：{2}
使用予定言語/プラットフォーム」：{3}


大会開催日1週間前には、当日の予定や対戦方式などの
ご案内のメールを配信いたします。


株式会社ネオジニア 代表 前田
",
 inputData.Nickname, inputData.Mailaddr, inputData.EntryClass, inputData.UseLanguage);

            //try
            //{
            //    //Gmailでメールを送信する
            //    System.Web.Mail.MailMessage mm = new System.Web.Mail.MailMessage();
            //    //送信者
            //    mm.From = "neogenia.dev@gmail.com";
            //    //送信先
            //    mm.To = inputData.Mailaddr;
            //    //題名
            //    mm.Subject = "[ネオ富豪] 申し込みを受け付けました。";
            //    //本文
            //    mm.Body = mailbody;
            //    //JISコードに変換する
            //    mm.BodyEncoding = System.Text.Encoding.GetEncoding(50220);
            //    //SMTPサーバーを指定する
            //    System.Web.Mail.SmtpMail.SmtpServer = "smtp.gmail.com";
            //    mm.Fields[
            //        "http://schemas.microsoft.com/cdo/configuration/smtpserverport"]
            //        = 587;                //送信する
            //    System.Web.Mail.SmtpMail.Send(mm);
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.ErrorMessage = ex.Message;
            //    return View("Index");
            //}


            //MailMessageの作成
            using (var msg = new MailMessage("contact@neogenia.co.jp", inputData.Mailaddr))
            {
                msg.CC.Add("neogenia.dev@gmail.com");
                msg.Subject = "[ネオ富豪] 申し込みを受け付けました。";
                msg.Body = mailbody;
                try
                {
                    using (var sc = new SmtpClient())
                    {
                        ////SMTPサーバーなどを設定する
                        //sc.Host = "smtp.gmail.com";
                        //sc.Port = 25;
                        //sc.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        ////ユーザー名とパスワードを設定する
                        ////sc.Credentials = new System.Net.NetworkCredential("azure_994343481dad509acdd87d1628c5033e@azure.com", "ifr8vchd");
                        //sc.Credentials = new System.Net.NetworkCredential("neogenia.dev@gmail.com", "ainegoen-2012");
                        ////SSLを使用する
                        //sc.EnableSsl = true;
                        ////メッセージを送信する
                        sc.Send(msg);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Index");
                }
            }
            return View();
        }
    }
}
