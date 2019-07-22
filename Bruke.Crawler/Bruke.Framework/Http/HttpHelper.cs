using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bruke.Framework.Http
{
  public class HttpHelper
  {
    public static string DownloadHtml(string url)
    {
      string html = string.Empty;
      try
      {
        HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
        request.Timeout = 30 * 1000;
        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
        request.ContentType = "text/html; charset=UTF-8";
        request.Host = "www.jd.com";
        request.Headers.Add("Cookie", @"__jdu=1842011284; user-key=cd66f846-1765-4aba-8bda-449f20880d46; cn=0; ipLoc-djd=1-72-2799-0; o2Control=webp|lastvisit=20; unpl=V2_ZzNtbUteRxAnXEFdcx9VUGIFRV0SB0VCJ1pGAXscXAxiVEYJclRCFXwUR1JnGFgUZwEZWUBcQhJFCHZXfBpaAmEBFl5yBBNNIEwEACtaDlwJBxRbR1VDHXUBRVBLKV8FVwMTbUJeRhZ0CEZXchxsNWAzIm1EVUQTdgt2VUsYbEczXxtcQ1NLFTgIT1F4GFwFZAoXbUNnQA%3d%3d; __jdc=122270672; __jdv=122270672|baidu-search|t_262767352_baidusearch|cpc|57743191825_0_8854ce68978d47f1aa7fcc1d14184fee|1530769930984; __jda=122270672.1842011284.1528280123.1530769931.1530848425.6; PCSYCityID=1607; __jdb=122270672.15.1842011284|6.1530848425");
        request.Method = "GET";
        Encoding encoding = Encoding.UTF8;
        using (HttpWebResponse response=request.GetResponse() as HttpWebResponse)
        {
          if (response.StatusCode!=HttpStatusCode.OK)
          {
            Console.WriteLine("抓取{0}地址返回失败,response.StatusCode为{1}",url,response.StatusCode);
          }
          else
          {
            try
            {
              StreamReader sr = new StreamReader(response.GetResponseStream(),encoding);
              html = sr.ReadToEnd();
              sr.Close();
            }
            catch (Exception ex)
            {
              Console.WriteLine(string.Format($"DownloadHtml抓取{url}失败"), ex);
              html = null;
              throw;
            }
          }
        }

      }
      catch (System.Net.WebException ex)
      {
        if (ex.Message.Equals("远程服务器返回错误: (306)。"))
        {
          Console.WriteLine("远程服务器返回错误: (306)。", ex);
          html = null;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(string.Format("DownloadHtml抓取{0}出现异常", url), ex);
        html = null;
      }
      return html;
    }
  }
}
