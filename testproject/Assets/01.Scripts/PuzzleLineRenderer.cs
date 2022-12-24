using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLineRenderer : MonoBehaviour
{
    public LineRenderer LineRenderer => GetComponent<LineRenderer>();
   public void SetVisible(bool b)
    {
        if(nowRoutine!=null)
        {
            StopCoroutine(nowRoutine);
        }
        nowRoutine = StartCoroutine(VisibleRoutine(b));
    }
    Coroutine nowRoutine;
    IEnumerator VisibleRoutine(bool b)
    {
        Color initialColor = LineRenderer.material.color;
        Color targetColor = b ? LineRenderer.material.color.ModifiedAlpha(.27f) : LineRenderer.material.color.ModifiedAlpha(0f);
        float accumTime = 0f;
        float totalTime = .3f;
        while (true)
        {
            accumTime += Time.deltaTime;
            float perone = accumTime / totalTime;
            Color tmpColor = Color.Lerp(initialColor, targetColor, perone);
            LineRenderer.material.color = tmpColor;
            yield return null;
        }
    }

    public void RefreshLineRenderer(TileSituation tileSituation)
    {
        List<Hexagon> hexagons = tileSituation.nowHexagons;
        LineRenderer.positionCount = hexagons.Count;
        for (int i = 0; i < hexagons.Count; i++)
        {
            LineRenderer.SetPosition(i, hexagons[i].transform.localPosition.ModifiedY(2f));
        }
    }


}
