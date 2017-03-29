#pragma once
using namespace Platform;
namespace BreadPlayer_LibraryModule
{
	ref class StringExtensions sealed
	{
	public:
		StringExtensions();
		static String ToSha1(String^ text);
		static Array<byte> ToBytes(String^ text);
		static String ToHex(Array<byte> bytes);

	};
}

