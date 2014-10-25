using System;

namespace Server
{
    public class AudioPublisher
    {
        private const string AzureStorageBaseUrl = "https://purpledragons.blob.core.windows.net/buses/";

        public AudioPublisher()
            : this(new BlobPublisher(), new AudioStreamCreator())
        {

        }
        public AudioPublisher(BlobPublisher publisher, AudioStreamCreator streamCreator)
        {

        }

        public string GenerateFileAndPublish(string textToConvert)
        {
            var filename = Guid.NewGuid() + ".wav";

            var streamCreator = new AudioStreamCreator();
            var audioStream = streamCreator.CreateAudioStream(textToConvert);

            var publisher = new BlobPublisher();
            publisher.SendToAzure(filename, audioStream);

            return AzureStorageBaseUrl + filename;
        }

    }
}