using System;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Library
{
	/// <summary>
	/// Summary description for Security.
	/// </summary>
	public class Security
	{
		private static string strAlphabeth;
		private static  string strViGenerateTable;
		private static  int intAlphabethLen;
		private static string codeKey;

		public Security()
		{
		}

		public static string CodeKey
		{
			get
			{
				return codeKey ;
			}
			set
			{   
				if (value == "") 
					throw new Exception("CodeKey Value Should not be null");					 

				string key = value.Trim();

				if ((key != null) && (key.Length > 1))
				{
					key = key.Substring(1,1);      
				}
				codeKey = key;
			}
		}   

		private static int GetIndexPosition(string strVal)
		{
			int intIndex = -1;
			for (int i=0 ; i < intAlphabethLen ;i++)
			{
				if (strVal == strAlphabeth.Substring(i,1))
				{
					intIndex = i;
					break;
				}  
			}
			if ((intIndex >= 0) && (intIndex < intAlphabethLen))
				return intIndex;
			else
				return -1; 
		}
		
		public static string EncryptStringXOR(string strPass)
		{
			int intIndex,intChar1,intChar2; 
			intIndex = strPass.Trim().Length;

			strAlphabeth = "HuIdtcrYl901p28zUVWXgTZPfaF56CQSbBoOx34JvLMweNADs7";
			intAlphabethLen = strAlphabeth.Length ;
			strViGenerateTable = strAlphabeth + strAlphabeth;
			
			codeKey = "V";    //  to be changed to "V" to be a senate compliant ...

			string workString = "";
			string encodeKey = codeKey;
            
			intChar1 = 0;
			intChar2 = 0;
			
			for (int i=0;i<intIndex;i++)
			{
				intChar1 = Convert.ToInt32(GetIndexPosition(encodeKey));
				intChar2 = Convert.ToInt32(GetIndexPosition(strPass.Trim().Substring(i,1)));
                  
				if (intChar2 != -1)
				{
					workString += strViGenerateTable.Substring(intChar1+intChar2+1,1);
					encodeKey = strViGenerateTable.Substring(intChar1+intChar2+1,1);
				}
				else
				{
					workString += strPass.Substring(i,1);
				}	
			}
			return workString;
		}

		public static string DecryptStringXOR(string strPass)
		{
			int intIndex,intChar1,intChar2;
			intIndex = strPass.Length;

			strAlphabeth = "HuIdtcrYl901p28zUVWXgTZPfaF56CQSbBoOx34JvLMweNADs7";
			intAlphabethLen = strAlphabeth.Length ;
			strViGenerateTable = strAlphabeth + strAlphabeth;
			
			codeKey = "V";    //  to be changed to "V" to be a senate compliant ...

			string decodeKey = codeKey;
			string workString = "";
			intChar1 = 0;
			intChar2 = 0;
			
			for (int i =0 ; i < intIndex; i++)
			{
				intChar1 = Convert.ToInt32(GetIndexPosition(decodeKey));
				intChar2 = Convert.ToInt32(GetIndexPosition(strPass.Substring(i,1)));
				
				if (intChar2 != -1)
				{
					workString += strViGenerateTable.Substring(intChar2-intChar1+intAlphabethLen-1,1);
					decodeKey = strPass.Substring(i,1);
				}
				else
				{
					workString += strPass.Substring(i,1);
				}
			}
			return workString;
		}

		/// <summary>
		///	Hashes a clean string using MD5 algorithm
		/// </summary>
		/// <param name="pstrCleanString">String for hashing</param>
		/// <returns>string</returns>
		public static string HashMD5(string pstrCleanString)
		{				
			string strPlain = String.Empty;
			byte[] yarrHashed = {}; 
			byte[] yarrHashResult = new byte[32]; 

			MD5CryptoServiceProvider MD5=new MD5CryptoServiceProvider();
			for(int i = 0; i<pstrCleanString.Length; i++) 
			{
				strPlain += pstrCleanString[i]; 
			}
			byte[] bMD5Hash = Encoding.ASCII.GetBytes(strPlain); 			
			yarrHashResult = MD5.ComputeHash(bMD5Hash); 
			yarrHashed = yarrHashResult; 
			return Convert.ToBase64String(yarrHashed); 			
		}

		
		/// <summary>
		/// Encrypts and Decrypts a string using TripleDESCryptoServiceProvider 
		/// </summary>
		/// <param name="pblnFlag">True - Encrypt / False - Decrypt</param>
		/// <param name="pstrOriginal">String for Encryption/Decryption</param>
		/// <returns>string</returns>
		public static string CryptTripleDES(bool pblnFlag, string pstrOriginal)
		{
			//if the string to be encrypted/decrypted is empty then an empty string will be returned
			if(pstrOriginal.Trim().Length == 0)
			{
				return "";
			}

			string strEncrypted = null;
			string strDecrypted = null; 
			string strSystem = null;
			string strReturn = null;
			TripleDESCryptoServiceProvider tripledes;
			MD5CryptoServiceProvider hashmd5;
			byte[] yarrPwdHash;
			byte[] yarrBuff;

			//create a secret password. the password is used to encrypt
			//and decrypt strings. Without the password, the encrypted
			//string cannot be decrypted and is just garbage. You must
			//use the same password to decrypt an encrypted string as the
			//string was originally encrypted with.
			strSystem = "Ac45@#3d$B%Lf!3R";

			//generate an MD5 hash from the password. 
			//a hash is a one way encryption meaning once you generate
			//the hash, you cant derive the password back from it.
			hashmd5 = new MD5CryptoServiceProvider();
			yarrPwdHash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strSystem));
			hashmd5 = null;

			//implement DES3 encryption
			tripledes = new TripleDESCryptoServiceProvider();

			//the key is the secret password hash.
			tripledes.Key = yarrPwdHash;

			//the mode is the block cipher mode which is basically the
			//details of how the encryption will work. There are several
			//kinds of ciphers available in DES3 and they all have benefits
			//and drawbacks. Here the Electronic Codebook cipher is used
			//which means that a given bit of text is always encrypted
			//exactly the same when the same password is used.
			tripledes.Mode = CipherMode.ECB; //CBC, CFB
			try 
			{
				if (pblnFlag.Equals(true)) 
				{
					//----- encrypt an un-encrypted string ------------
					//the original string, which needs encrypted, must be in byte
					//array form to work with the des3 class. everything will because
					//most encryption works at the byte level so you'll find that
					//the class takes in byte arrays and returns byte arrays and
					//you'll be converting those arrays to strings.
					yarrBuff = ASCIIEncoding.ASCII.GetBytes(pstrOriginal);

					//encrypt the byte buffer representation of the original string
					//and base64 encode the encrypted string. the reason the encrypted
					//bytes are being base64 encoded as a string is the encryption will
					//have created some weird characters in there. Base64 encoding
					//provides a platform independent view of the encrypted string 
					//and can be sent as a plain text string to wherever.
					strEncrypted = Convert.ToBase64String(tripledes.CreateEncryptor().TransformFinalBlock(yarrBuff, 0, yarrBuff.Length));
					strReturn = strEncrypted;
				}
				else 
				{
					//----- decrypt an encrypted string ------------
					//whenever you decrypt a string, you must do everything you
					//did to encrypt the string, but in reverse order. To encrypt,
					//first a normal string was des3 encrypted into a byte array
					//and then base64 encoded for reliable transmission. So, to 
					//decrypt this string, first the base64 encoded string must be 
					//decoded so that just the encrypted byte array remains.
					yarrBuff = Convert.FromBase64String(pstrOriginal);

					//decrypt DES 3 encrypted byte buffer and return ASCII string
					strDecrypted = ASCIIEncoding.ASCII.GetString(tripledes.CreateDecryptor().TransformFinalBlock(yarrBuff, 0, yarrBuff.Length));
					strReturn = strDecrypted;
				}
			}
			catch (Exception) 
			{
				//throw new Exception("Invalid Data ! Encrypt/Decrypt Failed !");
			}
			finally 
			{
				//cleanup
				tripledes = null;
			}
			return (strReturn);
		}

		public static bool CheckIP(string strRefererUrl, string strApplicationDomainName)
		{
			try
			{
				if(strRefererUrl == null || strApplicationDomainName == null)
				{
					return false;
				}

				//Take Application IPAddress from Web.config and decrypt it
				string strApplicationIP = CryptTripleDES(false,ConfigurationSettings.AppSettings["ApplicationIPADDRESS"]);
				
				bool blnResult = false;

				//Checking if Referer Url's domain name = Application's Domain name
				string strDomainName = GetDomainName(strRefererUrl.ToLower()).ToLower();			
				if(strDomainName == strApplicationDomainName.ToLower())
				{
					//Get the IP Address for the domain
					string strTest="";
					IPHostEntry IPHost = Dns.Resolve(strDomainName);
					IPAddress[] addresses = IPHost.AddressList;
					for(int i=0; i<addresses.Length; i++)
					{
						string strIPaddress = Convert.ToString(addresses[i],CultureInfo.InvariantCulture).Trim();
						strTest+=";"+strIPaddress;
						if(strApplicationIP == strIPaddress)
						{
							blnResult = true;
							return true;
							break;
						}
					}

					throw new Exception("Referer IP: "+strTest+" \n Ref Domain: "+strDomainName+" \n Application Domain Name: "+strApplicationDomainName+" \n Application IP: "+strApplicationIP);
				}

				
				return blnResult;
			}
			catch
			{
				//return false;
			
				return false;
			}
		}

		public static string GetDomainName(string strUrl)
		{
			Regex re = new Regex(".?://(www.)?([^/:]+)",RegexOptions.IgnoreCase);
			MatchCollection match;
			match = re.Matches(strUrl);
			string strDomain = Convert.ToString(match[0].Groups[2],CultureInfo.InvariantCulture).Trim();
			return strDomain;
		}


		
		
		public static string CryptTripleDES(bool pblnFlag, string pstrOriginal, string strSystem)
		{
			//if the string to be encrypted/decrypted is empty then an empty string will be returned
			if(pstrOriginal.Trim().Length == 0)
			{
				return "";
			}

			string strEncrypted = null;
			string strDecrypted = null; 
			//string strSystem = null;
			string strReturn = null;
			TripleDESCryptoServiceProvider tripledes;
			MD5CryptoServiceProvider hashmd5;
			byte[] yarrPwdHash;
			byte[] yarrBuff;

			//create a secret password. the password is used to encrypt
			//and decrypt strings. Without the password, the encrypted
			//string cannot be decrypted and is just garbage. You must
			//use the same password to decrypt an encrypted string as the
			//string was originally encrypted with.
			//strSystem = "IS123";            

			//generate an MD5 hash from the password. 
			//a hash is a one way encryption meaning once you generate
			//the hash, you cant derive the password back from it.
			hashmd5 = new MD5CryptoServiceProvider();
			yarrPwdHash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strSystem));
			hashmd5 = null;

			//implement DES3 encryption
			tripledes = new TripleDESCryptoServiceProvider();

			//the key is the secret password hash.
			tripledes.Key = yarrPwdHash;

			//the mode is the block cipher mode which is basically the
			//details of how the encryption will work. There are several
			//kinds of ciphers available in DES3 and they all have benefits
			//and drawbacks. Here the Electronic Codebook cipher is used
			//which means that a given bit of text is always encrypted
			//exactly the same when the same password is used.
			tripledes.Mode = CipherMode.ECB; //CBC, CFB
			try 
			{
				if (pblnFlag.Equals(true)) 
				{
					//----- encrypt an un-encrypted string ------------
					//the original string, which needs encrypted, must be in byte
					//array form to work with the des3 class. everything will because
					//most encryption works at the byte level so you'll find that
					//the class takes in byte arrays and returns byte arrays and
					//you'll be converting those arrays to strings.
					yarrBuff = ASCIIEncoding.ASCII.GetBytes(pstrOriginal);

					//encrypt the byte buffer representation of the original string
					//and base64 encode the encrypted string. the reason the encrypted
					//bytes are being base64 encoded as a string is the encryption will
					//have created some weird characters in there. Base64 encoding
					//provides a platform independent view of the encrypted string 
					//and can be sent as a plain text string to wherever.
					strEncrypted = Convert.ToBase64String(tripledes.CreateEncryptor().TransformFinalBlock(yarrBuff, 0, yarrBuff.Length));
					strReturn = strEncrypted;
				}
				else 
				{
					//----- decrypt an encrypted string ------------
					//whenever you decrypt a string, you must do everything you
					//did to encrypt the string, but in reverse order. To encrypt,
					//first a normal string was des3 encrypted into a byte array
					//and then base64 encoded for reliable transmission. So, to 
					//decrypt this string, first the base64 encoded string must be 
					//decoded so that just the encrypted byte array remains.
					yarrBuff = Convert.FromBase64String(pstrOriginal);

					//decrypt DES 3 encrypted byte buffer and return ASCII string
					strDecrypted = ASCIIEncoding.ASCII.GetString(tripledes.CreateDecryptor().TransformFinalBlock(yarrBuff, 0, yarrBuff.Length));
					strReturn = strDecrypted;
				}
			}
			catch (Exception) 
			{
				//throw new Exception("Invalid Data ! Encrypt/Decrypt Failed !");
			}
			finally 
			{
				//cleanup
				tripledes = null;
			}
			return (strReturn);
		}
	}
}
