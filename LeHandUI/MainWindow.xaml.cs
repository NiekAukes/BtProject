using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;


namespace LeHandUI
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
	//	public static IHighlightingDefinition LoadHighlightingDefinition(
	//string resourceName)
	//	{
	//		var fullName = typeof(System.Reflection.Assembly).Namespace + "." + resourceName;
	//		var stream = System.Reflection.Assembly.GetManifestResourceStream(fullName)
	//		var reader = new XmlTextReader(stream)
	//			return HighlightingLoader.Load(reader, HighlightingManager.Instance);
	//	}

		public MainWindow()
		{

			InitializeComponent();
			//Communicator.Init();


		}


    }
}