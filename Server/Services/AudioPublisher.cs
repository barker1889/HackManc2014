namespace Server.Services
{
    public class AudioPublisher
    {
        private readonly BlobPublisher _publisher;
        private readonly AudioStreamCreator _streamCreator;
        private const string AzureStorageBaseUrl = "https://purpledragons.blob.core.windows.net/buses/";

        //public AudioPublisher()
        //    : this(new BlobPublisher(), new AudioStreamCreator())
        //{

        //}

        public AudioPublisher(BlobPublisher publisher, AudioStreamCreator streamCreator)
        {
            _publisher = publisher;
            _streamCreator = streamCreator;
        }

        public string GenerateFileAndPublish(string filename, string textToConvert)
        {
            var audioStream = _streamCreator.CreateAudioStream(textToConvert);

            _publisher.SendToAzure(filename, audioStream);

            return AzureStorageBaseUrl + filename;
        }

    }
}