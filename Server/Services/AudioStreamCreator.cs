using System.IO;
using System.Speech.Synthesis;

namespace Server.Services
{
    public class AudioStreamCreator
    {
        readonly SpeechSynthesizer synthesiser;

        public AudioStreamCreator()
        {
            synthesiser = new SpeechSynthesizer();
        }

        public Stream CreateAudioStream(string text)
        {
            var audioStream = new MemoryStream();
            synthesiser.SetOutputToWaveStream(audioStream);
            synthesiser.Speak(text);
            synthesiser.Dispose();

            audioStream.Flush();
            audioStream.Position = 0;

            return audioStream;
        }
    }
}