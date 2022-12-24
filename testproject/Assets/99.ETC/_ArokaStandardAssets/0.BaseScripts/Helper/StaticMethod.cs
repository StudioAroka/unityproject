using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public static class StaticMethod
{
    #region 1티어
    public static Vector3 MousePosition
    {
        get
        {
            if (Input.touchCount > 0)
            {
                return Input.touches[0].position;
            }
            else
            {
                return Input.mousePosition;
            }
        }
    }


    public static Quaternion RandomQuaternion => Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
    public static Vector3 RandomVec => new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    public static Vector3 RandomVec01 => new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));
    public static bool RandomBool
    {
        get
        {
            return RandomPossibility(.5f);
        }

    }
    public static int RandomBoolInt
    {
        get
        {
            return RandomBool ? 1 : -1;
        }
    }
    public static List<Vector3> ToList(this Vector3[] v3s)
    {
        List<Vector3> v3sList = new List<Vector3>();
        v3sList.AddRange(v3s);
        return v3sList;
    }
    public static List<Vector2> ToList(this Vector2[] v2s)
    {
        List<Vector2> v2sList = new List<Vector2>();
        v2sList.AddRange(v2s);
        return v2sList;
    }
    public static Vector3 ModifiedX(this Vector3 v, float n)
    {
        return new Vector3(n, v.y, v.z);
    }
    public static Vector3 ModifiedY(this Vector3 v, float n)
    {
        return new Vector3(v.x, n, v.z);
    }
    public static Vector2 ModifiedX(this Vector2 v, float n)
    {
        return new Vector2(n, v.y);
    }
    public static Vector2 ModifiedY(this Vector2 v, float n)
    {
        return new Vector2(v.x, n);
    }
    public static Vector3 ModifiedZ(this Vector2 v, float n)
    {
        return new Vector3(v.x, v.y, n);
    }
    public static Vector3 ModifiedZ(this Vector3 v, float n)
    {
        return new Vector3(v.x, v.y, n);
    }
    public static Color ModifiedAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }
    public static Vector3 ToAvg(this List<Vector3> _vectorList)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < _vectorList.Count; i++)
        {
            sum += _vectorList[i];
        }
        return sum / _vectorList.Count;
    }
    public static void DestroyAllChildren(this Transform tr)
    {
        for (int i = tr.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(tr.GetChild(i).gameObject);
        }
    }
    public static void DestroyImmediateAllChildren(this Transform tr)
    {
        for (int i = tr.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(tr.GetChild(i).gameObject);
        }
    }
    public static void DestroyGameObjects(this List<GameObject> objectList)
    {
        for (int i = objectList.Count - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(objectList[i]);
        }
    }
    public static void DestroyImmediateGameObjects(this List<GameObject> objectList)
    {
        for (int i = objectList.Count - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(objectList[i]);
        }
    }
    public static void SetRayCastTargetRecursively(this Transform _parent, bool b)
    {
        if (null == _parent)
        {
            return;
        }
        if (_parent.GetComponent<Image>() != null)
        {
            _parent.GetComponent<Image>().raycastTarget = b;
        }
        foreach (Transform child in _parent)
        {
            if (null == child)
            {
                continue;
            }
            child.SetRayCastTargetRecursively(b);
        }
    }
    public static void SetLayerRecursively(this GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    public static void SetLayer(this GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
    }
    public static void SetLayerRecursively(this GameObject obj, LayerMask newLayerMask)
    {
        if (null == obj)
        {
            return;
        }
        obj.layer = newLayerMask.ToLayer();
        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayerMask.ToLayer());
        }
    }
    public static void SetLayer(this GameObject obj, LayerMask newLayerMask)
    {
        obj.layer = newLayerMask.ToLayer();
    }

    public static bool RandomPossibility(float perone)
    {
        return Random.Range(0f, 1f) < perone;
    }

    public static void CustomSetActive(this GameObject _gameObject, bool b)
    {
        if (_gameObject.GetComponent<ArokaAnimParent>())
        {
            _gameObject.GetComponent<ArokaAnimParent>().SetAnimAllChildren(b);
        }
        else if (_gameObject.GetComponent<ArokaAnim>())
        {
            _gameObject.GetComponent<ArokaAnim>().SetAnim(b);
        }
        else if (_gameObject.GetComponent<SimpleAnim>())
        {
            _gameObject.GetComponent<SimpleAnim>().SetAnim(b);
        }
        else if (_gameObject.GetComponent<Animator>())
        {
            _gameObject.GetComponent<Animator>().SetBool("Param_On", b);
        }
        else
        {
            _gameObject.SetActive(b);
        }
    }
    public static int EnumCount(this Type _type)
    {
        return Enum.GetValues(_type).Length;
    }

    #endregion
   
    public static RenderersData GetRenderersData(this Renderer[] renderers)
    {
        RenderersData renderersData = new RenderersData(renderers);
        return renderersData;
    }

    public static DateTime ToDate(this string _dateString)
    {
        return DateTime.Parse(_dateString);
    }

    public static string ToDateString(this DateTime _dateTime)
    {
        return _dateTime.ToString("yyyy'/'MM'/'dd");
    }

    public static bool IsContain(this GameObject[] _list, GameObject _target)
    {
        for (int i = 0; i < _list.Length; i++)
        {
            if (_list[i] == _target)
            {
                return true;
            }
        }
        return false;
    }
    
    public static ArokaAnim ArokaAnim(this Transform _tr)
    {
        return _tr.GetComponent<ArokaAnim>();
    }
    public static List<int> GetRandomIntList(this List<int> preliminaryList, int countToSelect)
    {
        List<int> selectedList = new List<int>();

        int[] preliminaryArr = preliminaryList.ToArray();
        int preliminaryCount = preliminaryArr.Length;

        for (int i = 0; i < preliminaryCount; ++i)
        {
            int ranIdx = Random.Range(i, preliminaryCount);
            int tmp = preliminaryArr[ranIdx];
            preliminaryArr[ranIdx] = preliminaryArr[i];
            preliminaryArr[i] = tmp;
        }

        for (int i = 0; i < countToSelect; i++)
        {
            if (i < preliminaryArr.Length)
                selectedList.Add(preliminaryArr[i]);
        }

        return selectedList;
    }
    public static List<int> MixUpIntList(this List<int> preliminaryList)
    {
        int[] preliminaryArr = preliminaryList.ToArray();
        int preliminaryCount = preliminaryArr.Length;

        for (int i = 0; i < preliminaryCount; ++i)
        {
            int ranIdx = Random.Range(i, preliminaryCount);
            int tmp = preliminaryArr[ranIdx];
            preliminaryArr[ranIdx] = preliminaryArr[i];
            preliminaryArr[i] = tmp;
        }

        return preliminaryArr.ToList();
    }

    public static float SmoothPingPong(float time)
    {
        return -0.5f * Mathf.Cos(2 * Mathf.PI * time) + .5f;
    }

    public static Texture2D ToTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public static bool IsSimilar(this float f1, float f2, float min = 0.01f)
    {
        if (Mathf.Abs(f1 - f2) < min)
        {
            return true;
        }
        return false;
    }

    public static int ToLayer(this LayerMask layerMask)
    {
        int bitmask = layerMask;
        int result = bitmask > 0 ? 0 : 31;
        while (bitmask > 1)
        {
            bitmask = bitmask >> 1;
            result++;
        }
        return result;
    }

    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);

    }


    public static float SmallestFloat(this List<float> floatList)
    {
        float smallestFloat = floatList[0];
        for (int i = 0; i < floatList.Count; i++)
        {
            if (floatList[i] < smallestFloat)
            {
                smallestFloat = floatList[i];
            }
        }
        return smallestFloat;
    }

    public static float BiggestFloat(this List<float> floatList)
    {
        float biggestFloat = floatList[0];
        for (int i = 0; i < floatList.Count; i++)
        {
            if (floatList[i] > biggestFloat)
            {
                biggestFloat = floatList[i];
            }
        }
        return biggestFloat;
    }

    public static float SmallestInteger(this List<int> intList)
    {
        float smallestInt = intList[0];
        for (int i = 0; i < intList.Count; i++)
        {
            if (intList[i] < smallestInt)
            {
                smallestInt = intList[i];
            }
        }
        return smallestInt;
    }
    public static float BiggestInteger(this List<int> intList)
    {
        float biggestInt = intList[0];
        for (int i = 0; i < intList.Count; i++)
        {
            if (intList[i] > biggestInt)
            {
                biggestInt = intList[i];
            }
        }
        return biggestInt;
    }

    public static float NearestVector(List<Vector3> vector3s, Vector3 standardVec)
    {
        List<float> distList = new List<float>();
        for (int i = 0; i < vector3s.Count; i++)
        {
            distList.Add(Vector3.Distance(vector3s[i], standardVec));
        }
        return distList.SmallestFloat();
    }

    public static float FarthestVector(List<Vector3> vector3s, Vector3 standardVec)
    {
        List<float> distList = new List<float>();
        for (int i = 0; i < vector3s.Count; i++)
        {
            distList.Add(Vector3.Distance(vector3s[i], standardVec));
        }
        return distList.BiggestFloat();
    }
    public static bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition
            = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();


        EventSystem.current
        .RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
    public static bool IsColliding(Collider colA, Collider colB)
    {
        Collider[] hits = Physics.OverlapBox(colA.bounds.center, colA.bounds.extents, colA.transform.rotation);
        foreach (Collider hit in hits)
        {
            if (hit == colB)
            {
                return true;
            }
        }
        return false;
    }

    public static float RemapFloat(this float In, Vector2 InMinMax, Vector2 OutMinMax)
    {
        return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
    }

    public static Vector3 GetRenderCenterPos(Renderer[] renderers)
    {
        float minX = renderers[0].bounds.min.x;
        float minY = renderers[0].bounds.min.y;
        float minZ = renderers[0].bounds.min.z;
        float maxX = renderers[0].bounds.max.x;
        float maxY = renderers[0].bounds.max.y;
        float maxZ = renderers[0].bounds.max.z;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].bounds.min.x < minX) minX = renderers[i].bounds.min.x;
            if (renderers[i].bounds.min.y < minY) minY = renderers[i].bounds.min.y;
            if (renderers[i].bounds.min.z < minZ) minZ = renderers[i].bounds.min.z;

            if (renderers[i].bounds.max.x > maxX) maxX = renderers[i].bounds.max.x;
            if (renderers[i].bounds.max.y > maxY) maxY = renderers[i].bounds.max.y;
            if (renderers[i].bounds.max.z > maxZ) maxZ = renderers[i].bounds.max.z;
        }
        return Vector3.Lerp(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ), 0.5f);
    }

    public static ArokaTransform ArokaTr(this Transform tr)
    {
        if (tr.GetComponent<ArokaTransform>() == null)
        {
            return tr.gameObject.AddComponent<ArokaTransform>();
        }
        else
        {
            return tr.GetComponent<ArokaTransform>();
        }
    }
    public static ArokaRenderer ArokaRend(this Transform tr)
    {
        if (tr.GetComponent<ArokaRenderer>() == null)
        {
            return tr.gameObject.AddComponent<ArokaRenderer>();
        }
        else
        {
            return tr.GetComponent<ArokaRenderer>();
        }
    }


    public static Material[] GetLightObjectMaterials(this Renderer render)
    {
        List<Material> mats = new List<Material>();
        for (int j = 0; j < render.materials.Length; j++)
        {
            Material mat = render.materials[j];
            if (mat.HasProperty("_LightObject"))
            {
                if (mat.ArokaToon().IsLightObject)
                {
                    mats.Add(mat);
                }
            }
        }
        return mats.ToArray();
    }

    public static ArokaToon ArokaToon(this Material _mat)
    {
        return new ArokaToon(_mat);
    }


    public static MeshRenderer ExtractGraphicAsChild(this Transform tr)
    {
        if (tr.GetComponent<MeshRenderer>() == null)
        {
            Debug.LogWarning("추출할 메쉬렌더러가 없습니다.");
            return null;
        }
        else
        {
            GameObject copiedGraphic = new GameObject("graphic");
            copiedGraphic.gameObject.AddComponent<MeshRenderer>();
            copiedGraphic.gameObject.AddComponent<MeshFilter>();
            copiedGraphic.transform.SetParent(tr);

            copiedGraphic.GetComponent<MeshRenderer>().sharedMaterials = tr.GetComponent<MeshRenderer>().sharedMaterials;
            copiedGraphic.GetComponent<MeshFilter>().sharedMesh = tr.GetComponent<MeshFilter>().sharedMesh;

            UnityEngine.Object.DestroyImmediate(tr.GetComponent<MeshRenderer>());
            UnityEngine.Object.DestroyImmediate(tr.GetComponent<MeshFilter>());
            return copiedGraphic.GetComponent<MeshRenderer>();
        }

    }

    public static MeshRenderer RestoreGraphicAsSelf(this Transform tr)
    {
        if (tr.GetComponent<MeshRenderer>() != null)
        {
            Debug.LogWarning("이미 메쉬렌더러를 가지고있는 transform입니다.");
            return null;
        }
        else
        {
            MeshRenderer meshRenderer = tr.GetComponentInChildren<MeshRenderer>();
            MeshFilter meshFilter = tr.GetComponentInChildren<MeshFilter>();

            tr.gameObject.AddComponent<MeshRenderer>();
            tr.gameObject.AddComponent<MeshFilter>();


            tr.GetComponent<MeshRenderer>().sharedMaterials = meshRenderer.sharedMaterials;
            tr.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;

            UnityEngine.Object.DestroyImmediate(meshRenderer);
            UnityEngine.Object.DestroyImmediate(meshFilter);
            return tr.GetComponent<MeshRenderer>();
        }
    }


    public static List<Hexagon> MixUpHexagonList(this List<Hexagon> preliminaryList)
    {
        Hexagon[] preliminaryArr = preliminaryList.ToArray();
        int preliminaryCount = preliminaryArr.Length;

        for (int i = 0; i < preliminaryCount; ++i)
        {
            int ranIdx = Random.Range(i, preliminaryCount);
            Hexagon tmp = preliminaryArr[ranIdx];
            preliminaryArr[ranIdx] = preliminaryArr[i];
            preliminaryArr[i] = tmp;
        }

        return preliminaryArr.ToList();
    }

}


