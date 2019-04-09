using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		/// <summary>
		/// This will recieve a Json file containing base64 form of image and will process it and send back the result inside a json file
		/// </summary>
		/// <returns></returns>
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
				var name = (string)jsonObj["name2"];
				var path = @"F:\Arun\B-TECH\project\ProjectHGR\ProjectHGR\Project\Images\Inputs\sample\" + name;
				ms.Write(imageBytes, 0, imageBytes.Length);
				System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
				image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
				var line = startProcess(path);
				//var line = name;
				var val = new { status = line };
				return Json(val, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				var val = new { status = e.Message };
				return Json(val, JsonRequestBehavior.AllowGet);

			}
			
			//return View();
		}

		public ActionResult GetOutPut(string id)
		{
			var path = id;
			//var path = @"C:\Users\Arun\Desktop\project\Matlab-Download\ProjectHGR\" + id + ".txt";
			var json = new { data = new Data().getImageOutput(path) };
			return Json(json, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// This will start the python script for recognizing gestures
		/// </summary>
		/// <param name="path">the location of the image which we had converted to the jpg format from base64 which we recieved from the client</param>
		/// <returns>result of the input image</returns>
		public string startProcess(string path)
		{
			ProcessStartInfo pStartInfo = new ProcessStartInfo();
			pStartInfo.FileName = @"python";
			//pStartInfo.RedirectStandardError=true;
			pStartInfo.RedirectStandardOutput = true;
			pStartInfo.UseShellExecute = false;
			string pythonScriptPath = Server.MapPath("~/App_Data/datamodeltestOnlyForTest.py");
			string pythonScriptArgForSpecifyingLocationOfModelWithJsonExtension= Server.MapPath("~/App_Data/files/model_basic.json");
			string pythonScriptArgForSpecifyingLocationOfModelContainingWeights = Server.MapPath("~/App_Data/files/model_basic.h5");

			pStartInfo.Arguments = $"{ pythonScriptPath} {path} {pythonScriptArgForSpecifyingLocationOfModelWithJsonExtension} {pythonScriptArgForSpecifyingLocationOfModelContainingWeights}";
			Process p = new Process();
			p.StartInfo = pStartInfo;
			p.Start();
			string line = "hello";
			p.WaitForExit();
			while (!p.StandardOutput.EndOfStream)
			{
				line = p.StandardOutput.ReadLine();
				// do something with line
			}

			Console.WriteLine(line);
			return line;
		}

	}
}
