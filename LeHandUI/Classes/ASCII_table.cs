using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeHandUI
{
    public class ASCII_table
    {
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
		public static Dictionary<string, int> ascii_table = new Dictionary<string, int>()
		{
			
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
			//arrow left, up , right, down have values 37,38,39,40

			{"SPACE",32},
			{"SPACEBAR",32},
			{"SPATIE",32},
			{" ", 32},//ik gebruik de decimale values
			{"!",33},
			{"#",35},
			{"$",36},
			{"%",37},
			{"&",38},
			{"`",39},
			{"*",42},
			{"+",43},//IS GOING TO BE USED TO CHAIN KEYPRESSES
			{",",44},
			{"-",45},
			{".",46},
			{"/",47},
			{"0",48},
			{"1",49},
			{"2",50},
			{"3",51},
			{"4",52},
			{"5",53},
			{"6",54},
			{"7",55},
			{"9",57},
			{":",58},
			{";",59},
			{"<",60},
			{"=",61},
			{">",62},
			{"?",63},
			{"@",64},
			{"A",65},
			{"B",66},
			{"C",67},
			{"D",68},
			{"E",69}, //nice
			{"F",70},
			{"G",71},
			{"H",72},
			{"I",73},
			{"J",74},
			{"K",75},
			{"L",76},
			{"M",77},
			{"N",78},
			{"O",79},
			{"P",80},
			{"Q",81},
			{"R",82},
			{"S",83},
			{"T",84},
			{"U",85},
			{"V",86},
			{"W",87},
			{"X",88},
			{"Y",89},
			{"Z",90},
			{"[",91},
			{"\\",92},
			{"]",93},
			{"^",94},
			{ "_",95},
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
			{"DELETE",127 }

		};

	}
}
