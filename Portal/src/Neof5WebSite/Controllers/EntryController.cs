using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

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

            var mailTo = Environment.GetEnvironmentVariable("GMAIL_ADDR");
            var passwd = Environment.GetEnvironmentVariable("GMAIL_PASSWD");


            //Mailの作成
            var msg = new MimeMessage("contact@neogenia.co.jp", inputData.Mailaddr);
            msg.From.Add(new MailboxAddress("システム管理者", mailTo));
            msg.Cc.Add(new MailboxAddress("システム管理者", mailTo));
            msg.Subject = "[ネオ富豪] 申し込みを受け付けました。";
            msg.Body = new TextPart("Plain") { Text = mailbody };
            try
            {
                using (var sc = new SmtpClient())
                {
                    sc.Send(msg);
                    sc.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    sc.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                    sc.Authenticate(mailTo, passwd);
                    sc.Send(msg);
                    sc.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Index");
            }

            return View();
        }
    }
}