public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }


    public static AnimDisc GetAnimDisc(this Animator thisAnimator, AnimationClip[] animationClips)
    {
        return new AnimDisc(thisAnimator, animationClips);
    }

    public static AnimDisc GetAnimDisc(this Animator thisAnimator, AnimationClip animationClip)
    {
        AnimationClip[] animationClips = new AnimationClip[] { animationClip };
        return new AnimDisc(thisAnimator, animationClips);
    }

}


public class ArokaToon
{
    public Material thisMat;

    public ArokaToon(Material _mat)
    {
        thisMat = _mat;
    }

    public void InitializeEmission()
    {
        EmissionPerone = 0f;
    }

    public IEnumerator EmissionRoutine(Material mat, AnimationCurve curv, float totalTime = 1f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        float accumTime = 0;
        float initialEmission = 0f;
        float targetEmission = 1f;
        while (true)
        {
            accumTime += Time.deltaTime;

            float perone = accumTime / totalTime;
            float curvPerone = curv.Evaluate(perone);
            mat.ArokaToon().EmissionPerone = Mathf.Lerp(initialEmission, targetEmission, curvPerone);

            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }

    public bool WrongEffect
    {
        get
        {
            return thisMat.GetFloat("_WrongEffectPerone") == 1;
        }
        set
        {
            thisMat.SetFloat("_WrongEffectPerone", value ? 1 : 0);
        }
    }

    public float WrongEffectPerone
    {
        get
        {
            return thisMat.GetFloat("_WrongEffectPerone");
        }
        set
        {
            thisMat.SetFloat("_WrongEffectPerone", value);
        }
    }

    public Color MainColor
    {
        get
        {
            return thisMat.GetColor("_MainColor");
        }
        set
        {
            thisMat.SetColor("_MainColor", value);
        }
    }

    public float UseUVColorPerone
    {
        get
        {
            return thisMat.GetFloat("_UseUVColorPerone");
        }
        set
        {
            thisMat.SetFloat("_UseUVColorPerone", value);
        }
    }

    public bool IsLightObject
    {
        get
        {
            return thisMat.GetInt("_LightObject") == 1;
        }
        set
        {
            thisMat.SetInt("_LightObject", value == true ? 1 : 0);
        }
    }

    public bool UseEmission
    {
        get
        {
            return thisMat.GetInt("_Emission") == 1;
        }
        set
        {
            thisMat.SetInt("_Emission", value == true ? 1 : 0);
        }
    }

    public float EmissionPerone
    {
        get
        {
            return thisMat.GetFloat("_EmissionPerone");
        }
        set
        {
            thisMat.SetFloat("_EmissionPerone", value);
        }
    }

    public float EmissionIntensity
    {
        get
        {
            return thisMat.GetFloat("_EmissionIntensity");
        }
        set
        {
            thisMat.SetFloat("_EmissionIntensity", value);
        }
    }
}
public class AnimDisc
{
    public AnimDisc(Animator _animator, AnimationClip[] _clips)
    {
        animator = _animator;
        clips = _clips;
    }

