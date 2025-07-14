//--------------------------------------------------------------------------
// File:    RegexExpressionStrings.cs
// Content:	Implementation of Regex Expression String Library
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Regex Expression String Library
	/// </summary>
	public static class RegexExpressionStrings
    {
        #region Regex Expression Strings

        /// <summary>Regex string: ^[a-zA-Z]*$</summary>
        public static readonly string AlphaExpression = @"^[a-zA-Z]*$";

        /// <summary>Regex string: ^[A-Z]*$</summary>
        public static readonly string AlphaUpperCaseExpression = @"^[A-Z]*$";

        /// <summary>Regex string: ^[a-z]*$</summary>
        public static readonly string AlphaLowerCaseExpression = @"^[a-z]*$";

        /// <summary>Regex string: ^[a-zA-Z0-9]*$</summary>
        public static readonly string AlphaNumericExpression = @"^[a-zA-Z0-9]*$";

        /// <summary>Regex string: ^[a-zA-Z0-9 ]*$</summary>
        public static readonly string AlphaNumericSpaceExpression = @"^[a-zA-Z0-9 ]*$";

        /// <summary>Regex string: ^[a-zA-Z0-9 \-]*$</summary>
        public static readonly string AlphaNumericSpaceDashExpression = @"^[a-zA-Z0-9 \-]*$";

        /// <summary>Regex string: ^[a-zA-Z0-9 \-_]*$</summary>
        public static readonly string AlphaNumericSpaceDashUnderscoreExpression = @"^[a-zA-Z0-9 \-_]*$";

        /// <summary>Regex string: ^[a-zA-Z0-9\. \-_]*$</summary>
        public static readonly string AlphaNumericSpaceDashUnderscorePeriodExpression = @"^[a-zA-Z0-9\. \-_]*$";

        /// <summary>Regex string: ^\-?[0-9]*\.?[0-9]*$</summary>
        public static readonly string NumericExpression = @"^\-?[0-9]*\.?[0-9]*$";

        /// <summary>Regex string: ^\-?[0-9]*,?[0-9]*$</summary>
        public static readonly string NumericGermanExpression = @"^\-?[0-9]*,?[0-9]*$";

        /// <summary>Regex string: ^([0-9a-zA-Z]+[-._+&amp;])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$</summary>
        public static readonly string EmailExpression = @"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$";

        /// <summary>Regex string: ^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_=]*)?$</summary>
        public static readonly string UrlExpression = @"^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$";

        /// <summary>Regex string: &lt;\s*br\s*/?\s*&gt;</summary>
        public static readonly string HtmlBreakExpression = @"<\s*br\s*/?\s*>";

        /// <summary>Regex string: &lt;\s*([bp]r?)\s*/?\s*&gt;</summary>
        public static readonly string HtmlBreakOrParagraphExpression = @"<\s*([bp]r?)\s*/?\s*>";

        /// <summary>Regex string: ^&lt;\s*([bp]r?)\s*/?\s*&gt;</summary>
        public static readonly string HtmlBreakOrParagraphTrimLeftExpression = @"^" + HtmlBreakOrParagraphExpression;

        /// <summary>Regex string: &lt;\s*([bp]r?)\s*/?\s*&gt;$</summary>
        public static readonly string HtmlBreakOrParagraphTrimRightExpression = HtmlBreakOrParagraphExpression + @"$";

        /// <summary>Regex string: &lt;\s*p\s*/?\s*&gt;</summary>
        public static readonly string HtmlParagraphExpression = @"<\s*p\s*/?\s*>";

        /// <summary>Regex string: &lt;\s*p\s*/?\s*&gt;</summary>
        public static readonly string UriChars = @"[^\s)<>\]}!([]+";

        #endregion
    }
}
