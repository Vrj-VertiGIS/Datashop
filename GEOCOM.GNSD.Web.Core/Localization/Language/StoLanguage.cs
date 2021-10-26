using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace GEOCOM.GNSD.Web.Core.Localization.Language
{
    /// <summary>
    ///   Stellt ein einfaches System zur Verfügung, um Anwendungen
    ///   sprachunabhängig zu machen. Die Sprachressourcen werden lediglich
    ///   als Textdateien in Unterverzeichnissen der Anwendung plaziert,
    ///   StoLanguage bestimmt automatisch die gewünschte Sprache.
    ///   Mit der "TranslateForm" Methode können automatisch ganze Forms
    ///   übersetzt werden, zur Entwurfszeit muss am Anfang der zu
    ///   übersetzenden Properties eine Zahl angegeben werden.
    /// </summary>
    /// <example>
    ///   In diesem Beispiel heisst die Anwendung
    ///     C:\Programs\HelloWorld.exe
    ///   und die Sprachdateien sind in den Unterverzeichnissen
    ///     C:\Programs\de\HelloWorld.lng
    ///     C:\Programs\en\HelloWorld.lng
    ///   die Anwendung hat ein Menu mit dem Property
    ///     ToolStripMenuItem.Text = "1001 Open"
    /// 
    ///   Sprachobjekt erstellen und vorbereiten:
    ///     StoLanguage lng = new StoLanguage();
    ///     lng.LoadFromFile();
    ///   Eine ganze Form übersetzen  
    ///     lng.TranslateForm(form1);
    ///   Meldung ausgeben
    ///     lng.LoadStr(3000, "Are you sure you want to delete");
    ///   
    ///   Beispiel einer Sprachressourcen:
    ///     // menu
    ///     1000 Datei
    ///     1001 Öffnen...
    ///     1002 Beenden
    ///     // gui
    ///     2000 LanguageTest
    ///     // messages
    ///     3000 Wollen Sie wirklich löschen 
    /// </example>
    /// <remarks>
    ///   Bei den Sprachressourcen handelt es sich um normale Textfiles
    ///   im Unicode Format. Jeder Eintrag steht auf einer Zeile. Eine
    ///   Zeile besteht aus Identifikator, Trenner und Text. Als Trenner
    ///   sollte ein Space verwendet werden.
    ///   Zur besseren Darstellung können Kommentare und Leerzeilen
    ///   eingesetzt werden, die Kommentare beginnen mit //.
    /// </remarks>
    public class StoLanguage
    {
        /// <summary>
        ///   Name der Sprachdatei ohne Pfad, nach welcher gesucht wird.
        ///   Der Name wird zu beginn mit (applicationname.lng)
        ///   initialisiert und muss normalerweise nicht verändert werden.
        /// </summary>
        public string ResFilename
        {
            get
            {
                return resFilename;
            }
            set
            {
                resFilename = Path.GetFileName(value);
            }
        }

        protected string AssemblyDirectory;

        protected string ResFileFallbackLng = "en";

        protected List<string> TranslatedProperties = new List<string> { "Text", "ToolTipText" };

        protected SortedList<int, string> ResTexts = new SortedList<int, string>();

        private string resFilename;
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StoLanguage"/> class.
        /// </summary>
        public StoLanguage()
        {
            // Default Dateiname für Sprachressource auf "AnwendungsName.lng" setzen
            //string exeOrDllName = Assembly.GetExecutingAssembly().Location;
            // 15.04.2009/MEL: Get the location from the calling Assembly
            var assemblyLocation = Assembly.GetCallingAssembly().Location;

            this.AssemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            this.resFilename = Path.ChangeExtension(Path.GetFileName(assemblyLocation), FindFavoredLanguageCode());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoLanguage"/> class.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        public StoLanguage(string assemblyLocation)
        {
            this.AssemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            
            this.resFilename = Path.ChangeExtension(Path.GetFileName(assemblyLocation), FindFavoredLanguageCode());
        } 

        #endregion

        internal void LoadFromResFile(string lng)
        {
            if (!string.IsNullOrEmpty(this.resFilename))
                this.resFilename = Path.ChangeExtension(this.resFilename, lng);

            this.LoadFromResFile();
        }

        /// <summary>
        ///   Sucht das passende Sprachfile und liest die enthaltenen Texte
        ///   ein. Mit dieser Funktion wird das Sprachobjekt initialisiert.
        /// </summary>
        /// <returns>
        ///   -1 wenn keine Sprachdatei gefunden werden wurde,
        ///   0 wenn Datei gefunden und erfolgreich eingelesen wurde,
        ///   Zeilennummer wenn doppelter Eintrag gefunden wurde.
        /// </returns>
        public int LoadFromResFile()
        {
            this.ResTexts.Clear();

            var lngFilename = this.FindResFile(this.resFilename, this.ResFileFallbackLng);

            if (lngFilename != null)
            {
                var result = 0;

                using (var lngFileReader = new StreamReader(lngFilename, true))
                {
                    var lineNumber = 0;
                    string lineText = null;
                    int resKey;
                    string resText;
                    // process all lines
                    while ((lineText = lngFileReader.ReadLine()) != null)
                    {
                        lineNumber++;
                        if (this.SplitLine(lineText, out resKey, out resText))
                        {
                            var resTextBuilder = new StringBuilder(resText);

                            resTextBuilder.Replace(@"\n", "\n");

                            resText = resTextBuilder.ToString();

                            try
                            {
                                this.ResTexts.Add(resKey, resText);
                            }
                            catch (ArgumentException)
                            { // remember line number of first error
                                if (result == 0)
                                    result = lineNumber;
                            }
                        }
                    }
                }

                return result;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the specified key is in this instance
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(int key)
        {
            return this.ResTexts.ContainsKey(key);
        }

        /// <summary>
        ///   Sucht anhand einer ID nach dem passenden Text, der geladenen
        ///   Sprachressource.
        /// </summary>
        /// <param name="key">Die ID des gesuchten Textes.</param>
        /// <param name="defaultText">Dieser Text wird zurückgeliefert,
        ///   falls kein Text mit der gesuchten ID vorhanden ist.</param>
        /// <returns>
        ///   Text mit der gewünschten ID, oder DefaultText falls Id nicht
        ///   gefunden wurde.</returns>
        public string LoadStr(int key, string defaultText)
        {
            string result;

            return this.ResTexts.TryGetValue(key, out result) 
                ? result
                : defaultText;
        }

        /// <summary>
        ///   Wie LoadStr, der Text wird aber zusätzlich noch formatiert.
        /// </summary>
        /// <param name="key">Die ID des gesuchten Textes.</param>
        /// <param name="defaultText">Dieser Text wird zurückgeliefert,
        ///   falls kein Text mit der gesuchten ID vorhanden ist.</param>
        /// <param name="args">Parameter zur Formatierung, siehe
        ///   "string.Format".</param>  
        /// <returns>
        ///   Formatierter Text mit der gewünschten ID, oder DefaultText
        ///   falls Id nicht gefunden wurde.</returns>
        public string FmtLoadStr(int key, string defaultText, params object[] args)
        {
            var unformattedText = this.LoadStr(key, defaultText);

            try
            {
                return string.Format(unformattedText, args);
            }
            catch
            {
                return unformattedText;
            }
        }

        /// <summary>
        /// Translates the properties.
        /// </summary>
        /// <param name="translatee">The translatee.</param>
        /// <param name="propertyNames">The property names.</param>
        protected void TranslateProperties(Object translatee, List<string> propertyNames)
        {
            var ctrlType = translatee.GetType();

            foreach (var propertyName in propertyNames)
            {
                var lngProperty = ctrlType.GetProperty(propertyName);

                if ((lngProperty != null) && lngProperty.CanRead && lngProperty.CanWrite && (lngProperty.PropertyType == typeof(String)))
                {
                    var valueText = lngProperty.GetValue(translatee, null);

                    if (valueText != null)
                    {
                        var controlText = valueText.ToString();

                        string originalText;

                        int key;

                        if (this.SplitLine(controlText, out key, out originalText))
                        {
                            var translatedText = this.LoadStr(key, originalText);

                            lngProperty.SetValue(translatee, translatedText, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Ermittelt den für die laufende Anwendung gewünschten
        ///   Sprachcode, abhängig von den GEONIS- und den Windows
        ///   Einstellungen. Der Sprachcode ist zweistellig und
        ///   kleingeschrieben.</summary>
        /// <example>"en" für Englisch, "de" für Deutsch.</example>
        /// <returns>Sprachcode, den die Anwendung verwenden sollte.</returns>
        public string FindFavoredLanguageCode()
        {
            return this.FindGeonisLanguageCode();
        }

        /// <summary>
        ///   Ermittelt den in GEONIS eingestellten Sprachcode. Der
        ///   Sprachcode ist zweistellig und kleingeschrieben.</summary>
        /// <returns>GEONIS Sprachcode, null falls Sprache auf
        ///   automatisch eingestellt ist.</returns>
        protected string FindGeonisLanguageCode()
        {
            var regKeyGeocom = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\GEOCOM", false);

            if (regKeyGeocom != null)
            {
                var regValueLng = regKeyGeocom.GetValue("DefaultLanguage");

                if (regValueLng != null)
                {
                    var regValueText = regValueLng.ToString();

                    if (regValueText.Length >= 2)
                        return regValueText.Substring(0, 2).ToLower();
                }
            }

            return null;
        }

        /// <summary>
        ///   Sucht nach der passenden Sprachdatei und gibt deren
        ///   Dateiname mit Pfad zurück.</summary>
        /// <param name="filename">Name der Sprachdatei ohne Pfad, nach
        ///   welcher gesucht wird.</param>
        /// <param name="fallbackLng">Sprachkürzel, welches alternativ
        ///   zur gewünschten Sprache verwendet werden kann.</param>
        /// <returns>Dateiname wenn Sprachdatei gefunden, sonst NULL.</returns>
        protected string FindResFile(string filename, string fallbackLng)
        {
            // search for the favored resource file
            var languageFilename = Path.Combine(AssemblyDirectory, filename);

            if (File.Exists(languageFilename))
                return languageFilename;

            // search for the alternative resource file
            languageFilename = Path.ChangeExtension(languageFilename, fallbackLng);

            return File.Exists(languageFilename) ? languageFilename : null;
        }

       

        /// <summary>
        ///   Teilt eine Zeile aus einer Sprachdatei in die Teile ID und
        ///   Text auf. Am Anfang der Zeile muss die ID stehen, danach
        ///   abgetrennt durch ein Leerzeichen der Text.
        /// </summary>
        /// <param name="line">Zeile aus der Sprachdatei.</param>
        /// <param name="key">Gefundene ID.</param>
        /// <param name="text">Gefundener Text.</param>
        /// <returns>true: falls Zeile aufgeteilt werden konnte,
        ///   false: wenn es sich um eine ungültige Zeile oder einen
        ///   Kommentar handelt.</returns>
        protected bool SplitLine(string line, out int key, out string text)
        {
            key = -1;
            text = null;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            line = line.Trim();

            var numberMatch = Regex.Match(line, @"^\d{1,}");

            if (numberMatch.Success)
            {
                var numberText = line.Substring(0, numberMatch.Length);

                key = int.Parse(numberText);
                
                numberText = line.Substring(numberMatch.Length);
                
                text = numberText.TrimStart();
                
                return true;
            }

            return false;
        }
    }
}