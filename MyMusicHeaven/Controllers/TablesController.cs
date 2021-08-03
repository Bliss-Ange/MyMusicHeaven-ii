using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyMusicHeaven.Models; //to access announcemententity
using Microsoft.WindowsAzure.Storage.Auth;

namespace MyMusicHeaven.Controllers
{
    public class TablesController : Controller
    {
        // 1. create a new function that link to the  storage account and link to correct table
        private CloudTable getTableStorageInformation()
        {
            //1.1 link with the appsetting.json (also to ensure the json file free from bug
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build();

            //1.2 get the access ke connection strng that your able able to link to the correct storage
            CloudStorageAccount accountdetails = CloudStorageAccount
                .Parse(configure["ConnectionStrings:TableStorageConnection"]);

            //1.3 create client object to refer to the correct table
            CloudTableClient clientagent = accountdetails.CreateCloudTableClient();
            CloudTable table = clientagent.GetTableReference("Announcement");

            return table;

        }

        // 2. create a new function build container based on the name in the getBlobContainerInformation()
        public IActionResult CreateTable()
        {
            //2.1 always refer to the getTableStorageInformation() function to get
            //storage account and table name
            CloudTable table = getTableStorageInformation ();

            //2.2 create container if table not exist
            ViewBag.result = table.CreateIfNotExistsAsync().Result;

            //2.3 get the name of the table
            ViewBag.TableName = table.Name;

            return View();

        }

        public ActionResult AddAnnouncement()
        {

            return View();

        }

        public ActionResult AddEntity(string PartitionKey, string RowKey, DateTime Announcement_Date, string Announcement)
        {
       
            //initialize announcementEntity class
            AnnouncementEntity ann = new AnnouncementEntity(PartitionKey, RowKey);
            ann.Announcement_Date = Announcement_Date;
            ann.Announcement = Announcement;

            //refer to the table
            CloudTable table = getTableStorageInformation();
            string message = null;

            try
            {
                //create action
                TableOperation insertOperation = TableOperation.Insert(ann); 

                //execute insert operation
                TableResult result = table.ExecuteAsync(insertOperation).Result;

                //update viewbag with the table name and result of insert operation
                message = "Insert successful! " + PartitionKey + " " + RowKey + " is inserted to the table";
            }
            catch (Exception ex)
            {
                message = "Unable to insert the data due to error: " + ex.ToString();
            }
            return RedirectToAction("SearchAnnouncement", "Tables", new { Message = message });
        }

        public ActionResult AddEntities()
        {
            //refer to the table
            CloudTable tableinfo = getTableStorageInformation();

            TableBatchOperation batchOperation = new TableBatchOperation(); //insert group of data
            IList<TableResult> results;

            string[,] announcement =
            {
                {"BTS", "Limited Special Gifts", "2021-07-07", "Buy BTS MEMORIES of 2020 DVD, and get 1 photo frame." },
                {"BTS", "Regular Album", "2021-04-04", "To be delivered after August 17th together with merch" },
            }; //array

            for (int i = 0; i < 2; i++)
            {
                AnnouncementEntity ann = new AnnouncementEntity(announcement[i, 0], announcement[i, 1]);
                DateTime thisDate = DateTime.Parse(announcement[i, 2]);
                ann.Announcement_Date = thisDate;
                ann.Announcement = announcement[i, 3];
                batchOperation.Insert(ann); //collect the data
            }

            try
            {
                results = tableinfo.ExecuteBatchAsync(batchOperation).Result;
                return View(results);
            }
            catch (Exception ex)
            {
                ViewBag.msg = "Error: " + ex.ToString();
            }
           
            return View();
        }

        public ActionResult SearchAnnouncement()
        {
            CloudTable tableinfo = getTableStorageInformation();

            string message = null;

            try
            {
                TableQuery<AnnouncementEntity> query = new TableQuery<AnnouncementEntity>();
                List<AnnouncementEntity> announcementList = new List<AnnouncementEntity>();
                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<AnnouncementEntity> queryResult = tableinfo.ExecuteQuerySegmentedAsync(query, token).Result;
                    token = queryResult.ContinuationToken;
                    foreach (AnnouncementEntity ann in queryResult.Results)
                    {
                        announcementList.Add(ann);
                    }
                } while (token != null);
                return View(announcementList);
            }
            catch(Exception ex)
            {
                message = "Unable to get the data. Error : " + ex.ToString();
            }
            return RedirectToAction("SearchAnnouncement", "Tables", new { Message = message });
        }


