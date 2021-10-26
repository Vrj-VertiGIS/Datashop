#include "stdafx.h"
#include "Glyph.h"

using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;
using namespace netDxf;
using namespace netDxf::Entities;

namespace  GEOCOM::GNSDatashop::TTF
{

#define FIXED2FLOAT(f) (f.fract / 65536.0) + f.value

#if _WIN64
#define INTPTRTOHANDLE(h) (HDC)h->ToInt64()
#else
#define INTPTRTOHANDLE(h) (HDC)h->ToInt32()
#endif

	// Protected
	CGlyph::CGlyph()
	{
		_hatchBoundaries = gcnew List<HatchBoundaryPath^>();
		_lpgm = new GLYPHMETRICS;	// Cannot declare unmanaged struct in managed class
	}

	// Protected
	CGlyph::CGlyph(IntPtr^ hdc, double sizeInDots)
		: CGlyph()
	{
		_hCdc = INTPTRTOHANDLE(hdc);

		_sizeInDots = sizeInDots;	

		_logPixelsX = GetDeviceCaps(_hCdc, LOGPIXELSX);
		_logPixelsY = GetDeviceCaps(_hCdc, LOGPIXELSY);
	}

	CGlyph::CGlyph(IntPtr^ hdc, unsigned int character, double sizeInDots, GlyphAlignment glyphAlignment, double offsetXDots, double offsetYDots, double rotation)
		: CGlyph(hdc, sizeInDots)
	{
		_glyphAlignment = glyphAlignment;

		InitRotation(rotation);
		InitAnchor(Vector2(-offsetXDots, -offsetYDots));
		InitOffset(0, 0);

		LPTTPOLYGONHEADER lpPh = NULL;

		size_t bufSize = GetGlyphData(character, &lpPh, RotationMatrixZero());	// Do not apply rotation to glyp selected by caller

		ProcessGlyphData(lpPh, bufSize);

		free(lpPh);
	}

	CGlyph::CGlyph(IntPtr^ hdc, unsigned int character, String^ familyName, String^ style, double sizeInDots, GlyphAlignment glyphAlignment, 
		Vector2 anchorPointOffset, double offsetXDots, double offsetYDots, double rotation)
		: CGlyph(hdc, sizeInDots)
	{
		_glyphAlignment = glyphAlignment;

		InitRotation(rotation);
		InitAnchor(anchorPointOffset);
		InitOffset(offsetXDots, offsetYDots);

		HGDIOBJ hFont = CreateFontFromName(familyName, style, sizeInDots);
		if (0 != hFont)
		{
			LPTTPOLYGONHEADER lpPh = NULL;

			size_t bufSize = GetGlyphData(character, &lpPh, hFont, RotationMatrixZero()); 
			ProcessGlyphData(lpPh, bufSize);
			DeleteObject(hFont);

			free(lpPh);
		}
	}

	CGlyph::CGlyph(IntPtr^ hdc, unsigned int character, String^ familyName, FontStyle style, double sizeInDots, GlyphAlignment glyphAlignment,
		Vector2 anchorPointOffset, double offsetXDots, double offsetYDots, double rotation)
		: CGlyph(hdc, sizeInDots)
	{
		_glyphAlignment = glyphAlignment;

		InitRotation(rotation);
		InitAnchor(anchorPointOffset);
		InitOffset(offsetXDots, offsetYDots);

		HGDIOBJ hFont = CreateFontFromName(familyName, style, sizeInDots);
		if (0 != hFont)
		{
			LPTTPOLYGONHEADER lpPh = NULL;

			size_t bufSize = GetGlyphData(character, &lpPh, hFont, RotationMatrixZero());
			ProcessGlyphData(lpPh, bufSize);
			DeleteObject(hFont);

			free(lpPh);
		}
	}

	CGlyph::~CGlyph()
	{
		if (NULL != _lpgm)
			delete _lpgm;	// New'ed in the constructor
		if (nullptr != _lpOutlineTextMetric)
			free(_lpOutlineTextMetric);
	}

