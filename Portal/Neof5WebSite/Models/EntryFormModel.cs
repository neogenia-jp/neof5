using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Neof5WebSite.Models
{
    public class EntryFormModel
    {
		[DisplayName("ニックネーム")]
		[Required]
        public string Nickname { get; set; }

		[DisplayName("メールアドレス")]
		[Required]
        public string Mailaddr { get; set; }

		[DisplayName("参加クラス")]
		[Required]
        public string EntryClass { get; set; }

		[DisplayName("使用予定言語")]
		[Required]
        public string UseLanguage { get; set; }

        public static IEnumerable<SelectListItem> ClassSelectItems()
        {
            yield return new SelectListItem { Text = "選択してください", Value = "" };
            yield return new SelectListItem { Text = "クラスＡ（簡易的ルール）", Value = "A" };
            yield return new SelectListItem { Text = "クラスＢ（難しいルール）", Value = "B" };
            yield return new SelectListItem { Text = "両方", Value = "AB" };
        }

        public static IEnumerable<SelectListItem> LanguageSelectItems()
        {
            yield return new SelectListItem { Text = "選択してください", Value = "" };
            yield return new SelectListItem { Text = "C/C++", Value = "C/C++" };
            yield return new SelectListItem { Text = "Objecive-C", Value = "Objecive-C" };
            yield return new SelectListItem { Text = "Java", Value = "Java" };
            yield return new SelectListItem { Text = "Java (Android)", Value = "Java (Android)" };
            yield return new SelectListItem { Text = "Scala", Value = "Scala" };
            yield return new SelectListItem { Text = "C#", Value = "C#" };
            yield return new SelectListItem { Text = "VB.NET", Value = "VB.NET" };
            yield return new SelectListItem { Text = "Perl", Value = "Perl" };
            yield return new SelectListItem { Text = "Ruby", Value = "Ruby" };
            yield return new SelectListItem { Text = "JavaScript", Value = "javaScript" };
            yield return new SelectListItem { Text = "その他", Value = "etc" };
        } 
    }
}
