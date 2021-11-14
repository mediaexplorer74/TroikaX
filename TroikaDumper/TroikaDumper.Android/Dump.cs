// Dump

using System;
using System.IO;
using System.Linq;
using Tag = Android.Nfc.Tag;
using MifareClassic = Android.Nfc.Tech.MifareClassic;
using Environment = Android.OS.Environment;
using Java.Util;
using System.Diagnostics;

namespace cc.troikadumper
{
	using HexUtils = cc.troikadumper.utils.HexUtils;

	public class Dump
	{
		public const string FILENAME_FORMAT = "%04d-%02d-%02d_%02d%02d%02d_%d_%dRUB.txt";
		public const string FILENAME_REGEXP = "([0-9]{4})-([0-9]{2})-([0-9]{2})_([0-9]{6})_([0-9]+)_([0-9]+)RUB.txt";

		public const int BLOCK_COUNT = 4;
		public static readonly int BLOCK_SIZE = MifareClassic.BlockSize;// BLOCK_SIZE
		public const int SECTOR_INDEX = 8;

		
		public static readonly sbyte[] KEY_0 = new sbyte[] 
		{ 
			(sbyte)0x00, (sbyte)0x00, (sbyte)0x00, (sbyte)0x00, (sbyte)0x00, (sbyte)0x00 
		};

		public static readonly sbyte[] KEY_A = new sbyte[]
		{
			unchecked((sbyte)0xA7), (sbyte)0x3F, (sbyte)0x5D, unchecked((sbyte)0xC1), unchecked((sbyte)0xD3), (sbyte)0x33
		};

		public static readonly sbyte[] KEY_B = new sbyte[]
		{
			unchecked((sbyte)0xE3), (sbyte)0x51, (sbyte)0x73, (sbyte)0x49, (sbyte)0x4A, unchecked((sbyte)0x81)
		};

		// raw
		public static sbyte[] uid;
		public static sbyte[][] data;

		// parsed
		public static int cardNumber;
		public static int balance;
		public static DateTime lastUsageDate;
		public static int lastValidatorId;

		public Dump(sbyte[] uidd, sbyte[][] sector_8)
		{
			uid = uidd; // this.uid = 
			data = sector_8; // this.data =
			parse();
		}


		//
		public static Dump fromTag(Tag tag)
		{
			MifareClassic mfc = getMifareClassic(tag);

			int blockCount = mfc.GetBlockCountInSector(SECTOR_INDEX);
			if (blockCount < BLOCK_COUNT)
			{
				throw new IOException("Wtf? Not enough blocks on this card");
			}


			sbyte[][] data = RectangularArrays.RectangularSbyteArray(BLOCK_COUNT, BLOCK_SIZE);

			for (int i = 0; i < BLOCK_COUNT; i++)
			{
				try
				{
					data[i] = ToSbyte(mfc.ReadBlock(mfc.SectorToBlock(SECTOR_INDEX) + i));
				}
				catch (Exception ex)
				{
					Debug.WriteLine("!! EXCEPTION !! : " + ex.Message);
					data[i][0] = 0;
					data[i][1] = 0;
					data[i][2] = 0;
					data[i][3] = 0;
					
				}
			}

			return new Dump(ToSbyte(tag.GetId()), data);
		}

		/*
		public static Dump fromFile(File file)
		{
			FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);

			Scanner scanner = new Scanner(fs, "US-ASCII");

			sbyte[] uid = HexUtils.fromString(scanner.NextLine());// scanner.nextLine());


			sbyte[][] data = RectangularArrays.RectangularSbyteArray(BLOCK_COUNT, BLOCK_SIZE);
			for (int i = 0; i < BLOCK_COUNT; i++)
			{
				data[i] = HexUtils.fromString(scanner.NextLine());
			}

			return new Dump(uid, data);
		}
		*/


		protected internal static MifareClassic getMifareClassic(Tag tag)
		{
			MifareClassic mfc = MifareClassic.Get(tag);
			mfc.Connect();

			// f****d up card
			if (mfc.AuthenticateSectorWithKeyA(SECTOR_INDEX, ToByte(KEY_0)) 
				&& mfc.AuthenticateSectorWithKeyB(SECTOR_INDEX, ToByte(KEY_0)))
			{
				return mfc;
			}

			// good card
			if (mfc.AuthenticateSectorWithKeyA(SECTOR_INDEX, ToByte(KEY_A)) 
				&& mfc.AuthenticateSectorWithKeyB(SECTOR_INDEX, ToByte(KEY_B)))
			{
				return mfc;
			}

			// not enough rights
			throw new IOException("No permissions");
		}

		protected internal virtual void parse()
		{
			// block#0 bytes#3-6
			cardNumber = intval(data[0][3], data[0][4], data[0][5], data[0][6]) >> 4;

			// incorrect
			//TODO: find correct field for validator ID
			lastValidatorId = intval(data[1][0], data[1][1]);

			int minutesDelta = intval(data[1][0], data[1][1], data[1][2]) >> 1;
			if (1==0)//(minutesDelta > 0)
			{
				//DateTime c;// = Calendar.GetInstance(Java.Util.TimeZone.GetTimeZone("GMT+3"));//.GetTimeZone("GMT+3"));
				//c = new DateTime(2018, 11, 30, 0, 0, 0); // (2018, 11, 31, 0, 0, 0);
				//c = c.AddMinutes(minutesDelta);
				//Dump.lastUsageDate = c;
			}
			else
			{
				Dump.lastUsageDate = DateTime.Now;//null;
			}

			balance = (int)((data[1][5] * 256 + data[1][6]) / 25); //intval((byte)(data[1][5]), (byte) data[1][6]) / 25;
		}

