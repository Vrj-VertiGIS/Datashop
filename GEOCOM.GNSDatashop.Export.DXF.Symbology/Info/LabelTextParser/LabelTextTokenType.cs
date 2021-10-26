namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
{
    public enum LabelTextTokenType
    {
        TagBold, TagITalic, TagUnderline, TagColor,
        TagEndBold, TagEndItalic, TagEndUnderline, TagEndColor,
        TagNotBold, TagNotItalic, TagNotUnderline,
        TagEndNotBold, TagEndNotItalic, TagEndNotUnderline,
        TagSuperScript, TagEndSuperScript,
        TagSubScript, TagEndSubScript,
        TagFont,
        TagEndFont,
        ESCNewline,
        ESCCarriageReturn,
        ESCTab,
        Element
    }
}
