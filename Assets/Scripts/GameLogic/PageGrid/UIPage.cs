using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPage : EventTrigger
{
    public Image image = null;
    public Image GetImage
    {
        get
        {
            if (image = null)
            {
                image = this.transform.GetChild(0).GetComponent<Image>();
            }
            return image;
        }
        set
        {
            image = value;
        }
    }

    public Text text = null;
    public Text GetText
    {
        get
        {
            if (text = null)
            {
                text = this.transform.GetChild(1).GetComponent<Text>();
            }
            return text;
        }
        set
        {
            text = value;
        }
    }
   



    //点击UI_Page
    public override void OnPointerClick(PointerEventData eventData)
    {
     
        if (this.transform.GetChild(1).GetComponent<Text>().text == "..." || this.transform.GetChild(1).GetComponent<Text>().text == "")
        {
            return;
        }
        PageGrid pg = this.transform.parent.GetComponent<PageGrid>();
        //如果点击的是前面几个ui（点击的是1-5）   
        if (int.Parse(this.transform.GetChild(1).GetComponent<Text>().text) < pg.uiPageArray.Length)
        {
            string text = this.transform.GetChild(1).GetComponent<Text>().text;

            //更新显示
            pg.UpadateUIPageFromHead();

            UIPage uiPage = pg.FindUIPageWithText(text);
            if (uiPage)
            {
                pg.ActivatUIPageImage(this.gameObject);
            }

            int index = int.Parse(text);
            pg.currtntPageIdex = index;
            MyEventSystem.TriggerEvent(pg.currtntPageIdex);
        }
        //点击最后几个（点击的是最后4个）
        else if (int.Parse(this.transform.GetChild(1).GetComponent<Text>().text) >= pg.maxPageIndex - 3)
        {
            string text = this.transform.GetChild(1).GetComponent<Text>().text;

            //更新显示
            pg.UpdateUIPageFromEnd();

            UIPage uiPage = pg.FindUIPageWithText(text);
            if (uiPage)
            {
                pg.ActivatUIPageImage(uiPage.gameObject);
            }
            int index = int.Parse(text);
            pg.currtntPageIdex = index;
            MyEventSystem.TriggerEvent(pg.currtntPageIdex);
        }
        else
        {
            string text = this.transform.GetChild(1).GetComponent<Text>().text;

            //更新显示
            pg.UpdateUIPageFromMiddle(text);
            /*由于数字向后移动，故image显示位置不需要改变*/
            int index = int.Parse(text);
            pg.currtntPageIdex = index;
            MyEventSystem.TriggerEvent(pg.currtntPageIdex);
        }
    }
}