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
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.sizeDelta = size;

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localPosition = position + new Vector2(-960, -1706.6665f);
    }

    private IEnumerator MoveObjectCoroutine(Vector2 endposition, float duration)
    {
        Vector2 startPosition = new Vector2(obj.transform.localPosition.x, obj.transform.localPosition.y);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (obj == null)
            {
                yield break;
            }
            obj.transform.localPosition = Vector2.Lerp(startPosition, endposition + new Vector2(-960, -1706.6665f), elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (obj != null)
        {
            obj.transform.localPosition = endposition + new Vector2(-960, -1706.6665f);
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

    public void Move(Vector2 endposition, float duration)
    {
        if (currco != null)
        {
            CoroutineRunner.instance.StopCoroutine(currco);
        }
        currco = CoroutineRunner.instance.StartCoroutine(MoveObjectCoroutine(endposition, duration));
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
    public enum DAMAGE_TYPE
    {
        BLAST,
        TNT
    }
    public gridobj(int column, int row, float gridstartx, float gridstarty, float gridwidth, Canvas canvas, Sprite sprite, GRID_TYPE _gridtype, Game gamevars)
    {
        this.column = column;
        this.row = row;
        this.gobj = new gameobj(sprite, new Vector2(gridstartx + gridwidth * column, gridstarty - (gridwidth * scaleh) * row), canvas, new Vector2(gridwidth, gridwidth));
        this.gridtype = _gridtype;

        EventTrigger trigger = gobj.obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { tap(gamevars); });
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
    public virtual void tap(Game gamevars)
    {
    }
    public virtual void getdamage(Game gamevars, DAMAGE_TYPE dt)
    {
    }
    public virtual void playparticles(Game gamevars)
    {
    }
    public Vector2 getcenter(Game gamevars)
    {
        return new Vector2(gamevars.gridstartx + Game.gridwidth * column, gamevars.gridstarty - (Game.gridwidth * scaleh) * row);
    }
}
