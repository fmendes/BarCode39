using System;
//using System.Collections.Generic;
//using System.Web;
//using System.Collections;
//using System.ComponentModel;
using System.Drawing;
//using System.Data;
using System.IO;

namespace CadCards
{
	/// <summary>
	/// Summary description for BarCode39
	/// </summary>
	public static class BarCode39
	{
		public enum AlignType
		{
			Left, Center, Right
		}

		public enum BarCodeWeight
		{
			Small = 1, Medium, Large
		}

		private static AlignType align = AlignType.Center;
		private static String code = "1234567890";
		private static int leftMargin = 10;
		private static int topMargin = 10;
		private static int height = 50;
		private static bool showHeader;
		private static bool showFooter;
		private static String headerText = "BarCode Demo";
		private static BarCodeWeight weight = BarCodeWeight.Small;
		private static Font headerFont = new Font("Courier", 18);
		private static Font footerFont = new Font("Courier", 8);

		public static AlignType VertAlign
		{
			get { return align; }
			set { align = value; }
		}

		public static String BarCode
		{
			get { return code; }
			set { code = value.ToUpper(); }
		}

		public static int BarCodeHeight
		{
			get { return height; }
			set { height = value; }
		}

		public static int LeftMargin
		{
			get { return leftMargin; }
			set { leftMargin = value; }
		}

		public static int TopMargin
		{
			get { return topMargin; }
			set { topMargin = value; }
		}

		public static bool ShowHeader
		{
			get { return showHeader; }
			set { showHeader = value; }
		}

		public static bool ShowFooter
		{
			get { return showFooter; }
			set { showFooter = value; }
		}

		public static String HeaderText
		{
			get { return headerText; }
			set { headerText = value; }
		}

		public static BarCodeWeight Weight
		{
			get { return weight; }
			set { weight = value; }
		}

		public static Font HeaderFont
		{
			get { return headerFont; }
			set { headerFont = value; }
		}

		public static Font FooterFont
		{
			get { return footerFont; }
			set { footerFont = value; }
		}

		static String alphabet39="0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

		static String [] coded39Char = 
		{
			/* 0 */ "000110100", 
			/* 1 */ "100100001", 
			/* 2 */ "001100001", 
			/* 3 */ "101100000",
			/* 4 */ "000110001", 
			/* 5 */ "100110000", 
			/* 6 */ "001110000", 
			/* 7 */ "000100101",
			/* 8 */ "100100100", 
			/* 9 */ "001100100", 
			/* A */ "100001001", 
			/* B */ "001001001",
			/* C */ "101001000", 
			/* D */ "000011001", 
			/* E */ "100011000", 
			/* F */ "001011000",
			/* G */ "000001101", 
			/* H */ "100001100", 
			/* I */ "001001100", 
			/* J */ "000011100",
			/* K */ "100000011", 
			/* L */ "001000011", 
			/* M */ "101000010", 
			/* N */ "000010011",
			/* O */ "100010010", 
			/* P */ "001010010", 
			/* Q */ "000000111", 
			/* R */ "100000110",
			/* S */ "001000110", 
			/* T */ "000010110", 
			/* U */ "110000001", 
			/* V */ "011000001",
			/* W */ "111000000", 
			/* X */ "010010001", 
			/* Y */ "110010000", 
			/* Z */ "011010000",
			/* - */ "010000101", 
			/* . */ "110000100", 
			/*' '*/ "011000100",
			/* $ */ "010101000",
			/* / */ "010100010", 
			/* + */ "010001010", 
			/* % */ "000101010", 
			/* * */ "010010100" 
		};

		static BarCode39()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static Image CreateBarCodeImage( string strInput, int BarWeight )
		{
			code	= strInput;

			int iWidth = ((strInput.Length - 3) * 11 + 35) * BarWeight;

			// get surface to draw on
			Image objImg = new System.Drawing.Bitmap( iWidth, height);
			Graphics objGR = Graphics.FromImage( objImg );

			String	intercharacterGap	= "0";
			String	str	= '*' + code.ToUpper() + '*';
			int	strLength	= str.Length;
			
			for ( int i = 0; i < code.Length; i++ ) 
			{
				if ( alphabet39.IndexOf( code[ i ] ) == -1 || code[ i ] == '*' )
				{
					throw new Exception( "INVALID BAR CODE TEXT" );
					return null;
				}
			}
			
			String encodedString = "";
			
			for ( int i = 0; i < strLength; i++ ) 
			{
				if ( i > 0 ) 
					encodedString	+= intercharacterGap;
					
				encodedString	+= coded39Char[ alphabet39.IndexOf( str[ i ] ) ];
			}
			
			int	encodedStringLength	= encodedString.Length;
			int	iWidthOfBarCodeString	= 0;
			double	wideToNarrowRatio	= 3;			
			
			if ( align != AlignType.Left )
			{
				for ( int i = 0; i < encodedStringLength; i++ )
				{
					if ( encodedString[ i ] == '1' ) 
						iWidthOfBarCodeString	+= (int)( wideToNarrowRatio * (int) weight );
					else 
						iWidthOfBarCodeString	+= (int) weight;
				}
			}

			int	x	= 0;
			int	wid	= 0;
			int	yTop	= 0;
			SizeF hSize = objGR.MeasureString(headerText, headerFont);
			SizeF fSize = objGR.MeasureString(code, footerFont);

			int	headerX	= 0;
			int	footerX	= 0;

			if (align == AlignType.Left)
			{
				x = leftMargin;
				headerX = leftMargin;
				footerX = leftMargin;
			}
			else if (align == AlignType.Center)
			{
				x = (iWidth - iWidthOfBarCodeString) / 2;
				headerX = (iWidth - (int)hSize.Width) / 2;
				footerX = (iWidth - (int)fSize.Width) / 2;
			}
			else
			{
				x = iWidth - iWidthOfBarCodeString - leftMargin;
				headerX = iWidth - (int)hSize.Width - leftMargin;
				footerX = iWidth - (int)fSize.Width - leftMargin;
			}

			if (showHeader)
			{
				yTop = (int)hSize.Height + topMargin;
				objGR.DrawString(headerText, headerFont, Brushes.Black, headerX, topMargin);
			}
			else
			{
				yTop = topMargin;
			}

			for ( int i=0;i<encodedStringLength; i++)
			{
				if ( encodedString[i]=='1' ) 
					wid=(int)(wideToNarrowRatio*(int)weight);
				else 
					wid=(int)weight;
				objGR.FillRectangle(i % 2 == 0 ? Brushes.Black : Brushes.White, x, yTop, wid, height);
				
				x+=wid;
			}

			yTop += height;

			if (showFooter)
				objGR.DrawString(code, footerFont, Brushes.Black, footerX, yTop);

			return objImg;
		}

		public static byte[] CreateBarCodePNGByteArray( string strInput, int BarWeight )
		{
			byte[] byteArray = new byte[0];
			using (MemoryStream stream = new MemoryStream())
			{
				Image objImg	= CreateBarCodeImage( strInput, BarWeight );

				objImg.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
				stream.Close();

				byteArray = stream.ToArray();
			}
			return byteArray;
		}
	}
}