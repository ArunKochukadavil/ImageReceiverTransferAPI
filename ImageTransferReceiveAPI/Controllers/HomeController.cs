using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageTransferReceiveAPI.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";

			return View();
		}
		public ActionResult ImageReceiver()
		{
			try
			{
				Stream req = Request.InputStream;
				req.Seek(0, System.IO.SeekOrigin.Begin);
				string json = new StreamReader(req).ReadToEnd();
				JObject jsonObj = JObject.Parse(json);
				var content = (string)jsonObj["base64"];
				byte[] imageBytes = Convert.FromBase64String(content);
				MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
				var name = (string)jsonObj["name"];
				var path = @"C:\Users\Arun\Desktop\project\Matlab-Download\ProjectHGR\" + name + ".jpg";
				ms.Write(imageBytes, 0, imageBytes.Length);
				System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
				image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
				var val = new { status = "Received" };
				return Json(val, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				var val = new { status = e.Message };
				return Json(val, JsonRequestBehavior.AllowGet);
			}
			
			//return View();
		}
	}
}
