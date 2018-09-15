using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AudioToolbox;
using AzureAITestMobile.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace AzureAITestMobile.iOS
{
    public class AudioService : IAudioService
    {
        OutputAudioQueue _queue;

        public AudioService()
        {
        }

        public unsafe Task PlayOnce(System.IO.Stream stream)
        {
            return Task.Run(() =>
            {
                try
                {
                    int sampleRate = 16000;
                    uint channels = 1;
                    uint bitsPerSample = 16;

                    if (_queue != null)
                    {
                        _queue.BufferCompleted -= Queue_BufferCompleted;
                        _queue.Stop(true);
                    }

                    var format = AudioStreamBasicDescription.CreateLinearPCM(sampleRate, channels, bitsPerSample);
                    _queue = new OutputAudioQueue(format);
                    _queue.BufferCompleted += Queue_BufferCompleted;
                    _queue.Volume = 1;

                    var buffer1 = new byte[stream.Length];
                    stream.Read(buffer1, 0, buffer1.Length);
                    _queue.AllocateBuffer(buffer1.Length, out AudioQueueBuffer* buffer);

                    GCHandle pinned = GCHandle.Alloc(buffer1, GCHandleType.Pinned);
                    IntPtr address = pinned.AddrOfPinnedObject();
                    buffer->CopyToAudioData(address, buffer1.Length);
                    buffer->AudioDataByteSize = (uint)buffer1.Length;

                    _queue.EnqueueBuffer(buffer, null);
                    _queue.Start();
                }
                catch (Exception ex)
                {

                }
            });
        }

        private void Queue_BufferCompleted(object sender, BufferCompletedEventArgs e)
        {
            _queue?.Stop(true);
        }
    }
}