using UnityEngine;
using System;
using System.Collections.Generic;

public class RealBattleScript
{
    #region DATA_TYPES
    public enum RealBattleTroops
    {
        Inf = 0, CavLeft = 1,
        CavRight = 2, Art = 3
        //,Ball = 4
    }
    public enum UnitStates
    {
        Stay = 0, MoveAll = 1,
        Fight = 2, MoveOne = 3
    }
    public class BattleUnit
    {
        public readonly bool IsOne;
        public readonly RealBattleTroops Type;
        public float Count;
        public float X; //center
        public float Y; //center
        public float Delta;
        public UnitStates State;
        public float Angle;

        public BattleUnit(bool isOne, RealBattleTroops type, float count, float angle)
        {
            this.IsOne = isOne;
            this.Type = type;
            this.Count = count;
            this.X = -1;
            this.Y = -1;
            this.Delta = 0;
            this.State = UnitStates.Stay;
            this.Angle = angle;
        }
    }
    public class BattleGroup
    {
        public readonly bool IsOne;
        public readonly RealBattleTroops Type;
        public Vector2 MarchCenter;
        public RealBattleTroops EnemyType;
        private List<BattleUnit> units = new List<BattleUnit>();
        private int countFight;
        private bool firstBlood;

        #region CTOR
        public BattleGroup(bool isOne, RealBattleTroops type)
        {
            this.IsOne = isOne;
            this.Type = type;
            this.countFight = 0;
            this.firstBlood = false;
        }
        #endregion

        #region UNITS
        public void AddUnit(float count, float angle)
        {
            units.Add(new BattleUnit(IsOne, Type, count, angle));
        }
        public int GetUnitsCount()
        {
            return units.Count;
        }
        public BattleUnit GetUnit(int index)
        {
            if (index < 0 || index >= units.Count)
            {
                Debug.Log("error in GetUnit in BaseGroup in RealBattleScript");
                return null;
            }

            return units[index];
        }
        #endregion

        #region COUNT_REAL
        public float GetCountReal()
        {
            float sum = 0;
            for (int i = 0; i < units.Count; i++)
            {
                sum += units[i].Count;
            }
            return sum;
        }
        #endregion

        #region COUNT_FIGHT
        public int GetCountFight()
        {
            return countFight;
        }
        public void IncCountFight()
        {
            if (countFight + 1 > units.Count)
            {
                Debug.Log("error in IncCountFight in InfGroup in RealBattleScript");
            }
            else
            {
                countFight++;
            }
        }
        public void DecCountFight()
        {
            if (countFight - 1 < 0)
            {
                Debug.Log("error in DecCountFight in InfGroup in RealBattleScript");
            }
            else
            {
                countFight--;
            }
        }
        #endregion

        #region FIRST_BLOOD
        public bool GetIsFirstBlood()
        {
            return firstBlood;
        }
        public void SetFirstBloodTrue()
        {
            firstBlood = true;
        }
        #endregion

        #region REMOVE_EMPTY
        public void RemoveEmpty()
        {
            for (int i = 0; i < units.Count; )
            {
                if (units[i].Count > 0)
                {
                    i++;
                }
                else
                {
                    if (units[i].State == UnitStates.Fight)
                    {
                        DecCountFight();
                    }
                    units.RemoveAt(i);
                }
            }
        }
        #endregion
    }
    public class BallUnit
    {
        public Vector2 Pos;
        public Vector2 Dest;
        public readonly float Damage;
        private bool isHitted;

        public BallUnit(Vector2 pos, Vector2 dest, float damage)
        {
            this.Pos = pos;
            this.Dest = dest;
            this.Damage = damage;
            this.isHitted = false;
        }

        public void Move(float dist)
        {
            Pos = Vector2.MoveTowards(Pos, Dest, dist);
        }

        #region HIT
        public bool GetIsHitted()
        {
            return isHitted;
        }
        public void SetHittedTrue()
        {
            if (!isHitted)
            {
                isHitted = true;
            }
            else
            {
                Debug.Log("error in SetHittedTrue in BallUnit in RealBattleScript");
            }
        }
        #endregion
    }
    public class BallGroup
    {
        public List<BallUnit> Units;
        public BallGroup(int capacity = 0)
        {
            this.Units = new List<BallUnit>(capacity);
        }
    }
    public class CellGrid
    {
        private struct CellCoor
        {
            public int Col;
            public int Row;
        }

        private int cellsCount = 0;
        private CellCoor[] cells = new CellCoor[RealBattleInfoScript.CellGrid_CountW * RealBattleInfoScript.CellGrid_CountH];
        private List<BattleUnit>[,] oneGrid = new List<BattleUnit>[RealBattleInfoScript.CellGrid_CountW, RealBattleInfoScript.CellGrid_CountH];
        private List<BattleUnit>[,] twoGrid = new List<BattleUnit>[RealBattleInfoScript.CellGrid_CountW, RealBattleInfoScript.CellGrid_CountH];

        public CellGrid()
        {
            for (int i = 0; i < RealBattleInfoScript.CellGrid_CountW; i++)
            {
                for (int j = 0; j < RealBattleInfoScript.CellGrid_CountH; j++)
                {
                    oneGrid[i, j] = new List<BattleUnit>();
                    twoGrid[i, j] = new List<BattleUnit>();
                }
            }
        }

        #region CLEAR
        public void Clear()
        {
            for (int i = 0; i < RealBattleInfoScript.CellGrid_CountW; i++)
            {
                for (int j = 0; j < RealBattleInfoScript.CellGrid_CountH; j++)
                {
                    oneGrid[i, j].Clear();
                    twoGrid[i, j].Clear();
                }
            }
        }
        #endregion

