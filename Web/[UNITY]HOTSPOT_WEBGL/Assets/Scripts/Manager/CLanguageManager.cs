using UnityEngine;
using System.Collections;
using System.Xml;
#if UNITY_WP8
using System.Xml.Linq;
#endif

public class CLanguageManager : MonoBehaviour 
{
	private int _iNumberOfLanguages = 2;

	public int GetNumberOfLanguages ()
	{
		return _iNumberOfLanguages;
	}

	private TextAsset textFile;
	#if UNITY_WP8
	private XDocument theXMLDocument = new XDocument();
	#else
	private XmlDocument theXMLDocument = new XmlDocument();
	#endif

	private int _iLanguage = 0;
	
	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------

	public string GetLanguage( )
	{
		return PlayerPrefs.GetString( "language" );
	}

	public int GetLanguageIndex( )
	{
		return _iLanguage;
	}

	public string GetLanguageCode( )
	{
		return PlayerPrefs.GetString ("language").ToUpper ();
	}

	public string GetLanguageCode( int a_iValue )
	{
		switch( a_iValue )
		{
		case 1:
			return "fr";
		default :
			return "en";
		}
	}

	public void SetLanguageIndex( int a_iValue )
	{
		_iLanguage = a_iValue;
		
		switch( _iLanguage )
		{
		case 1 :
			PlayerPrefs.SetString("language","fr");
			break;
		default :
			PlayerPrefs.SetString("language","en");
			break;
		}
		
		textFile = Resources.Load("Texts/content_"+PlayerPrefs.GetString("language"),typeof(TextAsset)) as TextAsset;
		
		if( textFile == null )
		{
			textFile = Resources.Load("Texts/content_en",typeof(TextAsset)) as TextAsset;
			_iLanguage = 0;
		}
		
		#if UNITY_WP8
		theXMLDocument = XDocument.Parse( textFile.text);
		#else
		theXMLDocument.LoadXml(textFile.text);
		#endif
	}

	///-----------------------------------------------------------------------------------
	/// <summary> Allows to define the app language </summary>
	/// <param name="a_strLanguage"> language to set </param>
	///-----------------------------------------------------------------------------------
	public void SetLanguage( string a_strLanguage )
	{
		switch( a_strLanguage )
		{
			case "French" :
				PlayerPrefs.SetString("language","fr");
				_iLanguage = 1;
				break;
			default :
				PlayerPrefs.SetString("language","en");
				_iLanguage = 0;
				break;
		}

		textFile = Resources.Load("Texts/content_"+PlayerPrefs.GetString("language"),typeof(TextAsset)) as TextAsset;

		if( textFile == null )
		{
			textFile = Resources.Load("Texts/content_en",typeof(TextAsset)) as TextAsset;
			_iLanguage = 0;
		}
		#if UNITY_WP8
		theXMLDocument = XDocument.Parse( textFile.text);
		#else
		theXMLDocument.LoadXml(textFile.text);
		#endif
	}

	///-----------------------------------------------------------------------------------
	/// <summary> Recovers a label </summary>
	/// <param name="a_strText"> the XML label </param>
	/// <returns> the string found </returns>
	///-----------------------------------------------------------------------------------
	public string GetText(string a_strText)
	{

		#if UNITY_WP8
		XElement content;
		try
		{
			content = theXMLDocument.Root.Element ( a_strText );
		}
		catch
		{
			return " ";
		}
		#else

		XmlNode content = theXMLDocument.SelectSingleNode("descendant::"+a_strText);
		#endif

		// return " " if the content does not exist, for debugging
		if (content == null )
		{
			return " ";
		}
		else
		{
			#if UNITY_WP8
			return content.Value;
			#else
			return content.InnerText;
			#endif
		}
	}
}