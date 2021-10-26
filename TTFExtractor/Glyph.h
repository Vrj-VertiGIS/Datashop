#pragma once
#include "stdafx.h"

#include "GlyphAlignment.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace netDxf;
using namespace netDxf::Entities;

namespace GEOCOM::GNSDatashop::TTF
{
	public ref class CGlyph 
	{
	// About suffixes in the code (variables/properties):	
	// All names suffixed by EM: units in em - font design units - dimensionless
	// All names suffixed by DU: device units - depends on current mapping mode (assumed: MM_TEXT)
	// All names suffixed by Dots: value in points (1/72")
	// All - public - numeric, unsuffixed values: values in points (1/72")
    //
	private:

		HDC _hCdc;

		List<HatchBoundaryPath^>^ _hatchBoundaries = nullptr;
		LPGLYPHMETRICS _lpgm = NULL;

		unsigned int _emSquare = 0;		// The font's em square size (em units)

		double _sizeInDots = 0.0;		// The glyph's desired size in dots

		double _anchorPointXDots = 0.0;	// Anchor point - relative coordinates 
		double _anchorPointYDots = 0.0;	// Anchor point - relative coordinates

		double _offsetXDots = 0.0;		// Offset symbol x dots
		double _offsetYDots = 0.0;		// offset symbol y dots

		// Rotation matrix components
		double _sinPhi; double _cosPhi;

		// Device caps
		int _logPixelsX = 0;		// Logical pixels (device units) in X - MM_TEXT assumed: 144 units/logical inch
		int _logPixelsY = 0;		// Logical pixels (device units) in y - MM_TEXT assumed: 144 units/logical iunch

		// Font info
		TEXTMETRIC* _lpTextMetric = nullptr;
		OUTLINETEXTMETRIC* _lpOutlineTextMetric = nullptr;

		GlyphAlignment _glyphAlignment = GlyphAlignment::middleCenter;

		// Get glyph data in EM
		size_t GetGlyphData(unsigned int character, LPTTPOLYGONHEADER* lppPolygonHeaders, MAT2* rotMat);
		// Get glyph data in EM - glyph/font design-time EM square size
		size_t GetGlyphData(unsigned int character, LPTTPOLYGONHEADER* lppPolygonHeaders, HGDIOBJ hFont, MAT2* rotmat);
		// Store glyph data in _boundaries. Scale to points 
		void ProcessGlyphData(LPTTPOLYGONHEADER lpPolygonHeaders, size_t bufsize);
		HGDIOBJ CreateFontFromName(String^ name, String^ style, double size);
		HGDIOBJ CreateFontFromName(String^ name, System::Drawing::FontStyle style, double size);
		HGDIOBJ CreateFontFromName(String^ name, bool bold, bool underline, bool italic, bool strikethrough, double size);
		void StyleFlagsFromString(String^ style, bool& bold, bool& underline, bool& italic, bool& striketrough);
		void StyleFlagsFromFontStyle(System::Drawing::FontStyle style, bool& bold, bool& underline, bool& italic, bool& striketrough);

		Vector2 ToDots(POINTFX point);
		Vector2 ToDots(LPPOINTFX lpPoint);
		Vector2 ToDots(double x, double y);

		void AlignGlyph(double& x, double& y);
		void LeftAlignGlyph(double& x);
		void CenterAlignGlyph(double& x);
		void RightAlignGlyph(double& x);
		void LeftAlignGlyphFull(double& x);			// Using full cell increment as glyph width
		void CenterAlignGlyphFull(double& x);		// Using full cell increment as glyph width
		void RightAlignGlyphFull(double& x);		// Using full cell increment as glyph width
		void RightAlignInMaxCharWidthBox(double& x);
		void TopAlignGlyph(double& y);
		void MiddleAlignGlyph(double& y);
		void BottomAlignGlyph(double& y);
		void BaseLineAlignGlyph(double& y);

		void InitRotation(double degrees);
		void RotateGlyph(double& x, double& y);
		void InitAnchor(Vector2 anchorPointOffset);
		void InitOffset(double xDots, double yDots);

		static MAT2* RotationMatrixZero();
		static inline double DegToRad(double degrees);
		static LPOUTLINETEXTMETRIC GetTextMetrics(HDC hDc);

	protected:
		CGlyph();
		CGlyph(IntPtr^ hdc, double sizeInDots);

		// The below 4 factors depend on the requested character size (in dots)
		//
		property double EmToDots		// Factor to convert em units to dots (1/72")
		{ double get(); }

	public:
		// Extract glyph selected into dc (active font).
		CGlyph  
		(
			IntPtr^ hdc,					// => Memory dc compatible to ouput drawing display context (GetCompatibleDC())
			unsigned int character,			// => Character to render glyph for
			double sizeInDots,				// => Size in dots to render glyph of
			GlyphAlignment glyphAlignment,	// => Glyph alignement (relative to glyph cell)
			double offsetXDots,				// => Offset (points) to shift glyph after applying rotation
			double offsetYDots,				// => Offset (points) to shift glyph after applying rotation
			double rotation					// => Rotatate glpyh (deg => cartesian)
		);

		// Extract glyph by given font name ans style (given as readable string - i.e. Bold Italic)
		CGlyph
		(
			IntPtr^ hdc,					// => Memory dc compatible to ouput drawing display context (GetCompatibleDC())
			unsigned int character,			// => Character to render glyph for
			String^ familyName,				// => Font to render glyph of
			String^ style,					// => Font style (^regular, bold, italic, underline - or a combination) to render glyph of.
			double sizeInDots,				// => Size in dots to render glyph of
			GlyphAlignment glyphAlignment,	// => Glyph alignement (relative to glyph cell)
			Vector2 anchorPointOffset,		// => Anchor point _offset_ of glyph (relative 0/0) - will offset glyph before applying rotation
			double offsetXDots,				// => Offset (points) to shift glyph after applying rotation
			double offsetYDots,				// => Offset (points) to shift glyph after applying rotation
			double rotation					// => Rotatate glpyh (deg => cartesian)
		);

		// Extract glyph by given font name ans style (given as .net System::FontStyle enum value combination).
		CGlyph
		(
			IntPtr^ hdc,					// => Memory dc compatible to ouput drawing display context (GetCompatibleDC())
			unsigned int character,			// => Character to render glyph for
			String^ familyName,				// => Font to render glyph of
			FontStyle style,		    	// => Font style (^regular, bold, italic, underline - or a combination) to render glyph of.
			double sizeInDots,				// => Size in dots to render glyph of
			GlyphAlignment glyphAlignment,	// => Glyph alignement (relative to glyph cell)
			Vector2 anchorPointOffset,		// => Anchor point _offset_ of glyph (relative 0/0) - will offset glyph before applying rotation
			double offsetXDots,				// => Offset (points) to shift glyph after applying rotation
			double offsetYDots,				// => Offset (points) to shift glyph after applying rotation
			double rotation					// => Rotatate glpyh (deg => cartesian)
		);


		~CGlyph();

		property IEnumerable<HatchBoundaryPath^>^ HatchBoundaries
		{ IEnumerable<HatchBoundaryPath^>^ get(); }

		property BoundingRectangle^ Blackbox
		{ BoundingRectangle^ get(); }

		property Vector2 BlackBoxMin
		{ Vector2 get(); }

		property Vector2 BlackBoxMax
		{ Vector2 get(); }

		property double CellWidth
		{ double get(); }

		property double CellHeight
		{ double get(); }

		/// <summary>
		/// Ascent of the character "f" - this is called the Typographic Ascent
		/// Given in Dots 1/72"
		/// </summary>
		property double TypographicAscent
		{ double get(); }

		/// <summary>
		/// Descent of the character "g" - this is called the Typographic Descent
		/// Given in Dots 1/72"
		/// </summary>
		property double TypographicDescent
		{ double get(); }

		/// <summary>
		/// The fonts maximum Acent - given in Dots (1/72")
		/// </summary>
		property double MaxAscent
		{ double get(); }

		/// <summary>
		/// The fonts maximum Descent - given in Dots (1/72")
		/// </summary>
		property double MaxDescent
		{ double get(); }

	};
}