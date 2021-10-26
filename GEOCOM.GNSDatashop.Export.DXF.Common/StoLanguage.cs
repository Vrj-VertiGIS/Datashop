using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

using System.Diagnostics;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
	/// <summary>
	///   Stellt ein einfaches System zur Verfügung, um Anwendungen
	///   sprachunabhängig zu machen. Die Sprachressourcen werden lediglich
	///   als Textdateien im Verzeichnis der Anwendung plaziert,
	///   TLanguage bestimmt automatisch die gewünschte Sprache.
	///   Mit der "TranslateForm" Methode können automatisch ganze Forms
	///   übersetzt werden, zur Entwurfszeit muss am Anfang der zu
	///   übersetzenden Properties eine Zahl angegeben werden.
	/// </summary>
	/// <example>
	///   In diesem Beispiel heisst die Anwendung
	///     C:\Programs\HelloWorld.exe
	///   und die Sprachdateien heissen wie folgt (der Präfix "Lng."
	///   ist fix vorgegeben).
	///     C:\Programs\Lng.HelloWorld.de
	///     C:\Programs\Lng.HelloWorld.en
	///   die Anwendung hat ein Menu mit dem Property
	///     ToolStripMenuItem.Text = "1001 Open"
	/// 
	///   Sprachobjekt erstellen:
	///     StoLanguage lng = new StoLanguage();
	///   Eine ganze Form übersetzen  
	///     lng.TranslateForm(form1);
	///   Meldung ausgeben
	///     lng.LoadStr(3000, "Are you sure you want to delete?");
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
	///   eingesetzt werden, die Kommentare beginnen mit //. Als
	///   Zeilenumbruch dient die /n Zeichenfolge.
	/// </remarks>
	public class StoLanguage
	{
		#region Fields

		protected string _assemblyDirectory;
		protected string _appName;
		protected string _resFileFallbackLng = "en";
		protected List<string> _translatedProperties =
			new List<string> { "Text", "ToolTipText" };
		protected SortedList<int, string> _resTexts = null;

		#endregion

		#region Object creation

		/// <summary>Konstruktor</summary>
		public StoLanguage()
		{
			// Mod: 26.10.2020 - wah - get calling assembly (of this constructor) from s
			// tack frame so it will be possible to put StoLanguage into a compiled assembly and
			// still have tha caller assembly correctly resolved.
			// This will also enable to have one single "handed-round" StoLanguage _lng object
			// in an entire assembly/application.
			var stackFrame = new StackFrame();
			var caller = stackFrame.GetMethod().DeclaringType.Assembly;
			string assemblyLocation = caller.Location;
			// Pfad des aufrundenden Assembly merken, dieser bestimmt das
			// Verzeichnis, indem die Sprachdatei gesucht wird.
			//string assemblyLocation = Assembly.GetCallingAssembly().Location;
			_assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
			// AppName mit exe oder dll namen initialisieren
			AppName = assemblyLocation;
		}

		public StoLanguage(string assemblyLocation)
		{
			// Pfad des aufrundenden Assembly merken, dieser bestimmt das
			// Verzeichnis, indem die Sprachdatei gesucht wird.
			_assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
			// AppName mit exe oder dll namen initialisieren
			AppName = assemblyLocation;
		}

		#endregion

		#region Public methods and properties

		/// <summary>
		/// Das Property enthält den Namen der Anwendungsdatei. Dieser Name
		/// wird verwendet um die passende Sprachdatei zu finden.
		/// Normalerweise muss das Property nicht selber gesetzt werden.
		/// Beispiel AppName="Sugus" dann wird nach einer Sprachdatei
		/// "Lng.Sugus.DE" gesucht.
		/// </summary>
		public string AppName
		{
			get { return _appName; }
			set
			{
				string filename = Path.GetFileNameWithoutExtension(value);
				// Nur übernehmen, falls geändert
				if (string.Compare(_appName, filename, StringComparison.OrdinalIgnoreCase) != 0)
				{
					_appName = filename;
					_resTexts = null;
				}
			}
		}

		/// <summary>
		/// Sucht anhand einer ID nach dem passenden Text, der geladenen
		/// Sprachressource. Bei Bedarf wird das Sprachfile automatisch
		/// geladen.
		/// </summary>
		/// <param name="key">Die ID des gesuchten Textes.</param>
		/// <param name="defaultText">Dieser Text wird zurückgeliefert,
		/// falls kein Text mit der gesuchten ID vorhanden ist.</param>
		/// <returns>
		/// Text mit der gewünschten ID, oder DefaultText falls Id nicht
		/// gefunden wurde.</returns>
		public string LoadStr(int key, string defaultText)
		{
			LoadFromResFile();
			string result;
			if (!_resTexts.TryGetValue(key, out result))
				result = defaultText;
			return result;
		}

		/// <summary>
		/// Entspricht der Funktion "LoadStr", zusätzlich können aber
		/// zu formatierende Parameter mitgegeben werden.
		/// </summary>
		/// <param name="key">Die ID des gesuchten Textes.</param>
		/// <param name="defaultText">Dieser Text wird zurückgeliefert,
		/// falls kein Text mit der gesuchten ID vorhanden ist.</param>
		/// <param name="args">Parameter, welche formatiert und in den
		/// Text eingefügt werden.</param>
		/// <returns>
		/// Text mit der gewünschten ID, oder DefaultText falls Id nicht
		/// gefunden wurde.</returns>
		public string FmtLoadStr(int key, string defaultText, params object[] args)
		{
			try
			{
				return string.Format(LoadStr(key, defaultText), args);
			}
			catch
			{
				return LoadStr(key, defaultText);
			}
		}

		/// <summary>
		/// Sucht das passende Sprachfile und liest die enthaltenen Texte
		/// ein. Wurde bereits ein Sprachfile eingelesen, so wird die Funktion
		/// nicht ausgeführt. Die Funktion muss nicht explizit aufgerufen
		/// werden, dies wird bei Bedarf automatisch gemacht.
		/// </summary>
		/// <returns>
		/// -1 wenn keine Sprachdatei gefunden werden wurde,
		/// 0 wenn Datei gefunden und erfolgreich eingelesen wurde,
		/// Zeilennummer wenn doppelter Eintrag gefunden wurde.
		/// </returns>
		public int LoadFromResFile()
		{
			// Wurde das Sprachfile bereits eingelesen?
			if (_resTexts != null)
				return 0;

			_resTexts = new SortedList<int, string>();
			string resFilename = FindResFile(_appName, _resFileFallbackLng);
			if (!string.IsNullOrEmpty(resFilename))
			{
				int result = 0;
				using (StreamReader resFileReader = new StreamReader(resFilename, true))
				{
					int lineNumber = 0;
					string lineText = null;
					int resKey;
					string resText;
					// process all lines
					while ((lineText = resFileReader.ReadLine()) != null)
					{
						lineNumber++;
						if (SplitLine(lineText, out resKey, out resText))
						{
							// replace special tags
							StringBuilder resTextBuilder = new StringBuilder(resText);
							resTextBuilder.Replace(@"\n", "\n");
							resText = resTextBuilder.ToString();
							// add to list
							try
							{
								_resTexts.Add(resKey, resText);
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
			else
				return -1;
		}

		/// <summary>
		///   Durchsucht das angegebene Objekt nach bestimmten Properties.
		///   Enthalten diese Properties am Anfang eine Zahl, so wird der
		///   entsprechende Text gesucht und das Property ersetzt.
		///   Die Funktion sucht rekursiv nach weiteren enthaltenen
		///   Objekten.</summary>
		/// <param name="Translatee">Zu übersetzendes Objekt, in der Regel
		///   ein Form Objekt.</param>
		public void TranslateForm(Control Translatee)
		{
			if (Translatee == null)
				throw new ArgumentNullException("Translatee");

			List<ToolTip> toolTips = new List<ToolTip>();
			// translate predefined properties
			TranslateProperties(Translatee, _translatedProperties);
			// check if the VS form designer has created a private
			// field called "components", which contains the non
			// visual components.
			FieldInfo componentsField = Translatee.GetType().GetField(
				"components", BindingFlags.NonPublic | BindingFlags.Instance);
			if (componentsField != null)
			{
				IContainer componentsContainer = componentsField.GetValue(
					Translatee) as IContainer;
				if (componentsContainer != null)
				{
					foreach (Component childComponent in componentsContainer.Components)
					{
						if (childComponent is ToolTip)
						{
							TranslateTooltip(childComponent as ToolTip);
							toolTips.Add(childComponent as ToolTip);
						}
						else if (childComponent is ToolStrip)
						{
							TranslateMenu(((ToolStrip)childComponent).Items);
						}
						else
							TranslateObject(childComponent, null);
					}
				}
			}

			// recursive call for child controls
			foreach (Control childControl in (Translatee as Control).Controls)
			{
				if (childControl is ToolStrip)
				{
					TranslateMenu((childControl as ToolStrip).Items);
				}
				else if (childControl is ToolStripContainer)
				{
					TranslateToolStripContainer(childControl as ToolStripContainer);
				}
				else if (childControl is ListView)
				{
					TranslateListView((ListView) childControl);
				}
				else if (childControl is TreeView)
				{
					TranslateTreeView((TreeView) childControl);
				}
				else if (childControl is SplitContainer)
				{
					TranslateSplitContainer((SplitContainer) childControl);
				}
				else
					TranslateObject(childControl, toolTips);
			}
		}

		public void TranslateProperties(Object Translatee, List<string> PropertyNames)
		{
			Type ctrlType = Translatee.GetType();
			if (ctrlType != null)
			{
				// translate all predefined properties
				PropertyInfo lngProperty;
				string controlText;
				int key;
				string originalText;
				string translatedText;
				object valueText;
				foreach (string propertyName in PropertyNames)
				{
					lngProperty = ctrlType.GetProperty(propertyName);
					if ((lngProperty != null) &&
							lngProperty.CanRead && lngProperty.CanWrite &&
							(lngProperty.PropertyType == typeof(String)))
					{
						valueText = lngProperty.GetValue(Translatee, null);
						if (valueText != null)
						{
							controlText = valueText.ToString();
							if (SplitLine(controlText, out key, out originalText))
							{
								translatedText = LoadStr(key, originalText);
								lngProperty.SetValue(Translatee, translatedText, null);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Ermittelt den für die laufende Anwendung gewünschten
		/// Sprachcode, abhängig von den GEONIS- und den Windows
		/// Einstellungen. Der Sprachcode ist zweistellig und
		/// kleingeschrieben.</summary>
		/// <example>"en" für Englisch, "de" für Deutsch.</example>
		/// <returns>Sprachcode, den die Anwendung verwenden sollte.</returns>
		public static string FindFavoredLanguageCode()
		{
			string result = FindGeonisLanguageCode();
			if (string.IsNullOrEmpty(result))
				result = FindWindowsLanguageCode();
			return result;
		}

		#endregion

		#region Protected methods

		protected void TranslateToolStripPanel(ToolStripPanel Panel)
		{
			foreach (ToolStripPanelRow row in Panel.Rows)
			{
				foreach (Control childControl in row.Controls)
				{
					if (childControl is ToolStrip)
						TranslateMenu((childControl as ToolStrip).Items);
					else
						TranslateObject(childControl, null);
				}
			}
		}

		protected void TranslateObject(Object Translatee, List<ToolTip> ToolTips)
		{
			// translate predefined properties
			TranslateProperties(Translatee, _translatedProperties);
			if (Translatee is Control)
			{
				Control translateeControl = Translatee as Control;
				if (ToolTips != null)
				{
					// translate extender properties
					string controlText;
					int key;
					string originalText;
					string translatedText;
					foreach (ToolTip toolTip in ToolTips)
					{
						controlText = toolTip.GetToolTip(translateeControl);
						if (SplitLine(controlText, out key, out originalText))
						{
							translatedText = LoadStr(key, originalText);
							toolTip.SetToolTip(translateeControl, translatedText);
						}
					}
				}
				// recursive call for child controls
				foreach (Control childControl in translateeControl.Controls)
				{
					TranslateObject(childControl, ToolTips);
				}
			}
		}

		protected void TranslateMenu(ToolStripItemCollection Translatee)
		{
			foreach (ToolStripItem menuItem in Translatee)
			{
				TranslateObject(menuItem, null);
				if (menuItem is ToolStripMenuItem)
					TranslateMenu((menuItem as ToolStripMenuItem).DropDownItems);
			}
		}

		protected void TranslateToolStripContainer(ToolStripContainer tsc)
		{
			TranslateToolStripPanel(tsc.LeftToolStripPanel);
			TranslateToolStripPanel(tsc.RightToolStripPanel);
			TranslateToolStripPanel(tsc.TopToolStripPanel);
			TranslateToolStripPanel(tsc.BottomToolStripPanel);
		}

		protected void TranslateListView(ListView lstView)
		{
			foreach (ColumnHeader column in lstView.Columns)
			{
				TranslateProperties(column, _translatedProperties);
			}
		}

		protected void TranslateTreeView(TreeView treeView)
		{
			treeView.ExpandAll();
			foreach (TreeNode node in treeView.Nodes)
			{
				TranslateTreeNodeRecursive(node);
			}
		}

		protected void TranslateSplitContainer(SplitContainer spContainer)
		{
			TranslateForm(spContainer.Panel1);
			TranslateForm(spContainer.Panel2);
		}

		protected void TranslateTooltip(ToolTip Translatee)
		{
			TranslateProperties(Translatee, new List<string> { "ToolTipTitle" });
		}

		/// <summary>
		/// Sucht nach der passenden Sprachdatei und gibt deren
		/// Dateiname mit Pfad zurück.</summary>
		/// <param name="appName">Name der Anwendung ohne Pfad und ohne
		/// Extension, deren Sprachdatei gesucht wird.</param>
		/// <param name="fallbackLng">Sprachkürzel, welches alternativ
		/// zur gewünschten Sprache verwendet werden kann (ohne Punkt).</param>
		/// <returns>Dateiname wenn Sprachdatei gefunden, sonst "".</returns>
		protected string FindResFile(string appName, string fallbackLng)
		{
			string result = string.Empty;
			string languageCode = FindFavoredLanguageCode();
			// Sprachfile in gewünschter Sprache suchen
			string resFilename = Path.Combine(_assemblyDirectory,
				"Lng." + appName + "." + languageCode);
			if (File.Exists(resFilename))
				result = resFilename;
			else
			{
				languageCode = fallbackLng;
				// Sprachfile in fallback Sprache suchen
				resFilename = Path.Combine(_assemblyDirectory,
					"Lng." + appName + "." + languageCode);
				if (File.Exists(resFilename))
					result = resFilename;
			}
			return result;
		}

		/// <summary>
		/// Ermittelt den in GEONIS eingestellten Sprachcode. Der
		/// Sprachcode ist zweistellig und kleingeschrieben.</summary>
		/// <returns>GEONIS Sprachcode, "" falls Sprache auf
		///   automatisch eingestellt ist.</returns>
		static protected string FindGeonisLanguageCode()
		{
			string result = string.Empty;
			RegistryKey regKeyGeocom = Registry.CurrentUser.OpenSubKey(
				@"SOFTWARE\GEOCOM", false);
			if (regKeyGeocom != null)
			{
				object regValueLng = regKeyGeocom.GetValue("DefaultLanguage");
				if (regValueLng != null)
				{
					string regValueText = regValueLng.ToString();
					if (regValueText.Length >= 2)
						result = regValueText.Substring(0, 2).ToLower();
				}
			}
			return result;
		}

		/// <summary>
		/// Ermittelt den Sprachcode der installierten Windows Version.
		/// Der Sprachcode ist zweistellig und kleingeschrieben.</summary>
		/// <returns>Windows Sprachcode.</returns>
		static protected string FindWindowsLanguageCode()
		{
			return CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
		}

		/// <summary>
		///   Teilt eine Zeile aus einer Sprachdatei in die Teile ID und
		///   Text auf. Am Anfang der Zeile muss die ID stehen, danach
		///   abgetrennt durch ein Leerzeichen der Text.
		/// </summary>
		/// <param name="Line">Zeile aus der Sprachdatei.</param>
		/// <param name="Key">Gefundene ID.</param>
		/// <param name="Text">Gefundener Text.</param>
		/// <returns>true: falls Zeile aufgeteilt werden konnte,
		///   false: wenn es sich um eine ungültige Zeile oder einen
		///   Kommentar handelt.</returns>
		static protected bool SplitLine(string Line, out int Key, out string Text)
		{
			Key = -1;
			Text = null;
			if (Line == null)
				return false;
			Line = Line.Trim();
			if (Line == string.Empty)
				return false;
			// auf führende nummer prüfen
			Match numberMatch = Regex.Match(Line, @"^\d{1,}");
			if (numberMatch.Success)
			{
				// aufteilen
				string numberText = Line.Substring(0, numberMatch.Length);
				Key = int.Parse(numberText);
				numberText = Line.Substring(numberMatch.Length);
				Text = numberText.TrimStart();
				return true;
			}
			else
				return false;
		}

		#endregion

		#region Private helpers

		private void TranslateTreeNodeRecursive(TreeNode node)
		{
			TranslateProperties(node, _translatedProperties);
			foreach (TreeNode child in node.Nodes)
			{
				TranslateTreeNodeRecursive(child);
			}
		}

		#endregion
	}
}
