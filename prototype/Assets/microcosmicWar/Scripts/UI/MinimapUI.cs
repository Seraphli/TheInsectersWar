using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class MinimapUI : zzInterfaceGUI
{
    public float textureAlpha = 0.6f;
    public MinimapImage Map;
    public Vector2 heroDefaultPos;
    public Vector2 MapSize;         //大地图尺寸
    public Vector2 MapPos;          //大地图位置(left,top)
    public Vector2 PlayerPos;       //玩家位置
    public Vector2 gizmosSize = new Vector2(10, 10);
    public Texture[] allyGizmos;
    public Texture[] enemyGizmos;
    public Texture[] otherGizmos;
    public Vector2[] soldierPos;
    public Vector2 basePos;
    public Vector2[] towerPos;
    public Vector2[] planePos;
    public ScaleMode scaleMode = ScaleMode.StretchToFill;
    public bool alphaBlend = true;
    public float imageAspect = 0;
    // Use this for initialization
    public void DrawGizmos(Texture atexture, Vector2 pos)
    {
        if (atexture)
            GUI.DrawTexture(new Rect(pos.x - gizmosSize.x / 2, pos.y - gizmosSize.y / 2, gizmosSize.x, gizmosSize.y), atexture, scaleMode, alphaBlend, imageAspect);
    }
    //GUI只能在OnGUI函数里绘制,在执行像Update这样的函数的时候 GUI 为null
    public override void impGUI(Rect rect)
    {
        heroDefaultPos = new Vector2(getParent().getWidth() / 2, getParent().getHeight() * 0.75f);
        Map.position.x = heroDefaultPos.x - PositionTransform1(PlayerPos).x;
        Map.position.y = heroDefaultPos.y - PositionTransform1(PlayerPos).y;
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, textureAlpha);
        DrawAllyGizmos();
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
    }
    public void DrawAllyGizmos()
    {
        DrawGizmos(allyGizmos[0], heroDefaultPos);
        foreach (Vector2 pos in soldierPos)
        {
            DrawGizmos(allyGizmos[1], PositionTransform2(pos));
        }
        //foreach (Vector2 pos in basePos)
        //{
            DrawGizmos(allyGizmos[2], PositionTransform2(basePos));
        //}
    }
    public Vector2 PositionTransform1(Vector2 pos)
    {
        float x = (pos.x - MapPos.x) / MapSize.x * position.width;
        float y = (pos.y - MapPos.y) / MapSize.y * position.height;
        return new Vector2(x, y);
    }
    public Vector2 PositionTransform2(Vector2 pos)
    {
        float x = (pos.x - MapPos.x) / MapSize.x * position.width + Map.position.x;
        float y = (pos.y - MapPos.y) / MapSize.y * position.height + Map.position.y;
        return new Vector2(x, y);
    }
}