    public Animator animator;
    public AnimationClip[] clips;

    public AnimationClip NowEquiedClip => animator.runtimeAnimatorController.animationClips[DiscPlayerNum];

    int DiscPlayerNum
    {
        get
        {
            return animator.GetInteger("AnimNum");
        }
        set
        {
            animator.SetInteger("AnimNum", value);
        }
    }

    public AnimationClip PlayTrackRandom(float speed = 1f)
    {
        int randomTrackNum = UnityEngine.Random.Range(0, clips.Length);
        return PlayTrack(randomTrackNum, speed);
    }

    public AnimationClip PlayTrack(int _trackNum = 0, float _speed = 1f)
    {
        int clampedTrackNum = Mathf.Clamp(_trackNum, 0, clips.Length);
        AnimationClip clip = clips[clampedTrackNum];
        int nowDiscPlayerNum = DiscPlayerNum;
        int nextDiscPlayerNum = GetNextDiscPlayerNum(nowDiscPlayerNum);
        EquipTrackOnDisc(animator, clip, nowDiscPlayerNum, nextDiscPlayerNum);
        DiscPlayerNum = nextDiscPlayerNum;
        PlayDiscPlayer(_speed);
        return clip;
    }

    int GetNextDiscPlayerNum(int nowDiscPlayerNum)
    {
        return nowDiscPlayerNum == 0 ? 1 : 0;
    }