	size_t CGlyph::GetGlyphData(unsigned int character, LPTTPOLYGONHEADER* lppPolygonHeaders, HGDIOBJ hFont, MAT2* rotMat)
	{
		HGDIOBJ replacedFont = SelectObject(_hCdc, hFont);
		if (replacedFont == 0)
			return 0;

		size_t bufSize = GetGlyphData(character, lppPolygonHeaders, rotMat);

		SelectObject(_hCdc, replacedFont);

		return bufSize;
	}

	// Get glyph data - glyph/font design-time EM square size (!)
	size_t CGlyph::GetGlyphData(unsigned int character, LPTTPOLYGONHEADER* lppPolygonHeaders, MAT2* rotMat)
	{
		auto lpOutlineTextMetric = GetTextMetrics(_hCdc);
		_emSquare = lpOutlineTextMetric->otmEMSquare;
		free(lpOutlineTextMetric);

		HGDIOBJ hOgirinalFont = GetCurrentObject(_hCdc, OBJ_FONT);
		if (NULL == hOgirinalFont)
			return 0;

		LOGFONT logFont;
		if (GetObject(hOgirinalFont, sizeof(LOGFONT), &logFont) == 0)
			return 0;

		// Create a font _emSquare device units (pixels) heigh. This allows to interpret
		// all point cuve geometry values in em units. No conversion from device units to em required.
		logFont.lfHeight = (-1) * _emSquare;
		logFont.lfEscapement = 0; 
		logFont.lfOrientation = 0;

		HGDIOBJ hEMFont = CreateFontIndirect(&logFont);
		if (hEMFont == 0)
			return 0;

		HGDIOBJ replacedFont = SelectObject(_hCdc, hEMFont);
		if (replacedFont == 0)
			return 0;

		// Get font metric data
		_lpOutlineTextMetric = GetTextMetrics(_hCdc);
		_lpTextMetric = &(_lpOutlineTextMetric->otmTextMetrics);

		// Need to get glyphmetrics in advance - as they are incorrect when retrieved in the same call
		// when retrieving the font outline.
		if (GDI_ERROR == GetGlyphOutline(_hCdc, character, GGO_METRICS, _lpgm, 0, NULL, rotMat))
			return 0;

		GLYPHMETRICS gm;
		size_t bufSize = GetGlyphOutline(_hCdc, character, GGO_NATIVE, &gm, 0, NULL, rotMat);
		if ((bufSize == 0) || (bufSize == GDI_ERROR))
			return 0;

		*lppPolygonHeaders = (LPTTPOLYGONHEADER)malloc(bufSize);
		if (lppPolygonHeaders == NULL)
			return 0;

		bufSize = GetGlyphOutline(_hCdc, character, GGO_NATIVE, &gm, (UINT)bufSize, *lppPolygonHeaders, rotMat);

		SelectObject(_hCdc, replacedFont);

		return bufSize;
	}

	void CGlyph::ProcessGlyphData(LPTTPOLYGONHEADER lpPolygonHeaders, size_t bufsize)
	{
		_hatchBoundaries->Clear();

		LPTTPOLYGONHEADER lpPgHTop = (LPTTPOLYGONHEADER)((BYTE*)lpPolygonHeaders + bufsize);
		for (LPTTPOLYGONHEADER lpPgH = lpPolygonHeaders; (lpPgH < lpPgHTop); lpPgH = (LPTTPOLYGONHEADER)((BYTE*)lpPgH + lpPgH->cb))
		{
			List<EntityObject^>^ contour = gcnew List<EntityObject^>();

			Vector2 prevP = ToDots(lpPgH->pfxStart);
			Vector2 contourStartPoint = prevP;	// We need this to be able to close the contour in case.

			LPTTPOLYCURVE lpCurve = (LPTTPOLYCURVE)((BYTE*)lpPgH + sizeof(TTPOLYGONHEADER));
			LPTTPOLYCURVE lpCurveTop = (LPTTPOLYCURVE)((BYTE*)lpPgH + lpPgH->cb);

			while (lpCurve < lpCurveTop)
			{
				LPPOINTFX ptTop = lpCurve->apfx + lpCurve->cpfx;
				switch (lpCurve->wType)
				{
				case TT_PRIM_LINE:
					for (LPPOINTFX pt = lpCurve->apfx; (pt < ptTop); pt++)
					{
						Vector2 lineP = ToDots(pt);
						contour->Add(gcnew Line(prevP, lineP));
						prevP = lineP;
					}
					break;
				case TT_PRIM_CSPLINE:
				case TT_PRIM_QSPLINE:
					List<SplineVertex^>^ path = gcnew List<SplineVertex^>();
					path->Add(gcnew SplineVertex(prevP));
					for (LPPOINTFX pt = lpCurve->apfx; (pt < ptTop); pt++)
					{
						prevP = ToDots(pt);
						path->Add(gcnew SplineVertex(prevP));
					}
					contour->Add(gcnew Spline(path, (short)2));
					break;
				}
				lpCurve = (LPTTPOLYCURVE)((BYTE*)lpCurve + sizeof(TTPOLYCURVE) + (lpCurve->cpfx - 1) * sizeof(POINTFX));
			}
			if (!prevP.Equals(contourStartPoint))
				contour->Add(gcnew Line(prevP, contourStartPoint));

			_hatchBoundaries->Add(gcnew HatchBoundaryPath(contour));
		}
	}

