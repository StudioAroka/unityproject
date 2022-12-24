using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class SimpleOutlineMaker : MonoBehaviour
{
    private int raycastRollDetailCount = 50;
    private float raycastRollRadius = 0.02f;
    public Renderer Rend => GetComponent<Renderer>();
    [Title("PRESET")]
    public Material DefaultMat => null;

    [Title("BAKE OPTIONS")]
    public LayerMask targetLayerMask = -1;

    [OnValueChanged("MakeOutline")]
    [Range(0, 3000)]
    public int raycastRollCount = 70;
    [OnValueChanged("MakeOutline")]
    [Range(0, 3)]
    public float lineWidth = 0.05f;
    [OnValueChanged("MakeOutline")]
    [Range(0, 3)]
    public float lineRendLocalPosY = 0.05f;

    [OnValueChanged("OVC_peroneRange")]
    [PropertyRange("$HeightMin", "$HeightMax")]
    public float rayStartHeight;

    [OnValueChanged("MakeOutline")]
    [Range(1,100)]
    public int term;

    [Title("RUNTIME")]
    public LineRenderer nowLineRenderer;
    public GameObject rayStartGizmoCube;

    public Vector2 HeightRange
    {
        get
        {
            return new Vector2(Rend.bounds.min.y, Rend.bounds.max.y);
        }
    }
    public float HeightMin => HeightRange.x;
    public float HeightMax => HeightRange.y;
    public float HeightCenter => (HeightMin + HeightMax) * .5f;

    public void OVC_peroneRange()
    {
        if (rayStartGizmoCube == null)
        {
            rayStartGizmoCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rayStartGizmoCube.transform.SetParent(transform);
            DestroyImmediate(rayStartGizmoCube.GetComponent<Collider>());
            rayStartGizmoCube.transform.name = "gizmo";
            rayStartGizmoCube.transform.localScale = new Vector3(0.03f, 0.03f, 4f);
        }
        rayStartGizmoCube.transform.position = rayStartGizmoCube.transform.position.ModifiedY(rayStartHeight).ModifiedZ(Rend.bounds.min.z - 2f).ModifiedX(Rend.bounds.center.x);
    }


    [Button("Make Outline", ButtonSizes.Gigantic)]
    public void MakeOutline()
    {
        OVC_peroneRange();

        if(nowLineRenderer != null)
        {
            DestroyImmediate(nowLineRenderer.gameObject);
        }
        Collider tmpCol = gameObject.AddComponent<MeshCollider>();

        Vector3 startVPos;
        Vector3 vNormal;

        Ray ray = new Ray();
        ray.origin = rayStartGizmoCube.transform.position;
        ray.direction = Vector3.forward;
        DestroyImmediate(rayStartGizmoCube);

        List<Vector3> linePoss = new List<Vector3>();
        if (Physics.Raycast(ray, out RaycastHit rHit, Mathf.Infinity, targetLayerMask))
        {
            if (rHit.transform != null)
            {
                startVPos = rHit.point;
                vNormal = rHit.normal;
                Debug.DrawRay(startVPos, Vector3.up, Color.red, 2f);
                Debug.DrawRay(rHit.point, rHit.normal, Color.green, 1.0f);

                for (int j = 0; j < raycastRollCount; j++)
                {
                    if (RaycastRoll(startVPos, vNormal, raycastRollRadius, out RaycastHit hit, targetLayerMask))
                    {
                        if (hit.transform != null)
                        {
                            linePoss.Add(hit.point);
                            startVPos = hit.point;
                            vNormal = hit.normal;

                            Debug.DrawLine(hit.point, hit.point + hit.normal * 0.05f, Color.green, 1.0f);
                        }
                    }
                }
            }
            else
            {

            }
        }
        else
        {

        }

        nowLineRenderer = new GameObject("OutLine Of The " + name).AddComponent<LineRenderer>();
        nowLineRenderer.transform.SetParent(transform);
        nowLineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        nowLineRenderer.widthMultiplier = lineWidth;
        nowLineRenderer.material = DefaultMat;
        nowLineRenderer.loop = false;
        nowLineRenderer.positionCount = linePoss.Count;
        nowLineRenderer.useWorldSpace = false;
        nowLineRenderer.SetPositions(linePoss.ToArray());


        DestroyImmediate(tmpCol);
        nowLineRenderer.transform.localPosition = nowLineRenderer.transform.localPosition.ModifiedY(lineRendLocalPosY);
    }


    public bool RaycastRoll(Vector3 startPos, Vector3 normalDirect, float radius, out RaycastHit outHit, LayerMask layerMask)
    {
        int tmpPointCount = raycastRollDetailCount;
        bool zMinus = normalDirect.z < 0;
        bool xMinus = normalDirect.x < 0;

        float diffAngle = Mathf.Deg2Rad * Vector3.Angle(xMinus ? Vector3.forward : Vector3.back, normalDirect);
        float angle = 2 * Mathf.PI / tmpPointCount;
        List<Vector3> tmpPoints = new List<Vector3>();
        for (int i = 0; i < tmpPointCount; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i + diffAngle), 0, Mathf.Sin(angle * i + diffAngle), 0),
                                                     new Vector4(-1 * Mathf.Sin(angle * i + diffAngle), 0, Mathf.Cos(angle * i + diffAngle), 0),
                                       new Vector4(0, 0, 1, 0),
                                       new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, xMinus ? radius : -radius, 0);
            tmpPoints.Add(startPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
        }

        for (int i = 0; i < tmpPoints.Count; i++)
        {

            Vector3 rayOrigin = tmpPoints[i];
            Vector3 rayTarget = tmpPoints[i != tmpPoints.Count - 1 ? i + 1 : 0];
            Vector3 rayDirection = (rayTarget - rayOrigin).normalized;

            Debug.DrawLine(rayOrigin , rayTarget, Color.red, 1.0f);

            if (Physics.Raycast(tmpPoints[i], rayDirection, out RaycastHit rayHit, Vector3.Distance(rayOrigin, rayTarget), layerMask))
            {
                if (rayHit.transform != null)
                {
                    outHit = rayHit;
                    return true;
                }
            }
        }
        outHit = new RaycastHit();
        return false;
    }

}
