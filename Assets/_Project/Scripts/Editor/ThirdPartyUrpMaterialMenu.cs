using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CarTrickRush.Editor
{
    /// =========================================================================================
    /// <summary>
    /// サードパーティの Built-in 向けマテリアルを URP 用に一括変換する.
    /// </summary>
    /// =========================================================================================
    public static class ThirdPartyUrpMaterialMenu
    {
        private const string _vehiclePhysicsRoot = "Assets/ThirdParty/Vehicle Physics Pro";
        private const string _bedrillRoot = "Assets/ThirdParty/BEDRILL";

        private static readonly int _baseMapId = Shader.PropertyToID("_BaseMap");
        private static readonly int _baseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int _bumpMapId = Shader.PropertyToID("_BumpMap");
        private static readonly int _bumpScaleId = Shader.PropertyToID("_BumpScale");
        private static readonly int _metallicGlossMapId = Shader.PropertyToID("_MetallicGlossMap");
        private static readonly int _metallicId = Shader.PropertyToID("_Metallic");
        private static readonly int _smoothnessId = Shader.PropertyToID("_Smoothness");
        private static readonly int _emissionMapId = Shader.PropertyToID("_EmissionMap");
        private static readonly int _emissionColorId = Shader.PropertyToID("_EmissionColor");
        private static readonly int _surfaceId = Shader.PropertyToID("_Surface");
        private static readonly int _blendId = Shader.PropertyToID("_Blend");
        private static readonly int _alphaClipId = Shader.PropertyToID("_AlphaClip");
        private static readonly int _cutoffId = Shader.PropertyToID("_Cutoff");
        private static readonly int _zWriteId = Shader.PropertyToID("_ZWrite");
        private static readonly int _srcBlendId = Shader.PropertyToID("_SrcBlend");
        private static readonly int _dstBlendId = Shader.PropertyToID("_DstBlend");

        #region ------------------ Public Methods ------------------

        [MenuItem("CarTrickRush/Rendering/Vehicle Physics Pro マテリアルを URP に変換")]
        private static void ConvertVehiclePhysicsProMaterials()
        {
            ConvertMaterialsUnderRoot(_vehiclePhysicsRoot, "Vehicle Physics Pro", skipWorldUvMapMaterials: true);
        }

        [MenuItem("CarTrickRush/Rendering/BEDRILL マテリアルを URP に変換")]
        private static void ConvertBedrillMaterials()
        {
            ConvertMaterialsUnderRoot(_bedrillRoot, "BEDRILL", skipWorldUvMapMaterials: false);
        }

        #endregion

        #region ------------------ Private Methods ------------------

        private static void ConvertMaterialsUnderRoot(string assetsRoot, string logLabel, bool skipWorldUvMapMaterials)
        {
            var urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit == null)
            {
                Debug.LogError("[CarTrickRush] URP Lit シェーダーが見つかりません。プロジェクトが URP か確認してください。");
                return;
            }

            if (!AssetDatabase.IsValidFolder(assetsRoot))
            {
                Debug.LogWarning($"[CarTrickRush] フォルダが見つかりません: {assetsRoot}");
                return;
            }

            var skybox6 = Shader.Find("Skybox/6 Sided");
            var guids = AssetDatabase.FindAssets("t:Material", new[] { assetsRoot });
            var converted = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null)
                {
                    continue;
                }

                if (ShouldSkipShader(mat.shader))
                {
                    continue;
                }

                if (skipWorldUvMapMaterials && IsWorldUvMapMaterial(mat))
                {
                    continue;
                }

                if (TryConvertSkyboxMaterial(mat, skybox6))
                {
                    EditorUtility.SetDirty(mat);
                    converted++;
                    continue;
                }

                if (mat.shader != null && mat.shader.name.StartsWith("Skybox/", StringComparison.Ordinal))
                {
                    continue;
                }

                ConvertBuiltInLikeMaterial(mat, urpLit);
                EditorUtility.SetDirty(mat);
                converted++;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[CarTrickRush] {logLabel}: {converted} 件のマテリアルを更新しました（スキップ・既存 URP は除く）。");
        }

        private static bool ShouldSkipShader(Shader shader)
        {
            if (shader == null)
            {
                return false;
            }

            var name = shader.name;
            return name.StartsWith("Universal Render Pipeline/", StringComparison.Ordinal)
                   || name.StartsWith("Shader Graphs/", StringComparison.Ordinal)
                   || name.StartsWith("UI/", StringComparison.Ordinal);
        }

        private static bool IsWorldUvMapMaterial(Material mat)
        {
            if (mat.shader != null && mat.shader.name == "Custom/World UV Map")
            {
                return true;
            }

            return GetSerializedTexture(mat, "_MainTexWall") != null
                   && GetSerializedTexture(mat, "_MainTexWall2") != null
                   && GetSerializedTexture(mat, "_MainTexFlr2") != null;
        }

        private static bool TryConvertSkyboxMaterial(Material mat, Shader skybox6)
        {
            if (skybox6 == null)
            {
                return false;
            }

            var shaderName = mat.shader != null ? mat.shader.name : string.Empty;
            if (shaderName == skybox6.name)
            {
                return false;
            }

            var looksLikeSixSided = GetSerializedTexture(mat, "_FrontTex") != null
                                    && GetSerializedTexture(mat, "_UpTex") != null;
            if (shaderName != "Skybox/6 Sided" && !looksLikeSixSided)
            {
                return false;
            }

            mat.shader = skybox6;
            return true;
        }

        private static void ConvertBuiltInLikeMaterial(Material mat, Shader urpLit)
        {
            var mainTex = GetSerializedTexture(mat, "_MainTex");
            var bumpMap = GetSerializedTexture(mat, "_BumpMap");
            var metallicGloss = GetSerializedTexture(mat, "_MetallicGlossMap");
            var emissionMap = GetSerializedTexture(mat, "_EmissionMap");

            var baseColor = GetSerializedColor(mat, "_Color", Color.white);
            var emissionColor = GetSerializedColor(mat, "_EmissionColor", Color.black);
            var metallic = GetSerializedFloat(mat, "_Metallic", 0f);
            var smoothness = GetSerializedFloat(mat, "_Glossiness", 0.5f);
            var bumpScale = GetSerializedFloat(mat, "_BumpScale", 1f);
            var cutoff = GetSerializedFloat(mat, "_Cutoff", 0.5f);
            var mode = Mathf.RoundToInt(GetSerializedFloat(mat, "_Mode", 0f));

            var keywords = mat.shader != null ? mat.shaderKeywords : Array.Empty<string>();
            var hadAlphaPremultiply = Array.IndexOf(keywords, "_ALPHAPREMULTIPLY_ON") >= 0;
            var hadAlphaBlend = Array.IndexOf(keywords, "_ALPHABLEND_ON") >= 0;
            var hadAlphaTest = Array.IndexOf(keywords, "_ALPHATEST_ON") >= 0;

            mat.shader = urpLit;
            mat.shaderKeywords = null;

            if (mainTex != null)
            {
                mat.SetTexture(_baseMapId, mainTex);
            }

            mat.SetColor(_baseColorId, baseColor);

            if (bumpMap != null)
            {
                mat.EnableKeyword("_NORMALMAP");
                mat.SetTexture(_bumpMapId, bumpMap);
                mat.SetFloat(_bumpScaleId, bumpScale);
            }

            if (metallicGloss != null)
            {
                mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                mat.SetTexture(_metallicGlossMapId, metallicGloss);
            }

            mat.SetFloat(_metallicId, metallic);
            mat.SetFloat(_smoothnessId, smoothness);

            if (emissionMap != null)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetTexture(_emissionMapId, emissionMap);
            }

            if (emissionColor.maxColorComponent > 0.001f || emissionMap != null)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor(_emissionColorId, emissionColor);
            }

            var transparent = mode == 2 || mode == 3 || hadAlphaBlend || hadAlphaPremultiply;
            var alphaClip = hadAlphaTest || mode == 1;

            if (alphaClip)
            {
                mat.SetFloat(_alphaClipId, 1f);
                mat.SetFloat(_cutoffId, cutoff);
                mat.EnableKeyword("_ALPHATEST_ON");
                mat.renderQueue = (int)RenderQueue.AlphaTest;
                mat.SetOverrideTag("RenderType", "TransparentCutout");
            }
            else if (transparent)
            {
                mat.SetFloat(_surfaceId, 1f);
                mat.SetFloat(_blendId, hadAlphaPremultiply ? 1f : 0f);
                mat.SetFloat(_zWriteId, 0f);
                mat.SetFloat(_srcBlendId, (float)BlendMode.SrcAlpha);
                mat.SetFloat(_dstBlendId, (float)BlendMode.OneMinusSrcAlpha);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                if (hadAlphaPremultiply)
                {
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                }

                mat.renderQueue = (int)RenderQueue.Transparent;
                mat.SetOverrideTag("RenderType", "Transparent");
            }
            else
            {
                mat.SetFloat(_surfaceId, 0f);
                mat.SetFloat(_zWriteId, 1f);
                mat.renderQueue = (int)RenderQueue.Geometry;
                mat.SetOverrideTag("RenderType", "Opaque");
            }
        }

        private static Texture GetSerializedTexture(Material mat, string slotName)
        {
            if (mat.HasProperty(slotName))
            {
                return mat.GetTexture(slotName);
            }

            using (var so = new SerializedObject(mat))
            {
                so.Update();
                var texEnvs = so.FindProperty("m_SavedProperties.m_TexEnvs");
                if (texEnvs == null || !texEnvs.isArray)
                {
                    return null;
                }

                for (var i = 0; i < texEnvs.arraySize; i++)
                {
                    var el = texEnvs.GetArrayElementAtIndex(i);
                    var fromPair = TryGetTexturePairFormat(el, slotName);
                    if (fromPair != null)
                    {
                        return fromPair;
                    }

                    var fromFlat = TryGetTextureFlatFormat(el, slotName);
                    if (fromFlat != null)
                    {
                        return fromFlat;
                    }
                }
            }

            return null;
        }

        private static Texture TryGetTexturePairFormat(SerializedProperty element, string slotName)
        {
            var first = element.FindPropertyRelative("first");
            if (first == null)
            {
                return null;
            }

            var nameProp = first.FindPropertyRelative("name");
            if (nameProp == null || nameProp.stringValue != slotName)
            {
                return null;
            }

            var second = element.FindPropertyRelative("second");
            var texRef = second?.FindPropertyRelative("m_Texture");
            return texRef != null ? texRef.objectReferenceValue as Texture : null;
        }

        private static Texture TryGetTextureFlatFormat(SerializedProperty element, string slotName)
        {
            var block = element.FindPropertyRelative(slotName);
            if (block == null)
            {
                return null;
            }

            var texRef = block.FindPropertyRelative("m_Texture");
            return texRef != null ? texRef.objectReferenceValue as Texture : null;
        }

        private static float GetSerializedFloat(Material mat, string name, float defaultValue)
        {
            if (mat.HasProperty(name))
            {
                return mat.GetFloat(name);
            }

            using (var so = new SerializedObject(mat))
            {
                so.Update();
                var floats = so.FindProperty("m_SavedProperties.m_Floats");
                if (floats == null || !floats.isArray)
                {
                    return defaultValue;
                }

                for (var i = 0; i < floats.arraySize; i++)
                {
                    var el = floats.GetArrayElementAtIndex(i);
                    var fromPair = TryGetFloatPairFormat(el, name);
                    if (fromPair.HasValue)
                    {
                        return fromPair.Value;
                    }

                    var v = el.FindPropertyRelative(name);
                    if (v != null && v.propertyType == SerializedPropertyType.Float)
                    {
                        return v.floatValue;
                    }
                }
            }

            return defaultValue;
        }

        private static float? TryGetFloatPairFormat(SerializedProperty element, string propName)
        {
            var first = element.FindPropertyRelative("first");
            if (first == null)
            {
                return null;
            }

            var nameProp = first.FindPropertyRelative("name");
            if (nameProp == null || nameProp.stringValue != propName)
            {
                return null;
            }

            var second = element.FindPropertyRelative("second");
            return second != null ? second.floatValue : null;
        }

        private static Color GetSerializedColor(Material mat, string name, Color defaultValue)
        {
            if (mat.HasProperty(name))
            {
                return mat.GetColor(name);
            }

            using (var so = new SerializedObject(mat))
            {
                so.Update();
                var colors = so.FindProperty("m_SavedProperties.m_Colors");
                if (colors == null || !colors.isArray)
                {
                    return defaultValue;
                }

                for (var i = 0; i < colors.arraySize; i++)
                {
                    var el = colors.GetArrayElementAtIndex(i);
                    var fromPair = TryGetColorPairFormat(el, name);
                    if (fromPair.HasValue)
                    {
                        return fromPair.Value;
                    }

                    var v = el.FindPropertyRelative(name);
                    if (v != null && v.propertyType == SerializedPropertyType.Color)
                    {
                        return v.colorValue;
                    }
                }
            }

            return defaultValue;
        }

        private static Color? TryGetColorPairFormat(SerializedProperty element, string propName)
        {
            var first = element.FindPropertyRelative("first");
            if (first == null)
            {
                return null;
            }

            var nameProp = first.FindPropertyRelative("name");
            if (nameProp == null || nameProp.stringValue != propName)
            {
                return null;
            }

            var second = element.FindPropertyRelative("second");
            return second != null ? second.colorValue : null;
        }

        #endregion
    }
}
