namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
{
    public class LabelTextTokenizer
    {
        public string Text { get; private set; }
        public int Length { get; private set; } = 0;

        public LabelTextTokenizer(string text)
        {
            Text = text;
            Length = text.Length;
        }

        public bool TryGetQuotedString(ref int position, ref string value)
        {
            int i = position;
            if (TrySkipQuotedString(ref i))
            {
                value = Text.Substring(position + 1, i - position - 2);   // Omit quotes
                position = i;
                return true;
            }
            return false;
        }

        public bool TryGetTag(ref int position, ref string token)
        {
            int i = position;
            if (TrySkipTag(ref i))
            {
                token = Text.Substring(position, i - position);
                position = i;
                return true;
            }
            return false;
        }

        public void GetElementString(ref int position, ref string element)
        {
            int i = position++;
            SkipElementString(ref position);
            element = Text.Substring(i, position - i);
        }

        public bool TryGetEscapeString(ref int position, ref string escaping)
        {
            int i = position;
            if (TrySkipEscaping(ref i))
            {
                escaping = Text.Substring(position, i - position);
                position = i;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip a text literal put inside quotation marks. Look for the closing opposite
        /// quoitation mark and return true if found. In thsi case return position just
        /// one character after the closing quotation mark. If not found, remain at the
        /// initial position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TrySkipQuotedString(ref int position)
        {
            var quotationMark = Text[position];
            int i;
            for (i = position + 1; (Length > i) && (Text[i] != quotationMark); i++) ;
            if (Length > i)
            {
                position = ++i;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip a tag. Assume Text[position] points to an opening tag brace.
        /// Try to find a closing brace while skipping any text character and
        /// skipping text literals put inside quotation marks.
        /// if found a closing bracket, return true and place position at the 
        /// character just after the closing bracket.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TrySkipTag(ref int position)
        {
            int i = position + 1;
            while ((Length > i) && (Text[i] != '>'))
            {
                var c = Text[i];
                if ((c != '"') && (c != '\''))
                    i++;
                else if (!TrySkipQuotedString(ref i))
                    i++;
            }
            if (Length > i)
            {
                position = ++i;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip element text (text not part of a tag)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public void SkipElementString(ref int position)
        {
            bool atEndOfElement = false;
            while ((Length > position) && (!atEndOfElement))
            {
                var c = Text[position];
                if ((c == '\r') || (c == '\n') || (c == '\t'))
                    atEndOfElement = true;
                else if (c == '<')
                    if ((Length > position + 1) && (Text[position + 1] == '<'))
                        position += 2;  // Skip entire < pair
                    else
                        atEndOfElement = true;
                else if (c == '\\')
                    if ((Length > (position + 1)) && (Text[position + 1] == '\''))
                        position += 2; // Skip entire \ pair
                    else
                        atEndOfElement = true;
                else if ((c == '"') || (c == '\''))
                {
                    if (!(atEndOfElement = TrySkipQuotedString(ref position)))
                        position++;    // Orphaned " or ' - skip to next character anyways
                }
                else
                    position++;
            }
        }

        /// <summary>
        /// Skip character escaping
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TrySkipEscaping(ref int position)
        {
            int i = position + 1;
            if (Length > i)
                if (Text[i] == '\\')
                {
                    if ((Length <= i + 1) || (Text[i + 1] != '\\'))
                    // We're on a escaing of the escape character : \\
                    {
                        position = i + 1;
                        return true;
                    }
                }
                else if (IsAThruZ(Text[i]))
                // Escaping \n, \p, \P...
                {
                    position = i + 1;
                    return true;
                }
                else if ((Length > i + 2) && Is0Thru9(Text[i]) && Is0Thru9(Text[i + 1]) && Is0Thru9(Text[i + 2]))
                {
                    position = i + 3;
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Try to skip an arbitrary operator ('=', '+', '-' or any given single character.
        /// </summary>
        /// <param name="op">Operator character</param>
        /// <param name="position">position after skipping</param>
        /// <returns>true if character found and skipped</returns>
        public bool TrySkipOperator(char op, ref int position)
        {
            int i = position;
            SkipBlanks(ref i);
            if ((Length > i) && (Text[i] == op))
            {
                ++i;
                SkipBlanks(ref i);
                position = i;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip blanks/whitespaces
        /// </summary>
        /// <param name="position"></param>
        public void SkipBlanks(ref int position)
            => Skip(' ', ref position);

        /// <summary>
        /// Skip any arbitrary character
        /// </summary>
        /// <param name="c"></param>
        /// <param name="position"></param>
        public void Skip(char c,  ref int position)
        {
            while ((Length > position) && (c == Text[position]))
                position++;
        }

        private bool IsAThruZ(char c) => (('a' <= c) && ('z' >= c)) || (('A' <= c) && ('Z' >= c));
        private bool Is0Thru9(char c) => ('0' <= c) && ('9' >= c);
    }
}