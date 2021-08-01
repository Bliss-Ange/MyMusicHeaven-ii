using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.AspNetCore.Http;

namespace MyMusicHeaven.Controllers
{
    public class BlobController : Controller
    {
        /// 1. create a new function that link to the blob storage account and link to correct container
        private CloudBlobContainer GetBlobContainerInformation()
        {
            //1.1 link with the appsetting.json (also to ensure the json file free from bug
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build();

            //1.2 get the access ke connection strng that your able able to link to the correct storage
            CloudStorageAccount accountdetails = CloudStorageAccount
                .Parse(configure["ConnectionStrings:BlobStorageConnection"]);

            //1.3 create client object to refer to the correct container
            CloudBlobClient clientagent = accountdetails.CreateCloudBlobClient();
            CloudBlobContainer container = clientagent.GetContainerReference("musicblob");

            return container;
        }

        // 2. create a new function build container based on the name in the getBlobContainerInformation()
        public IActionResult CreateContainer()
        {
            //2.1 always refer to the getBlobContainerInformation() function to get
            //storage account and container name
            CloudBlobContainer container = GetBlobContainerInformation();

            //2.2 create container if container not exist
            ViewBag.result = container.CreateIfNotExistsAsync().Result;

            //2.3 get the name of the container
            ViewBag.ContainerName = container.Name;

            return View();

        }
        
        //3. create a function adding text file to the blob storage
        public string UploadTextFile()
        {
            CloudBlobContainer container = GetBlobContainerInformation();

            //3.2 After creating container, give upload blob name
            CloudBlockBlob blobitem = container.GetBlockBlobReference("blobrelace.txt");

            //3.3 start adding new content to blob storage
            try
            {
                var filestream = System.IO.File.OpenRead(@"D:\Angelyn Degree\UC3F2001SE\DDAC\KpopMusic\BlobTest123.txt");
                using(filestream)
                {
                    blobitem.UploadFromStreamAsync(filestream).Wait();
                }
            }
            catch(Exception ex)
            {
                return "Technical Issue:" + ex.ToString() + ". Please upload file again...";
            }

            return blobitem.Name + "is successfully uploaded to the blob storage now. The URL is " + blobitem.Uri;
        }

        //4. How to use form data to upload a file
        public ActionResult UploadFileFromForm(String Message = null)
        {
            ViewBag.msg = Message;
            return View();
        }

        [HttpPost]
        public ActionResult UploadFileFromForm(List<IFormFile> files)
        {
            CloudBlobContainer container = GetBlobContainerInformation();
            CloudBlockBlob blobitem = null;
            string message = null;
            foreach(var file in files)
            {
                try
                {
                    blobitem = container.GetBlockBlobReference(file.FileName);
                    var stream = file.OpenReadStream();
                    blobitem.UploadFromStreamAsync(stream).Wait();
                    message = message + "The file of " + blobitem.Name + " has uploaded now! \\n";
                }
                catch(Exception ex)
                {
                    message = message + "The file of " + blobitem.Name + " is not able to upload to storage! \\n";
                    message = message + "Error Reason " + ex.ToString();
                }
            }
            return RedirectToAction("UploadFileFromForm", "Blob", new { Message = message});
        }

        //5. Learn how to view blob storage item in a page
        public ActionResult ListBlobAsGallery(string Message = null)
        {
            ViewBag.msg = Message;
            CloudBlobContainer container = GetBlobContainerInformation();

            //create empty listing
            List<string> bloblist = new List<string>();

            //get blob list from storage
            BlobResultSegment listing = container.ListBlobsSegmentedAsync(null).Result;

            //read blob by blob
            foreach(IListBlobItem item in listing.Results)
            {
                //check the blob type (page blob/ block blob / append blob / directory
                if(item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    if(Path.GetExtension(blob.Name)==".jpg" || Path.GetExtension(blob.Name) == ".png")
                    {
                        bloblist.Add(blob.Name + "#" + blob.Uri); //# is the delimeter
                    }
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {

                }
            }
            return View(bloblist);
        }

        //5b. Learn how to view blob storage item in a page (the customer news)
        public ActionResult NewsNUpdates(string Message = null)
        {
            ViewBag.msg = Message;
            CloudBlobContainer container = GetBlobContainerInformation();

            //create empty listing
            List<string> bloblist = new List<string>();

            //get blob list from storage
            BlobResultSegment listing = container.ListBlobsSegmentedAsync(null).Result;

            //read blob by blob
            foreach (IListBlobItem item in listing.Results)
            {
                //check the blob type (page blob/ block blob / append blob / directory
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob gblob = (CloudBlockBlob)item;

                    if (Path.GetExtension(gblob.Name) == ".gif")
                    {
                        bloblist.Add(gblob.Name + "#" + gblob.Uri); //# is the delimeter
                    }
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {

                }
            }
            return View(bloblist);
        }

        //5c. Learn how to view blob storage item in a page (for customer view and poster download)
        public ActionResult PosterGallery(string Message = null)
        {
            ViewBag.msg = Message;
            CloudBlobContainer container = GetBlobContainerInformation();

            //create empty listing
            List<string> bloblist = new List<string>();

            //get blob list from storage
            BlobResultSegment listing = container.ListBlobsSegmentedAsync(null).Result;

            //read blob by blob
            foreach (IListBlobItem item in listing.Results)
            {
                //check the blob type (page blob/ block blob / append blob / directory
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    if (Path.GetExtension(blob.Name) == ".jpg" || Path.GetExtension(blob.Name) == ".png")
                    {
                        bloblist.Add(blob.Name + "#" + blob.Uri); //# is the delimeter
                    }
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {

                }
            }
            return View(bloblist);
        }

        //6. Delete a blob from storage
        public ActionResult deleteblob(string imagename)
        {
            string message = null;
            CloudBlobContainer container = GetBlobContainerInformation();

            try
            {
                CloudBlockBlob item = container.GetBlockBlobReference(imagename);
                string name = item.Name;
                item.DeleteIfExistsAsync().Wait();
                message = " The blob of " + name + " has deleted! from storage now";
            }
            catch (Exception ex)
            {
                message = message + "The selected blob of " + imagename + " is unable to be deleted! Reason: " + ex.ToString();
            }
            return RedirectToAction("ListBlobAsGallery", "Blob", new { Message = message });
        }

        //7. Learn how to download a file from storage to client pc
        public async Task<IActionResult> downloadblob(string imagename, string url)
        {
            string message = null;
            CloudBlobContainer container = GetBlobContainerInformation();
            CloudBlockBlob poster = container.GetBlockBlobReference(imagename);

            try
            {
                await using (MemoryStream memoryStream = new MemoryStream())
                {
                    await poster.DownloadToStreamAsync(memoryStream);
                }
                Stream output = poster.OpenReadAsync().Result;
                return File(output, poster.Properties.ContentType, poster.Name);
            }
            catch (Exception ex)
            {
                message = message + "The selected blob of " + imagename + " is unable to download! Reason: " + ex.ToString();
            }

            return RedirectToAction("ListBlobAsGallery", "Blob", new { Message = message });
        }
    }
}
