/* 
	Macalifa. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Macalifa.Tags.ID3.ID3v2Frames.TextFrames;
using System.Collections;

namespace Macalifa.Tags.ID3.ID3v2Frames
{
    /// <summary>
    /// Provide a interface for Lenghtable Frames
    /// </summary>
    public interface ILengthable
    {
        /// <summary>
        /// Length of current object
        /// </summary>
        int Length
        {
            get;
        }
    }

    /// <summary>
    /// Provide a class for Price values
    /// </summary>
    public class Price
    {
        /// <summary>
        /// Value of current Price
        /// </summary>
        protected string _Value;
        /// <summary>
        /// Currency of current Price
        /// </summary>
        protected string _Currency;

        /// <summary>
        /// Create new Price class
        /// </summary>
        /// <param name="Currency">Currency of price</param>
        /// <param name="Value">value of price</param>
        public Price(string Currency, string Value)
        {
            this.Currency = Currency;
            this.Value = Value;
        }

        /// <summary>
        /// Create new price class
        /// </summary>
        /// <param name="Data">Data to read price from</param>
        /// <param name="Length">maximum length of data</param>
        public Price(TagStreamUWP TStream, int Length)
        {
            string temp = TStream.ReadText(Length, TextEncodings.Ascii);
            if (temp.Length < 4)
                return;

            _Currency = temp.Substring(0, 3);
            _Value = temp.Substring(3, temp.Length - 3);
        }

        /// <summary>
        /// Gets or sets value of current price
        /// </summary>
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (!IsValidValue(value))
                    throw (new ArgumentException("Value is numeric string can contain '.'"));

                _Value = value;
            }
        }

        /// <summary>
        /// Gets or sets currency of current price
        /// </summary>
        public string Currency
        {
            get
            { return _Currency; }
            set
            {
                if (!IsValidCurrency(value))
                    throw (new ArgumentException("Currency must be 3 letters"));

                _Currency = value.ToUpper();
            }
        }

        /// <summary>
        /// Gets Length of current frame
        /// </summary>
        public int Length
        {
            get
            {
                // 3: Currency
                // 1 Seprator
                return _Value.Length + 4;
            }
        }

        /// <summary>
        /// Returns a System.String that represent current Price
        /// </summary>
        public override string ToString()
        {
            return _Currency + _Value;
        }

        /// <summary>
        /// Indicate if current price is valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (IsValidValue(_Value) && IsValidCurrency(_Currency))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Indicate if specific string is valid currency value
        /// </summary>
        /// <param name="Currency">String to validate</param>
        /// <returns>True if valid otherwise false</returns>
        public static bool IsValidCurrency(string Currency)
        {
            if (Currency.Length != 3)
                return false;

            foreach (char ch in Currency)
                if (!char.IsLetter(ch))
                    return false;

            return true;
        }

        /// <summary>
        /// Indicate if specific string contains valid Price value
        /// </summary>
        /// <param name="Value">String to validate</param>
        public static bool IsValidValue(string Value)
        {
            foreach (char ch in Value)
                if (!char.IsDigit(ch) && ch != '.')
                    return false;

            return true;
        }
    }

    /// <summary>
    /// Provide a class to store date as string
    /// </summary>
    public class SDate
    {
        /* Why use this class instead of DateTime ( available class in .NET ) ?
         * 
         * The DateTime class can't get any type of date. for example
         * in PersianCalendar we have date like 1384/02/31. as you know
         * this is not valid value for DateTime class, becuase february don't have
         * more than 28 days and it lead to Exception
         * in this class all type of dates in any type of calendar is valid
         * for this class the important thing is only
         * Year between 1-9999
         * Month between 1-12
         * Day Between 1-31
         */
        private int _Year;
        private int _Month;
        private int _Day;

        /// <summary>
        /// New SDate
        /// </summary>
        /// <param name="Year">The Year (1-9999)</param>
        /// <param name="Month">The Month (1-12)</param>
        /// <param name="Day">The Day (1-31)</param>
        public SDate(int Year, int Month, int Day)
        {
            this.Year = Year;
            this.Month = Month;
            this.Day = Day;
        }

        /// <summary>
        /// New SDate
        /// </summary>
        /// <param name="DateString">String contain Date in format of yyyy/MM/dd</param>
        public SDate(string DateString)
        {
            string[] st = DateString.Split('/');
            Year = Int32.Parse(st[0]);
            Month = Int32.Parse(st[1]);
            Day = Int32.Parse(st[2]);
        }

        /// <summary>
        /// New SDate from specific FileStream
        /// </summary>
        /// <param name="Data">FileStream represent SDate data</param>
        public SDate(TagStreamUWP TStream)
        {
            string DateSt = TStream.ReadText(8, TextEncodings.Ascii, false);

            int Temp;
            if (Int32.TryParse(DateSt.Substring(0, 4), out Temp))
                Year = Temp;

            if (Int32.TryParse(DateSt.Substring(4, 2), out Temp))
                Month = Temp;

            if (Int32.TryParse(DateSt.Substring(6, 2), out Temp))
                Day = Temp;
        }

        /// <summary>
        /// The Year (1-9999)
        /// </summary>
        public int Year
        {
            get
            { return _Year; }
            private set
            {
                if (value >= 1 && value <= 9999)
                    _Year = value;
                else
                    throw (new ArgumentException(value.ToString() + " is out of range (1-9999)"));
            }
        }

        /// <summary>
        /// The Month (1-12)
        /// </summary>
        public int Month
        {
            get
            { return _Month; }
            private set
            {
                if (value >= 1 && value <= 12)
                    _Month = value;
                else
                    throw (new ArgumentException(value.ToString() + " is not valid month (1-12)"));
            }
        }

        /// <summary>
        /// The Day (1-31)
        /// </summary>
        public int Day
        {
            get
            { return _Day; }
            private set
            {
                if (value >= 1 && value <= 31)
                    _Day = value;
                else
                    throw (new ArgumentException(value.ToString() + " is out of range (1-31)"));
            }
        }

        /// <summary>
        /// Get Date in format of yyyyMMdd
        /// </summary>
        public string String
        {
            get
            {
                return Year.ToString("0000") +
                    Month.ToString("00") +
                    Day.ToString("00");
            }
        }

        /// <summary>
        /// Get Current SDate in format of yyyy/MM/dd
        /// </summary>
        /// <returns>System.String represent current SDate</returns>
        public override string ToString()
        {
            string RSt = "";
            RSt += Year.ToString("0000") + "/";
            RSt += Month.ToString("00") + "/";
            RSt += Day.ToString("00");
            return RSt;
        }

        /// <summary>
        /// Convert current SDate to DateTime
        /// </summary>
        /// <returns>DateTime represent current SDate</returns>
        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }

    /// <summary>
    /// Provide a class to read/write Languages
    /// </summary>
    public class Language
    {
        private string _LanguageID;
        private static Dictionary<string, string> _LanguagesDictionary; // Contain All available languages

        #region -> Constructors <-

        /// <summary>
        /// Create new language
        /// </summary>
        /// <param name="LanguageID">3 character LanguageID or string.empty</param>
        public Language(string LanguageID)
        {
            this.LanguageID = LanguageID;
        }

        /// <summary>
        /// Create new language
        /// </summary>
        public Language()
        { LanguageID = ""; }

        /// <summary>
        /// Create new language from specific Sream
        /// </summary>
        /// <param name="Data">Stream to read language from</param>
        Macalifa.Tags.TagStreamUWP TStream;
        public Language(Stream FS)
        {
            TStream = new Macalifa.Tags.TagStreamUWP(FS);
            Read();
        }

        #endregion

        #region -> Properties <-

        /// <summary>
        /// Gets or sets LanguageID of current language
        /// </summary>
        public string LanguageID
        {
            get
            { return _LanguageID; }
            set
            {
                if (!IsValidLanguageID(value))
                    throw (new ArgumentException("LanguageID is 3 character string or string.empty"));

                _LanguageID = value;
            }
        }

        /// <summary>
        /// Gets lanuguage name according to LanguageID
        /// </summary>
        public string Name
        {
            get
            {
                if (LanguagesDictionary.ContainsKey(_LanguageID.ToLower()))
                    return LanguagesDictionary[LanguageID.ToLower()];
                else
                    return "";
            }
        }

        /// <summary>
        /// Indicate is current language a valid language
        /// </summary>
        public bool IsValidLanguage
        {
            get
            { return IsValidLanguageID(_LanguageID); }
        }

        #endregion

        #region -> Methods <-

        /// <summary>
        /// Read language from specific FileStream
        /// </summary>
        /// <param name="Data">FileStream to read language from</param>
        private void Read()
        {
            byte[] Buf = new byte[3];
            TStream.FS.Read(Buf, 0, 3);
            if (Buf[0] == 0 && Buf[1] == 0 && Buf[2] == 0)
                _LanguageID = "";

            string Temp = Encoding.ASCII.GetString(Buf);

            if (IsValidLanguageID(Temp))
                _LanguageID = Temp;
            else
                _LanguageID = "";
        }

        /// <summary>
        /// Write LanguageID of current language to specific Stream
        /// </summary>
        /// <param name="Data"></param>
        public void Write(Stream Data)
        {
            byte[] Buf;
            if (LanguageID == "")
                Buf = new byte[] { 0, 0, 0 };
            else
                Buf = Encoding.ASCII.GetBytes(_LanguageID);
            Data.Write(Buf, 0, 3);
        }

        #endregion

        /// <summary>
        /// Indicate if specific string is valid LanguageID
        /// </summary>
        /// <param name="Language">LanguageID to control</param>
        /// <returns>true if valid otherwise false</returns>
        public static bool IsValidLanguageID(string Language)
        {
            if (Language.Length != 3)
                return false;
            else
                foreach (char ch in Language)
                    if (!char.IsLetter(ch))
                        return false;

            return true;
        }

        #region -> Languages List methods and properties <-

        /// <summary>
        /// Gets dictionary of languages
        /// </summary>
        private static Dictionary<string, string> LanguagesDictionary
        {
            get
            {
                if (_LanguagesDictionary == null)
                {
                    _LanguagesDictionary = new Dictionary<string, string>();
                    InitializeLanguages();
                }

                return _LanguagesDictionary;
            }
        }

        /// <summary>
        /// Add languages specification to _Languages Dictionary
        /// </summary>
        private static void InitializeLanguages()
        {
            _LanguagesDictionary.Add("", "");
            _LanguagesDictionary.Add("abk", "Abkhazian");
            _LanguagesDictionary.Add("ace", "Achinese");
            _LanguagesDictionary.Add("ach", "Acoli");
            _LanguagesDictionary.Add("ada", "Adangme");
            _LanguagesDictionary.Add("ady", "Adygei");
            _LanguagesDictionary.Add("aar", "Afar");
            _LanguagesDictionary.Add("afh", "Afrihili");
            _LanguagesDictionary.Add("afr", "Afrikaans");
            _LanguagesDictionary.Add("afa", "Afro-Asiatic (Other)");
            _LanguagesDictionary.Add("ain", "Ainu");
            _LanguagesDictionary.Add("aka", "Akan");
            _LanguagesDictionary.Add("akk", "Akkadian");
            _LanguagesDictionary.Add("alb", "Albanian");
            _LanguagesDictionary.Add("ale", "Aleut");
            _LanguagesDictionary.Add("alg", "Algonquian languages");
            _LanguagesDictionary.Add("tut", "Altaic (Other)");
            _LanguagesDictionary.Add("amh", "Amharic");
            _LanguagesDictionary.Add("anp", "Angika");
            _LanguagesDictionary.Add("apa", "Apache languages");
            _LanguagesDictionary.Add("ara", "Arabic");
            _LanguagesDictionary.Add("arg", "Aragonese");
            _LanguagesDictionary.Add("arc", "Aramaic");
            _LanguagesDictionary.Add("arp", "Arapaho");
            _LanguagesDictionary.Add("arw", "Arawak");
            _LanguagesDictionary.Add("arm", "Armenian");
            _LanguagesDictionary.Add("art", "Artificial (Other)");
            _LanguagesDictionary.Add("asm", "Assamese");
            _LanguagesDictionary.Add("ast", "Asturian; Bable");
            _LanguagesDictionary.Add("ath", "Athapascan languages");
            _LanguagesDictionary.Add("aus", "Australian languages");
            _LanguagesDictionary.Add("map", "Austronesian (Other)");
            _LanguagesDictionary.Add("ava", "Avaric");
            _LanguagesDictionary.Add("ave", "Avestan");
            _LanguagesDictionary.Add("awa", "Awadhi");
            _LanguagesDictionary.Add("aym", "Aymara");
            _LanguagesDictionary.Add("aze", "Azerbaijani");
            _LanguagesDictionary.Add("ban", "Balinese");
            _LanguagesDictionary.Add("bat", "Baltic (Other)");
            _LanguagesDictionary.Add("bal", "Baluchi");
            _LanguagesDictionary.Add("bam", "Bambara");
            _LanguagesDictionary.Add("bai", "Bamileke languages");
            _LanguagesDictionary.Add("bad", "Banda languages");
            _LanguagesDictionary.Add("bnt", "Bantu (Other)");
            _LanguagesDictionary.Add("bas", "Basa");
            _LanguagesDictionary.Add("bak", "Bashkir");
            _LanguagesDictionary.Add("baq", "Basque");
            _LanguagesDictionary.Add("btk", "Batak languages");
            _LanguagesDictionary.Add("bej", "Beja");
            _LanguagesDictionary.Add("bel", "Belarusian");
            _LanguagesDictionary.Add("bem", "Bemba");
            _LanguagesDictionary.Add("ben", "Bengali");
            _LanguagesDictionary.Add("ber", "Berber (Other)");
            _LanguagesDictionary.Add("bho", "Bhojpuri");
            _LanguagesDictionary.Add("bih", "Bihari");
            _LanguagesDictionary.Add("bik", "Bikol");
            _LanguagesDictionary.Add("bin", "Bini; Edo");
            _LanguagesDictionary.Add("bis", "Bislama");
            _LanguagesDictionary.Add("byn", "Blin; Bilin");
            _LanguagesDictionary.Add("bos", "Bosnian");
            _LanguagesDictionary.Add("bra", "Braj");
            _LanguagesDictionary.Add("bre", "Breton");
            _LanguagesDictionary.Add("bug", "Buginese");
            _LanguagesDictionary.Add("bul", "Bulgarian");
            _LanguagesDictionary.Add("bua", "Buriat");
            _LanguagesDictionary.Add("bur", "Burmese");
            _LanguagesDictionary.Add("cad", "Caddo");
            _LanguagesDictionary.Add("cau", "Caucasian (Other)");
            _LanguagesDictionary.Add("ceb", "Cebuano");
            _LanguagesDictionary.Add("cel", "Celtic (Other)");
            _LanguagesDictionary.Add("cai", "Central American Indian (Other)");
            _LanguagesDictionary.Add("khm", "Central Khmer");
            _LanguagesDictionary.Add("chg", "Chagatai");
            _LanguagesDictionary.Add("cmc", "Chamic languages");
            _LanguagesDictionary.Add("cha", "Chamorro");
            _LanguagesDictionary.Add("che", "Chechen");
            _LanguagesDictionary.Add("chr", "Cherokee");
            _LanguagesDictionary.Add("chy", "Cheyenne");
            _LanguagesDictionary.Add("chb", "Chibcha");
            _LanguagesDictionary.Add("chi", "Chinese");
            _LanguagesDictionary.Add("chn", "Chinook jargon");
            _LanguagesDictionary.Add("chp", "Chipewyan");
            _LanguagesDictionary.Add("cho", "Choctaw");
            _LanguagesDictionary.Add("chk", "Chuukese");
            _LanguagesDictionary.Add("chv", "Chuvash");
            _LanguagesDictionary.Add("nwc", "Classical Nepal Bhasa");
            _LanguagesDictionary.Add("rar", "Cook Islands Maori");
            _LanguagesDictionary.Add("cop", "Coptic");
            _LanguagesDictionary.Add("cor", "Cornish");
            _LanguagesDictionary.Add("cos", "Corsican");
            _LanguagesDictionary.Add("cre", "Cree");
            _LanguagesDictionary.Add("mus", "Creek");
            _LanguagesDictionary.Add("crp", "Creoles and pidgins (Other)");
            _LanguagesDictionary.Add("crh", "Crimean Turkish");
            _LanguagesDictionary.Add("scr", "Croatian");
            _LanguagesDictionary.Add("cus", "Cushitic (Other)");
            _LanguagesDictionary.Add("cze", "Czech");
            _LanguagesDictionary.Add("dak", "Dakota");
            _LanguagesDictionary.Add("dan", "Danish");
            _LanguagesDictionary.Add("dar", "Dargwa");
            _LanguagesDictionary.Add("del", "Delaware");
            _LanguagesDictionary.Add("din", "Dinka");
            _LanguagesDictionary.Add("doi", "Dogri");
            _LanguagesDictionary.Add("dgr", "Dogrib");
            _LanguagesDictionary.Add("dra", "Dravidian (Other)");
            _LanguagesDictionary.Add("dua", "Duala");
            _LanguagesDictionary.Add("dum", "Dutch, Middle (ca.1050-1350)");
            _LanguagesDictionary.Add("dyu", "Dyula");
            _LanguagesDictionary.Add("dzo", "Dzongkha");
            _LanguagesDictionary.Add("frs", "Eastern Frisian");
            _LanguagesDictionary.Add("efi", "Efik");
            _LanguagesDictionary.Add("egy", "Egyptian (Ancient)");
            _LanguagesDictionary.Add("eka", "Ekajuk");
            _LanguagesDictionary.Add("elx", "Elamite");
            _LanguagesDictionary.Add("eng", "English");
            _LanguagesDictionary.Add("cpe", "English based (Other)");
            _LanguagesDictionary.Add("enm", "English, Middle (1100-1500)");
            _LanguagesDictionary.Add("ang", "English, Old (ca.450-1100)");
            _LanguagesDictionary.Add("myv", "Erzya");
            _LanguagesDictionary.Add("epo", "Esperanto");
            _LanguagesDictionary.Add("est", "Estonian");
            _LanguagesDictionary.Add("ewe", "Ewe");
            _LanguagesDictionary.Add("ewo", "Ewondo");
            _LanguagesDictionary.Add("fan", "Fang");
            _LanguagesDictionary.Add("fat", "Fanti");
            _LanguagesDictionary.Add("fao", "Faroese");
            _LanguagesDictionary.Add("fij", "Fijian");
            _LanguagesDictionary.Add("fin", "Finnish");
            _LanguagesDictionary.Add("fiu", "Finno-Ugrian (Other)");
            _LanguagesDictionary.Add("dut", "Flemish");
            _LanguagesDictionary.Add("fon", "Fon");
            _LanguagesDictionary.Add("cpf", "French-based (Other)");
            _LanguagesDictionary.Add("fre", "French");
            _LanguagesDictionary.Add("frm", "French, Middle (ca.1400-1600)");
            _LanguagesDictionary.Add("fro", "French, Old (842-ca.1400)");
            _LanguagesDictionary.Add("fur", "Friulian");
            _LanguagesDictionary.Add("ful", "Fulah");
            _LanguagesDictionary.Add("gaa", "Ga");
            _LanguagesDictionary.Add("car", "Galibi Carib");
            _LanguagesDictionary.Add("glg", "Galician");
            _LanguagesDictionary.Add("lug", "Ganda");
            _LanguagesDictionary.Add("gay", "Gayo");
            _LanguagesDictionary.Add("gba", "Gbaya");
            _LanguagesDictionary.Add("gez", "Geez");
            _LanguagesDictionary.Add("geo", "Georgian");
            _LanguagesDictionary.Add("ger", "German");
            _LanguagesDictionary.Add("gmh", "German, Middle High (ca.1050-1500)");
            _LanguagesDictionary.Add("goh", "German, Old High (ca.750-1050)");
            _LanguagesDictionary.Add("gem", "Germanic (Other)");
            _LanguagesDictionary.Add("gil", "Gilbertese");
            _LanguagesDictionary.Add("gon", "Gondi");
            _LanguagesDictionary.Add("gor", "Gorontalo");
            _LanguagesDictionary.Add("got", "Gothic");
            _LanguagesDictionary.Add("grb", "Grebo");
            _LanguagesDictionary.Add("grc", "Greek, Ancient (to 1453)");
            _LanguagesDictionary.Add("gre", "Greek, Modern (1453-)");
            _LanguagesDictionary.Add("kal", "Greenlandic");
            _LanguagesDictionary.Add("grn", "Guarani");
            _LanguagesDictionary.Add("guj", "Gujarati");
            _LanguagesDictionary.Add("gwi", "Gwich´in");
            _LanguagesDictionary.Add("hai", "Haida");
            _LanguagesDictionary.Add("hat", "Haitian Creole");
            _LanguagesDictionary.Add("hau", "Hausa");
            _LanguagesDictionary.Add("haw", "Hawaiian");
            _LanguagesDictionary.Add("heb", "Hebrew");
            _LanguagesDictionary.Add("her", "Herero");
            _LanguagesDictionary.Add("hil", "Hiligaynon");
            _LanguagesDictionary.Add("him", "Himachali");
            _LanguagesDictionary.Add("hin", "Hindi");
            _LanguagesDictionary.Add("hmo", "Hiri Motu");
            _LanguagesDictionary.Add("hit", "Hittite");
            _LanguagesDictionary.Add("hmn", "Hmong");
            _LanguagesDictionary.Add("hun", "Hungarian");
            _LanguagesDictionary.Add("hup", "Hupa");
            _LanguagesDictionary.Add("iba", "Iban");
            _LanguagesDictionary.Add("ice", "Icelandic");
            _LanguagesDictionary.Add("ido", "Ido");
            _LanguagesDictionary.Add("ibo", "Igbo");
            _LanguagesDictionary.Add("ijo", "Ijo languages");
            _LanguagesDictionary.Add("ilo", "Iloko");
            _LanguagesDictionary.Add("smn", "Inari Sami");
            _LanguagesDictionary.Add("inc", "Indic (Other)");
            _LanguagesDictionary.Add("ine", "Indo-European (Other)");
            _LanguagesDictionary.Add("ind", "Indonesian");
            _LanguagesDictionary.Add("inh", "Ingush");
            _LanguagesDictionary.Add("ina", "Interlingua (International Auxiliary Language Association)");
            _LanguagesDictionary.Add("ile", "Interlingue");
            _LanguagesDictionary.Add("iku", "Inuktitut");
            _LanguagesDictionary.Add("ipk", "Inupiaq");
            _LanguagesDictionary.Add("ira", "Iranian (Other)");
            _LanguagesDictionary.Add("gle", "Irish");
            _LanguagesDictionary.Add("mga", "Irish, Middle (900-1200)");
            _LanguagesDictionary.Add("sga", "Irish, Old (to 900)");
            _LanguagesDictionary.Add("iro", "Iroquoian languages");
            _LanguagesDictionary.Add("ita", "Italian");
            _LanguagesDictionary.Add("jpn", "Japanese");
            _LanguagesDictionary.Add("jav", "Javanese");
            _LanguagesDictionary.Add("kac", "Jingpho");
            _LanguagesDictionary.Add("jrb", "Judeo-Arabic");
            _LanguagesDictionary.Add("jpr", "Judeo-Persian");
            _LanguagesDictionary.Add("kbd", "Kabardian");
            _LanguagesDictionary.Add("kab", "Kabyle");
            _LanguagesDictionary.Add("kam", "Kamba");
            _LanguagesDictionary.Add("kan", "Kannada");
            _LanguagesDictionary.Add("kau", "Kanuri");
            _LanguagesDictionary.Add("kaa", "Kara-Kalpak");
            _LanguagesDictionary.Add("krc", "Karachay-Balkar");
            _LanguagesDictionary.Add("krl", "Karelian");
            _LanguagesDictionary.Add("kar", "Karen languages");
            _LanguagesDictionary.Add("kas", "Kashmiri");
            _LanguagesDictionary.Add("csb", "Kashubian");
            _LanguagesDictionary.Add("kaw", "Kawi");
            _LanguagesDictionary.Add("kaz", "Kazakh");
            _LanguagesDictionary.Add("kha", "Khasi");
            _LanguagesDictionary.Add("khi", "Khoisan (Other)");
            _LanguagesDictionary.Add("kho", "Khotanese");
            _LanguagesDictionary.Add("kik", "Kikuyu; Gikuyu");
            _LanguagesDictionary.Add("kmb", "Kimbundu");
            _LanguagesDictionary.Add("kin", "Kinyarwanda");
            _LanguagesDictionary.Add("tlh", "Klingon");
            _LanguagesDictionary.Add("kom", "Komi");
            _LanguagesDictionary.Add("kon", "Kongo");
            _LanguagesDictionary.Add("kok", "Konkani");
            _LanguagesDictionary.Add("kor", "Korean");
            _LanguagesDictionary.Add("kos", "Kosraean");
            _LanguagesDictionary.Add("kpe", "Kpelle");
            _LanguagesDictionary.Add("kro", "Kru languages");
            _LanguagesDictionary.Add("kum", "Kumyk");
            _LanguagesDictionary.Add("kur", "Kurdish");
            _LanguagesDictionary.Add("kru", "Kurukh");
            _LanguagesDictionary.Add("kut", "Kutenai");
            _LanguagesDictionary.Add("kua", "Kwanyama");
            _LanguagesDictionary.Add("kir", "Kyrgyz");
            _LanguagesDictionary.Add("lad", "Ladino");
            _LanguagesDictionary.Add("lah", "Lahnda");
            _LanguagesDictionary.Add("lam", "Lamba");
            _LanguagesDictionary.Add("day", "Land Dayak languages");
            _LanguagesDictionary.Add("lao", "Lao");
            _LanguagesDictionary.Add("lat", "Latin");
            _LanguagesDictionary.Add("lav", "Latvian");
            _LanguagesDictionary.Add("ltz", "Letzeburgesch");
            _LanguagesDictionary.Add("lez", "Lezghian");
            _LanguagesDictionary.Add("lim", "Limburgish");
            _LanguagesDictionary.Add("lin", "Lingala");
            _LanguagesDictionary.Add("lit", "Lithuanian");
            _LanguagesDictionary.Add("jbo", "Lojban");
            _LanguagesDictionary.Add("nds", "Low German");
            _LanguagesDictionary.Add("dsb", "Lower Sorbian");
            _LanguagesDictionary.Add("loz", "Lozi");
            _LanguagesDictionary.Add("lub", "Luba-Katanga");
            _LanguagesDictionary.Add("lua", "Luba-Lulua");
            _LanguagesDictionary.Add("lui", "Luiseno");
            _LanguagesDictionary.Add("smj", "Lule Sami");
            _LanguagesDictionary.Add("lun", "Lunda");
            _LanguagesDictionary.Add("luo", "Luo (Kenya and Tanzania)");
            _LanguagesDictionary.Add("lus", "Lushai");
            _LanguagesDictionary.Add("rup", "Macedo-Romanian");
            _LanguagesDictionary.Add("mac", "Macedonian");
            _LanguagesDictionary.Add("mad", "Madurese");
            _LanguagesDictionary.Add("mag", "Magahi");
            _LanguagesDictionary.Add("mai", "Maithili");
            _LanguagesDictionary.Add("mak", "Makasar");
            _LanguagesDictionary.Add("mlg", "Malagasy");
            _LanguagesDictionary.Add("may", "Malay");
            _LanguagesDictionary.Add("mal", "Malayalam");
            _LanguagesDictionary.Add("div", "Maldivian");
            _LanguagesDictionary.Add("mlt", "Maltese");
            _LanguagesDictionary.Add("mnc", "Manchu");
            _LanguagesDictionary.Add("mdr", "Mandar");
            _LanguagesDictionary.Add("man", "Mandingo");
            _LanguagesDictionary.Add("mni", "Manipuri");
            _LanguagesDictionary.Add("mno", "Manobo languages");
            _LanguagesDictionary.Add("glv", "Manx");
            _LanguagesDictionary.Add("mao", "Maori");
            _LanguagesDictionary.Add("arn", "Mapuche");
            _LanguagesDictionary.Add("mar", "Marathi");
            _LanguagesDictionary.Add("chm", "Mari");
            _LanguagesDictionary.Add("mah", "Marshallese");
            _LanguagesDictionary.Add("mwr", "Marwari");
            _LanguagesDictionary.Add("mas", "Masai");
            _LanguagesDictionary.Add("myn", "Mayan languages");
            _LanguagesDictionary.Add("men", "Mende");
            _LanguagesDictionary.Add("mic", "Mi'kmaq; Micmac");
            _LanguagesDictionary.Add("min", "Minangkabau");
            _LanguagesDictionary.Add("mwl", "Mirandese");
            _LanguagesDictionary.Add("mis", "Miscellaneous languages");
            _LanguagesDictionary.Add("moh", "Mohawk");
            _LanguagesDictionary.Add("mdf", "Moksha");
            _LanguagesDictionary.Add("mol", "Moldavian");
            _LanguagesDictionary.Add("mkh", "Mon-Khmer (Other)");
            _LanguagesDictionary.Add("lol", "Mongo");
            _LanguagesDictionary.Add("mon", "Mongolian");
            _LanguagesDictionary.Add("mos", "Mossi");
            _LanguagesDictionary.Add("mul", "Multiple languages");
            _LanguagesDictionary.Add("mun", "Munda languages");
            _LanguagesDictionary.Add("nqo", "N'Ko");
            _LanguagesDictionary.Add("nah", "Nahuatl languages");
            _LanguagesDictionary.Add("nau", "Nauru");
            _LanguagesDictionary.Add("nav", "Navajo; Navaho");
            _LanguagesDictionary.Add("ndo", "Ndonga");
            _LanguagesDictionary.Add("nap", "Neapolitan");
            _LanguagesDictionary.Add("new", "Nepal Bhasa");
            _LanguagesDictionary.Add("nep", "Nepali");
            _LanguagesDictionary.Add("nia", "Nias");
            _LanguagesDictionary.Add("nic", "Niger-Kordofanian (Other)");
            _LanguagesDictionary.Add("ssa", "Nilo-Saharan (Other)");
            _LanguagesDictionary.Add("niu", "Niuean");
            _LanguagesDictionary.Add("nog", "Nogai");
            _LanguagesDictionary.Add("non", "Norse, Old");
            _LanguagesDictionary.Add("nai", "North American Indian");
            _LanguagesDictionary.Add("nde", "North Ndebele");
            _LanguagesDictionary.Add("frr", "Northern Frisian");
            _LanguagesDictionary.Add("sme", "Northern Sami");
            _LanguagesDictionary.Add("nso", "Northern Sotho");
            _LanguagesDictionary.Add("nor", "Norwegian");
            _LanguagesDictionary.Add("nob", "Norwegian Bokmal");
            _LanguagesDictionary.Add("nub", "Nubian languages");
            _LanguagesDictionary.Add("nym", "Nyamwezi");
            _LanguagesDictionary.Add("nya", "Nyanja");
            _LanguagesDictionary.Add("nyn", "Nyankole");
            _LanguagesDictionary.Add("nno", "Nynorsk Norwegian");
            _LanguagesDictionary.Add("nyo", "Nyoro");
            _LanguagesDictionary.Add("nzi", "Nzima");
            _LanguagesDictionary.Add("oci", "Occitan (post 1500)");
            _LanguagesDictionary.Add("xal", "Oirat");
            _LanguagesDictionary.Add("oji", "Ojibwa");
            _LanguagesDictionary.Add("chu", "Old Church Slavonic");
            _LanguagesDictionary.Add("ori", "Oriya");
            _LanguagesDictionary.Add("orm", "Oromo");
            _LanguagesDictionary.Add("osa", "Osage");
            _LanguagesDictionary.Add("oss", "Ossetic");
            _LanguagesDictionary.Add("oto", "Otomian languages");
            _LanguagesDictionary.Add("pal", "Pahlavi");
            _LanguagesDictionary.Add("pau", "Palauan");
            _LanguagesDictionary.Add("pli", "Pali");
            _LanguagesDictionary.Add("pam", "Pampanga");
            _LanguagesDictionary.Add("pag", "Pangasinan");
            _LanguagesDictionary.Add("pap", "Papiamento");
            _LanguagesDictionary.Add("paa", "Papuan (Other)");
            _LanguagesDictionary.Add("per", "Persian");
            _LanguagesDictionary.Add("peo", "Persian, Old (ca.600-400 B.C.)");
            _LanguagesDictionary.Add("phi", "Philippine (Other)");
            _LanguagesDictionary.Add("phn", "Phoenician");
            _LanguagesDictionary.Add("fil", "Pilipino");
            _LanguagesDictionary.Add("pon", "Pohnpeian");
            _LanguagesDictionary.Add("pol", "Polish");
            _LanguagesDictionary.Add("cpp", "Portuguese-based (Other)");
            _LanguagesDictionary.Add("por", "Portuguese");
            _LanguagesDictionary.Add("pra", "Prakrit languages");
            _LanguagesDictionary.Add("pro", "Provencal, Old (to 1500)");
            _LanguagesDictionary.Add("pan", "Punjabi");
            _LanguagesDictionary.Add("pus", "Pushto");
            _LanguagesDictionary.Add("que", "Quechua");
            _LanguagesDictionary.Add("raj", "Rajasthani");
            _LanguagesDictionary.Add("rap", "Rapanui");
            _LanguagesDictionary.Add("roa", "Romance (Other)");
            _LanguagesDictionary.Add("rum", "Romanian");
            _LanguagesDictionary.Add("roh", "Romansh");
            _LanguagesDictionary.Add("rom", "Romany");
            _LanguagesDictionary.Add("run", "Rundi");
            _LanguagesDictionary.Add("rus", "Russian");
            _LanguagesDictionary.Add("sal", "Salishan languages");
            _LanguagesDictionary.Add("sam", "Samaritan Aramaic");
            _LanguagesDictionary.Add("smi", "Sami languages (Other)");
            _LanguagesDictionary.Add("smo", "Samoan");
            _LanguagesDictionary.Add("sad", "Sandawe");
            _LanguagesDictionary.Add("sag", "Sango");
            _LanguagesDictionary.Add("san", "Sanskrit");
            _LanguagesDictionary.Add("sat", "Santali");
            _LanguagesDictionary.Add("srd", "Sardinian");
            _LanguagesDictionary.Add("sas", "Sasak");
            _LanguagesDictionary.Add("sco", "Scots");
            _LanguagesDictionary.Add("gla", "Scottish Gaelic");
            _LanguagesDictionary.Add("sel", "Selkup");
            _LanguagesDictionary.Add("sem", "Semitic (Other)");
            _LanguagesDictionary.Add("scc", "Serbian");
            _LanguagesDictionary.Add("srr", "Serer");
            _LanguagesDictionary.Add("shn", "Shan");
            _LanguagesDictionary.Add("sna", "Shona");
            _LanguagesDictionary.Add("iii", "Sichuan Yi");
            _LanguagesDictionary.Add("scn", "Sicilian");
            _LanguagesDictionary.Add("sid", "Sidamo");
            _LanguagesDictionary.Add("sgn", "Sign Languages");
            _LanguagesDictionary.Add("bla", "Siksika");
            _LanguagesDictionary.Add("snd", "Sindhi");
            _LanguagesDictionary.Add("sin", "Sinhala; Sinhalese");
            _LanguagesDictionary.Add("sit", "Sino-Tibetan (Other)");
            _LanguagesDictionary.Add("sio", "Siouan languages");
            _LanguagesDictionary.Add("sms", "Skolt Sami");
            _LanguagesDictionary.Add("den", "Slave (Athapascan)");
            _LanguagesDictionary.Add("sla", "Slavic (Other)");
            _LanguagesDictionary.Add("slo", "Slovak");
            _LanguagesDictionary.Add("slv", "Slovenian");
            _LanguagesDictionary.Add("sog", "Sogdian");
            _LanguagesDictionary.Add("som", "Somali");
            _LanguagesDictionary.Add("son", "Songhai languages");
            _LanguagesDictionary.Add("snk", "Soninke");
            _LanguagesDictionary.Add("wen", "Sorbian languages");
            _LanguagesDictionary.Add("sai", "South American Indian (Other)");
            _LanguagesDictionary.Add("nbl", "South Ndebele");
            _LanguagesDictionary.Add("sot", "Southern");
            _LanguagesDictionary.Add("alt", "Southern Altai");
            _LanguagesDictionary.Add("sma", "Southern Sami");
            _LanguagesDictionary.Add("spa", "Spanish");
            _LanguagesDictionary.Add("srn", "Sranan Tongo");
            _LanguagesDictionary.Add("suk", "Sukuma");
            _LanguagesDictionary.Add("sux", "Sumerian");
            _LanguagesDictionary.Add("sun", "Sundanese");
            _LanguagesDictionary.Add("sus", "Susu");
            _LanguagesDictionary.Add("swa", "Swahili");
            _LanguagesDictionary.Add("ssw", "Swati");
            _LanguagesDictionary.Add("swe", "Swedish");
            _LanguagesDictionary.Add("gsw", "Swiss German");
            _LanguagesDictionary.Add("syr", "Syriac");
            _LanguagesDictionary.Add("tgl", "Tagalog");
            _LanguagesDictionary.Add("tah", "Tahitian");
            _LanguagesDictionary.Add("tai", "Tai (Other)");
            _LanguagesDictionary.Add("tgk", "Tajik");
            _LanguagesDictionary.Add("tmh", "Tamashek");
            _LanguagesDictionary.Add("tam", "Tamil");
            _LanguagesDictionary.Add("tat", "Tatar");
            _LanguagesDictionary.Add("tel", "Telugu");
            _LanguagesDictionary.Add("ter", "Tereno");
            _LanguagesDictionary.Add("tet", "Tetum");
            _LanguagesDictionary.Add("tha", "Thai");
            _LanguagesDictionary.Add("tib", "Tibetan");
            _LanguagesDictionary.Add("tig", "Tigre");
            _LanguagesDictionary.Add("tir", "Tigrinya");
            _LanguagesDictionary.Add("tem", "Timne");
            _LanguagesDictionary.Add("tiv", "Tiv");
            _LanguagesDictionary.Add("tli", "Tlingit");
            _LanguagesDictionary.Add("tpi", "Tok Pisin");
            _LanguagesDictionary.Add("tkl", "Tokelau");
            _LanguagesDictionary.Add("tog", "Tonga (Nyasa)");
            _LanguagesDictionary.Add("ton", "Tonga (Tonga Islands)");
            _LanguagesDictionary.Add("tsi", "Tsimshian");
            _LanguagesDictionary.Add("tso", "Tsonga");
            _LanguagesDictionary.Add("tsn", "Tswana");
            _LanguagesDictionary.Add("tum", "Tumbuka");
            _LanguagesDictionary.Add("tup", "Tupi languages");
            _LanguagesDictionary.Add("tur", "Turkish");
            _LanguagesDictionary.Add("ota", "Turkish, Ottoman (1500-1928)");
            _LanguagesDictionary.Add("tuk", "Turkmen");
            _LanguagesDictionary.Add("tvl", "Tuvalu");
            _LanguagesDictionary.Add("tyv", "Tuvinian");
            _LanguagesDictionary.Add("twi", "Twi");
            _LanguagesDictionary.Add("udm", "Udmurt");
            _LanguagesDictionary.Add("uga", "Ugaritic");
            _LanguagesDictionary.Add("ukr", "Ukrainian");
            _LanguagesDictionary.Add("umb", "Umbundu");
            _LanguagesDictionary.Add("und", "Undetermined");
            _LanguagesDictionary.Add("hsb", "Upper Sorbian");
            _LanguagesDictionary.Add("urd", "Urdu");
            _LanguagesDictionary.Add("uig", "Uyghur");
            _LanguagesDictionary.Add("uzb", "Uzbek");
            _LanguagesDictionary.Add("vai", "Vai");
            _LanguagesDictionary.Add("cat", "Valencian");
            _LanguagesDictionary.Add("ven", "Venda");
            _LanguagesDictionary.Add("vie", "Vietnamese");
            _LanguagesDictionary.Add("vol", "Volapuk");
            _LanguagesDictionary.Add("vot", "Votic");
            _LanguagesDictionary.Add("wak", "Wakashan languages");
            _LanguagesDictionary.Add("wal", "Walamo");
            _LanguagesDictionary.Add("wln", "Walloon");
            _LanguagesDictionary.Add("war", "Waray");
            _LanguagesDictionary.Add("was", "Washo");
            _LanguagesDictionary.Add("wel", "Welsh");
            _LanguagesDictionary.Add("fry", "Western Frisian");
            _LanguagesDictionary.Add("wol", "Wolof");
            _LanguagesDictionary.Add("xho", "Xhosa");
            _LanguagesDictionary.Add("sah", "Yakut");
            _LanguagesDictionary.Add("yao", "Yao");
            _LanguagesDictionary.Add("yap", "Yapese");
            _LanguagesDictionary.Add("yid", "Yiddish");
            _LanguagesDictionary.Add("yor", "Yoruba");
            _LanguagesDictionary.Add("ypk", "Yupik languages");
            _LanguagesDictionary.Add("znd", "Zande languages");
            _LanguagesDictionary.Add("zap", "Zapotec");
            _LanguagesDictionary.Add("zza", "Zazaki");
            _LanguagesDictionary.Add("zen", "Zenaga");
            _LanguagesDictionary.Add("zha", "Zhuang; Chuang");
            _LanguagesDictionary.Add("zul", "Zulu");
            _LanguagesDictionary.Add("zun", "Zuni");
        }

        /// <summary>
        /// Gets list of Languages according to Format
        /// </summary>
        /// <param name="Format">Use LName and LID to make your format</param>
        /// <returns>string array contain all languages</returns>
        public static string[] LanguagesArray(string Format)
        {
            ArrayList StringArray = new ArrayList();
            foreach (string Key in LanguagesDictionary.Keys)
                StringArray.Add(Format.Replace("LID", Key)
                    .Replace("LName", LanguagesDictionary[Key]));
            return (string[])StringArray.ToArray(typeof(string));
        }

        /// <summary>
        /// Gets languages list with format of "Name [ID]"
        /// </summary>
        public static string[] LanguagesList
        {
            get
            { return LanguagesArray("LName [LID]"); }
        }

        #endregion

        /// <summary>
        /// Indicate if current language is ezual to specific one
        /// </summary>
        /// <param name="obj">Object to check equality</param>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            return (((Language)obj)._LanguageID != _LanguageID);
        }

        /// <summary>
        /// Get hashcode for current language
        /// </summary>
        /// <returns>int contains hash</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Convert current language to string
        /// </summary>
        /// <returns>system.String contains current language</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Converto current language to string according to specific format
        /// </summary>
        /// <param name="Format">format to convert language to string. LName, LID</param>
        /// <returns>System.String contains current language</returns>
        public string ToString(string Format)
        {
            return Format.Replace("LName", Name).Replace("LID", LanguageID);
        }
    }
}
