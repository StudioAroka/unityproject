using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[RequireComponent(typeof(MeshCollider))]
public class DrawableMesh : MonoBehaviour
{
    [System.Serializable]
    public class MaterialShaderData
    {
        public int index;
        public TexData[] texDatas;
        public bool IsHaveDrawable_Tex
        {
            get
            {
                for (int i = 0; i < texDatas.Length; i++)
                {
                    if (texDatas[i].isDrawable)
                        return true;
                }
                return false;
            }
        }
    }

    [System.Serializable]
    public class TexData
    {
        public string texName;
        public bool isDrawable;
        [ShowIf("isDrawable")]
        public TextureGroup textureGroup;

        public TexData(string _texName)
        {
            isDrawable = false;
            texName = _texName;
        }
    }

    //그리고 싶은 메테리얼의 텍스쳐의 이름
    public Renderer thisRender;
    public MeshCollider thisMeshCol;
    public List<MaterialShaderData> materialShaderDatas;

    private void Awake()
    {
        thisRender = transform.GetComponent<Renderer>();
    }

    public void InitializeDrawableMesh()
    {
        DrawableMeshManager.Instance.textureGroupHistroies = new List<TextureGroupHistroy>();
        thisRender.materials = CopiedMaterials(thisRender.materials);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        Bake();
    }

    [Button("Analytics Materials", ButtonSizes.Gigantic)]
    public void Bake()
    {
        thisRender = transform.GetComponent<Renderer>();
        thisMeshCol = transform.GetComponent<MeshCollider>();
        List<MaterialShaderData> tmp_MaterialShaderData = new List<MaterialShaderData>();
        for (int i = 0; i < thisRender.sharedMaterials.Length; i++)
        {
            MaterialShaderData made_DrawField = new MaterialShaderData();
            Material iMat = thisRender.sharedMaterials[i];

            Shader _shader = iMat.shader;
            List<TexData> tmpTexPlans = new List<TexData>();
            int count = ShaderUtil.GetPropertyCount(_shader);

            for (int j = 0; j < count; j++)
            {
                string st = ShaderUtil.GetPropertyName(_shader, j);
                tmpTexPlans.Add(new TexData(st));
            }
            made_DrawField.index = i;
            made_DrawField.texDatas = tmpTexPlans.ToArray();
            tmp_MaterialShaderData.Add(made_DrawField);
        }
        materialShaderDatas = tmp_MaterialShaderData;
    }

    public void AutoDrawableChecker(TextureGroup textureGroup)
    {
        for (int i = 0; i < materialShaderDatas.Count; i++)
        {
            Material inst_Mat = Instantiate(thisRender.sharedMaterials[i]);
            for (int j = 0; j < materialShaderDatas[i].texDatas.Length; j++)
            {
                if (materialShaderDatas[i].texDatas[j].texName.Contains("Drawable"))
                {
                    materialShaderDatas[i].texDatas[j].isDrawable = true;
                    materialShaderDatas[i].texDatas[j].textureGroup = textureGroup;
                }
            }
        }
    }
#endif


    //메테리얼을 복제하고 메테리얼속 texture도 복제해서 shared와 완전히 연결을 끊는다.
    Material[] CopiedMaterials(Material[] mats)
    {
        List<Material> newMats = new List<Material>();
        for (int i = 0; i < materialShaderDatas.Count; i++)
        {
            Material inst_Mat = Instantiate(thisRender.sharedMaterials[i]);
            newMats.Add(inst_Mat);

            MaterialShaderData materialShaderData = materialShaderDatas[i];
            bool isNeedDelink = materialShaderData.IsHaveDrawable_Tex;
            if (isNeedDelink)
            {
                WriteCopiedDataOnMaterial(inst_Mat, materialShaderData);
            }
            else
            {

            }
        }
        return newMats.ToArray();
    }
    void WriteCopiedDataOnMaterial(Material inst_mat, MaterialShaderData materialShaderData)
    {
        for (int j = 0; j < materialShaderData.texDatas.Length; j++)
        {
            if (materialShaderData.texDatas[j].isDrawable)
            {
                string _texName = materialShaderData.texDatas[j].texName;
                TextureGroup _Group = materialShaderData.texDatas[j].textureGroup;
                Texture2D originTex = inst_mat.GetTexture(_texName) as Texture2D;
                Texture2D targetTex;
                if (_Group == TextureGroup.None)
                {
                    targetTex = Instantiate(originTex);
                    targetTex.name = "NoneTexture";
                }
                else
                {
                    Texture2D texture2DfromHistory = DrawableMeshManager.Instance.GetTexture2D_InHistory(_Group);
                    if (texture2DfromHistory == null)
                    {
                        Texture2D instance_OriginTex = Instantiate(originTex);
                        instance_OriginTex.name = "EmptyAlpha_ " + _Group.ToString();
                        DrawableMeshManager.Instance.RegistTextureGroupHistory(_Group, instance_OriginTex);
                    }
                    targetTex = DrawableMeshManager.Instance.GetTexture2D_InHistory(_Group);
                }

                inst_mat.SetTexture(_texName, targetTex);
            }
            else
            {

            }
        }
    }


}