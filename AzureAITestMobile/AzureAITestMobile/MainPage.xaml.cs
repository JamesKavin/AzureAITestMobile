using AzureAITestMobile.Services;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AzureAITestMobile
{
	public partial class MainPage : ContentPage
    {
        byte[] _imageContent;

        public MainPage()
		{
			InitializeComponent();            
		}


        private async Task SayImgDesc()
        {
            string desc;

            if (Img.Source is UriImageSource uri)
            {
                var imgUrl = uri.Uri.ToString();

                if (string.IsNullOrEmpty(imgUrl))
                {
                    return;
                }

                desc = await Task.Run(async () => await new ComputerVisionService().GetImageDescription(imgUrl));
            }
            else if ((Img.Source is FileImageSource file || Img.Source is StreamImageSource stream1) && _imageContent != null)
            {
                desc = await Task.Run(async () => await new ComputerVisionService().GetImageDescription(_imageContent));
            }
            else return;

            DescriptionEN.Text = desc;

            var text = await Task.Run(async () => await new TranslatorTextService().Translate(desc));
            DescriptionPL.Text = text;

            var stream = await Task.Run(async () => await new BingSpeechService().Speak(text));
            var audioService = DependencyService.Get<IAudioService>();
            await audioService.PlayOnce(stream);
        }

        private async void DescribeBtn_Clicked(object sender, EventArgs e)
        {
            ChooseImageBtn.IsVisible = false;
            DescribeBtn.IsVisible = false;
            LoadingBar.IsVisible = true;

            await SayImgDesc();

            ChooseImageBtn.IsVisible = true;
            DescribeBtn.IsVisible = true;
            LoadingBar.IsVisible = false;
        }

        private async void ChooseImageBtn_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("Choose photo", "Cancel", "", "Choose from library", "Take a photo");

            if (result == "Choose from library")
            {
                await ChoosePhoto();
            }
            else
            {
                await TakePhoto();
            }
        }

        private async Task ChoosePhoto()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;

                Img.Source = ImageSource.FromFile(fileData.FilePath);

                _imageContent = fileData.DataArray;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        async Task TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "AzureAiSample",
                Name = "test.jpg"
            });

            if (file == null)
                return;
            
            Img.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                _imageContent = bytes;
                stream.Position = 0;
                return stream;
            });
        }
    }
}
