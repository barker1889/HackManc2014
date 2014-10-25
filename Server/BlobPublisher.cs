using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Server
{
    public class BlobPublisher
    {
        private const string AccountName = "purpledragons";
        private const string Key = "cTjll+/f3R3EaHTNm9qs33NbpuZS05U73hFlkH4uSi/lGCRb0VzGnHtuuo5M2PDHfTbGKpHRj+yW5aA49YyWCw==";

        public BlobPublisher()
        {

        }

        public void SendToAzure(string filename, Stream stream)
        {
            var creds = new StorageCredentials(AccountName, Key);
            var account = new CloudStorageAccount(creds, useHttps: true);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer sampleContainer = client.GetContainerReference("buses");
            sampleContainer.CreateIfNotExists();

            CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(filename);
            blob.UploadFromStream(stream);
        }
    }
}