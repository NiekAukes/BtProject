using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common.CommandLine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Input.Cursors;
using Label = System.Windows.Controls.Label;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

/*COLOUR SCHEME: #212121 , #065464 ,  #34acbc ,     #85c3cf ,      #7a7d84 
                   (33,33,33),   (6,84,100),(52,172,188), (133,195,207), (122,125,132)
                 very dark blue, dark blue, aqua, pale aqua,     greyish
*/

namespace LeHandUI
{
	public partial class SimpleMode : System.Windows.Controls.UserControl
	{
		public static string[] fileNames = SimpleFileManager.FileNames();
		public static List<UIElement> listBoxUIElemtents = new List<UIElement>();

		public List<FileData> CurrentLoadedFileData = new List<FileData>();

		Storyboard st;

		static SolidColorBrush transparent = new SolidColorBrush(Color.FromArgb(0, 100, 100, 100));
		static SolidColorBrush off_white = new SolidColorBrush(Color.FromArgb(255, 242, 242, 242));
		static SolidColorBrush almostTransparentWhite = new SolidColorBrush(Color.FromArgb(80, 242, 242, 242));
		static SolidColorBrush halfTransparentWhite = new SolidColorBrush(Color.FromArgb(140, 242, 242, 242));
		static SolidColorBrush white = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
		static SolidColorBrush black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
		static SolidColorBrush dark_blue = new SolidColorBrush(Color.FromArgb(255, 40, 120, 200));
		static SolidColorBrush light_blue = new SolidColorBrush(Color.FromArgb(225, 110, 130, 255));
		static SolidColorBrush vague_purple = new SolidColorBrush(Color.FromArgb(180, 160, 110, 255)); //nice purple

