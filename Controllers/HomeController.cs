using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using WcfFileClient.ServiceReference1;
using System.Threading.Tasks;
using System.Configuration;

namespace WcfFileClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly string UploadFileName = ConfigurationManager.AppSettings["UploadFileName"];
        private readonly string DownloadFileName = ConfigurationManager.AppSettings["DownloadFileName"];
        private readonly string DownloadedFilePath = ConfigurationManager.AppSettings["DownloadedFilePath"];

        public ActionResult Index()
        {
            // Create a client to consume the WCF service
            FileServiceClient client = new FileServiceClient();
            try
            {
                // Call the ListFiles operation
                var fileList = client.ListFiles();

                // Pass the file list to the view
                return View(fileList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View();
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Convert HttpPostedFileBase to a Stream
                var fileUploadMessage = new FileUploadMessage
                {
                    FileName = file.FileName,
                    FileData = file.InputStream
                };
                FileServiceClient client = new FileServiceClient();

                try
                {
                    // Upload the file
                    client.UploadFile(file.FileName, fileUploadMessage.FileData);
                    ViewBag.Message = "File uploaded successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An error occurred: " + ex.Message;
                }
                finally
                {
                    if (client != null)
                    {
                        client.Close();
                    }
                }
            }
            else
            {
                ViewBag.Error = "No file selected.";
            }

            return View("Index");
        }

        public ActionResult Download(string fileName)
        {
            FileServiceClient client = new FileServiceClient();
            //client.ClientCredentials.ClientCertificate.Certificate = await CertHelper.GetAsync();
            try
            {
                // Call the DownloadFile operation
                Stream fileStream = client.DownloadFile(fileName);

                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
                return View("Index");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }
    }
}