	// Point in dots (72'of inches), normalized, cell origin stripped)
	// We assume cell origin coordinates are fixed point (entire) values
	Vector2 CGlyph::ToDots(POINTFX point)
	{
		double x = FIXED2FLOAT(point.x);
		double y = FIXED2FLOAT(point.y);

		return ToDots(x, y);
	}

	Vector2 CGlyph::ToDots(double x, double y)
	{
		// Centering glyph blackbox on (0,0)
		// Center symbol on horizontal csymbol center and on vertical cell black box center.
		AlignGlyph(x, y);
		// Rotate the glyph data point by (0,0) - glyph center. The rotation by specifying a rotation
		// matrix as a parameter of CreateFont/CreateFontIndirect did not work as desired at the time
		// of writing this...
		RotateGlyph(x, y);

		auto emToDots = EmToDots;

		return Vector2(x * emToDots + _offsetXDots, y * emToDots + _offsetYDots);
	}

	Vector2 CGlyph::ToDots(LPPOINTFX lpPoint)
	{
		return ToDots(*lpPoint);
	}

	void CGlyph::AlignGlyph(double& x, double& y)
	{
		switch (_glyphAlignment)
		{
			case GlyphAlignment::bottomLeft:
				BottomAlignGlyph(y);
				LeftAlignGlyph(x);
				break;
			case GlyphAlignment::bottomCenter:
				BottomAlignGlyph(y);
				CenterAlignGlyph(x);
				break;
			case GlyphAlignment::bottomRight:
				BottomAlignGlyph(y);
				RightAlignGlyph(x);
				break;
			case GlyphAlignment::middleLeft:
				MiddleAlignGlyph(y);
				LeftAlignGlyph(x);
				break;
			case GlyphAlignment::middleCenter:
				MiddleAlignGlyph(y);
				CenterAlignGlyph(x);
				break;
			case GlyphAlignment::middleRight:
				MiddleAlignGlyph(y);
				RightAlignGlyph(x);
				break;

			// Horizontal alignments using full cell increment instead of glyph black box extent
			case GlyphAlignment::middleLeftFull:
				MiddleAlignGlyph(y);
				LeftAlignGlyphFull(x);
				break;
			case GlyphAlignment::middleCenterFull:
				MiddleAlignGlyph(y);
				CenterAlignGlyphFull(x);
				break;
			case GlyphAlignment::middleRightFull:
				MiddleAlignGlyph(y);
				RightAlignGlyphFull(x);
				break;

			case GlyphAlignment::topLeft:
				TopAlignGlyph(y);
				LeftAlignGlyph(x);
				break;
			case GlyphAlignment::topCenter:
				TopAlignGlyph(y);
				CenterAlignGlyph(x);
				break;
			case GlyphAlignment::topRight:
				TopAlignGlyph(y);
				RightAlignGlyph(x);
				break;
			case GlyphAlignment::baselineLeft:
				BaseLineAlignGlyph(y);
				LeftAlignGlyph(x);
				break;
			case GlyphAlignment::baselineCenter:
				BaseLineAlignGlyph(y);
				CenterAlignGlyph(x);
				break;
			case GlyphAlignment::baselineRight:
				BaseLineAlignGlyph(y);
				RightAlignGlyph(x);
				break;
			case GlyphAlignment::esriArrowMarker:
				MiddleAlignGlyph(y);
				RightAlignInMaxCharWidthBox(x);
				break;
		}

		x -= (_anchorPointXDots / EmToDots);
		y -= (_anchorPointYDots / EmToDots);
	}

