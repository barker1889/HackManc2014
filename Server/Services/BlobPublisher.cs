using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Server.Services
{
    public class BlobPublisher
    {
        private const string AccountName = "busbuddyjb";
        private const string Key = "bhjJtqFd402czcamUKeHbj5oLvNE5SwZNZ4Ms7Tif6b8oQsCjQGCqB5MW6l0DwKv9KYg5fnMOXpE26yMSmrmbA==";

        public void SendToAzure(string filename, Stream stream)
        {
            var creds = new StorageCredentials(AccountName, Key);
            var account = new CloudStorageAccount(creds, useHttps: true);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer sampleContainer = client.GetContainerReference("buses");
            sampleContainer.CreateIfNotExists();
            sampleContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(filename);
            blob.UploadFromStream(stream);
        }
    }
}