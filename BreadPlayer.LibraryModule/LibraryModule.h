#pragma once
#include "Mediafile.h"
using namespace Windows::Storage;

namespace BreadPlayer_LibraryModule
{
    public ref class LibraryModule sealed
    {
    public:
		LibraryModule();
		Windows::Foundation::IAsyncOperation<MediafileCpp^>^ CreateMediafile(StorageFile^ file);
		Windows::Foundation::IAsyncOperation<bool>^ SaveImageAsync(StorageFile^ file, String^ destFile);
		Windows::Foundation::IAsyncOperation<bool>^ SaveImageAsync(Windows::Storage::FileProperties::StorageItemThumbnail^ thumb, String^ destFileName);

	};
}
