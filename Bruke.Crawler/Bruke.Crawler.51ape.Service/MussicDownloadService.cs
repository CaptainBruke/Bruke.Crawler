using Bruke.Crawler._51ape.Interfac;
using Bruke.Crawler.Model;
using Bruke.DAL;
using Bruke.Framework.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Bruke.Framework.Log;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace Bruke.Crawler._51ape.Service
{
  public class MussicDownloadService : IMussicDownload
  {

    DalContext dalContext = new DalContext();
    private static Logger logger = new Logger(typeof(MussicDownloadService));
    public void Crawl()
    {



      //该网站 第一个有规则最大： 70276
      //该网站 第二个有规则最大：168387 （2018年7月18日14:27:39 完结）
      //从1爬到200,000(二十万)

      int taskNum = 2000;
      List<int> taskListNum = new List<int>();
      for (int i = 0; i < taskNum; i++)
      {
        taskListNum.Add(i);
      }


      List<Task> taskList = new List<Task>();

      foreach (var item in taskListNum)
      {
        taskList.Add(Task.Run(() => CrawlerTask(item)));
        if (taskList.Count > 10)
        {
          taskList = taskList.Where(t => !t.IsCompleted && !t.IsCanceled && !t.IsFaulted).ToList();
          Task.WaitAny(taskList.ToArray());
        }
        try { dalContext.SaveChanges(); }
        catch
        {
          Console.WriteLine($"***********保存异常*************");
          logger.Error($"***********保存异常*************");
        }

      }

      Task.WaitAll(taskList.ToArray());
      Console.WriteLine("爬虫完毕");

      ErroCrawlerAgain();


    }

    /// <summary>
    /// 获取歌词
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    private Array GetLyricArray(HtmlNodeCollection nodes)
    {
      string[] lyricArray = new string[nodes.Count];
      for (int i = 0; i < nodes.Count; i++)
      {
        lyricArray[i] = nodes[i].InnerText.Replace("&nbsp;", "");
      }
      return lyricArray;
    }


    /// <summary>
    /// 开始爬虫
    /// </summary>
    /// <param name="begin">开始值</param>
    private void CrawlerTask(int begin)
    {

      begin = begin * 100;
      for (int i = begin; i < begin + 100; i++)
      {
        Crawler(i);
      }
    }

    /// <summary>
    /// 爬虫单元
    /// </summary>
    /// <param name="i"></param>
    private void Crawler(int i)
    {
      string url1 = "http://www.51ape.com/ape/";
      string url = $"{url1}{i}.html";
      string html = HttpHelper.DownloadHtml(url);

      try
      {
        if (!string.IsNullOrEmpty(html))
        {
          HtmlDocument document = new HtmlDocument();
          document.LoadHtml(html);

          var nameNode = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/h1");
          if (nameNode != null)
          {
            string allName = nameNode.InnerText;
            string singer = allName.Split('-').First();
            string name = allName.Split('-').Last();
            var downlaodUrlNode = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/a");
            if (downlaodUrlNode != null)
            {
              string downlaodUrl = downlaodUrlNode.Attributes["href"].Value;
              var passwordNode = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/b/text()[2]");
              string password = passwordNode.InnerText.Replace("密码：", "").Replace("无", "");
              var album = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/h3[1]")?.InnerText.Replace("选自专辑", "");
              var rate = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/h3[2]/text()")?.InnerText;
              var size = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/h3[3]/text()")?.InnerText;
              var languge = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/h3[4]/text()")?.InnerText;
              var date = document.DocumentNode.SelectSingleNode("/html/body/div/div/div/h3[5]/text()")?.InnerText;
              var lyricist = document.DocumentNode.SelectSingleNode("//*[@id='newstext_2']/h3[1]/a")?.InnerText;
              var composer = document.DocumentNode.SelectSingleNode("//*[@id='newstext_2']/h3[2]/a")?.InnerText;
              var lyricNodes = document.DocumentNode.SelectNodes("//*[@id='newstext_2']/p");
              var lyric = string.Empty;
              if (lyricNodes != null)
              {
                var lyricArray = GetLyricArray(lyricNodes);
                lyric = Newtonsoft.Json.JsonConvert.SerializeObject(lyricArray);
              }

              dalContext.MussicDownLoads.Add(new MussicDownLoad()
              {
                crawlerId = i,
                url = url,
                allName = allName,
                name = name,
                singer = singer,
                dowloadUrl = downlaodUrl,
                passWord = password,
                album = album,
                rate = rate,
                size = size,
                languge = languge,
                date = DateTime.Parse(date),
                lyricist = lyricist,
                composer = composer,
                lyric = lyric
              });
              Console.WriteLine($"爬虫完毕-{allName}，id-{i}");
            }
          }
        }
        else
        {
          Console.WriteLine($"id-{i}");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"*********异常{i}*************");
        logger.Error(url);
      }


      Console.WriteLine($"id-{i}");
    }

    /// <summary>
    /// 根据爬虫失败日志重新爬取
    /// </summary>
    private void ErroCrawlerAgain()
    {
      FileStream fs = new FileStream(@"Log/log.txt", FileMode.Open);
      StreamReader sr = new StreamReader(fs);

      string log = sr.ReadToEnd();
      sr.Close();
      fs.Close();

      Regex reg = new Regex("ape/(.+).html");
      MatchCollection matchs = reg.Matches(log);

      foreach (var item in matchs)
      {
        int id = int.Parse(item.ToString().Replace("ape/", "").Replace(".html", ""));
        Crawler(id);
      }

      Console.WriteLine("重新爬虫成功！");
    }

    //把表去重后放在另一张表
    //    insert into Mussic
    //select crawlerId,Max(allName),Max(name),Max(singer),Max(url),Max(dowloadUrl),
    //Max([passWord]),Max(album),Max(rate),Max(languge),Max(size),Max([date]),Max(lyricist),Max(composer),Max(lyric)
    // from MussicDownLoads group by crawlerId
    //最后在新表建立索引

  }
}
