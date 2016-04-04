using SecretShareProject.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using sss;
using sss.config;
using sss.crypto.data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Identity;

namespace SecretShareProject.Controllers
{
    public class FileController : Controller
    {
        private CloudBlobClient blobStorage;
        private ApplicationDbContext db;
        private static string[] cloudStorageServices = new String[] { "amazon", "dropbox", "googledrive", "onedrive" };


        public ActionResult Upload()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Upload(FileUploadModel model)
        {
            if (ModelState.IsValid)
                if (model.minshares < model.numshares)
                {
                    if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        try
                        {
                            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                              ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
                            blobStorage = storageAccount.CreateCloudBlobClient();
                            db = ApplicationDbContext.Create();


                            String fileName = model.file.FileName;
                            String mimemap = MimeMapping.GetMimeMapping(fileName);
                            int numshares = model.numshares;
                            int minshares = model.minshares;

                            string[] sizes = { "B", "KB", "MB", "GB" };
                            double len = model.file.ContentLength;
                            int order = 0;
                            while (len >= 1024 && order + 1 < sizes.Length)
                            {
                                order++;
                                len = len / 1024;
                            }

                            string fileSize = String.Format("{0:0.#} {1}", len, sizes[order]);

                            BinaryReader b = new BinaryReader(model.file.InputStream);
                            byte[] binData = b.ReadBytes(model.file.ContentLength);
                            Share[] shares = byteArrToShares(binData, numshares, minshares);

                            FileInfoModel fileInfo = new FileInfoModel
                            {
                                fileName = fileName,
                                fileSize = fileSize,
                                mimetype = mimemap,
                                numshares = numshares,
                                minshares = minshares,
                                userID = User.Identity.GetUserId()
                            };

                            db.Files.Add(fileInfo);
                            db.SaveChanges();

                            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                            int i = 0;
                            foreach (Share s in shares)
                            {
                                CloudBlobContainer container = blobClient.GetContainerReference(cloudStorageServices[i]);
                                byte[] share = s.serialize();
                                string blobName = Guid.NewGuid().ToString();

                                ShareModel shareInfo = new ShareModel
                                {
                                    storageService = cloudStorageServices[i],
                                    shareName = blobName,
                                    fileId = fileInfo.Id
                                };
                                db.Shares.Add(shareInfo);
                                db.SaveChanges();
                                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                                blob.UploadFromByteArrayAsync(share, 0, share.Length);
                                i++;
                                if (i == 4)
                                {
                                    i = 0;
                                }
                            }
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    ViewBag.Message = "The number of shares cannot be less than the minimum number of shares";
                }
            return View();
        }

        public ActionResult ViewFiles()
        {
            if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string userid = User.Identity.GetUserId();
                db = ApplicationDbContext.Create();
                var FileModel = db.Files.Where(f => f.userID == userid);

                return View(FileModel.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Delete(String id)
        {
            if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                      ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
                blobStorage = storageAccount.CreateCloudBlobClient();

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                db = ApplicationDbContext.Create();

                var file = db.Files.Single(f => f.Id == id);

                var shares = db.Shares.Where(s => s.fileId == file.Id).ToList();

                foreach (ShareModel s in shares)
                {
                    CloudBlobContainer container = blobClient.GetContainerReference(s.storageService);
                    CloudBlockBlob blob = container.GetBlockBlobReference(s.shareName);
                    blob.FetchAttributes();
                    blob.DeleteAsync();
                }

                db.Shares.Where(s => s.fileId == file.Id).ToList().ForEach(s => db.Shares.Remove(s));
                db.SaveChanges();
                db.Files.Remove(file);
                db.SaveChanges();


                string userid = User.Identity.GetUserId();

                var FileModel = db.Files.Where(f => f.userID == userid);

                return View("ViewFiles", FileModel.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public FileContentResult Download(String id)
        {
            if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                      ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
                blobStorage = storageAccount.CreateCloudBlobClient();

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                db = ApplicationDbContext.Create();
                var fileModel = db.Files.Single(f => f.Id == id);

                var shareModel = db.Shares.Where(s => s.fileId == fileModel.Id).ToList();

                List<byte[]> data = new List<byte[]>();

                foreach (ShareModel s in shareModel)
                {
                    CloudBlobContainer container = blobClient.GetContainerReference(s.storageService);
                    CloudBlockBlob blob = container.GetBlockBlobReference(s.shareName);
                    blob.FetchAttributes();
                    long fileByteLength = blob.Properties.Length;
                    byte[] fileContent = new byte[fileByteLength];
                    blob.DownloadToByteArray(fileContent, 0);
                    data.Add(fileContent);
                }

                Share[] shares = new Share[fileModel.numshares];
                int j = 0;
                try
                {
                    data.ForEach(delegate (byte[] array)
                    {
                        shares[j++] = SerializableShare.deserialize(array);
                    });

                    byte[] file = sharesToByteArray(shares, fileModel.numshares, fileModel.minshares);
                    FileContentResult result = new FileContentResult(file, fileModel.mimetype)
                    {
                        FileDownloadName = fileModel.fileName
                    };
                    return result;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Share[] byteArrToShares(byte[] file, int numShares, int minShares)
        {
            int n = numShares, t = minShares;
            RandomSources r = RandomSources.SHA1;
            Encryptors e = Encryptors.ChaCha20;
            Algorithms a = Algorithms.CSS;
            Share[] shares = null;

            try
            {
                Facade f = new Facade(n, t, r, e, a);
                shares = f.split(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return shares;
        }

        public byte[] sharesToByteArray(Share[] shares, int numShares, int minShares)
        {
            int n = numShares, t = minShares;
            RandomSources r = RandomSources.SHA1;
            Encryptors e = Encryptors.ChaCha20;
            Algorithms a = Algorithms.CSS;
            byte[] output = null;

            try
            {
                Facade f = new Facade(n, t, r, e, a);
                output = f.join(shares);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return output;
        }
    }
}