using Bruke.Crawler.JD.Interface;
using Bruke.Framework.Log;
using Bruke.Framework.Http;
using HtmlAgilityPack;
using Bruke.Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Bruke.DAL;
using System.Data.SqlClient;

namespace Bruke.Crawler.JD.Service
{
  public class CategoryService : ICategaryInterface
  {
    private Logger logger = new Logger(typeof(CategoryService));

    public void Crawl()
    {
      string html = HttpHelper.DownloadHtml("https://www.jd.com/allSort.aspx");
      if (string.IsNullOrEmpty(html))
      {
        logger.Error("下载的html为空！");
      }

      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(html);

      //保存类别的实体集合
      List<Category> categoryModels = new List<Category>();

      //获取所有一级目录
      string firstPath = "//div[@class='category-item m']";
      HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(firstPath);

      if (nodes != null)
      {
        foreach (HtmlNode node in nodes)
        {
          string fistHtml = node.OuterHtml;
          HtmlDocument documentChild = new HtmlDocument();
          documentChild.LoadHtml(fistHtml);

          //获取一级数据
          HtmlNode nodeChild = documentChild.DocumentNode.SelectSingleNode("//div/h2/span");

          //保存一级目录
          Category firstCategory = new Category
          {
            Id = categoryModels.LastOrDefault() == null ? 1 : categoryModels.LastOrDefault().Id + 1,
            Level = 0,
            Name = nodeChild.InnerText
          };
          categoryModels.Add(firstCategory);

          //获取二级数据
          HtmlNodeCollection nodeSeconds = documentChild.DocumentNode.SelectNodes("//dl");
          foreach (var secondNode in nodeSeconds)
          {
            string scondHtml = secondNode.OuterHtml;
            HtmlDocument documentSecond = new HtmlDocument();
            documentSecond.LoadHtml(scondHtml);

            HtmlNode nodeSecondA = documentSecond.DocumentNode.SelectSingleNode("//dt/a");

            Category secondCategory = new Category
            {
              Id =  categoryModels.LastOrDefault().Id + 1,
              Level = 1,
              ParentId = firstCategory.Id,
              Name = nodeSecondA.InnerText,
              Url = nodeSecondA.Attributes["href"].Value.Replace("//", "")
            };
            categoryModels.Add(secondCategory);

            //获取三级数据
            HtmlNodeCollection nodeThirds = documentSecond.DocumentNode.SelectNodes("//dl/dd/a");
            foreach (var thirdNode in nodeThirds)
            {
              Category thirdCategory = new Category
              {
                Id =categoryModels.LastOrDefault().Id + 1,
                Level = 2,
                ParentId = secondCategory.Id,
                Name = thirdNode.InnerText,
                Url = thirdNode.Attributes["href"].Value.Replace("//", "")
              };
              categoryModels.Add(thirdCategory);
            }

          }
        }



      }
      if (categoryModels.Any())
      {
        logger.Info(JsonConvert.SerializeObject(categoryModels));
        DalContext dalContext = new DalContext();

        categoryModels.ForEach(f =>
        {
          //dalContext.Categorys.Add(f);
        });
        dalContext.SaveChanges();
      }



    }
  }
}
