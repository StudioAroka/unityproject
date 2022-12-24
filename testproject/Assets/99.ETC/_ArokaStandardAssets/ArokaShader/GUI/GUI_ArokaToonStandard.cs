#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GUI_ArokaToonStandard : ShaderGUI
{
    List<MaterialProperty> hideList = new List<MaterialProperty>();

    public enum SurfaceType
    {
        Opaque,
        Transparent
    }

    static class PropNames
    {
        public const string ShadingGradient = "_ShadingGradient";

        public const string MainColor = "_Color";
        public const string DarkColor = "_DarkColor";
        public const string ReceiveShadow = "_ReceiveShadow";

        public const string UV = "_MainTex";
        public const string UV_Tiling = "_UV_Tiling";
        public const string UV_Offset = "_UV_Offset";

        public const string UseRimLight = "_UseRimLight";
        public const string RimLightArea = "_RimLightArea";
        public const string RimLightStrength = "_RimLightStrength";
        public const string RimLightColor = "_RimLightColor";

        public const string UseHighLight = "_UseHighLight";
        public const string HighLightColor = "_HighLightColor";
        public const string HighLightStrength = "_HighLightStrength";
        public const string HighLightArea = "_HighLightArea";
        public const string UseEdgedHighLight = "_UseEdgedHighLight";
    }

    public SurfaceType surfaceType;
    public PropGCs propGCs;
    public class PropGCs
    {
        public GUIContent _MainColor;
        public GUIContent _DarkColor;
        public GUIContent _ReceiveShadow;

        public GUIContent _UV;
        public GUIContent _UV_Tiling;
        public GUIContent _UV_Offset;

        public GUIContent _UseRimLight;
        public GUIContent _RimLightColor;
        public GUIContent _RimLightArea;
        public GUIContent _RimLightStrength;

        public GUIContent _UseHighLight;
        public GUIContent _HighLightColor;
        public GUIContent _HighLightStrength;
        public GUIContent _HighLightArea;
        public GUIContent _UseEdgedHighLight;

        public PropGCs(ArokaToonStandardProps ATSprop)
        {
            _MainColor = new GUIContent(ATSprop._MainColor.displayName,"가장 밝은 상황에서의 색");
            _DarkColor = new GUIContent(ATSprop._DarkColor.displayName, "가장 어두운 상황에서의 색");
            _ReceiveShadow = new GUIContent(ATSprop._ReceiveShadow.displayName, "그림자를 받을 것인지");

            _UV = new GUIContent(ATSprop._UV.displayName, "메인 텍스쳐");
            _UV_Tiling = new GUIContent(ATSprop._UV_Tiling.displayName, "UV 타일링");
            _UV_Offset = new GUIContent(ATSprop._UV_Offset.displayName, "UV 오프셋");

            _UseRimLight = new GUIContent(ATSprop._UseRimLight.displayName, "은은한 빛 쓸것인지");
            _RimLightColor = new GUIContent(ATSprop._RimLightColor.displayName, "은은한 빛 색");
            _RimLightArea = new GUIContent(ATSprop._RimLightArea.displayName, "은은한 빛의 면적계수 power()");
            _RimLightStrength = new GUIContent(ATSprop._RimLightStrength.displayName, "은은한 빛의 색상 곱셈");

            _UseHighLight = new GUIContent(ATSprop._UseHighLight.displayName, "하이라이트 빛 쓸 것 인지");
            _HighLightColor = new GUIContent(ATSprop._HighLightColor.displayName, "하이라이트 색");
            _HighLightArea = new GUIContent(ATSprop._HighLightArea.displayName, "하이라이트 빛 면적 계수 power()");
            _HighLightStrength = new GUIContent(ATSprop._HighLightStrength.displayName, "하이라이트 나타냄 곱셈");
            _UseEdgedHighLight = new GUIContent(ATSprop._UseEdgedHighLight.displayName, "원형으로 딱 떨어지는 하이라이트");
        }

    }

    public ArokaToonStandardProps ATSprop;
    public struct ArokaToonStandardProps
    {
        public MaterialProperty _MainColor;
        public MaterialProperty _DarkColor;
        public MaterialProperty _ReceiveShadow;

        public MaterialProperty _UV;
        public MaterialProperty _UV_Tiling;
        public MaterialProperty _UV_Offset;

        public MaterialProperty _UseRimLight;
        public MaterialProperty _RimLightColor;
        public MaterialProperty _RimLightArea;
        public MaterialProperty _RimLightStrength;

        public MaterialProperty _UseHighLight;
        public MaterialProperty _HighLightColor;
        public MaterialProperty _HighLightStrength;
        public MaterialProperty _HighLightArea;
        public MaterialProperty _UseEdgedHighLight;

     
        public ArokaToonStandardProps(MaterialProperty[] properties)
        {
            _MainColor = ShaderGUI.FindProperty(PropNames.MainColor, properties);
            _DarkColor = ShaderGUI.FindProperty(PropNames.DarkColor, properties);
            _ReceiveShadow = ShaderGUI.FindProperty(PropNames.ReceiveShadow, properties);

            _UV = ShaderGUI.FindProperty(PropNames.UV, properties);
            _UV_Tiling = ShaderGUI.FindProperty(PropNames.UV_Tiling, properties);
            _UV_Offset = ShaderGUI.FindProperty(PropNames.UV_Offset, properties);

            _UseRimLight = ShaderGUI.FindProperty(PropNames.UseRimLight, properties);
            _RimLightColor = ShaderGUI.FindProperty(PropNames.RimLightColor, properties);
            _RimLightArea = ShaderGUI.FindProperty(PropNames.RimLightArea, properties);
            _RimLightStrength = ShaderGUI.FindProperty(PropNames.RimLightStrength, properties);

            _UseHighLight = ShaderGUI.FindProperty(PropNames.UseHighLight, properties);
            _HighLightColor = ShaderGUI.FindProperty(PropNames.HighLightColor, properties);
            _HighLightArea = ShaderGUI.FindProperty(PropNames.HighLightArea, properties);
            _HighLightStrength = ShaderGUI.FindProperty(PropNames.HighLightStrength, properties);
            _UseEdgedHighLight = ShaderGUI.FindProperty(PropNames.UseEdgedHighLight, properties);
        }
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        hideList.Clear();
        ATSprop = new ArokaToonStandardProps(properties);
        propGCs = new PropGCs(ATSprop);

        GUIStyle labelStyle = new GUIStyle
        {
            fontSize = 15
        };
        
        EditorGUILayout.Space(20);
        GUILayout.Label( "색상",labelStyle);
        materialEditor.ShaderProperty(ATSprop._MainColor, propGCs._MainColor);
        materialEditor.ShaderProperty(ATSprop._DarkColor, propGCs._DarkColor);

        EditorGUILayout.Space(20);
        GUILayout.Label("유브이", labelStyle);
        materialEditor.ShaderProperty(ATSprop._UV, propGCs._UV);
        bool isUVused = ATSprop._UV.textureValue != null;
        if (isUVused)
        {
            materialEditor.ShaderProperty(ATSprop._UV_Tiling, propGCs._UV_Tiling);
            materialEditor.ShaderProperty(ATSprop._UV_Offset, propGCs._UV_Offset);
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("림라이트", labelStyle);
        materialEditor.ShaderProperty(ATSprop._UseRimLight, propGCs._UseRimLight);
        bool isUseRimLight = ATSprop._UseRimLight.floatValue == 1;
        if (isUseRimLight)
        {
            materialEditor.ShaderProperty(ATSprop._RimLightColor, propGCs._RimLightColor);
            materialEditor.ShaderProperty(ATSprop._RimLightArea, propGCs._RimLightArea);
            materialEditor.ShaderProperty(ATSprop._RimLightStrength, propGCs._RimLightStrength);
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("하이라이트", labelStyle);
        materialEditor.ShaderProperty(ATSprop._UseHighLight, propGCs._UseHighLight);
        bool isUseHighLight = ATSprop._UseHighLight.floatValue == 1;
        if (isUseHighLight)
        {
            materialEditor.ShaderProperty(ATSprop._HighLightColor, propGCs._HighLightColor);
            materialEditor.ShaderProperty(ATSprop._HighLightArea, propGCs._HighLightArea);
            materialEditor.ShaderProperty(ATSprop._HighLightStrength, propGCs._HighLightStrength);
            materialEditor.ShaderProperty(ATSprop._UseEdgedHighLight, propGCs._UseEdgedHighLight);
        }


        EditorGUILayout.Space(20);
        GUILayout.Label("추가옵션", labelStyle);
        materialEditor.ShaderProperty(ATSprop._ReceiveShadow, propGCs._ReceiveShadow);
        materialEditor.EnableInstancingField();
        materialEditor.RenderQueueField();


        /*
        GUILayout.Label("SurfaceType", EditorStyles.boldLabel);
        surfaceType = (GUI_ArokaToonStandard.SurfaceType)EditorGUILayout.EnumPopup(surfaceType);
        */

    }

}
#endif


/*
   public ArokaToonStandardProps arokaToonStandardProps;
   public struct ArokaToonStandardProps
   {
       public MaterialProperty _UseRimLight;
       public MaterialProperty _RimLightColor;
       public MaterialProperty _RimLightArea;
       public MaterialProperty _RimLightStrength;

       public MaterialProperty _UseHighLight;
       public MaterialProperty _HighLightColor;
       public MaterialProperty _HighLightStrength;
       public MaterialProperty _HighLightArea;
       public MaterialProperty _UseEdgedHighLight;

       public MaterialProperty _UV;
       public MaterialProperty _UV_Tiling;
       public MaterialProperty _UV_Offset; 

       public ArokaToonStandardProps(MaterialProperty[] properties)
       {
           _UseRimLight = ShaderGUI.FindProperty(PropNames.UseRimLight, properties);
           _RimLightColor = ShaderGUI.FindProperty(PropNames.RimLightColor, properties);
           _RimLightArea = ShaderGUI.FindProperty(PropNames.RimLightArea, properties);
           _RimLightStrength = ShaderGUI.FindProperty(PropNames.RimLightStrength, properties);

           _UseHighLight = ShaderGUI.FindProperty(PropNames.UseHighLight, properties);
           _HighLightColor = ShaderGUI.FindProperty(PropNames.HighLightColor, properties);
           _HighLightStrength = ShaderGUI.FindProperty(PropNames.HighLightStrength, properties);
           _HighLightArea = ShaderGUI.FindProperty(PropNames.HighLightArea, properties);
           _UseEdgedHighLight = ShaderGUI.FindProperty(PropNames.UseEdgedHighLight, properties);

           _UV = ShaderGUI.FindProperty(PropNames.UV, properties);
           _UV_Tiling = ShaderGUI.FindProperty(PropNames.UV_Tiling, properties);
           _UV_Offset = ShaderGUI.FindProperty(PropNames.UV_Offset, properties);
       }
   }


   public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
   {
       Debug.Log("쉐이더 + " + material + " / " + oldShader + " / " + newShader);
   }*/
