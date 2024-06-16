using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class level
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;
}
public class gameobj
{
    public Sprite sprite;
    public Vector2 position;
    public Vector2 size;
    public Canvas canvas;
    public GameObject obj;
    Coroutine currco = null;

    public gameobj(Sprite sprite, Vector2 position, Canvas canvas, Vector2 size)
    {
        this.sprite = sprite;
        this.position = position;
        this.canvas = canvas;
        this.size = size;

        obj = new GameObject("gameobj");
        obj.transform.SetParent(canvas.transform);

        UnityEngine.UI.Image image = obj.AddComponent<UnityEngine.UI.Image>();
        image.sprite = sprite;
        image.raycastTarget = true;

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = size;

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localPosition = position + new Vector2(-960, -1706.6665f);
    }

    private IEnumerator MoveObjectCoroutine(Vector2 targetPosition, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (obj == null)
            {
                yield break;
            }
            obj.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (obj != null)
        {
            obj.transform.position = targetPosition;
        }
    }

    public void ChangeSprite(Sprite newSprite)
    {
        this.sprite = newSprite;

        if (obj != null)
        {
            UnityEngine.UI.Image image = obj.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                image.sprite = newSprite;
            }
        }
    }

    public void Move(Vector2 targetPosition, float duration)
    {
        if (currco != null)
        {
            CoroutineRunner.instance.StopCoroutine(currco);
        }
        currco = CoroutineRunner.instance.StartCoroutine(MoveObjectCoroutine(targetPosition, duration));
    }

    public void Destroy()
    {
        if (obj != null)
        {
            GameObject.Destroy(obj);
            obj = null;
        }
    }
    ~gameobj()
    {
        Destroy();
    }
}

public class gridobj
{
    public gameobj gobj;
    public int column;
    public int row;
    public float scaleh = 0.9f;
    public enum GRID_TYPE
    {
        OBSTACLE,
        TNT,
        CUBE
    }
    public GRID_TYPE gridtype;
    public gridobj(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, GRID_TYPE _gridtype, List<List<gridobj>> gamemap)
    {
        this.column = column;
        this.row = row;
        this.gobj = new gameobj(sprite, new Vector2(gridstartx + gridwidth * column, gridstarty - (gridwidth * scaleh) * row), canvas, new Vector2(gridwidth, gridwidth));
        this.gridtype = _gridtype;

        EventTrigger trigger = gobj.obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { tap(gamemap); });
        trigger.triggers.Add(entry);
    }
    public void Destroy()
    {
        this.gobj.Destroy();
    }
    ~gridobj()
    {
        Destroy();
    }
    public void move(int targetcolumn, int targetrow, float duration, float gridstartx, float gridstarty, float gridwidth)
    {
        this.column = targetcolumn;
        this.row = targetrow;
        this.gobj.Move(new Vector2(gridstartx + gridwidth * column, gridstarty - (gridwidth * scaleh) * row), duration);
    }
    public virtual void tap(List<List<gridobj>> gamemap)
    {
    }
}
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
        bool tntcandamage, bool canfall, OBSTACLE_TYPE _obstacletype, List<List<gridobj>> gamemap)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, GRID_TYPE.OBSTACLE, gamemap)
    {
        this.health = health;
        this.blastcandamage = blastcandamage;
        this.tntcandamage = tntcandamage;
        this.canfall = canfall;
        this.obstacletype = _obstacletype;
    }
}
public class stone : obstacle
{
    public stone(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, List<List<gridobj>> gamemap)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, 1, false, true, false, OBSTACLE_TYPE.STONE, gamemap)
    {
    }
}
public class box : obstacle
{
    public box(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, List<List<gridobj>> gamemap)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, 1, true, true, false, OBSTACLE_TYPE.BOX, gamemap)
    {
    }
}
public class vase : obstacle
{
    public vase(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, List<List<gridobj>> gamemap)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, 2, true, true, true, OBSTACLE_TYPE.VASE, gamemap)
    {
    }
}
public class tnt : gridobj
{
    public tnt(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, List<List<gridobj>> gamemap)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, GRID_TYPE.TNT, gamemap)
    {
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
    public cube(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, CUBE_TYPE _cubetype, List<List<gridobj>> gamemap)
        : base(column, row, gridstartx, gridstarty, gridwidth, canvas, sprite, GRID_TYPE.CUBE, gamemap)
    {
        this.tntstate = false;
        this.cubetype = _cubetype;
    }
}

