using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruke.Framework.Log
{
  public class Logger
  {
    static Logger()
    {
      XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CfgFiles\\log4net.cfg.xml")));
    }
    private ILog loger = null;
    public Logger(Type type)
    {
      loger = LogManager.GetLogger(type);
    }

    /// <summary>
    /// log4日志
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="ex"></param>
    public void Error(string msg="出现异常",Exception ex=null)
    {
      loger.Error(msg,ex);
    }

    public void Info(string msg)
    {
      loger.Info(msg);
    }

    public void Debug(string msg)
    {
      loger.Debug(msg);
    }

  }
}
