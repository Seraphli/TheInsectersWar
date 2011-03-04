using UnityEngine;
using System.Collections;

public class MinimapUI : zzInterfaceGUI
{
    public float textureAlpha = 0.6f;
    public Vector2 MapScale;
    public zzInterfaceGUI Map;
    public Transform ObjectManage;
    public Vector2 heroDefaultPos;
    public Vector2 heroStartupPos;
    public Vector2 pos1;
    public Vector2 mapDefaultPos;
    public Vector2 MapSize;         //大地图尺寸
    //public float MiniMapScale=1f;
    public Vector2 MapPos;          //大地图位置(left,top)
    public Vector2 PlayerPos;       //玩家位置
    public Vector2 gizmosSize = new Vector2(15, 15);
    public Texture[] allyGizmos;
    public Texture[] enemyGizmos;
    public Texture[] otherGizmos;
    public ScaleMode scaleMode = ScaleMode.StretchToFill;
    public bool alphaBlend = true;
    public float imageAspect = 0;
    // Use this for initialization
    void Start()
    {
        mapDefaultPos.x = Map.position.x;
        mapDefaultPos.y = Map.position.y;

        /*
                heroStartupPos.x = PlayerPos.x;
                heroStartupPos.y = PlayerPos.y;*/

        //MapPos.x = mapStartupPos.x;
        //MapPos.y = mapStartupPos.y;
        //MapPos.x = MainMap.transform.position.x - MapSize.x / 2;
        //MapPos.y = MainMap.transform.position.y - MapSize.y / 2;
    }
    public void DrawGizmos(Texture atexture, Vector2 pos)
    {
        if (atexture)
            GUI.DrawTexture(new Rect(pos.x - gizmosSize.x / 2, pos.y - gizmosSize.y / 2, gizmosSize.x, gizmosSize.y), atexture, scaleMode, alphaBlend, imageAspect);
    }
    //GUI只能在OnGUI函数里绘制,在执行像Update这样的函数的时候 GUI 为null
    public override void impGUI(Rect rect)
    {
        heroDefaultPos = new Vector2(getParent().getWidth() / 2, getParent().getHeight() * 0.75f);
        //Map.relativePosition.width =
        //MapScale.x = MapSize.x / (Map.getWidth()*Map.relativePosition.width);
        //MapScale.y = MapSize.y / (Map.getHeight()*Map.relativePosition.height);
        Map.position.x = (heroStartupPos.x - PlayerPos.x) * MapScale.x + mapDefaultPos.x;
        Map.position.y = -(heroStartupPos.y - PlayerPos.y) * MapScale.y + mapDefaultPos.y;
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, textureAlpha);
        DrawAllyGizmos(allyGizmos);
        DrawEnemyGizmos(enemyGizmos);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
    }
    public void DrawAllyGizmos(Texture[] atexture)
    {
        //PlayerPos.x = ObjectManage.FindChild("蜜蜂").FindChild("英雄").GetChild(0).position.x;
        //Debug.Log(ObjectManage.FindChild("Bee").FindChild("Hero").GetChildCount());
        //Draw Hero
        {
            if (ObjectManage.FindChild("Bee").FindChild("Hero").GetChildCount() > 0)
            {
                //Debug.Log(ObjectManage.FindChild("Bee").FindChild("Hero").GetChild(0));
                PlayerPos.x = ObjectManage.FindChild("Bee").FindChild("Hero").GetChild(0).position.x;
                PlayerPos.y = ObjectManage.FindChild("Bee").FindChild("Hero").GetChild(0).position.y;
            }
            DrawGizmos(allyGizmos[0], heroDefaultPos);
        }
        //Soldier
        DrawChildrenPos("Bee", "Soldier", atexture[1]);


        //Base
        DrawChildrenPos("Bee", "Stronghold", atexture[2]);


    }

    public void DrawEnemyGizmos(Texture[] atexture)
    {
        DrawChildrenPos("Pismire", "Soldier", atexture[1]);
    }

    public Vector2 PositionTransform(Vector2 pos)
    {
        float x = (pos.x - PlayerPos.x) * MapScale.x + heroDefaultPos.x;
        float y = -(pos.y - PlayerPos.y) * MapScale.y + heroDefaultPos.y;
        return new Vector2(x, y);
    }
    public void DrawChildrenPos(string parentName, string childrenName, Texture gizmos)
    {
        int n = ObjectManage.FindChild(parentName).FindChild(childrenName).GetChildCount();
        if (n > 0)
        {
            Vector2[] childrenpos = new Vector2[n];
            for (int i = 0; i < n; i++)
            {
                //Debug.Log(ObjectManage.FindChild("Bee").FindChild("Soldier").GetChild(i));
                childrenpos[i].x = ObjectManage.FindChild(parentName).FindChild(childrenName).GetChild(i).position.x;
                childrenpos[i].y = ObjectManage.FindChild(parentName).FindChild(childrenName).GetChild(i).position.y;
            }
            foreach (Vector2 pos in childrenpos)
            {
                DrawGizmos(gizmos, PositionTransform(pos));
            }
        }
    }
}
