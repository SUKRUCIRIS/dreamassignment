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

public class Game : MonoBehaviour
{
    public const float gridwidth = 160;
    public float gridstartx = 0;
    public float gridstarty = 0;
    public int currmovecount = 40;
    public Canvas canvas;
    public Sprite tntsprite;
    public Sprite blockred, blockgreen, blockblue,
        blockyellow, blockredtnt, blockgreentnt, blockbluetnt, blockyellowtnt;
    public Sprite vase1sprite, vase2sprite, stonesprite, boxsprite;
    public level currentlevel;
    public List<gridobj> gamemap = new List<gridobj>();
    public List<List<bool>> emptymap = new List<List<bool>>();
    public List<gameobj> goals = new List<gameobj>();
    public int levelgoalstone = 0, levelgoalvase = 0, levelgoalbox = 0, levelgoaltype = 0;
    public int currentgoalstone = 0, currentgoalvase = 0, currentgoalbox = 0;
    public GameObject goal1state, goal2state, goal3state, goal1state_goal1, goal1state_goal1text,
        goal1state_goal1tick, goal2state_goal1, goal2state_goal1text, goal2state_goal1tick,
        goal2state_goal2, goal2state_goal2text, goal2state_goal2tick, goal3state_goal1text,
        goal3state_goal1tick, goal3state_goal2text, goal3state_goal2tick, goal3state_goal3text,
        goal3state_goal3tick;
    public Material particlered, particlegreen, particleblue, particleyellow, particlebox1, particlebox2, particlebox3,
        particlestone1, particlestone2, particlestone3, particlevase1, particlevase2, particlevase3, particletnt1, particletnt2;
    public GameObject baseparticles, tntparticles;
    public GameObject winpanel, winstarcanvas;
    public GameObject starparticle;
    public bool won = false, lost = false;

