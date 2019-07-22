using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruke.Crawler.Model
{
  //[Table("Category")]
  public class Category//Model
  {
    /// <summary>
    /// 类别id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 类别名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 等级：0开始
    /// </summary>
    public int Level { get; set; }
    /// <summary>
    /// 父级
    /// </summary>
    public int ParentId { get; set; }
    /// <summary>
    /// 跳转地址
    /// </summary>
    public string Url { get; set; }
  }
}