		bool fAlreadySaving = false;
		#region ImageSourceFromBitmap_func
		//Dit is mijn mooie gekopieerde stackoverflow code
		//If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
		{
			var handle = bmp.GetHbitmap();
			try
			{
				return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally { DeleteObject(handle); }
		}
		#endregion

		#region Construction/Initialisation
		public SimpleMode()
		{
			InitializeComponent();

			addFileImage.Source = ImageSourceFromBitmap(Properties.Resources.AddFile64x64);
			removeFileImage.Source = ImageSourceFromBitmap(Properties.Resources.RemoveFile64x64);
			addRuleImage.Source = ImageSourceFromBitmap(Properties.Resources.PALE_GREEN_AddIcon64x64);
			removeRuleImage.Source = ImageSourceFromBitmap(Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);

			simpleModeFileListBox.ItemsSource = listBoxUIElemtents;

			st = new Storyboard();
			/* //DATA FOR TEST FILES
			FileData[] data = new FileData[6];
			FileData[] newdata = new FileData[1];
			newdata[0] = new FileData(0, 0.2, 0.8, 0, 30, 0);
			SimpleFileManager.ChangeFile("kiekoek", newdata);
			SimpleFileManager.ChangeFile("halloe", newdata);*/

			//INITIALIZE THE TEXTBOXARRAY, adds all textboxes of files to the textBoxes array so that we can check for not changed files and shit
			simpleModeFileListBox.HorizontalAlignment = HorizontalAlignment.Stretch;

			for (int i = 0; i < fileNames.Length; i++)
			{
				Label lbl = new Label();
				StyleNonSelectedLabel(lbl);
				lbl.Content = fileNames[i];


				listBoxUIElemtents.Add(lbl);
			}

			refreshFiles();

			//getfiledata returns null, pls fix volvo
			//FileData dat = SimpleFileManager.GetFileData(0)[0];
			//IDK WHAT THIS DOES, BUT I HAVE COMMENTED IT

		}
		#endregion

		private string prevname = "something wrong";


		public void refreshFiles()
		{
			listBoxUIElemtents.Clear();
			fileNames = SimpleFileManager.FileNames();  //returns all "Simple" file names by splitting the name in the '.' 
														//thus only getting the name, not the extension (ex: .lua gets deleted)

			//NIEK MOET DIT NOG GAAN IMPLEMENTEREN
			int currentOpenedFile = -1;//SimpleFileManager.currentLoadedIndex DOES NOT EXIST YET

			for (int i = 0; i < fileNames.Length; i++)
			{
				TextBox listboxTextBox = null;
				Label listboxLabel = new Label();
				listboxLabel.Content = fileNames[i];
				listBoxUIElemtents.Add(listboxLabel);

				//while (listBoxUIElemtents.Count < fileNames.Length) //to make the length of both arrays identical (well not arrays, a list and an array but fuck it)
				//{
				//	Label newemptylabel = new Label();
				//	listBoxUIElemtents.Add(newemptylabel);
				//}

				//if (listBoxUIElemtents[i] != null)
				//{
				//	try
				//	{
				//		listboxLabel = (Label)listBoxUIElemtents[i];
				//	}
				//	catch (Exception e)
				//	{
				//		listboxTextBox = (TextBox)listBoxUIElemtents[i];
				//		Debug.WriteLine("Caught exception (file is textbox, not label): " + e);
				//	}
				//}

			//	if (listboxTextBox != null || listboxLabel != null)
			//	{

			//		if (listBoxUIElemtents[i] != null || (string)((Label)(listBoxUIElemtents[i])).Content == fileNames[i])
			//		{
			//			continue;
			//		}
			//		else
			//		{
			//			Label lbl = new Label();
			//			lbl.Name = "txtbx " + i.ToString();
			//			lbl.Content = fileNames[i];

			//			if (currentOpenedFile == i)
			//			{
			//				StyleOpenedLabel(lbl);
			//			}

			//			else
			//			{
			//				StyleNonSelectedLabel(lbl);
			//			}

			//			listBoxUIElemtents.RemoveAt(i);
			//			listBoxUIElemtents.Insert(i, lbl);

			//		}
			//	}
			}

			simpleModeFileListBox.Items.Refresh();
		}

		#region LoadnSave
		#region ASCII dictionary of all the buttons that can be pressed
		/*			 Button_MLD  = 0x0002     left button down
					Button_MLU = 0x0004     left button up
				   Button_MRD = 0x0008     right button down
				  Button_MRU = 0x0010     right button up
				 Button_MMD = 0x0020     middle button down
				Button_MMU = 0x0040     middle button up
			   Button_MXD = 0x0080     x button down
			  Button_MXU = 0x0100     x button up
			 Button_MFW = 0x0800     wheel button rolled
			Button_MFHW = 0x01000   hwheel button rolled

			XAML versie in de combobox:
                -Left Mouse Button Click
                -Right Mouse Button Click
                -Middle Mouse Button Click
                -X button click
                -Mouse4 Click
                -Mouse5 Click
                -MouseWheel Up
                -MouseWheel Down
         */
		Dictionary<string, int> ascii_table = new Dictionary<string, int>()
		{
			//MouseButtons
			{"left button down"     , 0x0002},
			{"LBD"					, 0x0002},
			{"left button up"       , 0x0004},
			{"LBU"					, 0x0004},
			{"right button down"    , 0x0008},
			{"RBD"					, 0x0008},
			{"right button up"      , 0x0010},
			{"RBU"					, 0x0010},
			{"middle button down"   , 0x0020},
			{"MBD"					, 0x0020},
			{"middle button up"     , 0x0040},
			{"MBU"					, 0x0040},
			{"x button down"        , 0x0080},
			{"XBD"					, 0x0080},
			{"x button up"          , 0x0100},
			{"XBU"					, 0x0100},
			{"wheel button rolled 1", 0x0100},
			{"WBR1"					, 0x0100},
			{"wheel button rolled 2", 0x0100},
			{"WBR2"					, 0x0100},

			
			/* KeyButtons
			 *  Char Octal Dec Hex Description
					SP	40	32	20	Space
					!	41	33	21	Exclamation mark
					"	42	34	22	Quotation mark (&quot; in HTML)
					#	43	35	23	Cross hatch (number sign)
					$	44	36	24	Dollar sign
					%	45	37	25	Percent sign
					&	46	38	26	Ampersand
					`	47	39	27	Closing single quote (apostrophe)
					(	50	40	28	Opening parentheses
					)	51	41	29	Closing parentheses
					*	52	42	2a	Asterisk (star, multiply)
					+	53	43	2b	Plus
					,	54	44	2c	Comma
					-	55	45	2d	Hyphen, dash, minus
					.	56	46	2e	Period
					/	57	47	2f	Slash (forward or divide)
					0	60	48	30	Zero
					1	61	49	31	One
					2	62	50	32	Two
					3	63	51	33	Three
					4	64	52	34	Four
					5	65	53	35	Five
					6	66	54	36	Six
					7	67	55	37	Seven
					8	70	56	38	Eight
					9	71	57	39	Nine
					:	72	58	3a	Colon
					;	73	59	3b	Semicolon
					<	74	60	3c	Less than sign (&lt; in HTML)
					=	75	61	3d	Equals sign
					>	76	62	3e	Greater than sign (&gt; in HTML)
					?	77	63	3f	Question mark
					@	100	64	40	At-sign
					A	101	65	41	Upper case A
					B	102	66	42	Upper case B
					C	103	67	43	Upper case C
					D	104	68	44	Upper case D
					E	105	69	45	Upper case E
					F	106	70	46	Upper case F
					G	107	71	47	Upper case G
					H	110	72	48	Upper case H
					I	111	73	49	Upper case I
					J	112	74	4a	Upper case J
					K	113	75	4b	Upper case K
					L	114	76	4c	Upper case L
					M	115	77	4d	Upper case M
					N	116	78	4e	Upper case N
					O	117	79	4f	Upper case O
					P	120	80	50	Upper case P
					Q	121	81	51	Upper case Q
					R	122	82	52	Upper case R
					S	123	83	53	Upper case S
					T	124	84	54	Upper case T
					U	125	85	55	Upper case U
					V	126	86	56	Upper case V
					W	127	87	57	Upper case W
					X	130	88	58	Upper case X
					Y	131	89	59	Upper case Y
					Z	132	90	5a	Upper case Z
					[	133	91	5b	Opening square bracket
					\	134	92	5c	Backslash (Reverse slant)
					]	135	93	5d	Closing square bracket
					^	136	94	5e	Caret (Circumflex)
					_	137	95	5f	Underscore
					`	140	96	60	Opening single quote
					a	141	97	61	Lower case a
					b	142	98	62	Lower case b
					c	143	99	63	Lower case c
					d	144	100	64	Lower case d
					e	145	101	65	Lower case e
					f	146	102	66	Lower case f
					g	147	103	67	Lower case g
					h	150	104	68	Lower case h
					i	151	105	69	Lower case i
					j	152	106	6a	Lower case j
					k	153	107	6b	Lower case k
					l	154	108	6c	Lower case l
					m	155	109	6d	Lower case m
					n	156	110	6e	Lower case n
					o	157	111	6f	Lower case o
					p	160	112	70	Lower case p
					q	161	113	71	Lower case q
					r	162	114	72	Lower case r
					s	163	115	73	Lower case s
					t	164	116	74	Lower case t
					u	165	117	75	Lower case u
					v	166	118	76	Lower case v
					w	167	119	77	Lower case w
					x	170	120	78	Lower case x
					y	171	121	79	Lower case y
					z	172	122	7a	Lower case z
					{	173	123	7b	Opening curly brace
					|	174	124	7c	Vertical line
					}	175	125	7d	Closing curly brace
					~	176	126	7e	Tilde (approximate)
					DEL	177	127	7f	Delete (rubout), cross-hatch box
			 */
			{" ", 32},//ik gebruik de decimale values
			{"Spacebar",32},
			{"Space",32},
			{"Spatie",32},
			{"!",33},
			{"#",35},
			{"$",36},
			{"%",37},
			{"&",38},
			{"`",39},
			{"*",42},
			{"+",43 },//IS GOING TO BE USED TO CHAIN KEYPRESSES
			{",",44 },
			{"-",45 },
			{".", 46 },
			{"/",47 },
			{"0",48 },
			{"1",49 },
			{"2",50 },
			{"3",51 },
			{"4",52 },
			{"5",53 },
			{"6",54 },
			{"7",55 },
			{"9",57 },
			{":",58 },
			{";",59 },
			{"<",60 },
			{"=",61 },
			{">",62 },
			{"?",63 },
			{"@",64 },
			{"A",65 },
			{"B",66 },
			{"C",67 },
			{"D",68 },
			{"E",69 }, //nice
			{"F",70 },
			{"G",71 },
			{"H",72 },
			{"I",73 },
			{"J",74 },
			{"K",75 },
			{"L",76 },
			{"M",77 },
			{"N",78 },
			{"O",79 },
			{"P",80 },
			{"Q",81 },
			{"R",82 },
			{"S",83 },
			{"T",84 },
			{"U",85 },
			{"V",86 },
			{"W",87 },
			{"X",88 },
			{"Y",89 },
			{"Z",90 },
			{"[",91},
			{"\\",92 },
			{"]",93 },
			{"^",94 },
			{ "_",95 },
			//{"`",96 },
			/*a	141	97	61	Lower case a
			b	142	98	62	Lower case b
			c	143	99	63	Lower case c
			d	144	100	64	Lower case d
			e	145	101	65	Lower case e
			f	146	102	66	Lower case f
			g	147	103	67	Lower case g
			h	150	104	68	Lower case h
			i	151	105	69	Lower case i
			j	152	106	6a Lower case j
		   k	153	107	6b Lower case k
		  l	154	108	6c Lower case l
		 m	155	109	6d	Lower case m
		 n	156	110	6e	Lower case n
		 o	157	111	6f	Lower case o
		 p	160	112	70	Lower case p
		 q	161	113	71	Lower case q
		 r	162	114	72	Lower case r
		 s	163	115	73	Lower case s
		 t	164	116	74	Lower case t
		 u	165	117	75	Lower case u
		 v	166	118	76	Lower case v
		 w	167	119	77	Lower case w
		 x	170	120	78	Lower case x
		 y	171	121	79	Lower case y
		 z	172	122	7a Lower case z*/
			{"{",123 },
			{"|",124 },
			{"}",125},
			{"~",126 },
			{"DEL",127 },
			{"Delete",127 }
			
		};

        #endregion
        public void LoadFile(int selectedindex)
        {
			if (!fAlreadySaving)
			{
				fAlreadySaving = true;
				CurrentLoadedFileData = new List<FileData>(SimpleFileManager.GetFileData(selectedindex));
				//Clear stackpanel van parametershit
				parameterPanel.Children.Clear();
				for (int i = 0; i < CurrentLoadedFileData.Count; i++)
				{
					simpleModeParameterEditor parEdit = CurrentLoadedFileData[i].toSMPE();//for alle filedata in de file, maak nieuwe parametereditor.xaml en vul alles in de parametereditor xaml in

					parameterPanel.Children.Add(parEdit);
				}
				fAlreadySaving = false;
			}

        }
		public async Task FadePopup(Popup Popup = null)
        {
			saveErrorPopup.IsOpen = true;

			var myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 0.0;
			myDoubleAnimation.To = 1.0;

			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

			myDoubleAnimation.AutoReverse = true ;

			st.Children.Add(myDoubleAnimation);
			Storyboard.SetTargetName(myDoubleAnimation, saveErrorPopup.Name);
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(OpacityProperty));

			st.Begin(saveErrorPopup);

            st.Completed += Storyboard_Completed;
		}