    public void checkwinlose()
    {
        if (currmovecount <= 0 && (currentgoalstone != 0 || currentgoalvase != 0 || currentgoalbox != 0))
        {
            lost = true;
        }
        else if (currentgoalstone == 0 && currentgoalvase == 0 && currentgoalbox == 0)
        {
            if (PlayerPrefs.GetInt("level", 1) <= 10)
            {
                PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1) + 1);
                PlayerPrefs.Save();
            }
            won = true;
            winpanel.SetActive(true);
            winstarcanvas.SetActive(true);
        }
    }
    public void updategoals()
    {
        currentgoalstone = 0;
        currentgoalvase = 0;
        currentgoalbox = 0;
        foreach (gridobj grid in gamemap)
        {
            if (grid.gridtype == gridobj.GRID_TYPE.OBSTACLE)
            {
                obstacle obs = (obstacle)grid;
                if (obs.obstacletype == obstacle.OBSTACLE_TYPE.VASE)
                {
                    currentgoalvase++;
                }
                else if (obs.obstacletype == obstacle.OBSTACLE_TYPE.BOX)
                {
                    currentgoalbox++;
                }
                else if (obs.obstacletype == obstacle.OBSTACLE_TYPE.STONE)
                {
                    currentgoalstone++;
                }
            }
        }
        if (levelgoaltype == 1)
        {
            if (goal1state_goal1text != null && goal1state_goal1tick != null)
            {
                if (levelgoalbox > 0)
                {
                    if (currentgoalbox > 0)
                    {
                        goal1state_goal1text.GetComponent<TextMeshProUGUI>().text = currentgoalbox.ToString();
                    }
                    else
                    {
                        goal1state_goal1text.SetActive(false);

                        goal1state_goal1tick.SetActive(true);
                    }
                }
                else if (levelgoalstone > 0)
                {
                    if (currentgoalstone > 0)
                    {
                        goal1state_goal1text.GetComponent<TextMeshProUGUI>().text = currentgoalstone.ToString();
                    }
                    else
                    {
                        goal1state_goal1text.SetActive(false);
                        goal1state_goal1tick.SetActive(true);
                    }
                }
                else if (levelgoalvase > 0)
                {
                    if (currentgoalvase > 0)
                    {
                        goal1state_goal1text.GetComponent<TextMeshProUGUI>().text = currentgoalvase.ToString();
                    }
                    else
                    {
                        goal1state_goal1text.SetActive(false);
                        goal1state_goal1tick.SetActive(true);
                    }
                }
            }
        }
        else if (levelgoaltype == 2)
        {
            if (goal2state_goal1text != null && goal2state_goal1tick != null && goal2state_goal2text != null && goal2state_goal2tick != null)
            {
                if (levelgoalbox > 0 && levelgoalstone > 0)
                {
                    if (currentgoalbox > 0)
                    {
                        goal2state_goal1text.GetComponent<TextMeshProUGUI>().text = currentgoalbox.ToString();
                    }
                    else
                    {
                        goal2state_goal1text.SetActive(false);
                        goal2state_goal1tick.SetActive(true);
                    }
                    if (currentgoalstone > 0)
                    {
                        goal2state_goal2text.GetComponent<TextMeshProUGUI>().text = currentgoalstone.ToString();
                    }
                    else
                    {
                        goal2state_goal2text.SetActive(false);
                        goal2state_goal2tick.SetActive(true);
                    }
                }
                else if (levelgoalbox > 0 && levelgoalvase > 0)
                {
                    if (currentgoalbox > 0)
                    {
                        goal2state_goal1text.GetComponent<TextMeshProUGUI>().text = currentgoalbox.ToString();
                    }
                    else
                    {
                        goal2state_goal1text.SetActive(false);
                        goal2state_goal1tick.SetActive(true);
                    }
                    if (currentgoalvase > 0)
                    {
                        goal2state_goal2text.GetComponent<TextMeshProUGUI>().text = currentgoalvase.ToString();
                    }
                    else
                    {
                        goal2state_goal2text.SetActive(false);
                        goal2state_goal2tick.SetActive(true);
                    }
                }
                else if (levelgoalstone > 0 && levelgoalvase > 0)
                {
                    if (currentgoalstone > 0)
                    {
                        goal2state_goal1text.GetComponent<TextMeshProUGUI>().text = currentgoalstone.ToString();
                    }
                    else
                    {
                        goal2state_goal1text.SetActive(false);
                        goal2state_goal1tick.SetActive(true);
                    }
                    if (currentgoalvase > 0)
                    {
                        goal2state_goal2text.GetComponent<TextMeshProUGUI>().text = currentgoalvase.ToString();
                    }
                    else
                    {
                        goal2state_goal2text.SetActive(false);
                        goal2state_goal2tick.SetActive(true);
                    }
                }
            }
        }
        else if (levelgoaltype == 3)
        {
            if (goal3state_goal1text != null && goal3state_goal1tick != null)
            {
                if (currentgoalvase > 0)
                {
                    goal3state_goal1text.GetComponent<TextMeshProUGUI>().text = currentgoalvase.ToString();
                }
                else
                {
                    goal3state_goal1text.SetActive(false);
                    goal3state_goal1tick.SetActive(true);
                }
            }
            if (goal3state_goal2text != null && goal3state_goal2tick != null)
            {
                if (currentgoalbox > 0)
                {
                    goal3state_goal2text.GetComponent<TextMeshProUGUI>().text = currentgoalbox.ToString();
                }
                else
                {
                    goal3state_goal2text.SetActive(false);
                    goal3state_goal2tick.SetActive(true);
                }
            }
            if (goal3state_goal3text != null && goal3state_goal3tick != null)
            {
                if (currentgoalstone > 0)
                {
                    goal3state_goal3text.GetComponent<TextMeshProUGUI>().text = currentgoalstone.ToString();
                }
                else
                {
                    goal3state_goal3text.SetActive(false);
                    goal3state_goal3tick.SetActive(true);
                }
            }
        }
    }
    public bool existemptygrid()
    {
        for (int i = 0; i < emptymap.Count; i++)
        {
            for (int j = 0; j < emptymap[i].Count; j++)
            {
                if (emptymap[i][j])
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void updatemap()
    {
        int maxloop = 0;
        while (existemptygrid() && maxloop < 10)
        {
            for (int i = 0; i < gamemap.Count; i++)
            {
                if (gamemap[i].gridtype != gridobj.GRID_TYPE.OBSTACLE || ((obstacle)gamemap[i]).obstacletype == obstacle.OBSTACLE_TYPE.VASE)
                {
                    while (gamemap[i].row + 1 < currentlevel.grid_height && emptymap[gamemap[i].row + 1][gamemap[i].column])
                    {
                        gamemap[i].move(gamemap[i].column, gamemap[i].row + 1, 0.5f, gridstartx, gridstarty, gridwidth);
                        emptymap[gamemap[i].row][gamemap[i].column] = false;
                        if (gamemap[i].row - 1 >= 0)
                        {
                            emptymap[gamemap[i].row - 1][gamemap[i].column] = true;
                        }
                    }
                }
            }
            for (int i = 0; i < emptymap[0].Count; i++)
            {
                if (emptymap[0][i])
                {
                    addrandcube(i, -1);
                    gamemap.Last().move(i, 0, 0.5f, gridstartx, gridstarty, gridwidth);
                    emptymap[0][i] = false;
                }
            }
            maxloop++;
        }
        List<List<cube>> tntgroups = new List<List<cube>>();
        for (int i = 0; i < gamemap.Count; i++)
        {
            if (gamemap[i].gridtype == gridobj.GRID_TYPE.CUBE)
            {
                cube cubex = (cube)gamemap[i];
                var group = cubex.tntgroup(this);
                if (group != null)
                {
                    bool found = false;
                    foreach (var c in tntgroups)
                    {
                        if (c.Contains(group[0]))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        tntgroups.Add(group);
                    }
                }
            }
        }
        for (int i = 0; i < gamemap.Count; i++)
        {
            if (gamemap[i].gridtype == gridobj.GRID_TYPE.CUBE)
            {
                cube cubex = (cube)gamemap[i];
                cubex.tntstate = false;
                foreach (var c in tntgroups)
                {
                    if (c.Contains(cubex))
                    {
                        cubex.tntstate = true;
                    }
                }
                if (cubex.tntstate)
                {
                    if (cubex.cubetype == cube.CUBE_TYPE.BLUE)
                    {
                        cubex.gobj.ChangeSprite(blockbluetnt);
                    }
                    else if (cubex.cubetype == cube.CUBE_TYPE.RED)
                    {
                        cubex.gobj.ChangeSprite(blockredtnt);
                    }
                    else if (cubex.cubetype == cube.CUBE_TYPE.GREEN)
                    {
                        cubex.gobj.ChangeSprite(blockgreentnt);
                    }
                    else if (cubex.cubetype == cube.CUBE_TYPE.YELLOW)
                    {
                        cubex.gobj.ChangeSprite(blockyellowtnt);
                    }
                    UnityEngine.Color imagecolor = cubex.gobj.obj.GetComponent<UnityEngine.UI.Image>().color;
                    imagecolor.r = 1;
                    imagecolor.g = 1;
                    imagecolor.b = 1;
                    cubex.gobj.obj.GetComponent<UnityEngine.UI.Image>().color = imagecolor;
                }
                else
                {
                    if (cubex.cubetype == cube.CUBE_TYPE.BLUE)
                    {
                        cubex.gobj.ChangeSprite(blockblue);
                    }
                    else if (cubex.cubetype == cube.CUBE_TYPE.RED)
                    {
                        cubex.gobj.ChangeSprite(blockred);
                    }
                    else if (cubex.cubetype == cube.CUBE_TYPE.GREEN)
                    {
                        cubex.gobj.ChangeSprite(blockgreen);
                    }
                    else if (cubex.cubetype == cube.CUBE_TYPE.YELLOW)
                    {
                        cubex.gobj.ChangeSprite(blockyellow);
                    }
                    UnityEngine.Color imagecolor = cubex.gobj.obj.GetComponent<UnityEngine.UI.Image>().color;
                    if (tntgroups.Count > 0)
                    {
                        imagecolor.r = 0.7f;
                        imagecolor.g = 0.7f;
                        imagecolor.b = 0.7f;
                    }
                    else
                    {
                        imagecolor.r = 1;
                        imagecolor.g = 1;
                        imagecolor.b = 1;
                    }
                    cubex.gobj.obj.GetComponent<UnityEngine.UI.Image>().color = imagecolor;
                }
            }
        }
    }
    public void updatemovecount()
    {
        GameObject movecounttext = GameObject.FindWithTag("movecount");
        TextMeshProUGUI tmp = movecounttext.GetComponent<TextMeshProUGUI>();
        tmp.text = currmovecount.ToString();
    }
    public void addrandcube(int column, int row)
    {
        int r = Random.Range(0, 4);
        if (r == 0)
        {
            gamemap.Add(new cube(column, row, gridstartx, gridstarty, gridwidth, canvas, blockred, cube.CUBE_TYPE.RED, this));
        }
        else if (r == 1)
        {
            gamemap.Add(new cube(column, row, gridstartx, gridstarty, gridwidth, canvas, blockgreen, cube.CUBE_TYPE.GREEN, this));
        }
        else if (r == 2)
        {
            gamemap.Add(new cube(column, row, gridstartx, gridstarty, gridwidth, canvas, blockblue, cube.CUBE_TYPE.BLUE, this));
        }
        else if (r == 3)
        {
            gamemap.Add(new cube(column, row, gridstartx, gridstarty, gridwidth, canvas, blockyellow, cube.CUBE_TYPE.YELLOW, this));
        }
    }
    public void addtnt(int column, int row)
    {
        gamemap.Add(new tnt(column, row, gridstartx, gridstarty, gridwidth, canvas, tntsprite, this));
        emptymap[row][column] = false;
    }
    public gridobj getgridobj(int column, int row)
    {
        for (int i = 0; i < gamemap.Count; i++)
        {
            if (gamemap[i].row == row && gamemap[i].column == column) { return gamemap[i]; }
        }
        return null;
    }
    public void deletegridobj(int column, int row)
    {
        for (int i = 0; i < gamemap.Count; i++)
        {
            if (gamemap[i].row == row && gamemap[i].column == column)
            {
                gamemap[i].Destroy();
                gamemap[i] = null;
                gamemap.RemoveAt(i);
                emptymap[row][column] = true;
                break;
            }
        }
    }
    static Sprite loadsprite(string path)
    {
        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture != null)
        {
            // Create a new sprite from the texture
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        else
        {
            Debug.LogError("Image not found: " + path);
            return null;
        }
    }
    void loadlevel()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            //create event system if there is none
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        int level = PlayerPrefs.GetInt("level", 1);
        TextAsset jsonFile;
        if (level < 10)
        {
            jsonFile = Resources.Load<TextAsset>("Levels/level_0" + level.ToString());
        }
        else
        {
            jsonFile = Resources.Load<TextAsset>("Levels/level_10");
        }
        if (jsonFile != null)
        {
            currentlevel = JsonUtility.FromJson<level>(jsonFile.text);
            currmovecount = currentlevel.move_count;
        }
        else
        {
            Debug.LogError("Cannot find the JSON file!");
        }

        GameObject canvasObject = GameObject.FindWithTag("gamecanvas");
        canvas = canvasObject.GetComponent<Canvas>();

        blockblue = loadsprite("Cubes/DefaultState/blue");
        blockred = loadsprite("Cubes/DefaultState/red");
        blockgreen = loadsprite("Cubes/DefaultState/green");
        blockyellow = loadsprite("Cubes/DefaultState/yellow");

        blockbluetnt = loadsprite("Cubes/TntState/blue_tnt");
        blockredtnt = loadsprite("Cubes/TntState/red_tnt");
        blockgreentnt = loadsprite("Cubes/TntState/green_tnt");
        blockyellowtnt = loadsprite("Cubes/TntState/yellow_tnt");

        vase1sprite = loadsprite("Obstacles/Vase/vase_01");
        vase2sprite = loadsprite("Obstacles/Vase/vase_02");
        stonesprite = loadsprite("Obstacles/Stone/stone");
        boxsprite = loadsprite("Obstacles/Box/box");

        tntsprite = loadsprite("TNT/TNT");

        particleblue = Resources.Load<Material>("Cubes/Particles/bluecubep");
        particlered = Resources.Load<Material>("Cubes/Particles/redcubep");
        particlegreen = Resources.Load<Material>("Cubes/Particles/greencubep");
        particleyellow = Resources.Load<Material>("Cubes/Particles/yellowcubep");
        baseparticles = GameObject.FindWithTag("baseparticles");
        particlebox1 = Resources.Load<Material>("Obstacles/Box/Particles/particlebox1");
        particlebox2 = Resources.Load<Material>("Obstacles/Box/Particles/particlebox2");
        particlebox3 = Resources.Load<Material>("Obstacles/Box/Particles/particlebox3");
        particlestone1 = Resources.Load<Material>("Obstacles/Stone/Particles/particlestone1");
        particlestone2 = Resources.Load<Material>("Obstacles/Stone/Particles/particlestone2");
        particlestone3 = Resources.Load<Material>("Obstacles/Stone/Particles/particlestone3");
        particlevase1 = Resources.Load<Material>("Obstacles/Vase/Particles/particlevase1");
        particlevase2 = Resources.Load<Material>("Obstacles/Vase/Particles/particlevase2");
        particlevase3 = Resources.Load<Material>("Obstacles/Vase/Particles/particlevase3");
        tntparticles = GameObject.FindWithTag("tntparticles");
        particletnt1 = Resources.Load<Material>("TNT/Particles/particletnt1");
        particletnt2 = Resources.Load<Material>("TNT/Particles/particletnt2");

        winpanel = GameObject.FindWithTag("winpanel");
        winstarcanvas = GameObject.FindWithTag("winstarcanvas");
        starparticle = GameObject.FindWithTag("starparticle");
        winpanel.SetActive(false);
        winstarcanvas.SetActive(false);

        gridstartx = (1920 - (currentlevel.grid_width * gridwidth)) / 2.0f + gridwidth / 2.0f;
        gridstarty = 2303.333f - (2303.333f - (currentlevel.grid_height * gridwidth)) / 2.0f;

        {
            //resize background
            GameObject gridbg = GameObject.FindWithTag("gridbg");
            RectTransform rectTransform = gridbg.GetComponent<RectTransform>();
            float scaleh = 0.9f;
            float bordert = 80;
            rectTransform.sizeDelta = new Vector2(currentlevel.grid_width * gridwidth + bordert, currentlevel.grid_height * scaleh * gridwidth + bordert);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localPosition = new Vector2(gridstartx + rectTransform.sizeDelta.x / 2 - gridwidth / 2.0f - bordert / 2,
                gridstarty - rectTransform.sizeDelta.y / 2 + bordert / 2 + gridwidth / 2.0f) + new Vector2(-960, -1706.6665f);
        }
        {
            //change move count text
            GameObject movecounttext = GameObject.FindWithTag("movecount");
            TextMeshProUGUI tmp = movecounttext.GetComponent<TextMeshProUGUI>();
            tmp.text = currmovecount.ToString();
        }

        for (int row = currentlevel.grid_height - 1; row >= 0; row--)
        {
            emptymap.Add(new List<bool>());
        }
        for (int row = currentlevel.grid_height - 1; row >= 0; row--)
        {
            for (int col = 0; col < currentlevel.grid_width; col++)
            {
                int arrindex = (currentlevel.grid_width * currentlevel.grid_height - 1) - (row * currentlevel.grid_width + currentlevel.grid_width - col - 1);
                emptymap[row].Add(false);
                if (currentlevel.grid[arrindex] == "r")
                {
                    gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockred, cube.CUBE_TYPE.RED, this));
                }
                else if (currentlevel.grid[arrindex] == "g")
                {
                    gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockgreen, cube.CUBE_TYPE.GREEN, this));
                }
                else if (currentlevel.grid[arrindex] == "b")
                {
                    gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockblue, cube.CUBE_TYPE.BLUE, this));
                }
                else if (currentlevel.grid[arrindex] == "y")
                {
                    gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockyellow, cube.CUBE_TYPE.YELLOW, this));
                }
                else if (currentlevel.grid[arrindex] == "rand")
                {
                    int r = Random.Range(0, 4);
                    if (r == 0)
                    {
                        gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockred, cube.CUBE_TYPE.RED, this));
                    }
                    else if (r == 1)
                    {
                        gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockgreen, cube.CUBE_TYPE.GREEN, this));
                    }
                    else if (r == 2)
                    {
                        gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockblue, cube.CUBE_TYPE.BLUE, this));
                    }
                    else if (r == 3)
                    {
                        gamemap.Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockyellow, cube.CUBE_TYPE.YELLOW, this));
                    }
                }
                else if (currentlevel.grid[arrindex] == "t")
                {
                    gamemap.Add(new tnt(col, row, gridstartx, gridstarty, gridwidth, canvas, tntsprite, this));
                }
                else if (currentlevel.grid[arrindex] == "bo")
                {
                    gamemap.Add(new box(col, row, gridstartx, gridstarty, gridwidth, canvas, boxsprite, this));
                    levelgoalbox++;

                }
                else if (currentlevel.grid[arrindex] == "s")
                {
                    gamemap.Add(new stone(col, row, gridstartx, gridstarty, gridwidth, canvas, stonesprite, this));
                    levelgoalstone++;
                }
                else if (currentlevel.grid[arrindex] == "v")
                {
                    gamemap.Add(new vase(col, row, gridstartx, gridstarty, gridwidth, canvas, vase1sprite, this));
                    levelgoalvase++;
                }
            }
        }

        {
            // create goals
            currentgoalvase = levelgoalvase;
            currentgoalbox = levelgoalbox;
            currentgoalstone = levelgoalstone;
            levelgoaltype = 0;
            if (levelgoalvase > 0)
            {
                levelgoaltype++;
            }
            if (levelgoalbox > 0)
            {
                levelgoaltype++;
            }
            if (levelgoalstone > 0)
            {
                levelgoaltype++;
            }
            goal1state = GameObject.FindWithTag("goal1state");
            goal2state = GameObject.FindWithTag("goal2state");
            goal3state = GameObject.FindWithTag("goal3state");
            if (levelgoaltype == 1)
            {
                goal1state.SetActive(true);
                goal2state.SetActive(false);
                goal3state.SetActive(false);
                goal1state_goal1 = GameObject.FindWithTag("goal1state_goal1");
                goal1state_goal1text = GameObject.FindWithTag("goal1state_goal1text");
                goal1state_goal1tick = GameObject.FindWithTag("goal1state_goal1tick");
                goal1state_goal1tick.SetActive(false);

                if (levelgoalbox > 0)
                {
                    goal1state_goal1.GetComponent<UnityEngine.UI.Image>().sprite = boxsprite;
                    goal1state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalbox.ToString();
                }
                else if (levelgoalstone > 0)
                {
                    goal1state_goal1.GetComponent<UnityEngine.UI.Image>().sprite = stonesprite;
                    goal1state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalstone.ToString();
                }
                else if (levelgoalvase > 0)
                {
                    goal1state_goal1.GetComponent<UnityEngine.UI.Image>().sprite = vase1sprite;
                    goal1state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalvase.ToString();
                }
            }
            else if (levelgoaltype == 2)
            {
                goal1state.SetActive(false);
                goal2state.SetActive(true);
                goal3state.SetActive(false);
                goal2state_goal1 = GameObject.FindWithTag("goal2state_goal1");
                goal2state_goal1text = GameObject.FindWithTag("goal2state_goal1text");
                goal2state_goal1tick = GameObject.FindWithTag("goal2state_goal1tick");
                goal2state_goal1tick.SetActive(false);
                goal2state_goal2 = GameObject.FindWithTag("goal2state_goal2");
                goal2state_goal2text = GameObject.FindWithTag("goal2state_goal2text");
                goal2state_goal2tick = GameObject.FindWithTag("goal2state_goal2tick");
                goal2state_goal2tick.SetActive(false);

                if (levelgoalbox > 0 && levelgoalstone > 0)
                {
                    goal2state_goal1.GetComponent<UnityEngine.UI.Image>().sprite = boxsprite;
                    goal2state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalbox.ToString();

                    goal2state_goal2.GetComponent<UnityEngine.UI.Image>().sprite = stonesprite;
                    goal2state_goal2text.GetComponent<TextMeshProUGUI>().text = levelgoalstone.ToString();
                }
                else if (levelgoalbox > 0 && levelgoalvase > 0)
                {
                    goal2state_goal1.GetComponent<UnityEngine.UI.Image>().sprite = boxsprite;
                    goal2state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalbox.ToString();

                    goal2state_goal2.GetComponent<UnityEngine.UI.Image>().sprite = vase1sprite;
                    goal2state_goal2text.GetComponent<TextMeshProUGUI>().text = levelgoalvase.ToString();
                }
                else if (levelgoalstone > 0 && levelgoalvase > 0)
                {
                    goal2state_goal1.GetComponent<UnityEngine.UI.Image>().sprite = stonesprite;
                    goal2state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalstone.ToString();

                    goal2state_goal2.GetComponent<UnityEngine.UI.Image>().sprite = vase1sprite;
                    goal2state_goal2text.GetComponent<TextMeshProUGUI>().text = levelgoalvase.ToString();
                }
            }
            else if (levelgoaltype == 3)
            {
                goal1state.SetActive(false);
                goal2state.SetActive(false);
                goal3state.SetActive(true);
                goal3state_goal1text = GameObject.FindWithTag("goal3state_goal1text");
                goal3state_goal1tick = GameObject.FindWithTag("goal3state_goal1tick");
                goal3state_goal1tick.SetActive(false);
                goal3state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalvase.ToString();
                goal3state_goal2text = GameObject.FindWithTag("goal3state_goal2text");
                goal3state_goal2tick = GameObject.FindWithTag("goal3state_goal2tick");
                goal3state_goal2tick.SetActive(false);
                goal3state_goal2text.GetComponent<TextMeshProUGUI>().text = levelgoalbox.ToString();
                goal3state_goal3text = GameObject.FindWithTag("goal3state_goal3text");
                goal3state_goal3tick = GameObject.FindWithTag("goal3state_goal3tick");
                goal3state_goal3tick.SetActive(false);
                goal3state_goal3text.GetComponent<TextMeshProUGUI>().text = levelgoalstone.ToString();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        loadlevel();
        updatemap();
    }

    // Update is called once per frame
    void Update()
    {
        if (won)
        {
            if (!starparticle.GetComponent<ParticleSystem>().isPlaying)
            {
                SceneManager.LoadScene("mainmenu");
            }
        }
        else if (lost)
        {
            SceneManager.LoadScene("mainmenu");
        }
    }
}
