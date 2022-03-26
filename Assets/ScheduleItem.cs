using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScheduleItem : MonoBehaviour
{
    // Start is called before the first frame update

    public bool m_open;

    private const float ANIMATION_TIME = .125f;

    public Color m_textConfirmedColor;

    public Color m_textUnconfirmedColor;
    void Start()
    {
        transform.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(ShrinkExpand()); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShrinkExpand()
    {
        Vector2 size = ((RectTransform)transform).sizeDelta;
        float startSize = size.y;
        float newSize = m_open ? 200 : 500;
        DateTime startTime = DateTime.Now;

        if(m_open)
        {
            transform.Find("expand").gameObject.SetActive(false);
        }

        while((DateTime.Now - startTime).TotalSeconds > ANIMATION_TIME)
        {
            float pctge = (float)(DateTime.Now - startTime).TotalSeconds / ANIMATION_TIME;
            size.y = startSize + (pctge * (newSize - startSize));
            ((RectTransform)transform).sizeDelta = size;
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform.parent);
            yield return null;
        }

        size.y = newSize;
        ((RectTransform)transform).sizeDelta = size;

        m_open = !m_open;
        transform.Find("expand").gameObject.SetActive(m_open);

        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().color = m_open ? m_textUnconfirmedColor : m_textConfirmedColor;

        Vector3 rot = Vector3.zero;
        rot.z = m_open ? 180 : 0;
        transform.GetChild(2).GetChild(0).localEulerAngles = rot;
        
    }
}
