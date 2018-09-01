# Testing Azure Cognitive Services

This repository contains mobile app made in Xamarin.Forms. It's purpose is to present a few of Azure Cognitive Services:
- Computer Vision
- Translate Text
- Bing Speech

In the app you can choose a photo or take one. Then you can tap "Describe image" to get image description (from Computer Vision service) which gets translated from English to Polish (Translate Text service) and is read aloud (Bing Speech service).
All services use Rest Api.

# How to use

1. You have to have Xamarin installed.
2. Download repo.
3. You need to generate in your own Azure account keys for the 3 used services. There are free tiers for all of them. Services in Azure Portal are named the same as noted above, so just create them with Free Tier and go to Keys tab for each of them and copy the primary key.
4. Add secrets.json file to the core library. There is secrets.json.template, you can use it as a base. Add your keys to that file.
5. Run the app.

On Android you can use emulator or device. On iOS you would need device to take picture as camera is not available in simulator. Running app on device would require generating Provisioning Profile with App Id with iCloud feature enabled (library for file picker requires it) in your Apple Developer Account.
