using UnityEngine;
using System.Collections;

public class ArmyUnitScript
{
	private float infantry;
    private float cavalry;
    private float artillery;

    #region CTOR
    public ArmyUnitScript(float infantry, float cavalry, float artillery)
	{
		this.infantry = infantry;
		this.cavalry = cavalry;
		this.artillery = artillery;
	}
    #endregion

    #region INFANTRY
    public float GetInfantry()
	{
		return infantry;
	}
    #endregion

    #region CAVALRY
    public float GetCavalry()
	{
		return cavalry;
	}
    #endregion

    #region ARTILLERY
    public float GetArtillery()
	{
		return artillery;
	}
    #endregion
}
