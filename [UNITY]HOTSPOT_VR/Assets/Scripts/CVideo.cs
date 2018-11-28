using UnityEngine;
using System.Collections;

public class CVideo {

    private int _iVideoID;
    private string _sVideoName;
    private string _sPath;

	// Constructor
	public CVideo(int a_iID, string a_strName, string a_sPath)
	{
        _iVideoID = a_iID;
        _sVideoName = a_strName;
        _sPath = a_sPath;
	}

    public CVideo() { }

	///-----------------------------------------------------------------------------------
	///  GETTERS AND SETTERS															ID
	///-----------------------------------------------------------------------------------
	public void SetID(int a_iID)
	{
		this._iVideoID = a_iID;
	}
		
	public int GetID()
	{
		return _iVideoID;
	}

    public void SetName(string a_strName)
    {
        this._sVideoName = a_strName;
    }

    public string GetName()
    {
        return _sVideoName;
    }

    public void SetPath(string a_sPath)
    {
        this._sPath = a_sPath;
    }

    public string GetPath()
    {
        return _sPath;
    }
}