        public ActionResult getSingleEntity(string PartitionName, string RowName)
        {
            CloudTable tableinfo = getTableStorageInformation();

            string message = null;
            try
            {
                TableOperation retrieve = TableOperation.Retrieve<AnnouncementEntity>(PartitionName, RowName);
                TableResult result = tableinfo.ExecuteAsync(retrieve).Result;
                if (result.Etag != null)
                {
                    //convert the announcement info from tableresult to become announcement entity type
                    var announcement = result.Result as AnnouncementEntity;
                    return View(announcement);
                }

                else
                {
                    message = "Data not found in the table!";
                }

            }
            catch (Exception ex)
            {
                message = "Unable to get the data. Error : " + ex.ToString();
            }

            return RedirectToAction("SearchAnnouncement", "Tables", new { Message = message });
        }

        public ActionResult CustomerGetAnnouncement(string Message = null)
        {
            ViewBag.msg = Message;
            return View();
        }


        /*public ActionResult getAll(string PartitionName, string RowName)
        {
            CloudTable tableinfo = getTableStorageInformation();

            string message = null;
            try
            {
                TableOperation retrieve = TableOperation.Retrieve<AnnouncementEntity>(PartitionName, RowName);
                TableResult result = tableinfo.ExecuteAsync(retrieve).Result;
                if (result.Etag != null)
                {
                    //convert the announcement info from tableresult to become announcement entity type
                    var announcement = result.Result as AnnouncementEntity;
                    return View(announcement);
                }

                else
                {
                    message = "Data not found in the table!";
                }

            }
            catch (Exception ex)
            {
                message = "Unable to get the data. Error : " + ex.ToString();
            }

            return RedirectToAction("CustomerGetAnnouncement", "Tables", new { Message = message });
        }*/

        public ActionResult getAll(string PartitionKey)
        {
            CloudTable tableinfo = getTableStorageInformation();

            string message = null;
            try
            {
                var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey);
                var query = new TableQuery<AnnouncementEntity>().Where(condition);

                List<AnnouncementEntity> announcementList = new List<AnnouncementEntity>();
                TableContinuationToken token = null;

                do
                {
                    TableQuerySegment<AnnouncementEntity> queryResult = tableinfo.ExecuteQuerySegmentedAsync(query, token).Result;
                    token = queryResult.ContinuationToken;
                    foreach (AnnouncementEntity ann in queryResult.Results)
                    {
                        announcementList.Add(ann);
                    }
                } while (token != null);
                return View(announcementList);

            }
            catch (Exception ex)
            {
                message = "Unable to get the data. Error : " + ex.ToString();
            }

            return RedirectToAction("CustomerGetAnnouncement", "Tables", new { Message = message });
        }


        /*public async Task <ActionResult> DisplayAsync (string PartitionKey)
        {
       
            CloudTable tableinfo = getTableStorageInformation();

            string message = null;
            try
            {
                TableQuery<AnnouncementEntity> query = new TableQuery<AnnouncementEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey));
                List<AnnouncementEntity> announcementList = new List<AnnouncementEntity>();
                TableContinuationToken token = null;

                do
                {
                    TableQuerySegment<AnnouncementEntity> queryResult = tableinfo.ExecuteQuerySegmentedAsync(query, token).Result;
                    token = queryResult.ContinuationToken;
                    foreach (AnnouncementEntity ann in queryResult.Results)
                    {
                        announcementList.Add(ann);
                    }
                } while (token != null);
                return View(announcementList);

            }
            catch (Exception ex)
            {
                message = "Unable to get the data. Error : " + ex.ToString();
            }

            return RedirectToAction("CustomerGetAnnouncement", "Tables", new { Message = message });

        }*/
        public ActionResult DeleteEntity(string PartitionKey, string RowKey)
        {
            CloudTable tableinfo = getTableStorageInformation();
            string message = null;
            AnnouncementEntity deleteAnnouncement = new AnnouncementEntity(PartitionKey, RowKey) { ETag = "*" };

            try
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteAnnouncement);
                tableinfo.ExecuteAsync(deleteOperation);
                message = "Delete successful! " + PartitionKey + " " + RowKey + " is deleted from the table";
            }
            catch (Exception ex)
            {
                message = "Unable to delete the data due to error: " + ex.ToString();
            }

            return RedirectToAction("SearchAnnouncement", "Tables", new { Message = message });
        }


    }

}