public class Game : MonoBehaviour
{
    private const float gridwidth = 160;
    private float gridstartx = 0;
    private float gridstarty = 0;
    private int currmovecount = 40;
    private Canvas canvas;
    private Sprite tntsprite;
    private Sprite blockred, blockgreen, blockblue,
        blockyellow, blockredtnt, blockgreentnt, blockbluetnt, blockyellowtnt;
    private Sprite vase1sprite, vase2sprite, stonesprite, boxsprite;
    private level currentlevel;
    private List<List<gridobj>> gamemap = new List<List<gridobj>>();
    private List<gameobj> goals = new List<gameobj>();
    private int levelgoalstone = 0, levelgoalvase = 0, levelgoalbox = 0, levelgoaltype = 0;
    private int currentgoalstone = 0, currentgoalvase = 0, currentgoalbox = 0;

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
            gamemap.Add(new List<gridobj>());
        }
        for (int row = currentlevel.grid_height - 1; row >= 0; row--)
        {
            for (int col = 0; col < currentlevel.grid_width; col++)
            {
                int arrindex = (currentlevel.grid_width * currentlevel.grid_height - 1) - (row * currentlevel.grid_width + col);
                if (currentlevel.grid[arrindex] == "r")
                {
                    gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockred, cube.CUBE_TYPE.RED, gamemap));
                }
                else if (currentlevel.grid[arrindex] == "g")
                {
                    gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockgreen, cube.CUBE_TYPE.GREEN, gamemap));
                }
                else if (currentlevel.grid[arrindex] == "b")
                {
                    gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockblue, cube.CUBE_TYPE.BLUE, gamemap));
                }
                else if (currentlevel.grid[arrindex] == "y")
                {
                    gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockyellow, cube.CUBE_TYPE.YELLOW, gamemap));
                }
                else if (currentlevel.grid[arrindex] == "rand")
                {
                    int r = Random.Range(0, 4);
                    if (r == 0)
                    {
                        gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockred, cube.CUBE_TYPE.RED, gamemap));
                    }
                    else if (r == 1)
                    {
                        gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockgreen, cube.CUBE_TYPE.GREEN, gamemap));
                    }
                    else if (r == 2)
                    {
                        gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockblue, cube.CUBE_TYPE.BLUE, gamemap));
                    }
                    else if (r == 3)
                    {
                        gamemap[row].Add(new cube(col, row, gridstartx, gridstarty, gridwidth, canvas, blockyellow, cube.CUBE_TYPE.YELLOW, gamemap));
                    }
                }
                else if (currentlevel.grid[arrindex] == "t")
                {
                    gamemap[row].Add(new tnt(col, row, gridstartx, gridstarty, gridwidth, canvas, tntsprite, gamemap));
                }
                else if (currentlevel.grid[arrindex] == "bo")
                {
                    gamemap[row].Add(new box(col, row, gridstartx, gridstarty, gridwidth, canvas, boxsprite, gamemap));
                    levelgoalbox++;

                }
                else if (currentlevel.grid[arrindex] == "s")
                {
                    gamemap[row].Add(new stone(col, row, gridstartx, gridstarty, gridwidth, canvas, stonesprite, gamemap));
                    levelgoalstone++;
                }
                else if (currentlevel.grid[arrindex] == "v")
                {
                    gamemap[row].Add(new vase(col, row, gridstartx, gridstarty, gridwidth, canvas, vase1sprite, gamemap));
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
            GameObject goal1state = GameObject.FindWithTag("goal1state");
            GameObject goal2state = GameObject.FindWithTag("goal2state");
            GameObject goal3state = GameObject.FindWithTag("goal3state");
            if (levelgoaltype == 1)
            {
                goal1state.SetActive(true);
                goal2state.SetActive(false);
                goal3state.SetActive(false);
                GameObject goal1state_goal1 = GameObject.FindWithTag("goal1state_goal1");
                GameObject goal1state_goal1text = GameObject.FindWithTag("goal1state_goal1text");
                GameObject goal1state_goal1tick = GameObject.FindWithTag("goal1state_goal1tick");
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
                GameObject goal2state_goal1 = GameObject.FindWithTag("goal2state_goal1");
                GameObject goal2state_goal1text = GameObject.FindWithTag("goal2state_goal1text");
                GameObject goal2state_goal1tick = GameObject.FindWithTag("goal2state_goal1tick");
                goal2state_goal1tick.SetActive(false);
                GameObject goal2state_goal2 = GameObject.FindWithTag("goal2state_goal2");
                GameObject goal2state_goal2text = GameObject.FindWithTag("goal2state_goal2text");
                GameObject goal2state_goal2tick = GameObject.FindWithTag("goal2state_goal2tick");
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
                GameObject goal3state_goal1text = GameObject.FindWithTag("goal3state_goal1text");
                GameObject goal3state_goal1tick = GameObject.FindWithTag("goal3state_goal1tick");
                goal3state_goal1tick.SetActive(false);
                goal3state_goal1text.GetComponent<TextMeshProUGUI>().text = levelgoalvase.ToString();
                GameObject goal3state_goal2text = GameObject.FindWithTag("goal3state_goal2text");
                GameObject goal3state_goal2tick = GameObject.FindWithTag("goal3state_goal2tick");
                goal3state_goal2tick.SetActive(false);
                goal3state_goal2text.GetComponent<TextMeshProUGUI>().text = levelgoalbox.ToString();
                GameObject goal3state_goal3text = GameObject.FindWithTag("goal3state_goal3text");
                GameObject goal3state_goal3tick = GameObject.FindWithTag("goal3state_goal3tick");
                goal3state_goal3tick.SetActive(false);
                goal3state_goal3text.GetComponent<TextMeshProUGUI>().text = levelgoalstone.ToString();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        loadlevel();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