        private void Storyboard_Completed(object sender, EventArgs e)
        {
			saveErrorPopup.IsOpen = false;
        }

        public void SaveChanges()
        {
			//pak alle instellingen van parametereditor xaml user interfaces
			//gooi ze in een filedata list
			//parse de filedata nar simplemodefilemanager
			//SimpleFileManager.ChangeFile() dus
			if (!fAlreadySaving)
			{
				fAlreadySaving = true;
				List<FileData> savedData = new List<FileData>();
				for (int i = 0; i < parameterPanel.Children.Count; i++)
				{
					simpleModeParameterEditor currEditor = (simpleModeParameterEditor)parameterPanel.Children[i];
					FileData newFileData = new FileData();
					newFileData.variable = (byte)currEditor.varChooser.SelectedIndex;
					newFileData.beginRange = currEditor.lowerSlider.Value;
					newFileData.endRange = currEditor.upperSlider.Value;
					newFileData.actionId = (byte)currEditor.actionChooser.SelectedIndex;

					switch (currEditor.actionChooser.SelectedIndex)
					{
						case 0:
							try
							{
								newFileData.arg1 = ascii_table[currEditor.KeyPressChooser.Text];//(char)currEditor.KeyPressChooser.Text;

							}
							catch (KeyNotFoundException)
							{
								Console.WriteLine("key could not be found in dictionary ascii_table");
								//SHOW POPUP THAT SAVING COULD NOT BE COMPLETED

							}

							break;
						case 1: //keypress (insert selected index of mousepresschooser into arg1)
							newFileData.arg1 = currEditor.MousePressChooser.SelectedIndex;
							break;

						case 2: //mousemove (insert arg1 and arg2 with mouse1 and mouse2
							
							//CONVERTS STRINGS INTO LONGS, typing a character into the textbox is prevented in the textbox handlers.
							newFileData.arg1 = long.Parse(currEditor.MouseMoveBox1.Text);
                            
							break;
						case 3:
							//do nothing because the program exit does not require any arguments
							break;
					}

					savedData.Add(newFileData);
				}
				fAlreadySaving = false;
			}
        }
        #endregion

