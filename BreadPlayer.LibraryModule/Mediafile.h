using namespace Platform;
#pragma once
namespace BreadPlayer_LibraryModule
{
	public ref class MediafileCpp sealed
	{
	public:
		MediafileCpp();

		property String^ FilePath;
		property String^ Title;
		property String^ Artist;
		property String^ Album;
		property Windows::Foundation::TimeSpan Length;
		property unsigned int Year;
		property Windows::Foundation::Collections::IVector<String^>^ Genre;
	};
}


