using System;
using System.IO;
using System.Media;


namespace CybersecurityAwarenessBot
{
    public class VoiceGreeting
    {
        public void PlayGreeting()
        {
            //locate the WAV file
            string baseDir = AppDomain.CurrentDomain.BaseDirectory + "Audio\\greeting.wav";
            //Check that the file exists
            if (File.Exists(baseDir))
            {
                Console.WriteLine("The file exists.");
            }
            else
            {
                Console.WriteLine("The file does not exist.");
            }
                //Create a SoundPlayer
                SoundPlayer playSound = new SoundPlayer(baseDir);
            //Play the sound method
            playSound.PlaySync();
        }
    }
}

