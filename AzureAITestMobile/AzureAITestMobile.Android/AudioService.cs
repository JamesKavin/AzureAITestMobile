using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AzureAITestMobile.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace AzureAITestMobile.Droid
{
    public class AudioService : IAudioService
    {
        AudioTrack _playOnceAudioTrack;
        public async Task PlayOnce(System.IO.Stream stream)
        {
            await Task.Run(() =>
            {
                try
                {
                    int sampleRate = 16000;
                    var channel = ChannelOut.Mono;
                    var encoding = Android.Media.Encoding.Pcm16bit;
                    var buffSize = AudioTrack.GetMinBufferSize(sampleRate, channel, encoding);

                    if (_playOnceAudioTrack == null)
                    {
                        _playOnceAudioTrack = new AudioTrack(Stream.Music, sampleRate, channel, encoding, buffSize, AudioTrackMode.Stream);
                    }

                    _playOnceAudioTrack.Stop();
                    _playOnceAudioTrack.Flush();

                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    _playOnceAudioTrack.Play();
                    _playOnceAudioTrack.Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {

                }
            });
        }
    }
}