using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Compilation;
using System.Web.UI;
using GEOCOM.Common.Logging;

namespace GEOCOM.GNSD.Web.Core.Localization.Language
{
    /// <summary>
    /// Singleton class that provides language texts
    /// </summary>
    [ExpressionPrefix("Txt")]
    public class WebLanguage : ExpressionBuilder
    {
        #region Public Properties

        /// <summary>
        /// Gets the language.
        /// </summary>
        public static string Language
        {
            get
            {
                var extension = Path.GetExtension(lng.ResFilename) ?? string.Empty;

                return extension.Length == 0
                    ? DefaultLanguage
                    : extension.Substring(1)
                               .ToLower();
            }
        }

        /// <summary>
        /// Override to support NoCompile webpages
        /// </summary>
        public override bool SupportsEvaluate
        {
            get
            {
                return !Process.GetCurrentProcess()
                               .ProcessName.Equals("devenv", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        #endregion

        #region Private members

        /// <summary>
        /// default language
        /// </summary>
        private const string DefaultLanguage = "de";

        /// <summary>
        /// Holds the logger for this class
        /// </summary>
        private static readonly IMsg log = new Msg(typeof(WebLanguage));

        /// <summary>
        /// holds the StoLanguage instance
        /// </summary>
        private static StoLanguage lng;

        /// <summary>
        /// Holds the custom language override StoLanguage instance
        /// </summary>
        private static StoLanguage lngCustom;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="WebLanguage"/> class.
        /// </summary>
        static WebLanguage()
        {
            try
            {
                LoadLanguageFile();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("WebLanguage {0}", ex.Message), ex);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the string
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns></returns>
        public static string LoadStr(int key, string defaultText)
        {
            if (lngCustom != null && lngCustom.ContainsKey(key))
                return lngCustom.LoadStr(key, defaultText);

            return lng.LoadStr(key, defaultText);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static object GetText(string expression)
        {
            try
            {
                if (VerifyLanguageChanged())
                    LoadLanguageFile();

                var s = expression.Split(new[] { ' ' }, 2);

                return s.Length == 2 
                    ? LoadStr(Convert.ToInt32(s[0]), s[1])
                    : expression;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Base class overrides

        /// <summary>
        /// When overridden in a derived class, returns code that is used during page execution to obtain the evaluated expression.
        /// </summary>
        /// <param name="entry">The object that represents information about the property bound to by the expression.</param>
        /// <param name="parsedData">The object containing parsed data as returned by <see cref="M:System.Web.Compilation.ExpressionBuilder.ParseExpression(System.String,System.Type,System.Web.Compilation.ExpressionBuilderContext)"/>.</param>
        /// <param name="context">Contextual information for the evaluation of the expression.</param>
        /// <returns>
        /// A <see cref="T:System.CodeDom.CodeExpression"/> that is used for property assignment.
        /// </returns>
        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            try
            {
                var property = (string)parsedData;

                var prim = new CodePrimitiveExpression(property);
                var args = new CodeExpression[] { prim };

                var refType = new CodeTypeReferenceExpression(base.GetType());

                return new CodeMethodInvokeExpression(refType, "GetText", args);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("GetCodeExpression {0}", ex.Message), ex);

                return null;
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns an object that represents the parsed expression.
        /// </summary>
        /// <param name="expression">The value of the declarative expression.</param>
        /// <param name="propertyType">The type of the property bound to by the expression.</param>
        /// <param name="context">Contextual information for the evaluation of the expression.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> containing the parsed representation of the expression; otherwise, null if <see cref="M:System.Web.Compilation.ExpressionBuilder.ParseExpression(System.String,System.Type,System.Web.Compilation.ExpressionBuilderContext)"/> is not implemented.
        /// </returns>
        public override object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context)
        {
            return expression;
        }

        /// <summary>
        /// When overridden in a derived class, returns an object that represents an evaluated expression.
        /// </summary>
        /// <param name="target">The object containing the expression.</param>
        /// <param name="entry">The object that represents information about the property bound to by the expression.</param>
        /// <param name="parsedData">The object containing parsed data as returned by <see cref="M:System.Web.Compilation.ExpressionBuilder.ParseExpression(System.String,System.Type,System.Web.Compilation.ExpressionBuilderContext)"/>.</param>
        /// <param name="context">Contextual information for the evaluation of the expression.</param>
        /// <returns>
        /// An object that represents the evaluated expression; otherwise, null if the inheritor does not implement <see cref="M:System.Web.Compilation.ExpressionBuilder.EvaluateExpression(System.Object,System.Web.UI.BoundPropertyEntry,System.Object,System.Web.Compilation.ExpressionBuilderContext)"/>.
        /// </returns>
        public override object EvaluateExpression(object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            try
            {
                var txt = (string)parsedData;

                return GetText(txt);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("EvaluateExpression {0}", ex.Message), ex);

                return ex.Message;
            }
        }

        #endregion

        #region Non public methods

        /// <summary>
        /// Verifies the language changed.
        /// </summary>
        /// <returns></returns>
        private static bool VerifyLanguageChanged()
        {
            return lng.ResFilename != ConstructLanguageFilename();
        }

        /// <summary>
        /// Loads the language file.
        /// </summary>
        private static void LoadLanguageFile()
        {
            var assembly = BuildManager.GetGlobalAsaxType()
                .BaseType.Assembly;

            var uri = new Uri(assembly.EscapedCodeBase);

            var dllName = Path.GetFileName(uri.LocalPath);

            var textDirectory = Path.Combine(Path.GetDirectoryName(uri.LocalPath), "Text");

            var fullName = Path.Combine(textDirectory, dllName);

            var fileName = ConstructLanguageFilename();

            lng = new StoLanguage(fullName) { ResFilename = fileName };
            lng.LoadFromResFile();

            var overrideFilename = ConstructLanguageFilename(true);

            var overrideFilePath = Path.Combine(textDirectory, overrideFilename);

            if (File.Exists(overrideFilePath))
            {
                lngCustom = new StoLanguage(fullName) { ResFilename = overrideFilename };
                lngCustom.LoadFromResFile();
            }
            else
                lngCustom = null;
        }

        /// <summary>
        /// Constructs the language filename from the entry point assembly (web project context) and the UI Culture of the current thread
        /// </summary>
        /// <param name="overrideFile">if set to <c>true</c> [override file].</param>
        /// <returns></returns>
        private static string ConstructLanguageFilename(bool overrideFile = false)
        {
            var assemblyName = BuildManager.GetGlobalAsaxType()
                .BaseType.Assembly.FullName
                .Split(',')
                .First();

            var languageCode = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();

            return overrideFile 
                ? string.Format("{0}.Custom.{1}", assemblyName, languageCode) 
                : string.Format("{0}.{1}", assemblyName, languageCode); 
        }

        #endregion
    }
}