    int EquipTrackOnDisc(Animator _animator, AnimationClip animationClip, int nowDiscPlayerNum, int nextDiscPlayerNum)
    {
        AnimatorOverrideController newAnimCon = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        AnimationClip[] nowClips = _animator.runtimeAnimatorController.animationClips;
        newAnimCon["SimpleAnimator_MasterAnim_" + nowDiscPlayerNum] = nowClips[nowDiscPlayerNum];
        newAnimCon["SimpleAnimator_MasterAnim_" + nextDiscPlayerNum] = animationClip;
        _animator.runtimeAnimatorController = newAnimCon;
        return nextDiscPlayerNum;
    }

    void PlayDiscPlayer(float speed)
    {
        animator.SetTrigger("AnimTrigger");
        animator.SetFloat("AnimSpeed", speed);
    }


}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}
public class RenderersData
{
    public Renderer[] thisRenderers;
    public RenderersData(Renderer[] renderers)
    {
        thisRenderers = renderers;
        Anaylize(renderers);
    }

    public Vector3 center;
    public Vector3 minX, minY, minZ;
    public Vector3 maxX, maxY, maxZ;
    public float distX, distY, distZ;

    void Anaylize(Renderer[] renderers)
    {
        if(renderers.Length == 0)
        {
            Debug.LogError("Hey, Renderers Length is Zero");
        }
        float __minX = renderers[0].bounds.min.x;
        float __minY = renderers[0].bounds.min.y;
        float __minZ = renderers[0].bounds.min.z;
        float __maxX = renderers[0].bounds.max.x;
        float __maxY = renderers[0].bounds.max.y;
        float __maxZ = renderers[0].bounds.max.z;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].bounds.min.x < __minX) __minX = renderers[i].bounds.min.x;
            if (renderers[i].bounds.min.y < __minY) __minY = renderers[i].bounds.min.y;
            if (renderers[i].bounds.min.z < __minZ) __minZ = renderers[i].bounds.min.z;

            if (renderers[i].bounds.max.x > __maxX) __maxX = renderers[i].bounds.max.x;
            if (renderers[i].bounds.max.y > __maxY) __maxY = renderers[i].bounds.max.y;
            if (renderers[i].bounds.max.z > __maxZ) __maxZ = renderers[i].bounds.max.z;
        }

        center = Vector3.Lerp(new Vector3(__minX, __minY, __minZ), new Vector3(__maxX, __maxY, __maxZ), 0.5f);
        distX = Vector3.Distance(Vector3.right * __minX, Vector3.right * __maxX);
        distY = Vector3.Distance(Vector3.up * __minY, Vector3.up * __maxY);
        distZ = Vector3.Distance(Vector3.forward * __minZ, Vector3.forward * __maxZ);
        maxX = Vector3.right * __maxX;
        maxY = Vector3.up * __maxY;
        maxZ = Vector3.forward * __maxZ;
        minX = Vector3.right * __minX;
        minY = Vector3.up * __minY;
        minZ = Vector3.forward * __minZ;
    }
}