using UnityEngine;
using System.Collections;

public class LoadingCircle : MonoBehaviour
{
	private bool _bIsEnabled = true;
	private Vector2 _v2CursorCounterTextureOffset;
	private float _fCursorPointerTime = 0.0F;
	private float _fTimeToSelect;
	private bool _bIsSelected;

    private Texture _textureNormal;
    private Texture _textureHover;

	public bool IsEnabled()
	{
		return _bIsEnabled;
	}
	public void Enable( bool a_bValue )
	{
		_bIsEnabled = a_bValue;
	}

	public void IncreaseTimer()
	{
		if (_bIsEnabled) 
		{
			_fCursorPointerTime = Mathf.Clamp (_fCursorPointerTime + Time.deltaTime, 0, _fTimeToSelect);
		}
	}

    public void HoverCursor()
    {
        GameObject.Find("crosshair").GetComponent<Renderer>().material.mainTexture = _textureHover;
    }


	public void ResetCursor()
	{
		_bIsSelected = false;
		_fCursorPointerTime = 0.0F;
        GameObject.Find("crosshair").GetComponent<Renderer>().material.mainTexture = _textureNormal;
	}

	void Start()
	{
		_bIsSelected = false;
		_fTimeToSelect = 2.0f;
		_v2CursorCounterTextureOffset = GetComponent<Renderer>().material.mainTextureOffset;
		_textureHover = Resources.Load<Texture>("Textures/pointer_hover");
		_textureNormal = Resources.Load<Texture>("Textures/pointer_normal");
	}

	void Update( )
	{
		_v2CursorCounterTextureOffset.x = (-1 / _fTimeToSelect) * _fCursorPointerTime;
		GetComponent<Renderer>().material.mainTextureOffset = _v2CursorCounterTextureOffset;

		if (_fCursorPointerTime >= _fTimeToSelect) 
		{
			_bIsSelected = true;
		}

        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Video360")
        {
            GameObject.Find("crosshair").GetComponent<Renderer>().enabled = false;
        }
        else
        {
            GameObject.Find("crosshair").GetComponent<Renderer>().enabled = true;
        }
	}

	IEnumerator Activate()
	{
		_bIsSelected = true;
		yield return null;
		ResetCursor ();
	}
		
	public bool IsSelected()
	{
		return _bIsSelected;
	}
}

