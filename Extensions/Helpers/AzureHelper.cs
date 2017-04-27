
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage
{
    public static class AzureHelper
    {

        static AzureHelper()
        {
            _url = ConfigurationManager.AppSettings["AzureUri"];
            _user = ConfigurationManager.AppSettings["AzureUser"];
            _pass = ConfigurationManager.AppSettings["AzureKey"];
        }



        private static CloudBlobClient GetBlobClient()
        {
            try
            {
                // return AzureHelper.GetBlobClient();
                var credentials = new StorageCredentials(_user, _pass);
                //var storage = new StorageCredentialsAccountAndKey(_user, _pass);

                return new CloudBlobClient(new Uri(_url), credentials);

            }
            catch (Exception e)
            {
                throw new Exception("Error when connecting to Azure , please try again", e);
            }
        }

        public static bool ExistBlob(string packageFileName)
        {

            try
            {
                packageFileName = packageFileName.Replace("\\", "/");

                var blobClient = GetBlobClient();

                // var blob = blobClient.GetBlockBlobReference(packageFileName);

                var blob = blobClient.GetContainerReference(containerName)
                    .GetBlockBlobReference(packageFileName);


                blob.FetchAttributes();

                return blob.Properties.Length > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }


        private static CloudBlobContainer GetContainer(string containerName)
        {
            // containerName = Packages.DecorateName(containerName);
            return GetBlobClient().GetContainerReference(containerName);
        }

        private static List<CloudBlobDirectory> GetCloudBlobDirectories(string containerName, string path = "")
        {

            var container = GetContainer(containerName);

            if (!string.IsNullOrEmpty(path))
            {
                return container.GetDirectoryReference(path).ListBlobs()
                    .ToList().OfType<CloudBlobDirectory>().ToList();
            }

            return container.ListBlobs().ToList().OfType<CloudBlobDirectory>().ToList();
        }




        public static bool UploadImage(string sourceImageFilename)
        {

            try
            {

                var imageFilename = GetAzureImageFormat(sourceImageFilename);

                if (ExistBlob(imageFilename)) return true;

                var blobClient = GetBlobClient();

                var container = blobClient.GetContainerReference(containerName);

                container.CreateIfNotExists();

                var blob = blobClient.GetBlobReferenceFromServer(new Uri(imageFilename));

                var content = File.ReadAllBytes(sourceImageFilename);
                using (var stream = new MemoryStream(content))
                {
                    blob.UploadFromStream(stream);
                }

                return true;
            }
            catch (Exception ex)
            {
                //TODO log Exceptions.
                return false;
            }

        }

        public static async Task DeleteImage(string filepath)
        {


            if (!ExistBlob(filepath))
            {
                filepath = GetAzureImageFormat(filepath);
                if (!ExistBlob(filepath)) return;
            }

            var blobClient = GetBlobClient();

            var container = blobClient.GetContainerReference(containerName);

            var blob = blobClient.GetBlobReferenceFromServer(new Uri(filepath));

            await blob.DeleteAsync();


        }

        #region Async Upload


   

        /// <summary>
        /// Overwrite file By default .
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="allowOverWriteFile"></param>
        /// <returns></returns>
        public static async Task<string> UploadFileAsync(string path, Stream stream,bool allowOverWriteFile = true)
        {
            return await Task.Run(() => Upload(path, stream,allowOverWriteFile));
        }
        public static string Upload(string path, Stream stream,bool allowOverWriteFile = true)
        {

            try
            {
                var imageFilename = GetAzureImageFormat(path);

                if (!allowOverWriteFile && ExistBlob(imageFilename)) return null;

                var blobClient = GetBlobClient();

                var container = blobClient.GetContainerReference(containerName);

                container.CreateIfNotExists();

                var b = blobClient.GetContainerReference(containerName)
                    .GetBlockBlobReference(path.Replace("\\", "/").ToLower());

                // var blob = blobClient.GetBlobReferenceFromServer(new Uri(imageFilename));
                b.UploadFromStream(stream);
                return imageFilename;

            }
            catch (Exception ex)
            {
                //TODO log Exceptions.
                return null;
            }

        }

        public static async Task RemoveFileAsync(string f, string customContainerName = "eviction")
        {
            try
            {
                var imageFilename = GetAzureImageFormat(f,customContainerName);
                
                var blobClient = GetBlobClient();


                var b = blobClient.GetContainerReference(customContainerName ?? containerName)
                    .GetBlockBlobReference(f.Replace("\\", "/").ToLower());


                await b.DeleteAsync() ;

            }
            catch (Exception ex)
            {

            }
        }
        public static  void RemoveFile(string f, string customContainerName = "eviction")
        {
            try
            {
                var imageFilename = !f.Contains("http")?  GetAzureImageFormat(f, customContainerName) : f.Split('/').LastOrDefault();

                var blobClient = GetBlobClient();


                var b = blobClient.GetContainerReference(customContainerName ?? containerName)
                    .GetBlockBlobReference(f.Replace("\\", "/").ToLower());


                b.Delete();

            }
            catch (Exception ex)
            {

            }
        }

        public static void Upload(string fullPath, Stream content,string customContainerName =null, bool allowOverWriteFile = true)
        {
            try
            {
                if (!allowOverWriteFile && ExistBlob(fullPath)) return;

                var blobClient = GetBlobClient();

                var container = blobClient.GetContainerReference(customContainerName?? containerName);

                container.CreateIfNotExists();

                var b = blobClient.GetContainerReference(customContainerName?? containerName).GetBlockBlobReference(fullPath);

                b.UploadFromStream(content);
                


            }
            catch (Exception ex)
            {
                //TODO log Exceptions.

            }
        }
        #endregion


        public static string GetAzureImageFormat(string filename,string customContainerName=null)
        {
            var file = string.Format("{0}{1}/{2}", _url, customContainerName?? containerName, filename);

            return file.Replace("\\", "/").ToLower();
        }

        public static bool ExistsFile(string path)
        {
            var imageFilename = GetAzureImageFormat(path);

            return ExistBlob(imageFilename);
        }

        private static string FriendlyName(this string value)
        {
            return value.Replace("/", "").Replace("-", " ");
        }

        public static string PathCache
        {
            get
            {
                return string.Empty;
                var cache = ConfigurationManager.AppSettings["LocalSourceCache"];



                return string.IsNullOrEmpty(cache) ? cache : "";
            }
        }


        public static  async Task< string> UploadDocumentAsync(Stream content,string fileName, string customContainerName="eviction")
        {
            var azurePath = GetAzureImageFormat(fileName, customContainerName);

            await Task.Run(() => Upload(fileName.ToLower(), content, customContainerName));
                                                 
            return azurePath;     
        }
         
        
        private static CloudBlockBlob GetEvictionDocumentBlob(string fullfileName ,string customContainerName ="eviction")
        {
            var fileName = fullfileName;

         
            if (!fullfileName.Contains("http") && !fullfileName.Contains("Templates"))
            {
                var pathParts = fullfileName.Split('/').ToList();

                var name = pathParts.LastOrDefault();

                pathParts.Remove(name);

                  fileName = string.Format("{0}/{1}", pathParts.LastOrDefault(), name);
            }
            else
            {
                fileName= fullfileName.Replace("/"+customContainerName+"/", "*").Split('*').LastOrDefault()?? fileName;
                 
            }
            var blobClient = GetBlobClient();
            
            var container = blobClient.GetContainerReference(customContainerName ?? containerName);

           return container.GetBlockBlobReference(fileName);
        }

      
        public static string GetSharedAccessFor(string fullfileName ,string customContainerName = "eviction")
        {
            if (string.IsNullOrEmpty(fullfileName)) return string.Empty;

            var blob = GetEvictionDocumentBlob(fullfileName, customContainerName);
            //Set the expiry time and permissions for the blob.
            //In this case the start time is specified as a few minutes in the past, to mitigate clock skew.
            //The shared access signature will be valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;

        }

        public static async Task<Stream> GetFileContentAsync(string fullfileName, string customContainerName = "files")
        {
            var blob = GetEvictionDocumentBlob(fullfileName, customContainerName);

            var memory = new MemoryStream();
            await blob.DownloadToStreamAsync(memory);

            memory.Position = 0;
            return memory;
        }

        public static async Task<byte[]> GetFileByteArrayAsync(string fullfileName,string customContainerName="files")
        {
            
           
            try
            {
                var blob = GetEvictionDocumentBlob(fullfileName, customContainerName);
                await blob.FetchAttributesAsync();


                byte[] content = new byte[blob.Properties.Length];
                for (int i = 0; i < blob.Properties.Length; i++)
                {
                    content[i] = 0x20;
                }

                await blob.DownloadToByteArrayAsync(content, 0);

                return content;
            }
            catch(Exception ex)
            {
                // TODO log this
                return null;
            }

        }


        public static string GetLicenseeLogo(int licenseeId)
        {
            return string.Empty;
        }

          
         

        public static string containerName = ConfigurationManager.AppSettings["containerName"];

        
        //private const string containerName = "parrasStore";
        private static string _user = string.Empty;
        private static string _pass = string.Empty;
        private static string _url = string.Empty;

    }
}