	void CGlyph::LeftAlignGlyph(double& x)
	{
		x -= _lpgm->gmptGlyphOrigin.x;
	}
	void CGlyph::LeftAlignGlyphFull(double& x)
	{
		x -= 0;
	}
	void CGlyph::CenterAlignGlyph(double& x)
	{
		LeftAlignGlyph(x);
		x -= _lpgm->gmBlackBoxX / 2;	// gmBlackBoxX is em equivalent - used font is em-size device units high
	}
	void CGlyph::CenterAlignGlyphFull(double& x)
	{
		LeftAlignGlyphFull(x);
		x -= _lpgm->gmCellIncX / 2;		// gmCellIncX is em equivalent - used font is em-size device units high
	}
	void CGlyph::RightAlignGlyph(double& x)
	{
		LeftAlignGlyph(x);
		x -= _lpgm->gmBlackBoxX;		// gmBlackBoxX is em equivalent - used font is em-size device units high
	}
	void CGlyph::RightAlignGlyphFull(double& x)
	{
		LeftAlignGlyphFull(x);
		x -= _lpgm->gmCellIncX;			// gmCellIncX is em equivalent - used font is em-size device units high
	}
	void CGlyph::RightAlignInMaxCharWidthBox(double& x)
	{
		LeftAlignGlyph(x);
		x -= _lpTextMetric->tmMaxCharWidth;
	}
	void CGlyph::TopAlignGlyph(double& y)
	{
		y -= _lpgm->gmptGlyphOrigin.y;		// gmptGlyphOrigin.y is em equivalent - used font is em-size device units high
	}
	void CGlyph::MiddleAlignGlyph(double& y)
	{
		TopAlignGlyph(y);
		y += _lpgm->gmBlackBoxY / 2;		// gmBlackBoxY is em equivalent - used font is em-size device units high
	}
	void CGlyph::BottomAlignGlyph(double& y)
	{
		TopAlignGlyph(y);
		y += _lpgm->gmBlackBoxY;			// gmBlackBoxY is em equivalent - used font is em-size device units high
	}
	void CGlyph::BaseLineAlignGlyph(double& y)
	{
		// Simply do nothing - fonts are defined baseline aligned - that is - the zero y coordinate
		// is always at the baseline
	}

	void CGlyph::RotateGlyph(double& x, double& y)
	{
		double x0 = x; double y0 = y;
		x = x0 * _cosPhi - y0 * _sinPhi;
		y = x0 * _sinPhi + y0 * _cosPhi;
	}

	void CGlyph::InitOffset(double xDots, double yDots)
	{
		_offsetXDots += xDots;
		_offsetYDots += yDots;
	}

	void CGlyph::InitAnchor(Vector2 anchorPointOffset)
	{
		_anchorPointXDots = anchorPointOffset.X;
		_anchorPointYDots = anchorPointOffset.Y;
	}

	void CGlyph::InitRotation(double degrees)
	{
		double phi = DegToRad(degrees);
		_cosPhi = cos(phi);
		_sinPhi = sin(phi);
	}

	double CGlyph::DegToRad(double degrees)
	{
		double radians = Math::PI / 180 * degrees;
		// Normalize to within [0..2*Pi]
		while (0 > radians)
			radians += 2 * Math::PI;
		while (2 * Math::PI < radians)
			radians -= 2 * Math::PI;
		return radians;
	}

	IEnumerable<HatchBoundaryPath^>^ CGlyph::HatchBoundaries::get()
	{
		return _hatchBoundaries;
	}

	BoundingRectangle^ CGlyph::Blackbox::get() {
		return gcnew BoundingRectangle(BlackBoxMin, BlackBoxMax);
	}

	Vector2 CGlyph::BlackBoxMin::get() {
		return ToDots(_lpgm->gmptGlyphOrigin.x, _lpgm->gmptGlyphOrigin.y - _lpgm->gmBlackBoxY);
	}