        #region SET_CELLS
        private void _AddCell(int column, int row)
        {
            if (column >= 0 && column < RealBattleInfoScript.CellGrid_CountW &&
                row >= 0 && row < RealBattleInfoScript.CellGrid_CountH)
            {
                cells[cellsCount].Col = column;
                cells[cellsCount].Row = row;
                cellsCount++;
            }
        }
        private void _SetCells(BattleUnit unit, bool isArtFar)
        {
            if (!isArtFar || unit.Type != RealBattleTroops.Art)
            {
                #region COLUMNS_AND_ROWS
                float halfSide = RealBattleInfoScript.GetUnitSide(unit.Type) / 2.0f;

                int colLeftTop = (int)((unit.X - RealBattleInfoScript.FieldLeft - halfSide) / RealBattleInfoScript.CellGrid_Side);
                int rowLeftTop = (int)((unit.Y - RealBattleInfoScript.FieldTop - halfSide) / RealBattleInfoScript.CellGrid_Side);

                int colRightTop = (int)((unit.X - RealBattleInfoScript.FieldLeft + halfSide) / RealBattleInfoScript.CellGrid_Side);
                int rowRightTop = (int)((unit.Y - RealBattleInfoScript.FieldTop - halfSide) / RealBattleInfoScript.CellGrid_Side);

                int colLeftBottom = (int)((unit.X - RealBattleInfoScript.FieldLeft - halfSide) / RealBattleInfoScript.CellGrid_Side);
                int rowLeftBottom = (int)((unit.Y - RealBattleInfoScript.FieldTop + halfSide) / RealBattleInfoScript.CellGrid_Side);

                int colRightBottom = (int)((unit.X - RealBattleInfoScript.FieldLeft + halfSide) / RealBattleInfoScript.CellGrid_Side);
                int rowRightBottom = (int)((unit.Y - RealBattleInfoScript.FieldTop + halfSide) / RealBattleInfoScript.CellGrid_Side);
                #endregion

                #region ADD_TO_ARRAY
                cellsCount = 0;
                cells[cellsCount].Col = colLeftTop;
                cells[cellsCount].Row = rowLeftTop;
                cellsCount++;

                if (colRightTop != colLeftTop)
                {
                    cells[cellsCount].Col = colRightTop;
                    cells[cellsCount].Row = rowRightTop;
                    cellsCount++;
                }
                if (rowLeftBottom != rowLeftTop)
                {
                    cells[cellsCount].Col = colLeftBottom;
                    cells[cellsCount].Row = rowLeftBottom;
                    cellsCount++;
                }
                if (colRightBottom != colLeftTop && rowRightBottom != rowLeftTop)
                {
                    cells[cellsCount].Col = colRightBottom;
                    cells[cellsCount].Row = rowRightBottom;
                    cellsCount++;
                }
                #endregion
            }
            else
            {
                #region COLUMN_AND_ROW
                int column = (int)((unit.X - RealBattleInfoScript.FieldLeft) / RealBattleInfoScript.CellGrid_Side);
                int row = (int)((unit.Y - RealBattleInfoScript.FieldTop) / RealBattleInfoScript.CellGrid_Side);
                #endregion

                #region ADD_TO_ARRAY
                cellsCount = 0;
                int n = 6;
                //int n = 4;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i + j <= n)
                        {
                            _AddCell(column + i, row + j);
                            if (j != 0) _AddCell(column + i, row - j);
                            if (i != 0) _AddCell(column - i, row + j);
                            if (i != 0 && j != 0) _AddCell(column - i, row - j);
                        }
                    }
                }
                #endregion
            }
        }
        private void _SetCells_AllField(BattleUnit unit)
        {
            #region COLUMN_AND_ROW
            int column = (int)((unit.X - RealBattleInfoScript.FieldLeft) / RealBattleInfoScript.CellGrid_Side);
            int row = (int)((unit.Y - RealBattleInfoScript.FieldTop) / RealBattleInfoScript.CellGrid_Side);
            #endregion

            #region ADD_TO_ARRAY
            cellsCount = 0;
            _AddCell(column, row);

            int maxCol = Math.Max(column, RealBattleInfoScript.CellGrid_CountH - column);
            for (int i = 1; i < maxCol + 1; i++)
            {
                for (int j = -i; j < i; j++)
                {
                    _AddCell(column + i, row + j);
                    _AddCell(column - i, row + j + 1);
                    _AddCell(column + j + 1, row + i);
                    _AddCell(column - j - 1, row - i);
                }
            }
            #endregion
        }
        #endregion

        #region ADD_UNITS
        public void AddGroup(BattleGroup group)
        {
            for (int i = 0; i < group.GetUnitsCount(); i++)
            {
                _AddUnit(group.GetUnit(i));
            }
        }
        private void _AddUnit(BattleUnit unit)
        {
            _SetCells(unit, false);
            List<BattleUnit>[,] grid = unit.IsOne ? oneGrid : twoGrid;

            for (int i = 0; i < cellsCount; i++)
            {
                grid[cells[i].Col, cells[i].Row].Add(unit);
            }
        }
        #endregion

        #region CHECK_NEAR
        private BattleUnit _CheckNear_ProcessCell(BattleUnit unit, List<BattleUnit> cell, float meDist)
        {
            for (int i = 0; i < cell.Count; i++)
            {
                float len = Mathf.Sqrt(Mathf.Pow(cell[i].X - unit.X, 2) + Mathf.Pow(cell[i].Y - unit.Y, 2));
                float dist = meDist + RealBattleInfoScript.GetUnitSide(cell[i].Type) / 2.0f;

                if (len <= dist)
                {
                    return cell[i];
                }
            }

            return null;
        }
        public BattleUnit CheckNear(BattleUnit unit)
        {
            _SetCells(unit, true);
            float meDist = unit.Type == RealBattleTroops.Art ? RealBattleInfoScript.ArtShotDist : RealBattleInfoScript.GetUnitSide(unit.Type) / 2.0f;
            BattleUnit enemy = null;
            List<BattleUnit>[,] grid = !unit.IsOne ? oneGrid : twoGrid;

            for (int i = 0; i < cells.Length; i++)
            {
                if (enemy == null)
                {
                    CellCoor cellCoor = cells[i];
                    enemy = _CheckNear_ProcessCell(unit, grid[cellCoor.Col, cellCoor.Row], meDist);
                }
                else break;
            }

            return enemy;
        }
        #endregion

        #region FIND_NEAREST
        public BattleUnit FindNearest(BattleUnit unit)
        {
            _SetCells_AllField(unit);
            List<BattleUnit>[,] grid = !unit.IsOne ? oneGrid : twoGrid;

            for (int i = 0; i < cells.Length; i++)
            {
                List<BattleUnit> cell = grid[cells[i].Col, cells[i].Row];
                if (cell.Count > 0)
                {
                    return cell[0];
                }
            }

            return null;
        }
        #endregion

        #region CAN_MOVE
        private bool _CanMove_ProcessCell(BattleUnit me, Vector2 newMe, List<BattleUnit> cell, float meDist)
        {
            for (int i = 0; i < cell.Count; i++)
            {
                BattleUnit other = cell[i];
                if (me != other)
                {
                    float len = Mathf.Sqrt(Mathf.Pow(cell[i].X - newMe.x, 2) + Mathf.Pow(cell[i].Y - newMe.y, 2));
                    float dist = meDist + RealBattleInfoScript.GetUnitSide(cell[i].Type) / 2.0f;

                    if (len <= dist)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public bool CanMove(BattleUnit unit, Vector2 dest, float dist)
        {
            _SetCells(unit, false);
            Vector2 newMe = Vector2.MoveTowards(new Vector2(unit.X, unit.Y), dest, dist);
            float meDist = RealBattleInfoScript.GetUnitSide(unit.Type) / 2.0f;
            bool canMove = true;
            List<BattleUnit>[,] grid = unit.IsOne ? oneGrid : twoGrid;

            if (newMe.x <= RealBattleInfoScript.FieldLeft ||
                newMe.x >= RealBattleInfoScript.FieldLeft + RealBattleInfoScript.FieldWidth ||
                newMe.y <= RealBattleInfoScript.FieldTop ||
                newMe.y >= RealBattleInfoScript.FieldTop + RealBattleInfoScript.FieldHeight)
            {
                return false;
            }

            for (int i = 0; i < cells.Length; i++)
            {
                if (canMove)
                {
                    CellCoor cellCoor = cells[i];
                    canMove = _CanMove_ProcessCell(unit, newMe, grid[cellCoor.Col, cellCoor.Row], meDist);
                }
                else break;
            }

            return canMove;
        }
        #endregion

        //FindEnenmyNearMe (Stay & One & All) | CheckNear
        //FindNearestEnemy (One) | FindNearest
        //CanMove (All & One) | CanMove
    }
    #endregion

    #region FIELDS
    private CellGrid grid = new CellGrid();
    public BattleGroup InfOne = new BattleGroup(true, RealBattleTroops.Inf);
    public BattleGroup InfTwo = new BattleGroup(false, RealBattleTroops.Inf);
    public BattleGroup CavLeftOne = new BattleGroup(true, RealBattleTroops.CavLeft);
    public BattleGroup CavRightOne = new BattleGroup(true, RealBattleTroops.CavRight);
    public BattleGroup CavLeftTwo = new BattleGroup(false, RealBattleTroops.CavLeft);
    public BattleGroup CavRightTwo = new BattleGroup(false, RealBattleTroops.CavRight);
    public BattleGroup ArtOne = new BattleGroup(true, RealBattleTroops.Art);
    public BattleGroup ArtTwo = new BattleGroup(false, RealBattleTroops.Art);
    public BallGroup BallOne = new BallGroup();
    public BallGroup BallTwo = new BallGroup();
    #endregion

    #region CTOR
    public RealBattleScript(ArmyUnitScript leftArmy, ArmyUnitScript rightArmy)
    {
        #region GROUPS

        #region INF

        #region ONE
        int quot = (int)(leftArmy.GetInfantry() / 1000);
        float rem = leftArmy.GetInfantry() - quot * 1000;

        for (int i = 0; i < quot; i++)
        {
            InfOne.AddUnit(1000, 0);
        }
        if ((int)rem != 0)
        {
            InfOne.AddUnit(rem, 0);
        }

        if (InfOne.GetUnitsCount() > RealBattleInfoScript.Inf_MaxCount)
        {
            Debug.Log("error in RealBattleScript(constructor) : InfOne");
        }
        #endregion

        #region TWO
        quot = (int)(rightArmy.GetInfantry() / 1000);
        rem = rightArmy.GetInfantry() - quot * 1000;

        for (int i = 0; i < quot; i++)
        {
            InfTwo.AddUnit(1000, 0);
        }
        if ((int)rem != 0)
        {
            InfTwo.AddUnit(rem, 0);
        }

        if (InfTwo.GetUnitsCount() > RealBattleInfoScript.Inf_MaxCount)
        {
            Debug.Log("error in RealBattleScript(constructor) : InfTwo");
        }
        #endregion

        #endregion

        #region CAV

        #region ONE
        float cavOneHalf = leftArmy.GetCavalry() / 2.0f;
        quot = (int)(cavOneHalf / 1000);
        rem = cavOneHalf - quot * 1000;

        for (int i = 0; i < quot; i++)
        {
            CavLeftOne.AddUnit(1000, 180);
            CavRightOne.AddUnit(1000, 180);
        }
        if ((int)rem != 0)
        {
            CavLeftOne.AddUnit(rem, 180);
            CavRightOne.AddUnit(rem, 180);
        }

        if (CavLeftOne.GetUnitsCount() + CavRightOne.GetUnitsCount() > RealBattleInfoScript.Cav_MaxCount)
        {
            Debug.Log("error in RealBattleScript(constructor) : CavOne");
        }
        #endregion

        #region TWO
        float cavTwoHalf = rightArmy.GetCavalry() / 2.0f;
        quot = (int)(cavTwoHalf / 1000);
        rem = cavTwoHalf - quot * 1000;

        for (int i = 0; i < quot; i++)
        {
            CavLeftTwo.AddUnit(1000, 0);
            CavRightTwo.AddUnit(1000, 0);
        }
        if ((int)rem != 0)
        {
            CavLeftTwo.AddUnit(rem, 0);
            CavRightTwo.AddUnit(rem, 0);
        }

        if (CavLeftTwo.GetUnitsCount() + CavRightTwo.GetUnitsCount() > RealBattleInfoScript.Cav_MaxCount)
        {
            Debug.Log("error in RealBattleScript(constructor) : CavTwo");
        }
        #endregion

        #endregion

        #region ART

        #region ONE
        quot = (int)(leftArmy.GetArtillery() / 1000);
        rem = leftArmy.GetArtillery() - quot * 1000;

        for (int i = 0; i < quot; i++)
        {
            ArtOne.AddUnit(1000, 0);
        }
        if ((int)rem != 0)
        {
            ArtOne.AddUnit(rem, 0);
        }

        if (ArtOne.GetUnitsCount() > RealBattleInfoScript.Art_MaxCount)
        {
            Debug.Log("error in RealBattleScript(constructor) : ArtOne");
        }
        #endregion

        #region TWO
        quot = (int)(rightArmy.GetArtillery() / 1000);
        rem = rightArmy.GetArtillery() - quot * 1000;

        for (int i = 0; i < quot; i++)
        {
            ArtTwo.AddUnit(1000, 0);
        }
        if ((int)rem != 0)
        {
            ArtTwo.AddUnit(rem, 0);
        }

        if (ArtTwo.GetUnitsCount() > RealBattleInfoScript.Art_MaxCount)
        {
            Debug.Log("error in RealBattleScript(constructor) : ArtOne");
        }
        #endregion

        #endregion

        #endregion

        #region PALCES

        #region INF

        #region ONE
        Vector2 center = CalcStartPoint(RealBattleTroops.Inf, true);
        for (int i = 0; i < InfOne.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y + 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.Inf, true, center, dest, i);
            InfOne.GetUnit(i).X = startPoint.x;
            InfOne.GetUnit(i).Y = startPoint.y;
        }
        #endregion

        #region TWO
        center = CalcStartPoint(RealBattleTroops.Inf, false);
        for (int i = 0; i < InfTwo.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y - 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.Inf, false, center, dest, i);
            InfTwo.GetUnit(i).X = startPoint.x;
            InfTwo.GetUnit(i).Y = startPoint.y;
        }
        #endregion

        #endregion

        #region CAV

        #region ONE
        center = CalcStartPoint(RealBattleTroops.CavLeft, true);
        for (int i = 0; i < CavLeftOne.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y + 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.CavLeft, true, center, dest, i);
            CavLeftOne.GetUnit(i).X = startPoint.x;
            CavLeftOne.GetUnit(i).Y = startPoint.y;
        }

        center = CalcStartPoint(RealBattleTroops.CavRight, true);
        for (int i = 0; i < CavRightOne.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y + 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.CavRight, true, center, dest, i);
            CavRightOne.GetUnit(i).X = startPoint.x;
            CavRightOne.GetUnit(i).Y = startPoint.y;
        }
        #endregion

        #region TWO
        center = CalcStartPoint(RealBattleTroops.CavLeft, false);
        for (int i = 0; i < CavLeftTwo.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y - 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.CavLeft, false, center, dest, i);
            CavLeftTwo.GetUnit(i).X = startPoint.x;
            CavLeftTwo.GetUnit(i).Y = startPoint.y;
        }

        center = CalcStartPoint(RealBattleTroops.CavRight, false);
        for (int i = 0; i < CavRightTwo.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y - 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.CavRight, false, center, dest, i);
            CavRightTwo.GetUnit(i).X = startPoint.x;
            CavRightTwo.GetUnit(i).Y = startPoint.y;
        }
        #endregion

        #endregion

        #region ART

        #region ONE
        center = CalcStartPoint(RealBattleTroops.Art, true);
        for (int i = 0; i < ArtOne.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y + 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.Art, true, center, dest, i);
            ArtOne.GetUnit(i).X = startPoint.x;
            ArtOne.GetUnit(i).Y = startPoint.y;
        }
        #endregion

        #region TWO
        center = CalcStartPoint(RealBattleTroops.Art, false);
        for (int i = 0; i < ArtTwo.GetUnitsCount(); i++)
        {
            Vector2 dest = new Vector2(center.x, center.y - 1);
            Vector2 startPoint = GetMarchPlace(RealBattleTroops.Art, false, center, dest, i);
            ArtTwo.GetUnit(i).X = startPoint.x;
            ArtTwo.GetUnit(i).Y = startPoint.y;
        }
        #endregion

        #endregion

        #endregion

        #region ENEMY_TYPES
        foreach (RealBattleTroops troop in Enum.GetValues(typeof(RealBattleTroops)))
        {
            _GetGroup(true, troop).EnemyType = _GetEnemyType(true, troop);
            _GetGroup(false, troop).EnemyType = _GetEnemyType(false, troop);
        }
        #endregion
    }

    private void _GetCountRowsColumns(bool isOne, RealBattleTroops type, out int rows, out int columns)
    {
        if (type == RealBattleTroops.Inf)
        {
            int n = isOne ? InfOne.GetUnitsCount() : InfTwo.GetUnitsCount();

            columns = (int)Mathf.Ceil(Mathf.Sqrt(n * RealBattleInfoScript.Inf_Ratio_WidthToHeight));
            if (columns > n)
            {
                columns = n;
            }
            if (columns > RealBattleInfoScript.Inf_MaxColumns)
            {
                columns = RealBattleInfoScript.Inf_MaxColumns;
            }
            rows = (int)Mathf.Ceil(n / (float)columns);
        }
        else if (type == RealBattleTroops.CavLeft || type == RealBattleTroops.CavRight)
        {
            int n = isOne ? CavLeftOne.GetUnitsCount() : CavLeftTwo.GetUnitsCount();
            if (type == RealBattleTroops.CavRight)
            {
                n = isOne ? CavRightOne.GetUnitsCount() : CavRightTwo.GetUnitsCount();
            }

            columns = (int)Mathf.Ceil(Mathf.Sqrt(n * RealBattleInfoScript.Cav_Ratio_WidthToHeight));
            if (columns == 0)
            {
                columns = 1;
            }
            rows = (int)Mathf.Ceil(n / (float)columns);
        }
        else
        {
            int n = isOne ? ArtOne.GetUnitsCount() : ArtTwo.GetUnitsCount();

            columns = (int)Mathf.Ceil(Mathf.Sqrt(n * RealBattleInfoScript.Art_Ratio_WidthToHeight));
            if (columns > n)
            {
                columns = n;
            }
            if (columns > RealBattleInfoScript.Art_MaxColumns)
            {
                columns = RealBattleInfoScript.Art_MaxColumns;
            }
            rows = (int)Mathf.Ceil(n / (float)columns);
        }
    }

    private Vector2 CalcStartPoint(RealBattleTroops type, bool isOne)
    {
        Vector2 point = new Vector2(0, 0);
        int rows;
        int columns;

        #region INF
        if (type == RealBattleTroops.Inf)
        {
            _GetCountRowsColumns(isOne, type, out rows, out columns);
            //point.x = RealBattleInfoScript.FieldLeft + RealBattleInfoScript.FieldWidth / 2.0f;
            point.x = RealBattleInfoScript.InfOne_X;

            #region POINT_Y
            if (isOne)
            {
                //point.y = RealBattleInfoScript.FieldTop + RealBattleInfoScript.FieldHeight * RealBattleInfoScript.Field_V_OnePartPer;
                //point.y -= rows / 2.0f * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
                point.y = RealBattleInfoScript.InfOne_Y;
            }
            else
            {
                //point.y = RealBattleInfoScript.FieldTop + RealBattleInfoScript.FieldHeight * (RealBattleInfoScript.Field_V_OnePartPer + RealBattleInfoScript.Field_V_FightPartPer);
                //point.y += rows / 2.0f * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
                point.y = RealBattleInfoScript.InfTwo_Y;
            }
            #endregion
        }
        #endregion

        #region CAV
        else if (type == RealBattleTroops.CavLeft || type == RealBattleTroops.CavRight)
        {
            _GetCountRowsColumns(isOne, type, out rows, out columns);

            #region POINT_X
            if (type == RealBattleTroops.CavLeft)
            {
                //point.x = RealBattleInfoScript.FieldLeft +
                //    RealBattleInfoScript.FieldWidth * (RealBattleInfoScript.Field_H_FreePer
                //    + RealBattleInfoScript.Field_H_FreeCavPer / 2.0f);
                point.x = RealBattleInfoScript.CavLeftOne_X;
            }
            else
            {
                //point.x = RealBattleInfoScript.FieldLeft +
                //    RealBattleInfoScript.FieldWidth * (1.0f - RealBattleInfoScript.Field_H_FreePer
                //    - RealBattleInfoScript.Field_H_FreeCavPer / 2.0f);
                point.x = RealBattleInfoScript.CavRightOne_X;
            }
            #endregion

            #region POINT_Y
            if (isOne)
            {
                //point.y = RealBattleInfoScript.FieldTop + RealBattleInfoScript.FieldHeight * RealBattleInfoScript.Field_V_OnePartPer;
                //point.y -= rows / 2.0f * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
                point.y = RealBattleInfoScript.CavLeftOne_Y;
            }
            else
            {
                //point.y = RealBattleInfoScript.FieldTop + RealBattleInfoScript.FieldHeight * (RealBattleInfoScript.Field_V_OnePartPer + RealBattleInfoScript.Field_V_FightPartPer);
                //point.y += rows / 2.0f * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
                point.y = RealBattleInfoScript.CavLeftTwo_Y;
            }
            #endregion

        }
        #endregion

        #region ART
        else
        {
            _GetCountRowsColumns(isOne, type, out rows, out columns);
            //point.x = RealBattleInfoScript.FieldLeft + RealBattleInfoScript.FieldWidth / 2.0f;
            point.x = RealBattleInfoScript.ArtOne_X;

            #region POINT_Y
            if (isOne)
            {
                //point.y = RealBattleInfoScript.FieldTop + RealBattleInfoScript.FieldHeight *
                //    (RealBattleInfoScript.Field_V_OnePartPer - RealBattleInfoScript.Field_V_FreeArtFreeInfPer
                //    - RealBattleInfoScript.Field_V_FreeArtFreePer);
                //point.y -= rows / 2.0f * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Art) + RealBattleInfoScript.UnitMarchSpace);
                point.y = RealBattleInfoScript.ArtOne_Y;
            }
            else
            {
                //point.y = RealBattleInfoScript.FieldTop + RealBattleInfoScript.FieldHeight *
                //    (RealBattleInfoScript.Field_V_OnePartPer + RealBattleInfoScript.Field_V_FightPartPer
                //    + RealBattleInfoScript.Field_V_FreeArtFreeInfPer + RealBattleInfoScript.Field_V_FreeArtFreePer);
                //point.y += rows / 2.0f * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Art) + RealBattleInfoScript.UnitMarchSpace);
                point.y = RealBattleInfoScript.ArtTwo_Y;
            }
            #endregion
        }
        #endregion

        return point;
    }
    #endregion

    #region UPDATE
    public void Update(float delta)
    {
        //проверить, что еще есть с кем сражаться

        #region REMOVE_EMPTY
        RemoveEmty();
        #endregion

        #region ENEMY_TYPE
        EnemyTypeUpdate();
        #endregion

        #region CENTER
        MarchCenterUpdate();
        #endregion

        #region CELL_GRID
        CellGridUpdate();
        #endregion

        #region INF
        int n = Mathf.Max(InfOne.GetUnitsCount(), InfTwo.GetUnitsCount());
        for (int i = 0; i < n; i++)
        {
            if (InfOne.GetUnitsCount() > i)
            {
                InfOne.GetUnit(i).Delta += delta;
                UpdateUnit(true, RealBattleTroops.Inf, i);
            }
            if (InfTwo.GetUnitsCount() > i)
            {
                InfTwo.GetUnit(i).Delta += delta;
                UpdateUnit(false, RealBattleTroops.Inf, i);
            }
        }
        #endregion

        #region CAV
        n = Mathf.Max(CavLeftOne.GetUnitsCount(), CavLeftTwo.GetUnitsCount());
        for (int i = 0; i < n; i++)
        {
            if (CavLeftOne.GetUnitsCount() > i)
            {
                CavLeftOne.GetUnit(i).Delta += delta;
                UpdateUnit(true, RealBattleTroops.CavLeft, i);
            }
            if (CavLeftTwo.GetUnitsCount() > i)
            {
                CavLeftTwo.GetUnit(i).Delta += delta;
                UpdateUnit(false, RealBattleTroops.CavLeft, i);
            }
        }

        n = Mathf.Max(CavRightOne.GetUnitsCount(), CavRightTwo.GetUnitsCount());
        for (int i = 0; i < n; i++)
        {
            if (CavRightOne.GetUnitsCount() > i)
            {
                CavRightOne.GetUnit(i).Delta += delta;
                UpdateUnit(true, RealBattleTroops.CavRight, i);
            }
            if (CavRightTwo.GetUnitsCount() > i)
            {
                CavRightTwo.GetUnit(i).Delta += delta;
                UpdateUnit(false, RealBattleTroops.CavRight, i);
            }
        }
        #endregion

        #region ART
        n = Mathf.Max(ArtOne.GetUnitsCount(), ArtTwo.GetUnitsCount());
        for (int i = 0; i < n; i++)
        {
            if (ArtOne.GetUnitsCount() > i)
            {
                ArtOne.GetUnit(i).Delta += delta;
                UpdateUnit(true, RealBattleTroops.Art, i);
            }
            if (ArtTwo.GetUnitsCount() > i)
            {
                ArtTwo.GetUnit(i).Delta += delta;
                UpdateUnit(false, RealBattleTroops.Art, i);
            }
        }
        #endregion

        #region BALL
        n = Mathf.Max(BallOne.Units.Count, BallTwo.Units.Count);
        for (int i = 0; i < n; i++)
        {
            if (BallOne.Units.Count > i)
            {
                UpdateBall(true, i);
            }
            if (BallTwo.Units.Count > i)
            {
                UpdateBall(false, i);
            }
        }

        for (int i = 0; i < BallOne.Units.Count; )
        {
            if (BallOne.Units[i].GetIsHitted())
            {
                BallOne.Units.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        for (int i = 0; i < BallTwo.Units.Count; )
        {
            if (BallTwo.Units[i].GetIsHitted())
            {
                BallTwo.Units.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        #endregion
    }

    #region REMOVE_EMPTY
    private void RemoveEmty()
    {
        foreach (RealBattleTroops troop in Enum.GetValues(typeof(RealBattleTroops)))
        {
            _GetGroup(true, troop).RemoveEmpty();
            _GetGroup(false, troop).RemoveEmpty();
        }
    }
    #endregion

    #region ENEMY_TYPE
    private RealBattleTroops _GetEnemyType(bool isOne, RealBattleTroops myType)
    {
        BattleGroup inf = isOne ? InfTwo : InfOne;
        BattleGroup cavL = isOne ? CavLeftTwo : CavLeftOne;
        BattleGroup cavR = isOne ? CavRightTwo : CavRightOne;
        BattleGroup art = isOne ? ArtTwo : ArtOne;

        #region INF
        if (myType == RealBattleTroops.Inf)
        {
            if (inf.GetCountReal() > 0)
            {
                return RealBattleTroops.Inf;
            }
            else if (cavL.GetCountReal() > 0)
            {
                return RealBattleTroops.CavLeft;
            }
            else if (cavR.GetCountReal() > 0)
            {
                return RealBattleTroops.CavRight;
            }
            else
            {
                return RealBattleTroops.Art;
            }
        }
        #endregion

        #region CAV_L
        else if (myType == RealBattleTroops.CavLeft)
        {
            if (cavL.GetCountReal() > 0)
            {
                return RealBattleTroops.CavLeft;
            }
            else if (art.GetCountReal() > 0)
            {
                return RealBattleTroops.Art;
            }
            else if (cavR.GetCountReal() > 0)
            {
                return RealBattleTroops.CavRight;
            }
            else
            {
                return RealBattleTroops.Inf;
            }
        }
        #endregion

        #region CAV_R
        else if (myType == RealBattleTroops.CavRight)
        {
            if (cavR.GetCountReal() > 0)
            {
                return RealBattleTroops.CavRight;
            }
            else if (art.GetCountReal() > 0)
            {
                return RealBattleTroops.Art;
            }
            else if (cavL.GetCountReal() > 0)
            {
                return RealBattleTroops.CavLeft;
            }
            else
            {
                return RealBattleTroops.Inf;
            }
        }
        #endregion

        #region ART
        else
        {
            if (inf.GetCountReal() > 0)
            {
                return RealBattleTroops.Inf;
            }
            else if (cavL.GetCountReal() > 0)
            {
                return RealBattleTroops.CavLeft;
            }
            else if (cavR.GetCountReal() > 0)
            {
                return RealBattleTroops.CavRight;
            }
            else
            {
                return RealBattleTroops.Art;
            }
        }
        #endregion
    }
    private void EnemyTypeUpdate()
    {
        foreach (RealBattleTroops troop in Enum.GetValues(typeof(RealBattleTroops)))
        {
            BattleGroup group = _GetGroup(true, troop);
            RealBattleTroops ex_type = group.EnemyType;
            group.EnemyType = _GetEnemyType(true, troop);

            group = _GetGroup(false, troop);
            ex_type = group.EnemyType;
            group.EnemyType = _GetEnemyType(false, troop);
        }
    }
    #endregion

    #region MARCH_CENTER
    private void MarchCenterUpdate()
    {
        foreach (RealBattleTroops troop in Enum.GetValues(typeof(RealBattleTroops)))
        {
            BattleGroup group = _GetGroup(true, troop);
            if (group.GetUnitsCount() != 0)
            {
                group.MarchCenter = CalcMarchCenter(group);
            }

            group = _GetGroup(false, troop);
            if (group.GetUnitsCount() != 0)
            {
                group.MarchCenter = CalcMarchCenter(group);
            }
        }
    }
    private Vector2 CalcMarchCenter(BattleGroup group)
    {
        int rows, columns, index_1 = 0, index_2 = 0;
        _GetCountRowsColumns(group.IsOne, group.Type, out rows, out columns);

        if (rows == 1)
        {
            index_1 = 0;
            index_2 = group.GetUnitsCount() - 1;
        }
        else if (rows % 2 == 0)
        {
            index_1 = 0;
            index_2 = (rows - 1) * columns;
        }
        else
        {
            index_1 = columns - 1;
            index_2 = (rows - 1) * columns;
        }

        BattleUnit unit1 = group.GetUnit(index_1);
        BattleUnit unit2 = group.GetUnit(index_2);

        return new Vector2((unit1.X + unit2.X) / 2.0f, (unit1.Y + unit2.Y) / 2.0f);
    }
    #endregion

    #region CELL_GRID
    private void CellGridUpdate()
    {
        grid.Clear();

        foreach (RealBattleTroops troop in Enum.GetValues(typeof(RealBattleTroops)))
        {
            grid.AddGroup(_GetGroup(true, troop));
            grid.AddGroup(_GetGroup(false, troop));
        }
    }
    #endregion

    #region UPDATE_UNIT
    private BattleGroup _GetGroup(bool isOne, RealBattleTroops type)
    {
        BattleGroup group = null;
        switch (type)
        {
            case RealBattleTroops.Inf:
                group = isOne ? InfOne : InfTwo;
                break;
            case RealBattleTroops.CavLeft:
                group = isOne ? CavLeftOne : CavLeftTwo;
                break;
            case RealBattleTroops.CavRight:
                group = isOne ? CavRightOne : CavRightTwo;
                break;
            case RealBattleTroops.Art:
                group = isOne ? ArtOne : ArtTwo;
                break;
            default:
                Debug.Log("error in _GetGroup in RealBattleScript");
                break;
        }

        return group;
    }
    private void UpdateUnit(bool isOne, RealBattleTroops type, int index)
    {
        BattleUnit unit = _GetGroup(isOne, type).GetUnit(index);

        #region UPDATE
        switch (unit.State)
        {
            case UnitStates.Stay:
                Update_Stay(isOne, type, unit);
                break;
            case UnitStates.MoveAll:
                Update_MoveAll(isOne, type, index);
                break;
            case UnitStates.Fight:
                Update_Fight(isOne, type, unit);
                break;
            case UnitStates.MoveOne:
                UpdateInf_MoveOne(isOne, type, unit);
                break;
            default:
                Debug.Log("error in UpdateInf in RealBattleScript");
                break;
        }
        #endregion
    }

    private void Update_Stay(bool isOne, RealBattleTroops type, BattleUnit me)
    {
        BattleGroup group = _GetGroup(isOne, type);
        //if (FindEnenmyNearMe(isOne, me, type) != null)
        if (grid.CheckNear(me) != null)
        {
            me.State = UnitStates.Fight;
            group.IncCountFight();
        }
        else
        {
            if (group.GetCountFight() != 0 || group.GetIsFirstBlood())
            {
                me.State = UnitStates.MoveOne;
            }
            else
            {
                me.State = UnitStates.MoveAll;
            }
        }
    }
    private void Update_Fight(bool isOne, RealBattleTroops type, BattleUnit me)
    {
        BattleGroup group = _GetGroup(isOne, type);
        //BattleUnit enemy = FindEnenmyNearMe(isOne, me, type);
        BattleUnit enemy = grid.CheckNear(me);
        if (enemy != null)
        {
            if (!group.GetIsFirstBlood())
            {
                group.SetFirstBloodTrue();
            }

            if (BattleDeltaTimeScript.IsNextFrame(type, ref me.Delta))
            {
                if (type != RealBattleTroops.Art)
                {
                    enemy.Count -= me.Count * 0.1f;
                }
                else
                {
                    BallGroup ballGroup = isOne ? BallOne : BallTwo;
                    Vector2 pos = new Vector2(me.X, me.Y);
                    Vector2 dest = new Vector2(enemy.X, enemy.Y);
                    float damage = me.Count * RealBattleInfoScript.ArtBallDamagePerUnit;
                    ballGroup.Units.Add(new BallUnit(pos, dest, damage));
                }

                if (enemy.Count < 0)
                {
                    enemy.Count = 0;
                }
            }
        }
        else
        {
            group.DecCountFight();

            if (group.GetCountFight() != 0 || group.GetIsFirstBlood())
            {
                me.State = UnitStates.MoveOne;
            }
            else
            {
                me.State = UnitStates.MoveAll;
            }
        }
    }
    private void Update_MoveAll(bool isOne, RealBattleTroops type, int index)
    {
        //BattleUnit unit = _GetGroup(isOne, type).GetUnit(index);
        //if (isOne)
        //{
        //    unit.Y += RealBattleInfoScript.GetSpeed(type) * BattleDeltaTimeScript.SpeedTime;
        //}
        //else
        //{
        //    unit.Y -= RealBattleInfoScript.GetSpeed(type) * BattleDeltaTimeScript.SpeedTime;
        //}

        BattleGroup group = _GetGroup(isOne, type);
        //group.SetMoveAllTrue();

        #region MOVE
        Vector2 myCenter = group.MarchCenter;
        Vector2 enemyCenter = _GetGroup(!isOne, group.EnemyType).MarchCenter;
        float dist = RealBattleInfoScript.GetSpeed(type) * BattleDeltaTimeScript.SpeedTime;
        Vector2 myNewCenter = Vector2.MoveTowards(myCenter, enemyCenter, dist);

        //if (myNewCenter.x == enemyCenter.x && myNewCenter.y == enemyCenter.y)
        //{
        //    Debug.Log("OLOLO");
        //}

        BattleUnit meUnit = group.GetUnit(index);
        Vector2 meVec = new Vector2(meUnit.X, meUnit.Y);
        Vector2 newBestMe = GetMarchPlace(type, isOne, myNewCenter, enemyCenter, index);
        Vector2 newRealMe = Vector2.MoveTowards(meVec, newBestMe, dist);


        //if (true || CanMove(isOne, meUnit, newRealMe, dist))
        //{
        //    meUnit.X = newRealMe.x;
        //    meUnit.Y = newRealMe.y;
        //    if (meVec.x != newRealMe.x || meVec.y != newRealMe.y)
        //    {
        //        group.SetMovedTrue();
        //    }
        //}

        #region CHANGE_COORS
        meUnit.X = newRealMe.x;
        meUnit.Y = newRealMe.y;
        //if (meVec.x != newRealMe.x || meVec.y != newRealMe.y)
        //{
        //    group.SetMovedTrue();
        //}
        #endregion

        #endregion

        #region CHANGE_STATE
        //if (FindEnenmyNearMe(isOne, meUnit, type) != null)
        if (grid.CheckNear(meUnit) != null)
        {
            meUnit.State = UnitStates.Fight;
            group.IncCountFight();
        }
        else if (group.GetCountFight() != 0)
        {
            meUnit.State = UnitStates.MoveOne;
        }
        #endregion
    }
    private void UpdateInf_MoveOne(bool isOne, RealBattleTroops type, BattleUnit meUnit)
    {
        //BattleUnit enemy = FindNearestEnemy(isOne, meUnit, type);
        BattleUnit enemy = grid.FindNearest(meUnit);
        if (enemy == null)
        {
            meUnit.State = UnitStates.Stay;
            return;
        }

        float dist = RealBattleInfoScript.GetSpeed(type) * BattleDeltaTimeScript.SpeedTime;
        //if (IsIntersect(isOne, meUnit, type))
        if (false)
        {
            Vector2 newMe = Vector2.MoveTowards(new Vector2(meUnit.X, meUnit.Y), new Vector2(enemy.X, enemy.Y), dist);
            meUnit.X = newMe.x;
            meUnit.Y = newMe.y;
            Debug.Log("SHEEEEET - " + isOne + " - " + type);
        }
        else
        {
            Vector2 target = new Vector2(enemy.X, enemy.Y);
            Vector2 me = new Vector2(meUnit.X, meUnit.Y);
            Vector2 newMe = Vector2.MoveTowards(new Vector2(meUnit.X, meUnit.Y), target, dist);

            #region ANGLE
            int angleConst = 5;
            int count = 360 / angleConst;
            float angle = angleConst;

            //Vector2 line = CalcMarchCenter(!isOne, type) - CalcMarchCenter(isOne, type);
            //Vector2 myLine = CalcMarchCenter(!isOne, type) - me;
            Vector2 line = _GetGroup(!isOne, type).MarchCenter - _GetGroup(isOne, type).MarchCenter;
            Vector2 myLine = _GetGroup(!isOne, type).MarchCenter - me;
            if (line.x * myLine.y - myLine.x * line.y > 0)
            {
                angle *= -1;
            }
            angle *= Mathf.Deg2Rad;
            #endregion

            //string exstr = "";
            //while (!CanMove(isOne, meUnit, newMe, dist, type) && count > 0)
            while (!grid.CanMove(meUnit, newMe, dist) && count > 0)
            {
                #region ROTATE
                float xNew = me.x + (target.x - me.x) * Mathf.Cos(angle) - (target.y - me.y) * Mathf.Sin(angle);
                float yNew = me.y + (target.y - me.y) * Mathf.Cos(angle) + (target.x - me.x) * Mathf.Sin(angle);
                target.x = xNew;
                target.y = yNew;
                #endregion

                newMe = Vector2.MoveTowards(new Vector2(meUnit.X, meUnit.Y), target, dist);
                count--;

                //exstr += string.Format("plot({0}, {1}, 'r.', 'MarkerSize', 25);", xNew, yNew);
            }

            if (count > 0)
            {
                //if (type == RealBattleTroops.CavLeft || type == RealBattleTroops.CavRight)
                //{
                //    CavUnit cavUnit = meUnit as CavUnit;
                //    if (cavUnit == null)
                //    {
                //        Debug.Log("error in UpdateInf_MoveOne(cavUnit == null) in RealBattleScript");
                //    }
                //    else
                //    {
                //        float oldAngle = cavUnit.GetAngle();
                //        float shiftAngle = GetCavAngle(cavUnit, new Vector2(newMe.x, newMe.y));

                //        //if (Mathf.Abs(shiftAngle) > RealBattleInfoScript.CavMaxRotateAngle)
                //        //{
                //        //    int sign = (int)Mathf.Sign(shiftAngle);
                //        //    cavUnit.SetAngle(oldAngle + sign * RealBattleInfoScript.CavMaxRotateAngle);
                //        //}
                //        //else
                //        //{
                //        cavUnit.SetAngle(oldAngle + shiftAngle);
                //        //}
                //        meUnit.X = newMe.x;
                //        meUnit.Y = newMe.y;
                //    }
                //}
                //else
                //{
                meUnit.X = newMe.x;
                meUnit.Y = newMe.y;
                //}
                //if (IsIntersect(isOne, meUnit, type))
                //{
                //    Debug.Log("BAAAKA");
                //}
            }
            else
            {
                meUnit.State = UnitStates.Stay;
                return;
            }
        }

        #region CHANGE_STATE
        BattleGroup group = _GetGroup(isOne, type);
        //if (FindEnenmyNearMe(isOne, meUnit, type) != null)
        if (grid.CheckNear(meUnit) != null)
        {
            meUnit.State = UnitStates.Fight;
            group.IncCountFight();
        }
        else
        {
            if (group.GetCountFight() == 0 && !group.GetIsFirstBlood())
            {
                meUnit.State = UnitStates.MoveAll;
            }
        }
        #endregion
    }
    #endregion

    #region UPDATE_BALL
    private void UpdateBall(bool isOne, int index)
    {
        BallGroup group = isOne ? BallOne : BallTwo;
        BallUnit unit = group.Units[index];
        BattleUnit enemy = CheckBallCollision(isOne, unit);

        if (enemy == null)
        {
            if (unit.Pos.x != unit.Dest.x || unit.Pos.y != unit.Dest.y)
            {
                float dist = RealBattleInfoScript.BallSpeed_PerSec * BattleDeltaTimeScript.SpeedTime;
                unit.Move(dist);
            }
            else
            {
                unit.SetHittedTrue();
            }
        }
        else
        {
            enemy.Count -= unit.Damage;
            if (enemy.Count < 0)
            {
                enemy.Count = 0;
            }

            unit.SetHittedTrue();
        }
    }

    private BattleUnit _CheckBallCollision_FindInGroup(BallUnit me, BattleGroup enemyGroup, float dist)
    {
        for (int i = 0; i < enemyGroup.GetUnitsCount(); i++)
        {
            BattleUnit enemy = enemyGroup.GetUnit(i);
            float len = Mathf.Sqrt(Mathf.Pow(enemy.X - me.Pos.x, 2) + Mathf.Pow(enemy.Y - me.Pos.y, 2));

            if (len <= dist)
            {
                return enemy;
            }
        }

        return null;
    }
    private BattleUnit CheckBallCollision(bool isOne, BallUnit me)
    {
        BattleUnit enemy = null;
        foreach (RealBattleTroops troop in Enum.GetValues(typeof(RealBattleTroops)))
        {
            if (enemy == null)
            {
                float dist = (RealBattleInfoScript.Ball_UnitSide + RealBattleInfoScript.GetUnitSide(troop)) / 2.0f;
                enemy = _CheckBallCollision_FindInGroup(me, _GetGroup(!isOne, troop), dist);
            }
            else break;
        }

        return enemy;
    }
    #endregion

    #region SORT_GROUP
    private void SortGroup(bool isOne)
    {
        BattleGroup group = null;

        #region ONE
        if (isOne)
        {
            group = InfOne;
        }
        #endregion

        #region TWO
        else
        {
            group = InfTwo;
        }
        #endregion

        //group.Units.Sort((a, b) =>
        //{
        //    int xdiff = a.Y.CompareTo(b.Y);
        //    if (xdiff != 0) return xdiff;
        //    else return a.X.CompareTo(b.X);
        //});
    }
    #endregion

    #region FIND_ENEMY_NEAR_ME
    private BattleUnit _FindEnenmyNearMe_FindInGroup(BattleUnit me, BattleGroup enemyGroup, float dist)
    {
        for (int i = 0; i < enemyGroup.GetUnitsCount(); i++)
        {
            BattleUnit enemy = enemyGroup.GetUnit(i);
            float len = Mathf.Sqrt(Mathf.Pow(enemy.X - me.X, 2) + Mathf.Pow(enemy.Y - me.Y, 2));

            if (len <= dist)
            {
                return enemy;
            }
        }

        return null;
    }
    //private BattleUnit FindEnenmyNearMe(bool isOne, BattleUnit me, RealBattleTroops type)
    //{
    //    float dist = type == RealBattleTroops.Art ? RealBattleInfoScript.ArtShotDist : RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf);
    //    BattleUnit enemy = null;
    //    foreach (RealBattleTroops troop in Enum.GetValues(typeof(RealBattleTroops)))
    //    {
    //        if (enemy == null)
    //        {
    //            enemy = _FindEnenmyNearMe_FindInGroup(me, _GetGroup(!isOne, troop), dist);
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }

    //    return enemy;
    //}
    #endregion

    #region FIND_NEAREST_ENEMY
    private BattleUnit _FindNearestEnemy_FindInGroup(BattleUnit me, BattleUnit near, BattleGroup group, RealBattleTroops myType, RealBattleTroops groupType)
    {
        float len = float.PositiveInfinity;
        if (near != null)
        {
            len = Mathf.Sqrt(Mathf.Pow(near.X - me.X, 2) + Mathf.Pow(near.Y - me.Y, 2));
        }

        for (int i = 0; i < group.GetUnitsCount(); i++)
        {
            BattleUnit enemy = group.GetUnit(i);
            float currLen = Mathf.Sqrt(Mathf.Pow(enemy.X - me.X, 2) + Mathf.Pow(enemy.Y - me.Y, 2));

            if (currLen < len)
            {
                near = enemy;
            }
        }
        return near;
    }
    //private BattleUnit FindNearestEnemy(bool isOne, BattleUnit me, RealBattleTroops myType)
    //{
    //    BattleGroup groupInf = isOne ? InfTwo : InfOne;
    //    BattleGroup groupCavLeft = isOne ? CavLeftTwo : CavLeftOne;
    //    BattleGroup groupCavRight = isOne ? CavRightTwo : CavRightOne;
    //    BattleGroup groupArt = isOne ? ArtTwo : ArtOne;

    //    BattleUnit near = _FindNearestEnemy_FindInGroup(me, null, groupInf, myType, RealBattleTroops.Inf);
    //    near = _FindNearestEnemy_FindInGroup(me, near, groupCavLeft, myType, RealBattleTroops.CavLeft);
    //    near = _FindNearestEnemy_FindInGroup(me, near, groupCavRight, myType, RealBattleTroops.CavRight);
    //    return _FindNearestEnemy_FindInGroup(me, near, groupArt, myType, RealBattleTroops.Art);
    //}
    #endregion

    #region CAN_MOVE
    private bool _CanMove_CheckGroup(BattleUnit me, Vector2 newMe, BattleGroup group, RealBattleTroops myType, RealBattleTroops groupType)
    {
        float dist = (RealBattleInfoScript.GetUnitSide(myType) + RealBattleInfoScript.GetUnitSide(groupType)) / 2.0f;
        for (int i = 0; i < group.GetUnitsCount(); i++)
        {
            BattleUnit other = group.GetUnit(i);
            if (me != other)
            {
                float len = Mathf.Sqrt(Mathf.Pow(other.X - newMe.x, 2) + Mathf.Pow(other.Y - newMe.y, 2));

                if (len <= dist)
                {
                    return false;
                }
            }
        }

        return true;
    }
    //private bool CanMove(bool isOne, BattleUnit me, Vector2 dest, float dist, RealBattleTroops myType)
    //{
    //    Vector2 newMe = Vector2.MoveTowards(new Vector2(me.X, me.Y), dest, dist);
    //    BattleGroup groupInf = isOne ? InfOne : InfTwo;
    //    BattleGroup groupCavLeft = isOne ? CavLeftOne : CavLeftTwo;
    //    BattleGroup groupCavRight = isOne ? CavRightOne : CavRightTwo;
    //    BattleGroup groupArt = isOne ? ArtOne : ArtTwo;

    //    return _CanMove_CheckGroup(me, newMe, groupInf, myType, RealBattleTroops.Inf) &&
    //           _CanMove_CheckGroup(me, newMe, groupCavLeft, myType, RealBattleTroops.CavLeft) &&
    //           _CanMove_CheckGroup(me, newMe, groupCavRight, myType, RealBattleTroops.CavRight) &&
    //           _CanMove_CheckGroup(me, newMe, groupArt, myType, RealBattleTroops.Art);
    //}
    #endregion

    #region IS_INTERSECT
    //private bool IsIntersect(bool isOne, BattleUnit meUnit, RealBattleTroops myType)
    //{
    //    BattleGroup groupInf = isOne ? InfOne : InfTwo;
    //    BattleGroup groupCavLeft = isOne ? CavLeftOne : CavLeftTwo;
    //    BattleGroup groupCavRight = isOne ? CavRightOne : CavRightTwo;
    //    BattleGroup groupArt = isOne ? ArtOne : ArtTwo;
    //    return _IsIntersect_CheckGroup(meUnit, groupInf, myType, RealBattleTroops.Inf) ||
    //           _IsIntersect_CheckGroup(meUnit, groupCavLeft, myType, RealBattleTroops.CavLeft) ||
    //           _IsIntersect_CheckGroup(meUnit, groupCavRight, myType, RealBattleTroops.CavRight) ||
    //           _IsIntersect_CheckGroup(meUnit, groupArt, myType, RealBattleTroops.Art);
    //}

    private bool _IsIntersect_CheckGroup(BattleUnit meUnit, BattleGroup group, RealBattleTroops myType, RealBattleTroops groupType)
    {
        float dist = (RealBattleInfoScript.GetUnitSide(myType) + RealBattleInfoScript.GetUnitSide(groupType)) / 2.0f;
        Vector2 me = new Vector2(meUnit.X, meUnit.Y);

        for (int i = 0; i < group.GetUnitsCount(); i++)
        {
            BattleUnit other = group.GetUnit(i);
            if (meUnit != other)
            {
                float len = Mathf.Sqrt(Mathf.Pow(other.X - me.x, 2) + Mathf.Pow(other.Y - me.y, 2));

                if (len + 0.1f < dist)
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region GET_CAV_ANGLE
    //public float GetCavAngle(CavUnit unit, Vector2 dest)
    //{
    //    Vector2 curr = Vector2.up;
    //    float angleCav = unit.GetAngle();
    //    float xNew = curr.x * Mathf.Cos(angleCav) - curr.y * Mathf.Sin(angleCav);
    //    float yNew = curr.y * Mathf.Cos(angleCav) + curr.x * Mathf.Sin(angleCav);

    //    float perpDot = xNew * dest.y - yNew * dest.x;
    //    float scalar = xNew * dest.x + yNew + dest.y;
    //    float den = Mathf.Sqrt(xNew * xNew + yNew * yNew) * Mathf.Sqrt(dest.x * dest.x + dest.y * dest.y);

    //    return Mathf.Atan2(perpDot, scalar / den) * Mathf.Rad2Deg;
    //}

    //private float signedAngle(Vector2 v1, Vector2 v2)
    //{

    //    float perpDot = v1.x * v2.y - v1.y * v2.x;

    //    return Mathf.Atan2(perpDot, Vector2.Dot(v1, v2)) * Mathf.Rad2Deg;
    //}
    #endregion

    #region GET_MARCH_PLACE
    private Vector2 GetMarchPlace(RealBattleTroops type, bool isOne, Vector2 center, Vector2 dest, int index)
    {
        Vector2 point = new Vector2(0, 0);
        int rows;
        int columns;
        int rowI;
        int columnI;
        float angle;

        #region INF
        if (type == RealBattleTroops.Inf)
        {
            _GetCountRowsColumns(isOne, type, out rows, out columns);

            #region ROW_I_AND_COLUMN_I
            rowI = (int)Mathf.Ceil(index / columns);
            columnI = index - (int)(index / columns) * columns;

            if (rowI % 2 == 1)
            {
                columnI = columns - columnI - 1;
            }
            #endregion

            #region POINT

            #region SHIFT
            point.y = center.y + (rowI - (rows - 1) / 2.0f) * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
            point.x = center.x + (columnI - (columns - 1) / 2.0f) * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
            #endregion

            #region ANGLE
            Vector2 v1 = dest - center;
            Vector2 v2 = Vector2.right;
            angle = (Mathf.Atan2(v1.y, v1.x) - Mathf.Atan2(v2.y, v2.x)) * Mathf.Rad2Deg;
            angle += 90.0f;
            angle *= Mathf.Deg2Rad;
            #endregion

            #region ROTATE
            float xNew = center.x + (point.x - center.x) * Mathf.Cos(angle) - (point.y - center.y) * Mathf.Sin(angle);
            float yNew = center.y + (point.y - center.y) * Mathf.Cos(angle) + (point.x - center.x) * Mathf.Sin(angle);
            point.x = xNew;
            point.y = yNew;
            #endregion

            #endregion
        }
        #endregion

        #region CAV
        else if (type == RealBattleTroops.CavLeft || type == RealBattleTroops.CavRight)
        {
            _GetCountRowsColumns(isOne, type, out rows, out columns);

            #region ROW_I_AND_COLUMN_I
            rowI = (int)Mathf.Ceil(index / columns);
            columnI = index - (int)(index / columns) * columns;

            if (rowI % 2 == 1)
            {
                columnI = columns - columnI - 1;
            }
            #endregion

            #region POINT

            #region SHIFT
            point.y = center.y + (rowI - (rows - 1) / 2.0f) * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
            point.x = center.x + (columnI - (columns - 1) / 2.0f) * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Inf) + RealBattleInfoScript.UnitMarchSpace);
            #endregion

            #region ANGLE
            Vector2 v1 = dest - center;
            Vector2 v2 = Vector2.right;
            angle = (Mathf.Atan2(v1.y, v1.x) - Mathf.Atan2(v2.y, v2.x)) * Mathf.Rad2Deg;
            angle += 90.0f;
            angle *= Mathf.Deg2Rad;
            #endregion

            #region ROTATE
            float xNew = center.x + (point.x - center.x) * Mathf.Cos(angle) - (point.y - center.y) * Mathf.Sin(angle);
            float yNew = center.y + (point.y - center.y) * Mathf.Cos(angle) + (point.x - center.x) * Mathf.Sin(angle);
            point.x = xNew;
            point.y = yNew;
            #endregion

            #endregion
        }
        #endregion

        #region ART
        else
        {
            _GetCountRowsColumns(isOne, type, out rows, out columns);

            #region ROW_I_AND_COLUMN_I
            rowI = (int)Mathf.Ceil(index / columns);
            columnI = index - (int)(index / columns) * columns;

            if (rowI % 2 == 1)
            {
                columnI = columns - columnI - 1;
            }
            #endregion

            #region POINT

            #region SHIFT
            point.y = center.y + (rowI - (rows - 1) / 2.0f) * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Art) + RealBattleInfoScript.UnitMarchSpace);
            point.x = center.x + (columnI - (columns - 1) / 2.0f) * (RealBattleInfoScript.GetUnitSide(RealBattleTroops.Art) + RealBattleInfoScript.UnitMarchSpace);
            #endregion

            #region ANGLE
            Vector2 v1 = dest - center;
            Vector2 v2 = Vector2.right;
            angle = (Mathf.Atan2(v1.y, v1.x) - Mathf.Atan2(v2.y, v2.x)) * Mathf.Rad2Deg;
            angle += 90.0f;
            angle *= Mathf.Deg2Rad;
            #endregion

            #region ROTATE
            float xNew = center.x + (point.x - center.x) * Mathf.Cos(angle) - (point.y - center.y) * Mathf.Sin(angle);
            float yNew = center.y + (point.y - center.y) * Mathf.Cos(angle) + (point.x - center.x) * Mathf.Sin(angle);
            point.x = xNew;
            point.y = yNew;
            #endregion

            #endregion
        }
        #endregion

        return point;
    }
    #endregion

    #endregion
}