        #region StyleFunctions

        public void StyleNonSelectedLabel(Label lbl)
        {
			lbl.Focusable = true;

			//txtbox.Cursor = Cursors.Arrow;

			lbl.Background = transparent;
			lbl.Foreground = off_white;
			lbl.BorderThickness = new Thickness(0);
			lbl.BorderBrush = transparent;

			lbl.LostKeyboardFocus	+= Lbl_LostKeyboardFocus;
			lbl.PreviewMouseLeftButtonUp	+= LblMouseUp;
            lbl.MouseEnter			+= Lbl_MouseEnter;
			lbl.MouseLeave			+= Lbl_MouseLeave;

			//lbl.Style = this.FindResource("labelSimpleModeStyle") as Style;

		}

		public void styleMouseOverLabel(Label lbl)
        {
			lbl.Cursor = Cursors.Hand;

			if (lbl != simpleModeFileListBox.SelectedItem)
			{
				lbl.Background = almostTransparentWhite;
				lbl.Foreground = white;
			}
			else if(lbl == simpleModeFileListBox.SelectedItem)
            {
				StyleSelectedLabel(lbl);
            }

		}

		public void StyleSelectedLabel(Label lbl)
        {
			lbl.Cursor = Cursors.Hand;
			if (lbl == simpleModeFileListBox.SelectedItem)
			{
				lbl.Background = white;
				lbl.Foreground = black;
				lbl.BorderThickness = new Thickness(0);
				lbl.BorderBrush = white;
			}
            else if(lbl != simpleModeFileListBox.SelectedItem)
            {
				StyleNonSelectedLabel(lbl);
            }

		}