	Vector2 CGlyph::BlackBoxMax::get() {
		return ToDots(_lpgm->gmptGlyphOrigin.x + _lpgm->gmBlackBoxX, _lpgm->gmptGlyphOrigin.y);
	}

	double CGlyph::CellHeight::get() {
		return _lpgm->gmCellIncY * EmToDots; ;
	}

	double CGlyph::CellWidth::get() {
		return _lpgm->gmCellIncX * EmToDots;
	}

	double CGlyph::TypographicAscent::get() {
		return _lpOutlineTextMetric->otmAscent * EmToDots;
	}

	double CGlyph::TypographicDescent::get() {
		return _lpOutlineTextMetric->otmDescent * EmToDots;
	}

	double CGlyph::MaxAscent::get() {
		return fabs(_lpTextMetric->tmAscent * EmToDots);
	}

	double CGlyph::MaxDescent::get() {
		return fabs(_lpTextMetric->tmDescent * EmToDots);
	}

	double CGlyph::EmToDots::get() {
		return _sizeInDots / _emSquare;
	}

	HGDIOBJ CGlyph::CreateFontFromName(String^ name, String^ style, double size) {
		bool bold, underline, italic, striketrough;
		StyleFlagsFromString(style, bold, underline, italic, striketrough);

		return CreateFontFromName(name, bold, underline, italic, striketrough, size);
	}

	HGDIOBJ CGlyph::CreateFontFromName(String^ name, System::Drawing::FontStyle style, double size) {
		bool bold, underline, italic, striketrough;
		StyleFlagsFromFontStyle(style, bold, underline, italic, striketrough);

		return CreateFontFromName(name, bold, underline, italic, striketrough, size);
	}

	HGDIOBJ CGlyph::CreateFontFromName(String^ name, bool bold, bool underline, bool italic, bool strikethrough, double size)
	{
		int nHeight = -MulDiv((int)size, _logPixelsY, 72);

		LPCTSTR familyName = (LPCTSTR)Marshal::StringToHGlobalUni(name).ToPointer();
		return CreateFontW(nHeight, 0, 0, 0, (bold) ? 700 : 400, italic, underline, strikethrough,
			DEFAULT_CHARSET, OUT_TT_PRECIS, CLIP_DEFAULT_PRECIS, DEFAULT_QUALITY, DEFAULT_PITCH | FF_DONTCARE,
			(LPCTSTR)familyName);
	}

	void CGlyph::StyleFlagsFromString(String^ style, bool& bold, bool& underline, bool& italic, bool& striketrough) {
		bold = (0 <= style->IndexOf("bold", StringComparison::OrdinalIgnoreCase));
		underline = (0 <= style->IndexOf("underline", StringComparison::OrdinalIgnoreCase));
		italic = (0 <= style->IndexOf("italic", StringComparison::OrdinalIgnoreCase));
		striketrough = (0 <= style->IndexOf("strike", StringComparison::OrdinalIgnoreCase));
	}

	void CGlyph::StyleFlagsFromFontStyle(System::Drawing::FontStyle style, bool& bold, bool& underline, bool& italic, bool& striketrough) {
		bold = (0 != (int) (style & System::Drawing::FontStyle::Bold));
		underline = (0 != (int)(style & System::Drawing::FontStyle::Underline));
		italic = (0 != (int)(style & System::Drawing::FontStyle::Italic));
		striketrough = (0 != (int)(style & System::Drawing::FontStyle::Strikeout));
	}

	// When extracting a glyph from a dc from ArcMap, the rotation has already been
	// applyied to the font selected into the dc - so we _must not_ apply any rotation
	// to the glyph extracted in that case.
	MAT2* CGlyph::RotationMatrixZero() {
		static MAT2 rotMat0 = MAT2{ { 0, 1 },{ 0, 0 },{ 0, 0 },{ 0, 1 } };

		return &rotMat0;
	}

	LPOUTLINETEXTMETRIC CGlyph::GetTextMetrics(HDC hDc)
	{
		size_t otmSize = GetOutlineTextMetrics(hDc, 0, NULL);
		auto lpOutlineTextMetric = (LPOUTLINETEXTMETRIC)malloc(otmSize);
		return (0 != GetOutlineTextMetrics(hDc, (UINT)otmSize, lpOutlineTextMetric))
			? lpOutlineTextMetric
			: NULL;
	}
}