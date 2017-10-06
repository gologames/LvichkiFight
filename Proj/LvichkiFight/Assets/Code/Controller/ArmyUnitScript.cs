using UnityEngine;
using System.Collections;

public class ArmyUnitScript
{
    private int unic_id;

	private float morale;

	private float infantry;
    private float cavalry;
    private float artillery;

	private int countryID; //-1 - повстанцы
	private int currAreaID;

	private bool isMoving;
	private int destAreaID;

    private bool isDeath;
    private bool isMoraleChange;
    private bool isMoraleUp;

    #region CTOR
    public ArmyUnitScript(float infantry, float cavalry, float artillery, int countryID, int currAreaID)
	{
        this.unic_id = ArmyInfoScript.GetUnicID();
		this.morale = ArmyInfoScript.Morale_Default;
		this.infantry = infantry;
		this.cavalry = cavalry;
		this.artillery = artillery;
		this.countryID = countryID;
		this.currAreaID = currAreaID;
		this.isMoving = false;
		this.destAreaID = -1;

        this.isDeath = false;
        this.isMoraleChange = false;
        this.isMoraleUp = false;
	}

    public ArmyUnitScript(float morale, float infantry, float cavalry, float artillery, int countryID, int currAreaID, bool isMoving, int destAreaID)
    {
        this.unic_id = ArmyInfoScript.GetUnicID();
        this.morale = morale;
        this.infantry = infantry;
        this.cavalry = cavalry;
        this.artillery = artillery;
        this.countryID = countryID;
        this.currAreaID = currAreaID;
        this.isMoving = isMoving;
        this.destAreaID = destAreaID;

        this.isDeath = false;
        this.isMoraleChange = false;
        this.isMoraleUp = false;
    }
    #endregion

    #region UNIC_ID
    public int GetUnicID()
    {
        return unic_id;
    }
    #endregion

    #region COUNTRY_ID
    public int GetCountryID()
    {
        return countryID;
    }
    #endregion

    #region DEST_AREA
    public int GetDestAreaID()
	{
		return destAreaID;
	}
	public void SendArmy(int areaID)
	{
        if (this.currAreaID != areaID && !this.isMoving)
        {
            this.destAreaID = areaID;
            this.isMoving = true;
        }
        else
        {
            Debug.Log("error in SetDestArea in ArmyUnitScript");
        }
	}
    #endregion

    #region MOVING
    public bool GetIsMoving()
	{
		return isMoving;
	}
	public void StopMoving()
	{
        if (this.isMoving)
        {
            this.isMoving = false;
            this.destAreaID = -1;
        }
        else
        {
            Debug.Log("error in StopMoving in ArmyUnitScript");
        }
	}
    #endregion

    #region CURR_AREA
    public int GetCurrAreaID()
	{
		return currAreaID;
	}
	public void _MoveArmy(int nextAreaID)
	{
        if (this.currAreaID != nextAreaID)
        {
            this.currAreaID = nextAreaID;
        }
        else
        {
            Debug.Log("error in _MoveArmy in ArmyUnitScript");
        }
	}
    #endregion

    #region MORALE
    public float GetMorale()
	{
		return morale;
	}
	public void AddMorale(float gain)
	{
		if (gain >= 0 && this.morale + gain <= ArmyInfoScript.Morale_Max)
		{
			this.morale += gain;
		}
		else
		{
			Debug.Log("error in AddMorale in ArmyUnitScript");
		}
	}
    public void SubMorale(float loss)
	{
		if (loss >= 0 && this.morale - loss >= ArmyInfoScript.Morale_Min)
		{
			this.morale -= loss;
		}
		else
		{
			Debug.Log("error in SubMorale in ArmyUnitScript");
		}
	}
	public void _SetMorale(float morale)
	{
		this.morale = morale;
	}
    #endregion

    #region INFANTRY
    public float GetInfantry()
	{
		return infantry;
	}
    public void AddInfantry(float gain)
	{
		if (gain >= 0.0f)
		{
			this.infantry += gain;
		}
		else
		{
			Debug.Log("error in AddInfantry in ArmyUnitScript");
		}
	}
    public void SubInfantry(float loss)
	{
		if (loss >= 0.0f && this.infantry - loss >= 0.0f)
		{
			this.infantry -= loss;
		}
		else
		{
			Debug.Log("error in SubInfantry in ArmyUnitScript");
		}
	}
	public void MulInfantry(float mul)
	{
		if (mul >= 0.0f)
		{
			this.infantry *= mul;
		}
		else
		{
			Debug.Log("error in MulInfantry in ArmyUnitScript");
		}
	}
    #endregion

    #region CAVALRY
    public float GetCavalry()
	{
		return cavalry;
	}
    public void AddCavalry(float gain)
	{
		if (gain >= 0.0f)
		{
			this.cavalry += gain;
		}
		else
		{
			Debug.Log("error in AddCavalry in ArmyUnitScript");
		}
	}
    public void SubCavalry(float loss)
	{
		if (loss >= 0 && this.cavalry - loss >= 0.0f)
		{
			this.cavalry -= loss;
		}
		else
		{
			Debug.Log("error in SubCavalry in ArmyUnitScript");
		}
	}
	public void MulCavalry(float mul)
	{
		if (mul >= 0.0f)
		{
			this.cavalry *= mul;
		}
		else
		{
			Debug.Log("error in MulCavalry in ArmyUnitScript");
		}
	}
    #endregion

    #region ARTILLERY
    public float GetArtillery()
	{
		return artillery;
	}
    public void AddArtillery(float gain)
	{
		if (gain >= 0.0f)
		{
			this.artillery += gain;
		}
		else
		{
			Debug.Log("error in AddArtillery in ArmyUnitScript");
		}
	}
    public void SubArtillery(float loss)
	{
		if (loss >= 0.0f && this.artillery - loss >= 0.0f)
		{
			this.artillery -= loss;
		}
		else
		{
			Debug.Log("error in SubArtillery in ArmyUnitScript");
		}
	}
    public void MulArtillery(float mul)
	{
		if (mul >= 0.0f)
		{
			this.artillery *= mul;
		}
		else
		{
			Debug.Log("error in MulArtillery in ArmyUnitScript");
		}
    }
    #endregion

    #region IS_DEATH
    public bool GetIsDeath()
    {
        return isDeath;
    }
    public void SetIsDeath(bool isDeath)
    {
        this.isDeath = isDeath;
    }
    #endregion

    #region MORALE_CHANGE
    public bool GetIsMoraleChange()
    {
        return isMoraleChange;
    }
    public void SetIsMoraleChange(bool isMoraleChange)
    {
        this.isMoraleChange = isMoraleChange;
    }

    public bool GetIsMoraleUp()
    {
        if (isMoraleChange)
        {
            return isMoraleUp;
        }
        else
        {
            Debug.Log("error in GetIsMoraleUp in ArmyUnitScript");
            return false;
        }
    }
    public void SetIsMoraleUp(bool isMoraleUp)
    {

        if (isMoraleChange)
        {
            this.isMoraleUp = isMoraleUp;
        }
        else
        {
            Debug.Log("error in SetIsMoraleUp in ArmyUnitScript");
        }
    }
    #endregion
}
