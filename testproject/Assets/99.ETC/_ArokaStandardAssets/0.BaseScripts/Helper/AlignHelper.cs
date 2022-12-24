using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class AlignHelper : MonoBehaviour
{



    [BoxGroup("Data Child Align", centerLabel: true)]
    public bool isCenterAlign;
    [BoxGroup("Data Child Align")]
    public Vector3 distance = Vector3.zero;
    [GUIColor(0, 1, 0)]
    [BoxGroup("Data Child Align")]
    [Button("Child Align Distance", ButtonSizes.Small)]
    public void Align()
    {
        if (distance == Vector3.zero)
        {
            Debug.LogWarning("No data Distance");
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (isCenterAlign)
            {
                transform.GetChild(i).transform.localPosition = (i - ((transform.childCount - 1) * 0.5f)) * distance;
            }
            else
            {
                transform.GetChild(i).transform.localPosition = distance * i;

            }
        }
    }

    [BoxGroup("Data Child Random Rot", centerLabel: true)]
    public Vector3 axis = Vector3.up;
    [MinMaxSlider(-360, 360, true)]
    [BoxGroup("Data Child Random Rot")]
    public Vector2 rotMinMax;
    [BoxGroup("Data Child Random Rot")]
    [GUIColor(0, 1, 0)]
    [Button("Child Random Rot", ButtonSizes.Small)]
    public void ChildRandomRot()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.localRotation = Quaternion.Euler(axis * Random.Range(rotMinMax.x, rotMinMax.y));
        }
    }

    [BoxGroup("Data Child Random Pos", centerLabel: true)]
    public bool xRandom;
    [BoxGroup("Data Child Random Pos", centerLabel: true)]
    public bool yRandom;
    [BoxGroup("Data Child Random Pos", centerLabel: true)]
    public bool zRandom;
    [BoxGroup("Data Child Random Pos", centerLabel: true)]
    [MinMaxSlider(-360, 360, true)]
    public Vector2 moveMinMax;
    [GUIColor(0, 1, 0)]
    [BoxGroup("Data Child Random Pos", centerLabel: true)]

    [Button("Child Random Pos", ButtonSizes.Small)]
    public void ChildRandomPos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            float x = xRandom ? Random.Range(moveMinMax.x, moveMinMax.y) : 0;
            float y = yRandom ? Random.Range(moveMinMax.x, moveMinMax.y) : 0;
            float z = zRandom ? Random.Range(moveMinMax.x, moveMinMax.y) : 0;
            transform.GetChild(i).transform.localPosition += new Vector3(x, y, z);
        }
    }

    [BoxGroup("Data Child Random Scale", centerLabel: true)]
    public Vector3 standardScale = Vector3.one;
    [BoxGroup("Data Child Random Scale", centerLabel: true)]
    [MinMaxSlider(0.0001f, 10f, true)]
    [BoxGroup("Data Child Random Scale", centerLabel: true)]
    public Vector2 scaleMinMax = new Vector2(0.1f, 3f);
    [GUIColor(0, 1, 0)]
    [BoxGroup("Data Child Random Scale", centerLabel: true)]
    [Button("Child Random Scale", ButtonSizes.Small)]

    public void ChildRandomScale()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.localScale = standardScale * Random.Range(scaleMinMax.x, scaleMinMax.y);
        }
    }

    [BoxGroup("Data Child Random Mesh", centerLabel: true)]
    public List<Mesh> meshes;
    int MeshCount => meshes.Count - 1;
    [BoxGroup("Data Child Random Mesh", centerLabel: true)]
    [MinMaxSlider(0, "MeshCount", true)]
    [BoxGroup("Data Child Random Mesh", centerLabel: true)]
    public Vector2Int meshesIndexRange;
    [GUIColor(0, 1, 0)]
    [BoxGroup("Data Child Random Mesh", centerLabel: true)]
    [Button("Child Random Mesh", ButtonSizes.Small)]

    public void ChildRandomMesh()
    {
        if (meshes.Count == 0)
        {
            Debug.LogWarning("No Meshes");
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.GetComponent<MeshFilter>().mesh = meshes[Random.Range(meshesIndexRange.x, meshesIndexRange.y + 1)];
        }
    }


    [BoxGroup("RandomColor(isPlaying)", centerLabel: true)]
    public List<Color> colors;
    [BoxGroup("RandomColor(isPlaying)", centerLabel: true)]
    [GUIColor(0, 1, 0)]
    [Button("ColorTest", ButtonSizes.Small)]

    public void TestColor()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Color targetColor = colors[i % colors.Count];
                transform.GetChild(i).gameObject.GetComponentInChildren<Renderer>().materials[0].color = targetColor;
            }
        }
    }

    [BoxGroup("RandomDestroy", centerLabel: true)]
    [Range(0, 1)]
    [BoxGroup("RandomDestroy", centerLabel: true)]
    public float destroyPossibilityPerone;
    [BoxGroup("RandomDestroy", centerLabel: true)]
    [GUIColor(0, 1, 0)]

    [Button("RandomDestroy", ButtonSizes.Small)]

    public void RandomDestroy()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (StaticMethod.RandomPossibility(destroyPossibilityPerone))
            {
                DestroyImmediate(transform.GetChild(i).gameObject);

            }
        }

    }
    

    [BoxGroup("GradientTest", centerLabel: true)]
    public Gradient gradient;
    [BoxGroup("GradientTest", centerLabel: true)]
    public int colorPartIndex;
    [BoxGroup("GradientTest", centerLabel: true)]
    public List<Renderer> renderers;
    [BoxGroup("GradientTest", centerLabel: true)]
    [GUIColor(0, 1, 0)]
    [Button("Gradient Test !", ButtonSizes.Small)]
    public void GradientTest()
    {
        if (!Application.isPlaying) return;
        for (int i = 0; i < renderers.Count; i++)
        {
            float perone = (float)i / (renderers.Count - 1);
            renderers[i].materials[colorPartIndex].color = gradient.Evaluate(perone);
        }
    }

    [BoxGroup("RandomSharedMat", centerLabel: true)]
    public List<Material> mats;
    [BoxGroup("RandomSharedMat", centerLabel: true)]
    [GUIColor(0, 1, 0)]
    [Button("Random Shared Mat", ButtonSizes.Small)]

    public void RandomSharedMat()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Material material = mats[i % mats.Count];
            transform.GetChild(i).gameObject.GetComponentInChildren<Renderer>().sharedMaterial = material;
        }
    }

    public Material mat;
    [Button("Mat", ButtonSizes.Gigantic)]
    public void ETSET()
    {
        MeshRenderer[] outlines = transform.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < outlines.Length; i++)
        {
            outlines[i].sharedMaterial = mat;
        }
    }




    /*
    [Button("Destory", ButtonSizes.Gigantic)]
    public void ETSET()
    {
        Hexagon[] outlines = transform.GetComponentsInChildren<Hexagon>();
        for (int i = 0; i < outlines.Length; i++)
        {
            DestroyImmediate(outlines[i]);
        }
    }*/
}