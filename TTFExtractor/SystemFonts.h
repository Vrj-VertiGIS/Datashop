#pragma once
#include "stdafx.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;

namespace  GEOCOM::GNSDatashop::TTF
{
	public ref class CSystemFonts
	{
	private:
		typedef IDWriteFontFile* IDWriteFontFilePtr;	// Keep some indirections a little bit simpler

		UINT32 GetFaceFiles(IDWriteFontFace* face, IDWriteFontFilePtr** files);
		UINT32 GetFileCount(IDWriteFontFace* face);

		IDWriteFontFace* FontFaceFromHandle(HFONT hFont);

		IDWriteGdiInterop* _gdiInterop = NULL;
		HDC _hdc;

	public:
		CSystemFonts();
		~CSystemFonts();

		property IEnumerable<String^>^ FaceFilePaths[IDWriteFontFace*]
		{ IEnumerable<String^>^ get(IDWriteFontFace* face); };

		property IEnumerable<String^>^ FaceFilePaths[CComPtr<::IFontDisp> &]
		{ IEnumerable<String^>^ get(CComPtr<IFontDisp> &fontDisp); };

		property IEnumerable<String^>^ FaceFilePaths[stdole::IFontDisp^]
		{ IEnumerable<String^>^ get(stdole::IFontDisp^ fontDisp); };

		IEnumerable<String^>^ GetFaceFilePaths(String^ familyName, System::Drawing::FontStyle^ fontStyle);
	};
}