		/*
		public virtual void write(Tag tag)
		{
			MifareClassic mfc = getMifareClassic(tag);

			if (!tag.getId().SequenceEqual(this.Uid))
			{
				throw new IOException("Card UID mismatch: \n" + HexUtils.toString(tag.getId()) + " (card) != " + HexUtils.toString(Uid) + " (dump)");
			}

			int numBlocksToWrite = BLOCK_COUNT - 1; // do not overwrite last block (keys)
			int startBlockIndex = mfc.sectorToBlock(SECTOR_INDEX);
			for (int i = 0; i < numBlocksToWrite; i++)
			{
				mfc.writeBlock(startBlockIndex + i, data[i]);
			}
		}

		
		public virtual File save(File dir)
		{
			string state = Environment.getExternalStorageState();
			if (!Environment.MEDIA_MOUNTED.Equals(state))
			{
				throw new IOException("Can not write to external storage");
			}

			if (!dir.isDirectory())
			{
				throw new IOException("Not a dir");
			}

			if (!dir.exists() && !dir.mkdirs())
			{
				throw new IOException("Can not make save dir");
			}

			File file = new File(dir, makeFilename());
			FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write);
			StreamWriter @out = new StreamWriter(stream);
			@out.Write(UidAsString + "\r\n");
			foreach (string block in DataAsStrings)
			{
				@out.Write(block + "\r\n");
			}
			@out.Close();

			return file;
		}
		*/

		protected internal virtual string makeFilename()
		{
			DateTime now = DateTime.Now;
			return String.Format(FILENAME_FORMAT, now.Year + 1900, now.Month + 1, now.Day, now.Hour, now.Minute, now.Second);//, CardNumber, Balance);
		}

		public sbyte[] Uid
		{
			get
			{
				return uid;
			}
		}

		public virtual string UidAsString
		{
			get
			{
				return HexUtils.toString( Uid );
			}

		}//UidAsString end

		

		public sbyte[][] Data
		{
			get
			{
				return data;
			}
		}

		public string[] DataAsStrings
		{
			get
			{
				string[] blocks = new string[data.Length];
				for (int i = 0; i < data.Length; i++)
				{
					blocks[i] = HexUtils.toString(data[i]);
				}
				return blocks;
			}
		}

		public DateTime LastUsageDate
		{
			get
			{
				return lastUsageDate;
			}
		}

		public string LastUsageDateAsString
		{
			get
			{
				if (lastUsageDate == null)
				{
					return "<NEVER USED>";
				}
				return lastUsageDate.ToString();//DateFormat.getDateTimeInstance(DateFormat.MEDIUM, DateFormat.SHORT).format(lastUsageDate);
			}
		}

		public int LastValidatorId
		{
			get
			{
				return lastValidatorId;
			}
		}

		public string LastValidatorIdAsString
		{
			get
			{
				return "ID# " + LastValidatorId;
			}
		}

		public int Balance
		{
			get
			{
				return balance;
			}
		}

		public string BalanceAsString
		{
			get
			{
				return "" + Balance + " RUB";
			}
		}

		public int CardNumber
		{
			get
			{
				return cardNumber;
			}
		}

		public string CardNumberAsString
		{
			get
			{
				return formatCardNumber(cardNumber);
			}
		}

		public static string formatCardNumber(int cardNumber)
		{
			int cardNum3 = cardNumber % 1000;
			int cardNum2 = (int)Math.Floor((double)cardNumber / 1000) % 1000;
			int cardNum1 = (int)Math.Floor((double)cardNumber / 1000000) % 1000;
			return string.Format("{0:D4} {1:D3} {2:D3}", cardNum1, cardNum2, cardNum3);
		}

		public override string ToString()
		{
			return "[Card UID=" + UidAsString + " " + BalanceAsString + "RUR]";
		}

		protected internal static int intval(params sbyte[] bytes)
		{
			int value = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				int x = (int)bytes[bytes.Length - i - 1];
				while (x < 0)
				{
					x = 256 + x;
				}
				value += (int)( x * Math.Pow(0x100, i) );
			}
			return value;
		}

		// ******************************************


		// Translate from SByte to Byte
		public static byte[] ToByte(sbyte[] sb)
		{
			byte[] b = new byte[sb.Length];

			for (int i = 0; i < sb.Length; i++)
			{
				b[i] = (byte)sb[i];
			}
			return b;

		}//ToByte end


		// // Translate from Byte to SByte
		public static sbyte[] ToSbyte(byte[] b)
		{
			sbyte[] sb = new sbyte[b.Length];

			for (int i = 0; i < b.Length; i++)
			{
				sb[i] = (sbyte)b[i];
			}
			return sb;

		}//ToSbyte end

		//
	}//class end 

}//namespace end


