#define _CRT_SECURE_NO_DEPRECATE
#include "pch.h"
#include "LibraryModule.h"
#include "Mediafile.h"

using namespace BreadPlayer_LibraryModule;
using namespace Platform;
using namespace std;
using namespace Concurrency;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

LibraryModule::LibraryModule()
{
}
Windows::Foundation::IAsyncOperation<MediafileCpp^>^ LibraryModule::CreateMediafile(StorageFile^ file)
{
	return create_async([this, file]() -> MediafileCpp^
	{
		//auto file = create_task(StorageFile::GetFileFromPathAsync(filePath)).get();
		MediafileCpp^ mediaFile = ref new MediafileCpp();
		mediaFile->FilePath = file->Path;
		
			Windows::Storage::FileProperties::MusicProperties^ properties = create_task(file->Properties->GetMusicPropertiesAsync()).get();
			mediaFile->Title = properties->Title;
			mediaFile->Artist = properties->Artist;
			mediaFile->Album = properties->Album;
			mediaFile->Year = properties->Year;
			mediaFile->Length = properties->Duration;
			mediaFile->Genre = properties->Genre;
		
		return mediaFile;
	});
}

Windows::Foundation::IAsyncOperation<bool>^ LibraryModule::SaveImageAsync(StorageFile^ file, String^ destFileName)
{
	return create_async([this, file, destFileName]() -> bool
	{
		try {
			
			
			auto albumartFolder = ApplicationData::Current->LocalFolder;
			StorageFile^ destStorageFile = create_task(albumartFolder->CreateFileAsync("\\AlbumArts\\" + destFileName + ".jpg")).get();

			TagLibUWP::TagManager^ manager = ref new TagLibUWP::TagManager();
			TagLibUWP::AudioFileInfo^ tagFile = manager->ReadFile(file);
			if(tagFile->Tag->Image != nullptr)
			create_task(FileIO::WriteBytesAsync(destStorageFile, tagFile->Tag->Image->Data)).get();
			
			return true;
		}
		catch (Exception^  ex)
		{
			return false;
		}
	});
}
Windows::Foundation::IAsyncOperation<bool>^ LibraryModule::SaveImageAsync(Windows::Storage::FileProperties::StorageItemThumbnail^ thumb, String^ destFileName)
{
	return create_async([this, thumb, destFileName]() -> bool
	{
		try {

			if (thumb->Size > 0) 
			{
				auto albumartFolder = ApplicationData::Current->LocalFolder;
				StorageFile^ destStorageFile = create_task(albumartFolder->CreateFileAsync("\\AlbumArts\\" + destFileName + ".jpg")).get();

				IBuffer^ buf;
				Buffer^ inputBuffer = ref new Buffer(thumb->Size);
				while((buf = create_task(thumb->ReadAsync(inputBuffer, inputBuffer->Capacity, InputStreamOptions::None)).get())->Length > 0)
				{
					create_task(FileIO::WriteBufferAsync(destStorageFile, buf)).get();
				}
				
			}
			
			return true;
		}
		catch (COMException^ ex)
		{
			return false;
		}
	});
}