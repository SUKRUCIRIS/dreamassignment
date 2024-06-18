using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static obstacle;
using static UnityEngine.ParticleSystem;

public class obstacle : gridobj
{
    public int health;
    public bool blastcandamage;
    public bool tntcandamage;
    public bool canfall;
    public enum OBSTACLE_TYPE
    {
        STONE,
        BOX,
        VASE
    }
    public OBSTACLE_TYPE obstacletype;
    public obstacle(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, int health, bool blastcandamage,
        bool tntcandamage, bool canfall, OBSTACLE_TYPE _obstacletype, Game gamevars)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, GRID_TYPE.OBSTACLE, gamevars)
    {
        this.health = health;
        this.blastcandamage = blastcandamage;
        this.tntcandamage = tntcandamage;
        this.canfall = canfall;
        this.obstacletype = _obstacletype;
    }
    public override void getdamage(Game gamevars, DAMAGE_TYPE dt)
    {
        if (dt == DAMAGE_TYPE.BLAST && this.blastcandamage)
        {
            this.health--;
            if (this.health <= 0)
            {
                gamevars.deletegridobj(this.column, this.row);
            }
        }
        else if (dt == DAMAGE_TYPE.TNT && this.tntcandamage)
        {
            this.health--;
            if (this.health <= 0)
            {
                gamevars.deletegridobj(this.column, this.row);
            }
        }
        if (this.health == 1 && obstacletype == OBSTACLE_TYPE.VASE)
        {
            this.gobj.ChangeSprite(gamevars.vase2sprite);
        }
        playparticles(gamevars);
    }
    public override void playparticles(Game gamevars)
    {
        ParticleSystem particleSystemTemplate = gamevars.baseparticles.GetComponent<ParticleSystem>();
        if (particleSystemTemplate == null)
        {
            Debug.LogError("Please ensure particle system template is assigned.");
        }
        for (int i = 0; i < 3; i++)
        {
            ParticleSystem psInstance = GameObject.Instantiate(particleSystemTemplate,
                new Vector3(getcenter(gamevars).x - 960, getcenter(gamevars).y - 1706.6665f, -100), Quaternion.identity);
            psInstance.transform.SetParent(gamevars.canvas.transform, false);
            ParticleSystemRenderer psRenderer = psInstance.GetComponent<ParticleSystemRenderer>();
            if (i == 0)
            {
                if (this.obstacletype == OBSTACLE_TYPE.VASE)
                {
                    psRenderer.material = gamevars.particlevase1;
                }
                else if (this.obstacletype == OBSTACLE_TYPE.BOX)
                {
                    psRenderer.material = gamevars.particlebox1;
                }
                else if (this.obstacletype == OBSTACLE_TYPE.STONE)
                {
                    psRenderer.material = gamevars.particlestone1;
                }
            }
            else if (i == 1)
            {
                if (this.obstacletype == OBSTACLE_TYPE.VASE)
                {
                    psRenderer.material = gamevars.particlevase2;
                }
                else if (this.obstacletype == OBSTACLE_TYPE.BOX)
                {
                    psRenderer.material = gamevars.particlebox2;
                }
                else if (this.obstacletype == OBSTACLE_TYPE.STONE)
                {
                    psRenderer.material = gamevars.particlestone2;
                }
            }
            else if (i == 2)
            {
                if (this.obstacletype == OBSTACLE_TYPE.VASE)
                {
                    psRenderer.material = gamevars.particlevase3;
                }
                else if (this.obstacletype == OBSTACLE_TYPE.BOX)
                {
                    psRenderer.material = gamevars.particlebox3;
                }
                else if (this.obstacletype == OBSTACLE_TYPE.STONE)
                {
                    psRenderer.material = gamevars.particlestone3;
                }
            }
            EmissionModule emissionModule = psInstance.emission;
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(5);
            psInstance.Play();
        }
    }
}
public class stone : obstacle
{
    public stone(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, Game gamevars)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, 1, false, true, false, OBSTACLE_TYPE.STONE, gamevars)
    {
    }
}
public class box : obstacle
{
    public box(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, Game gamevars)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, 1, true, true, false, OBSTACLE_TYPE.BOX, gamevars)
    {
    }
}
public class vase : obstacle
{
    public vase(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, Game gamevars)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, 2, true, true, true, OBSTACLE_TYPE.VASE, gamevars)
    {
    }
}
public class tnt : gridobj
{
    public tnt(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, Game gamevars)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, GRID_TYPE.TNT, gamevars)
    {
    }
    public override void tap(Game gamevars)
    {
        if (!gamevars.won && !gamevars.lost)
        {
            gamevars.currmovecount--;
            gamevars.deletegridobj(column, row);
            List<tnt> combo = new List<tnt>();
            combo.Add(this);
            getcombo(combo, gamevars);
            playparticles(gamevars);
            if (combo.Count > 1)
            {
                foreach (tnt t in combo)
                {
                    //remove the one adjacent tnt that creates combo
                    if ((this.column == t.column + 1 && this.row == t.row) ||
                        (this.column == t.column - 1 && this.row == t.row) ||
                        (this.column == t.column && this.row == t.row + 1) ||
                        (this.column == t.column && this.row == t.row - 1))
                    {
                        gamevars.deletegridobj(t.column, t.row);
                        combo.Remove(t);
                        break;
                    }
                }
                explode(gamevars, 3);
            }
            else
            {
                explode(gamevars, 2);
            }
            gamevars.updatemovecount();
            gamevars.updatemap();
            gamevars.updategoals();
            gamevars.checkwinlose();
        }
    }
    private void explode(Game gamevars, int range)
    {
        for (int col = this.column - range; col <= this.column + range; col++)
        {
            for (int row = this.row - range; row <= this.row + range; row++)
            {
                gridobj grid = gamevars.getgridobj(col, row);
                if (grid != null && grid != this)
                {
                    grid.getdamage(gamevars, DAMAGE_TYPE.TNT);
                }
            }
        }
    }
    public override void getdamage(Game gamevars, DAMAGE_TYPE dt)
    {
        gamevars.deletegridobj(column, row);
        explode(gamevars, 2);
        playparticles(gamevars);
    }
    private void getcombo(List<tnt> combo, Game gamevars)
    {
        void recursivecalculate(int searchcolumn, int searchrow)
        {
            gridobj xgrid = gamevars.getgridobj(searchcolumn, searchrow);
            if (xgrid != null && xgrid.gridtype == gridobj.GRID_TYPE.TNT)
            {
                tnt xcube = (tnt)xgrid;
                if (!combo.Contains(xcube))
                {
                    combo.Add(xcube);
                    getcombo(combo, gamevars);
                }
            }
        }
        int currentcolumn = combo.Last().column;
        int currentrow = combo.Last().row;
        recursivecalculate(currentcolumn - 1, currentrow);
        recursivecalculate(currentcolumn + 1, currentrow);
        recursivecalculate(currentcolumn, currentrow - 1);
        recursivecalculate(currentcolumn, currentrow + 1);
    }
    public override void playparticles(Game gamevars)
    {
        ParticleSystem particleSystemTemplate = gamevars.tntparticles.GetComponent<ParticleSystem>();
        if (particleSystemTemplate == null)
        {
            Debug.LogError("Please ensure particle system template is assigned.");
        }
        for (int i = 0; i < 2; i++)
        {
            ParticleSystem psInstance = GameObject.Instantiate(particleSystemTemplate,
                new Vector3(getcenter(gamevars).x - 960, getcenter(gamevars).y - 1706.6665f, -100), Quaternion.identity);
            psInstance.transform.SetParent(gamevars.canvas.transform, false);
            ParticleSystemRenderer psRenderer = psInstance.GetComponent<ParticleSystemRenderer>();
            if (i == 0)
            {
                psRenderer.material = gamevars.particletnt1;
            }
            else if (i == 1)
            {
                psRenderer.material = gamevars.particletnt2;
            }
            EmissionModule emissionModule = psInstance.emission;
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(100);
            psInstance.Play();
        }
    }
}
public class cube : gridobj
{
    public enum CUBE_TYPE
    {
        RED,
        BLUE,
        GREEN,
        YELLOW
    };
    public bool tntstate;
    public CUBE_TYPE cubetype;
    public cube(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, CUBE_TYPE _cubetype, Game gamevars)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, GRID_TYPE.CUBE, gamevars)
    {
        this.tntstate = false;
        this.cubetype = _cubetype;
    }
    public override void tap(Game gamevars)
    {
        if (!gamevars.won && !gamevars.lost)
        {
            List<cube> blastcubes = new List<cube>();
            blastcubes.Add(this);
            recursivecontrol(blastcubes, gamevars);
            if (blastcubes.Count() >= 2)
            {
                blastobstacles(blastcubes, gamevars);
                gamevars.currmovecount--;
                for (int i = 0; i < blastcubes.Count(); i++)
                {
                    blastcubes[i].playparticles(gamevars);
                    gamevars.deletegridobj(blastcubes[i].column, blastcubes[i].row);
                }
                if (blastcubes.Count() >= 5)
                {
                    gamevars.addtnt(this.column, this.row);
                }
                gamevars.updatemovecount();
            }
            gamevars.updatemap();
            gamevars.updategoals();
            gamevars.checkwinlose();
        }
    }
    private void blastobstacles(List<cube> blastcubes, Game gamevars)
    {
        List<obstacle> obstacles = new List<obstacle>();
        for (int j = 0; j < gamevars.gamemap.Count(); j++)
        {
            if (gamevars.gamemap[j].gridtype == gridobj.GRID_TYPE.OBSTACLE)
            {
                obstacle obs = (obstacle)gamevars.gamemap[j];
                for (int i = 0; i < blastcubes.Count(); i++)
                {
                    if ((obs.column == blastcubes[i].column + 1 && obs.row == blastcubes[i].row) ||
                        (obs.column == blastcubes[i].column - 1 && obs.row == blastcubes[i].row) ||
                        (obs.column == blastcubes[i].column && obs.row == blastcubes[i].row + 1) ||
                        (obs.column == blastcubes[i].column && obs.row == blastcubes[i].row - 1))
                    {
                        obstacles.Add(obs);
                        break;
                    }
                }
            }
        }
        foreach (obstacle obs in obstacles)
        {
            obs.getdamage(gamevars, DAMAGE_TYPE.BLAST);
        }
    }
    private void recursivecontrol(List<cube> blastcubes, Game gamevars)
    {
        void recursivecalculate(int searchcolumn, int searchrow)
        {
            gridobj xgrid = gamevars.getgridobj(searchcolumn, searchrow);
            if (xgrid != null && xgrid.gridtype == gridobj.GRID_TYPE.CUBE)
            {
                cube xcube = (cube)xgrid;
                if (blastcubes.Last().cubetype == xcube.cubetype && !blastcubes.Contains(xcube))
                {
                    blastcubes.Add(xcube);
                    recursivecontrol(blastcubes, gamevars);
                }
            }
        }
        int currentcolumn = blastcubes.Last().column;
        int currentrow = blastcubes.Last().row;
        recursivecalculate(currentcolumn - 1, currentrow);
        recursivecalculate(currentcolumn + 1, currentrow);
        recursivecalculate(currentcolumn, currentrow - 1);
        recursivecalculate(currentcolumn, currentrow + 1);
    }
    public List<cube> tntgroup(Game gamevars)
    {
        List<cube> blastcubes = new List<cube>();
        blastcubes.Add(this);
        recursivecontrol(blastcubes, gamevars);
        if (blastcubes.Count >= 5)
        {
            return blastcubes;
        }
        return null;
    }
    public override void getdamage(Game gamevars, DAMAGE_TYPE dt)
    {
        gamevars.deletegridobj(this.column, this.row);
        playparticles(gamevars);
    }
    public override void playparticles(Game gamevars)
    {
        ParticleSystem particleSystemTemplate = gamevars.baseparticles.GetComponent<ParticleSystem>();
        if (particleSystemTemplate == null)
        {
            Debug.LogError("Please ensure particle system template is assigned.");
        }
        ParticleSystem psInstance = GameObject.Instantiate(particleSystemTemplate,
            new Vector3(getcenter(gamevars).x - 960, getcenter(gamevars).y - 1706.6665f, -100), Quaternion.identity);
        psInstance.transform.SetParent(gamevars.canvas.transform, false);
        ParticleSystemRenderer psRenderer = psInstance.GetComponent<ParticleSystemRenderer>();
        if (this.cubetype == cube.CUBE_TYPE.BLUE)
        {
            psRenderer.material = gamevars.particleblue;
        }
        else if (this.cubetype == cube.CUBE_TYPE.RED)
        {
            psRenderer.material = gamevars.particlered;
        }
        else if (this.cubetype == cube.CUBE_TYPE.GREEN)
        {
            psRenderer.material = gamevars.particlegreen;
        }
        else if (this.cubetype == cube.CUBE_TYPE.YELLOW)
        {
            psRenderer.material = gamevars.particleyellow;
        }
        psInstance.Play();
    }
}
