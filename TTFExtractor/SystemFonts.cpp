#include "stdafx.h"
#include "SystemFonts.h"

using namespace std;
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

#define CHECK_HR(x) {HRESULT hr = x; if (!SUCCEEDED(hr)) throw gcnew COMException("DWrite failure", hr);}

namespace GEOCOM::GNSDatashop::TTF
{
	CSystemFonts::CSystemFonts()
	{
		CComPtr<IDWriteFactory> dWriteFactory;

		CHECK_HR(DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(IDWriteFactory), reinterpret_cast<IUnknown**>(&dWriteFactory)));
		IDWriteGdiInterop* gdiInterop;
		CHECK_HR(dWriteFactory->GetGdiInterop(&gdiInterop));
		_gdiInterop = gdiInterop;	// Put to managed interor_ptr 

		_hdc = CreateCompatibleDC(GetDC(0));
	}

	CSystemFonts::~CSystemFonts()
	{
		_gdiInterop->Release();
		DeleteDC(_hdc);
	}

	UINT32 CSystemFonts::GetFileCount(IDWriteFontFace* face)
	{
		UINT32 count = 0;
		CHECK_HR(face->GetFiles(&count, NULL));
		return count;
	}

	UINT32 CSystemFonts::GetFaceFiles(IDWriteFontFace* face, IDWriteFontFilePtr** files)
	{
		UINT32 count = GetFileCount(face);
		if (0 < count)
		{
			UINT32 workCount = count;	// Paranoja;
			*files = new IDWriteFontFile*[workCount];
			CHECK_HR(face->GetFiles(&workCount, *files));
		}
		return count;
	}

	IEnumerable<String^>^ CSystemFonts::FaceFilePaths::get(IDWriteFontFace* face)
	{
		IDWriteFontFilePtr* pFiles = NULL;
		int count = GetFaceFiles(face, &pFiles);
		auto fileNames = gcnew List<String^>(count);
		for (IDWriteFontFile* pFile = (*pFiles + count - 1); (pFile >= *pFiles); pFile--)
		{
			const void* refKey = NULL;

			UINT32 refKeySize = 0;
			CHECK_HR(pFile->GetReferenceKey(&refKey, &refKeySize));

			IDWriteFontFileLoader* pLoader = NULL;
			CHECK_HR(pFile->GetLoader(&pLoader));
			if (NULL != pLoader)
			{
				IDWriteLocalFontFileLoader* pLocalFFLoader = NULL;
				if ((S_OK == pLoader->QueryInterface(&pLocalFFLoader)) && (NULL != pLocalFFLoader))
				{
					UINT32 pathSize = NULL;
					CHECK_HR(pLocalFFLoader->GetFilePathLengthFromKey(refKey, refKeySize, &pathSize));
					auto path = new wchar_t[++pathSize];
					CHECK_HR(pLocalFFLoader->GetFilePathFromKey(refKey, refKeySize, path, pathSize));
					fileNames->Add(gcnew String(path));

					pLocalFFLoader->Release();
					delete path;
				}
				pLoader->Release();
			}
			pFile->Release();
		}
		delete pFiles;
		
		fileNames->Reverse();

		return fileNames;
	}

	IEnumerable<String^>^ CSystemFonts::FaceFilePaths::get(CComPtr<::IFontDisp> &fontDisp)
	{
		CComPtr<IFont> font; font = fontDisp;
		HFONT hFont;
		CHECK_HR(font->get_hFont(&hFont));
		CComPtr<IDWriteFontFace> lpFace = FontFaceFromHandle(hFont);

		return FaceFilePaths[lpFace];
	}

	IEnumerable<String^>^ CSystemFonts::FaceFilePaths::get(stdole::IFontDisp^ fontDisp)
	{
		auto nativeFontDisp = (IFontDisp *) Marshal::GetIDispatchForObject(fontDisp).ToPointer();
		return FaceFilePaths[CComPtr<::IFontDisp>(nativeFontDisp)];
	}

	IEnumerable<String^>^ CSystemFonts::GetFaceFilePaths(String^ familyName, System::Drawing::FontStyle^ fontStyle)
	{
		int cHeight = -MulDiv(12, GetDeviceCaps(_hdc, LOGPIXELSY), 72);	// Use a standard 12 point font height
		HFONT hFont = CreateFont(cHeight, 0, 0, 0,
			(fontStyle->HasFlag(System::Drawing::FontStyle::Bold)) ? 700 : 400,
			fontStyle->HasFlag(System::Drawing::FontStyle::Italic),
			fontStyle->HasFlag(System::Drawing::FontStyle::Underline),
			fontStyle->HasFlag(System::Drawing::FontStyle::Strikeout),
			DEFAULT_CHARSET,
			OUT_TT_ONLY_PRECIS,
			CLIP_DEFAULT_PRECIS,
			ANTIALIASED_QUALITY,
			FF_DONTCARE,
			(LPCTSTR)Marshal::StringToHGlobalAnsi(familyName).ToPointer());

		CComPtr<IDWriteFontFace> lpFace = FontFaceFromHandle(hFont);

		DeleteObject(hFont);

		return FaceFilePaths[lpFace];
	}

	IDWriteFontFace* CSystemFonts::FontFaceFromHandle(HFONT hFont)
	{
		HGDIOBJ hPreviousFont = SelectObject(_hdc, hFont);
		CComPtr<IDWriteFontFace> face;
		CHECK_HR(_gdiInterop->CreateFontFaceFromHdc(_hdc, &face));
		SelectObject(_hdc, hPreviousFont);	// Paranoja

		return face.Detach();
	}
}