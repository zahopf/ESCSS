using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ESCS.Web.Apps;
using ESCS.Domain;
using ESCS.Core;
using ESCS.Service;

namespace ESCS.Web.Controllers
{
    public class Default1Controller : Controller
    {
        //
        // GET: /Default1/
        public ActionResult Index()
        {
            return View();
        }
        public string Upload()
        {
            ///获取上传的文件
            var file = Request.Files[0];

            //获取上传文件的文件名
            string fileName = file.FileName;

            //上传路径
            string filePath = Path.Combine(@"E:\易购商城Easy Shopping Centre System\ESCS\ESCS.Web\Content", fileName);//E:\易购商城Easy Shopping Centre System\ESCS\ESCS.Web\Content

            //定义缓存数组
            byte[] buffer;

            //将文件数据塞到流里
            var inputStream = file.InputStream;

            ///获取读取数据的长度
            int readLength = Convert.ToInt32(inputStream.Length);

            ///给缓存数组指定大小
            buffer = new byte[readLength];

            //设置指针的位置为 最开始 的位置
            inputStream.Seek(0, SeekOrigin.Begin);

            //从位置 0 开始读取上传的文件的数据，数据读取到第一个参数buffer(缓存区)中
            inputStream.Read(buffer, 0, readLength);

            //创建输出文件流,指定文件的输出位置,模式为创建该新文件,读写权限为 写
            using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                //设置指针的位置为 最开始 的位置
                outputStream.Seek(0, SeekOrigin.Begin);
                //从起始位置 将 第一个参数 buffer（缓存区）里的数据写入到 filePath 指定的文件中
                outputStream.Write(buffer, 0, buffer.Length);
            }

            AppHelper.Picture = "~/Content/"+fileName;
            
            //向前台返回上传文件的文件名，表示上传成功
            return JsonConvert.SerializeObject(new { Name = fileName });
        }

        public ActionResult Indexdd()
        {
            

            IList<Goods> list = Container.Instance.Resolve<IGoodsService>().GetAll();

            return View(list);
        }
	}
}