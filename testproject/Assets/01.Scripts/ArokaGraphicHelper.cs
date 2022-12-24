using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class ArokaGraphicHelper : MonoBehaviour
{
    [Button("Modify", ButtonSizes.Gigantic)]
    public void Modify()
    {
        GameObject graphic = Instantiate(gameObject, transform);
        graphic.transform.name = "graphic";
        DestroyImmediate(GetComponent<MeshRenderer>());
        DestroyImmediate(GetComponent<MeshFilter>());
        DestroyImmediate(graphic.GetComponent<TileObj>());
        graphic.transform.localPosition = transform.localPosition;
        graphic.transform.localScale = transform.localScale;
        graphic.transform.localRotation = transform.localRotation;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        DestroyImmediate(graphic.GetComponent<ArokaGraphicHelper>());
    }

}
