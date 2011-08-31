using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public partial class ObjectPropertySetting : MonoBehaviour
{
    struct UiItem
    {
        public UiItem(UiAttributeBase pUiType, MemberInfo pMemberInfo)
        {
            uiType = pUiType;
            memberInfo = pMemberInfo;
        }

        public UiAttributeBase uiType;
        public MemberInfo memberInfo;
    }

    /// <summary>
    /// 水平显示属性内容
    /// </summary>
    class HorizontalPropertyGUI : IPropertyGUI
    {
        UiItem[] uiItemList;

        public HorizontalPropertyGUI(UiItem[] pList)
        {
            uiItemList = pList;
        }

        public override void endImpGUI()
        {
            foreach (var lUiItem in uiItemList)
            {
                lUiItem.uiType.clearBuffer();
            }
        }

        public override void OnPropertyGUI(MonoBehaviour pObject)
        {
            GUILayout.BeginHorizontal();
            foreach (var lUiItem in uiItemList)
            {
                var lUiType = lUiItem.uiType;
                lUiType.skin = skin;
                lUiType.windowRect = windowRect;
                lUiType.impUI(pObject, lUiItem.memberInfo);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

    }

    /// <summary>
    /// 垂直显示属性内容
    /// </summary>
    class VerticalPropertyGUI : IPropertyGUI
    {
        UiItem[] uiItemList;

        public VerticalPropertyGUI(UiItem[] pList)
        {
            uiItemList = pList;
        }

        public override void endImpGUI()
        {
            foreach (var lUiItem in uiItemList)
            {
                lUiItem.uiType.clearBuffer();
            }
        }

        public override void OnPropertyGUI(MonoBehaviour pObject)
        {
            GUILayout.BeginVertical();
            foreach (var lUiItem in uiItemList)
            {
                GUILayout.BeginHorizontal();
                var lUiType = lUiItem.uiType;
                lUiType.skin = skin;
                lUiType.windowRect = windowRect;
                lUiType.impUI(pObject, lUiItem.memberInfo);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

    }

    class CPropertyGUI : IPropertyGUI
    {
        //按行列优先级排序
        IPropertyGUI[] uiList;

        public CPropertyGUI(UiItem[][] pList)
        {
            //foreach (var lUiItems in pList)
            //{
            //    foreach (var lUiItem in lUiItems)
            //    {
            //        print(lUiItem.memberInfo.Name + " "
            //            + lUiItem.uiType.verticalDepth
            //            + lUiItem.uiType.horizontalDepth);
            //    }
            //}
            uiList = new IPropertyGUI[pList.Length];
            int i = 0;
            foreach (var lUiItems in pList)
            {
                //如果水平方向没有排序,则使用垂直显示,因为已经排序好,所以只用对比始末元素
                if (lUiItems[0].uiType.horizontalDepth
                    != lUiItems[lUiItems.Length - 1].uiType.horizontalDepth)
                    uiList[i] = new HorizontalPropertyGUI(lUiItems);
                else
                    uiList[i] = new VerticalPropertyGUI(lUiItems);
                ++i;
            }
        }

        public override void endImpGUI()
        {
            foreach (var lUiItem in uiList)
            {
                lUiItem.endImpGUI();
            }
        }

        public override void OnPropertyGUI(MonoBehaviour pObject)
        {
            foreach (var lUiItem in uiList)
            {
                lUiItem.skin = skin;
                lUiItem.windowRect = windowRect;
                lUiItem.OnPropertyGUI(pObject);
            }
        }

    }


    /// <summary>
    /// 存储 获取排序好的 属性表
    /// </summary>
    class UiItemUV
    {
        //<行,元素>
        Dictionary<int, List<UiItem>> mData = new Dictionary<int, List<UiItem>>();
        public void Add(UiItem pUiItem)
        {
            int lIndex = pUiItem.uiType.verticalDepth;
            List<UiItem> lList;
            if (!mData.TryGetValue(lIndex, out lList))
            {
                lList = new List<UiItem>();
                mData[lIndex] = lList;
            }
            lList.Add(pUiItem);
        }

        public UiItem[][] toSortedArray()
        {
            List<UiItem[]> lOut = new List<UiItem[]>(mData.Count);
            foreach (var lDir in mData)
            {
                lDir.Value.Sort((a,b)=>a.uiType.horizontalDepth
                    .CompareTo(b.uiType.horizontalDepth));

                bool lInserted = false;
                for (int i = 0; i < lOut.Count; ++i)
                {
                    if (lDir.Key < lOut[i][0].uiType.verticalDepth)
                    {
                        lOut.Insert(i, lDir.Value.ToArray());
                        lInserted = true;
                        break;
                    }
                }

                if (!lInserted)
                    lOut.Add(lDir.Value.ToArray());
            }

            return lOut.ToArray();

        }
    }

    private static UiItem[][] getUiItemList(System.Type lType)
    {
        UiItemUV lOut = new UiItemUV();
        var lMembers = lType.GetMembers();
        foreach (var lMember in lMembers)
        {
            UiAttributeBase[] lUIAttributes =
                (UiAttributeBase[])lMember.GetCustomAttributes(typeof(UiAttributeBase), false);
            foreach (var lAttribute in lUIAttributes)
            {
                lOut.Add(new UiItem(lAttribute, lMember));
            }
            //if (lUIAttributes.Length > 0)
            //{
            //    lOut.Add(new UiItem(lUIAttributes[0], lMember));
            //}
        }

        return lOut.toSortedArray();

    }


}