		public void StyleOpenedLabel(Label lbl)
        {
			lbl.Cursor = Cursors.Hand;

			lbl.Foreground = white;
			lbl.Background = dark_blue;
			lbl.BorderBrush = dark_blue;
			lbl.BorderThickness = new Thickness(0);


			for (int i =0; i < simpleModeFileListBox.Items.Count; i++)
            {
				if(simpleModeFileListBox.Items[i] != simpleModeFileListBox.SelectedItem)
                {
					StyleNonSelectedLabel((Label)(simpleModeFileListBox.Items[i]));
                }
            }
		}

        #endregion

        #region saveFileFunc!MOET NOG SPUL GEBEUREN AUKES
        public void saveCurrentFile(string name)
		{
			//get all rules from current file
			int selectedIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedIndex != -1)
			{
				//IList<FileData> currentFileData = SimpleFileManager.GetFileData(selectedIndex);
				IList<FileData> dat = new List<FileData>();

				SimpleFileManager.ChangeFile(name, dat);
			}

			//fix the rest lazy piece of shit douwe

		}
		public void saveCurrentFile()
		{
			//get all rules from current file
			int selectedIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedIndex != -1)
			{
				//IList<FileData> currentFileData = SimpleFileManager.GetFileData(selectedIndex);
				IList<FileData> dat = new List<FileData>();

				//simpleModeParameterEditor[] smpe = 

				SimpleFileManager.ChangeFile((string)((Label)(simpleModeFileListBox.SelectedItem)).Content, dat);
			}

