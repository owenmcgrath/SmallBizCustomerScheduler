using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class MenuControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public enum MenuType
    {
        e_schedule,
        e_contact,
        e_count
    }

    public Transform m_menu;

    public Transform m_menuButtons;

    public Transform m_slidingLine;

    public Dictionary<MenuType, string> m_menuNames;

    public MenuType m_selectedButton = (MenuType)0;

    public Color m_selectedColor;

    public Color m_unselectedColor;

    public bool m_dragging = false;

    public Vector3 m_firstTouchPos;

    public Vector3 m_firstMenuPos;

    private const float ANIMATION_TIME = .125f;

    // Start is called before the first frame update
    void Start()
    {

        m_menuNames = new Dictionary<MenuType, string>();
        m_menuNames[MenuType.e_schedule] = "SCHEDULE";
        m_menuNames[MenuType.e_contact] = "CONTACT";

        Vector2 lineSize = ((RectTransform)m_slidingLine).sizeDelta;
        lineSize.x = ((RectTransform)transform.parent).sizeDelta.x / (float)MenuType.e_count;
        ((RectTransform)m_slidingLine).sizeDelta = lineSize;

        for(int i = 0; i < m_menuButtons.transform.childCount; i++)
        {
            int childIdx = i;
            Transform child = m_menuButtons.GetChild(i);
            child.name = m_menuNames[(MenuType)i];
            child.GetComponentInChildren<TextMeshProUGUI>().text = child.name;
            child.GetComponent<Button>().onClick.AddListener(delegate { OnMenuButtonPress((MenuType)childIdx); });

            if(i==0)
            {
                child.GetComponent<Image>().color = m_selectedColor;

                Vector3 pos = m_slidingLine.localPosition;
                pos.x = child.transform.localPosition.x;
                m_slidingLine.localPosition = pos;
            }
            else 
            {
                child.GetComponent<Image>().color = m_unselectedColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if(m_dragging)
        {
            float delta = Input.mousePosition.x - m_firstTouchPos.x;
            Vector3 newPos = m_firstMenuPos;
            newPos.x = newPos.x + delta;
            ((RectTransform)m_menu).anchoredPosition = newPos;
        }

    }

    void OnMenuButtonPress(MenuType type)
    {

        m_menuButtons.GetChild((int)m_selectedButton).GetComponent<Image>().color = m_unselectedColor;
        m_menuButtons.GetChild((int)type).GetComponent<Image>().color = m_selectedColor;
        //launch animations!!!
        Vector3 targetPos = ((RectTransform)m_menu).anchoredPosition;
        Vector3 slidingTargetPos = ((RectTransform)m_slidingLine).anchoredPosition;

        double pctge = ((double)type / (double)MenuType.e_count);
        IEnumerator menuAnimation = SlideX(m_menu, -(float)pctge * ((RectTransform)m_menu).sizeDelta.x);
        IEnumerator lineAnimation = SlideX(m_slidingLine, (float)pctge * ((RectTransform)transform.parent).sizeDelta.x);
        StartCoroutine(menuAnimation);
        StartCoroutine(lineAnimation);

        m_selectedButton = type;
    }

    IEnumerator SlideX(Transform go, float x)
    {
        Vector3 pos = ((RectTransform)go).anchoredPosition;
        float start = pos.x;
        DateTime time = DateTime.Now;
        while((DateTime.Now - time).TotalSeconds < ANIMATION_TIME)
        {
            pos.x = start + (float)((DateTime.Now - time).TotalSeconds / ANIMATION_TIME) * (x - start);
            ((RectTransform)go).anchoredPosition = pos;
            yield return null;
        }

        pos.x = x;
        ((RectTransform)go).anchoredPosition = pos;
    }

    private void OnMouseDrag() 
    {
        Debug.Log("DRAG");
        float dist = Input.mousePosition.x - m_firstTouchPos.x;
        Vector3 menuPos = ((RectTransform)m_menu).anchoredPosition;
        menuPos.x = m_firstMenuPos.x + dist;
        ((RectTransform)m_menu).anchoredPosition = menuPos;
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        m_firstMenuPos = ((RectTransform)m_menu).anchoredPosition;
        m_firstTouchPos = Input.mousePosition;
        m_dragging = true;
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_dragging = false;

        //calculate the delta
        float delta = Input.mousePosition.x - m_firstTouchPos.x;
        Debug.Log(delta + " "  +((RectTransform)transform.parent).sizeDelta.x * .5);
        if(Math.Abs(delta) > ((RectTransform)transform.parent).sizeDelta.x * .5)
        {
            //snap either to the right or the left;
            int newSelection = (int)m_selectedButton - (int)Math.Sign(delta);
            
            //clamp
            newSelection = Math.Max(0, newSelection);
            newSelection = Math.Min((int)MenuType.e_count - 1, newSelection);
            OnMenuButtonPress((MenuType)newSelection);
        }
        else 
        {
            OnMenuButtonPress(m_selectedButton);
        }

    }
}
