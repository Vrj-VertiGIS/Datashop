#pragma once

namespace GEOCOM::GNSDatashop::TTF
{

	// Define the reference position (coordinate 0/0) of the glyph 
	// Returned Hatch will follow this coordinate alignment.
	//
	public enum class GlyphAlignment
	{
		topLeft, topCenter, topRight,
		middleLeft, middleCenter, middleRight,
		bottomLeft, bottomCenter, bottomRight,
		baselineLeft, baselineCenter, baselineRight,
		middleLeftFull, middleCenterFull, middleRightFull,
		// Custom alignments - facilitate the glyph alignment 
		// as it can best be accomplished within the glyph extraction routine
		esriArrowMarker		// ESRI Arrow marker are aligned middle and right (within the font's max char width box)
	};
}	