			//fix the rest lazy piece of shit douwe

		}
		#endregion

        #region Label/txtbox Handlers
        private void Txtbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				LabelOrTxtboxLostFocus(sender);
			}
		}
		private void Lbl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			LabelOrTxtboxLostFocus(sender);
			
		}

		private void Lbl_MouseEnter(object sender, MouseEventArgs e) //voor style
        {
			Label lbl = (Label)sender;

			if (lbl != simpleModeFileListBox.SelectedItem)
			{
				styleMouseOverLabel(lbl);
			}
        }

		private void Lbl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) //ook puur voor stijl
		{
			MainWindow.inst.Focus();

			if (sender != simpleModeFileListBox.SelectedItem)
			{
				Label lbl = (Label)sender;
				StyleNonSelectedLabel(lbl);
			}

		}

		public Label prevlbl = null;
		private async void LblMouseUp(object sender, MouseEventArgs e)
		{
			
			Label lbl = (Label)sender;
			
			simpleModeFileListBox.SelectedItem = lbl;
			
			if (lbl != prevlbl) {
				StyleSelectedLabel(lbl);

				//skips loading or saving unless cooldown of 100ms is finished.

				LoadFile(simpleModeFileListBox.SelectedIndex);
					
			}
			if (prevlbl != null)
            {
				StyleNonSelectedLabel(prevlbl);

				SaveChanges();
			}
			prevlbl = lbl;

			//Open the file:
		}

		private void LabelOrTxtboxLostFocus(object sender) //if the text is altered in the textbox, replace the name in the filemanager with the textbox
		{
			Label lbl = (Label)sender;

			if (lbl != simpleModeFileListBox.SelectedItem) //werkt dit? geen flauw idee, vast wel
			{
				StyleNonSelectedLabel(lbl);
			}

			if (((string)(lbl.Content)).Length > 1)
			{
				SimpleFileManager.ChangeName(prevname, (string)lbl.Content);
				refreshFiles();
			}

			else
			{
				lbl.Content = prevname;

				int index = simpleModeFileListBox.Items.IndexOf(sender);
				listBoxUIElemtents.RemoveAt(index);
				listBoxUIElemtents.Insert(index, lbl);
			}
		}

		#endregion

		#region ListBox Handlers

		public static int LastFileOpened = 0xFFFF;
        private void onTextChanged(object sender, TextChangedEventArgs e)
		{
			/*TextBox listitem = (TextBox)sender;
			int index = simpleModeFileListBox.SelectedIndex;
			if (index != -1)
			{
				SimpleFileManager.ChangeName(fileNames[index], listitem.Content);
				refreshFiles();
				dosumShit(sender);
			}*/
		}
		
		#endregion

		#region Button Handlers

		public int totaladdedfiles = 0;
        private void addFileButton_Click(object sender, RoutedEventArgs e)
		{
			//add a filename and ruleset
			FileData[] emptyfiledata = new FileData[1];
			while (true)
			{
				string newFileName = "New file " + totaladdedfiles.ToString();
				if (!SimpleFileManager.CheckName(newFileName))
				{
					SimpleFileManager.ChangeFile(newFileName, emptyfiledata);
					break;
				}
				else totaladdedfiles++;
			}
			totaladdedfiles++;

			refreshFiles();
			saveCurrentFile();
			simpleModeFileListBox.Focus();
		}
		private void removeFileButton_Click(object sender, RoutedEventArgs e)
		{
			int selectedItemIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedItemIndex != -1)
			{
				SimpleFileManager.DeleteFile(selectedItemIndex);
			}
			refreshFiles();
		}

		private async void addRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//add a rule to the ruleset file:
			//naam character: 0x00 + character voor variabele + 2 doubles + character voor action id + 2 keer 4 bytes voor args
			//voorbeeld: 0x01 (als variabele in deze range zit) + 0x01 (één v.d. vingers, xyz as van acceleratie of rotatie) + 0x03 (...) + arg1 t/m arg4
			//voorbeeld: 0x01 0x01 0x03 0x45 0x74 0x19 0x20

			//Moet een usercontrol invoegen (simplemodeparametereditor.xaml) aan de stackpanel in simplemode
			//moet naam geven denk ik

			//await FadePopup(saveErrorPopup);
			List <simpleModeParameterEditor> parameterEditorList = new List<simpleModeParameterEditor>();

			int selectedIndex = simpleModeFileListBox.SelectedIndex; //loopen door de filedata en alles in parameter editor ui zetten
			if (selectedIndex != -1)
			{
				IList<FileData> information = SimpleFileManager.GetFileData(selectedIndex);
				for(int i = 0; i < information.Count; i++)
                {
					simpleModeParameterEditor newEditor = new simpleModeParameterEditor();
					
                }
			}


			saveCurrentFile();
		}
		private void removeRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//remove the shit above

			saveCurrentFile();
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void simpleModeFileListBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			saveCurrentFile();

		}

		private void simpleModeFileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SaveChanges();
		}
	}
}
