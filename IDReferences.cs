﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityProjectDumper
{
    internal static class IDReferences
    {
        public static Tuple<string, string>[] list = new Tuple<string, string>[]
        {
            Tuple.Create("0", "Object"),
            Tuple.Create("1", "GameObject"),
            Tuple.Create("2", "Component"),
            Tuple.Create("3", "LevelGameManager"),
            Tuple.Create("4", "Transform"),
            Tuple.Create("5", "TimeManager"),
            Tuple.Create("6", "GlobalGameManager"),
            Tuple.Create("8", "Behaviour"),
            Tuple.Create("9", "GameManager"),
            Tuple.Create("11", "AudioManager"),
            Tuple.Create("13", "InputManager"),
            Tuple.Create("18", "EditorExtension"),
            Tuple.Create("19", "Physics2DSettings"),
            Tuple.Create("20", "Camera"),
            Tuple.Create("21", "Material"),
            Tuple.Create("23", "MeshRenderer"),
            Tuple.Create("25", "Renderer"),
            Tuple.Create("27", "Texture"),
            Tuple.Create("28", "Texture2D"),
            Tuple.Create("29", "OcclusionCullingSettings"),
            Tuple.Create("30", "GraphicsSettings"),
            Tuple.Create("33", "MeshFilter"),
            Tuple.Create("41", "OcclusionPortal"),
            Tuple.Create("43", "Mesh"),
            Tuple.Create("45", "Skybox"),
            Tuple.Create("47", "QualitySettings"),
            Tuple.Create("48", "Shader"),
            Tuple.Create("49", "TextAsset"),
            Tuple.Create("50", "Rigidbody2D"),
            Tuple.Create("53", "Collider2D"),
            Tuple.Create("54", "Rigidbody"),
            Tuple.Create("55", "PhysicsManager"),
            Tuple.Create("56", "Collider"),
            Tuple.Create("57", "Joint"),
            Tuple.Create("58", "CircleCollider2D"),
            Tuple.Create("59", "HingeJoint"),
            Tuple.Create("60", "PolygonCollider2D"),
            Tuple.Create("61", "BoxCollider2D"),
            Tuple.Create("62", "PhysicsMaterial2D"),
            Tuple.Create("64", "MeshCollider"),
            Tuple.Create("65", "BoxCollider"),
            Tuple.Create("66", "CompositeCollider2D"),
            Tuple.Create("68", "EdgeCollider2D"),
            Tuple.Create("70", "CapsuleCollider2D"),
            Tuple.Create("72", "ComputeShader"),
            Tuple.Create("74", "AnimationClip"),
            Tuple.Create("75", "ConstantForce"),
            Tuple.Create("78", "TagManager"),
            Tuple.Create("81", "AudioListener"),
            Tuple.Create("82", "AudioSource"),
            Tuple.Create("83", "AudioClip"),
            Tuple.Create("84", "RenderTexture"),
            Tuple.Create("86", "CustomRenderTexture"),
            Tuple.Create("89", "Cubemap"),
            Tuple.Create("90", "Avatar"),
            Tuple.Create("91", "AnimatorController"),
            Tuple.Create("93", "RuntimeAnimatorController"),
            Tuple.Create("94", "ScriptMapper"),
            Tuple.Create("95", "Animator"),
            Tuple.Create("96", "TrailRenderer"),
            Tuple.Create("98", "DelayedCallManager"),
            Tuple.Create("102", "TextMesh"),
            Tuple.Create("104", "RenderSettings"),
            Tuple.Create("108", "Light"),
            Tuple.Create("109", "CGProgram"),
            Tuple.Create("110", "BaseAnimationTrack"),
            Tuple.Create("111", "Animation"),
            Tuple.Create("114", "MonoBehaviour"),
            Tuple.Create("115", "MonoScript"),
            Tuple.Create("116", "MonoManager"),
            Tuple.Create("117", "Texture3D"),
            Tuple.Create("118", "NewAnimationTrack"),
            Tuple.Create("119", "Projector"),
            Tuple.Create("120", "LineRenderer"),
            Tuple.Create("121", "Flare"),
            Tuple.Create("122", "Halo"),
            Tuple.Create("123", "LensFlare"),
            Tuple.Create("124", "FlareLayer"),
            Tuple.Create("125", "HaloLayer"),
            Tuple.Create("126", "NavMeshProjectSettings"),
            Tuple.Create("128", "Font"),
            Tuple.Create("129", "PlayerSettings"),
            Tuple.Create("130", "NamedObject"),
            Tuple.Create("134", "PhysicMaterial"),
            Tuple.Create("135", "SphereCollider"),
            Tuple.Create("136", "CapsuleCollider"),
            Tuple.Create("137", "SkinnedMeshRenderer"),
            Tuple.Create("138", "FixedJoint"),
            Tuple.Create("141", "BuildSettings"),
            Tuple.Create("142", "AssetBundle"),
            Tuple.Create("143", "CharacterController"),
            Tuple.Create("144", "CharacterJoint"),
            Tuple.Create("145", "SpringJoint"),
            Tuple.Create("146", "WheelCollider"),
            Tuple.Create("147", "ResourceManager"),
            Tuple.Create("150", "PreloadData"),
            Tuple.Create("153", "ConfigurableJoint"),
            Tuple.Create("154", "TerrainCollider"),
            Tuple.Create("156", "TerrainData"),
            Tuple.Create("157", "LightmapSettings"),
            Tuple.Create("158", "WebCamTexture"),
            Tuple.Create("159", "EditorSettings"),
            Tuple.Create("162", "EditorUserSettings"),
            Tuple.Create("164", "AudioReverbFilter"),
            Tuple.Create("165", "AudioHighPassFilter"),
            Tuple.Create("166", "AudioChorusFilter"),
            Tuple.Create("167", "AudioReverbZone"),
            Tuple.Create("168", "AudioEchoFilter"),
            Tuple.Create("169", "AudioLowPassFilter"),
            Tuple.Create("170", "AudioDistortionFilter"),
            Tuple.Create("171", "SparseTexture"),
            Tuple.Create("180", "AudioBehaviour"),
            Tuple.Create("181", "AudioFilter"),
            Tuple.Create("182", "WindZone"),
            Tuple.Create("183", "Cloth"),
            Tuple.Create("184", "SubstanceArchive"),
            Tuple.Create("185", "ProceduralMaterial"),
            Tuple.Create("186", "ProceduralTexture"),
            Tuple.Create("187", "Texture2DArray"),
            Tuple.Create("188", "CubemapArray"),
            Tuple.Create("191", "OffMeshLink"),
            Tuple.Create("192", "OcclusionArea"),
            Tuple.Create("193", "Tree"),
            Tuple.Create("195", "NavMeshAgent"),
            Tuple.Create("196", "NavMeshSettings"),
            Tuple.Create("198", "ParticleSystem"),
            Tuple.Create("199", "ParticleSystemRenderer"),
            Tuple.Create("200", "ShaderVariantCollection"),
            Tuple.Create("205", "LODGroup"),
            Tuple.Create("206", "BlendTree"),
            Tuple.Create("207", "Motion"),
            Tuple.Create("208", "NavMeshObstacle"),
            Tuple.Create("210", "SortingGroup"),
            Tuple.Create("212", "SpriteRenderer"),
            Tuple.Create("213", "Sprite"),
            Tuple.Create("214", "CachedSpriteAtlas"),
            Tuple.Create("215", "ReflectionProbe"),
            Tuple.Create("218", "Terrain"),
            Tuple.Create("220", "LightProbeGroup"),
            Tuple.Create("221", "AnimatorOverrideController"),
            Tuple.Create("222", "CanvasRenderer"),
            Tuple.Create("223", "Canvas"),
            Tuple.Create("224", "RectTransform"),
            Tuple.Create("225", "CanvasGroup"),
            Tuple.Create("226", "BillboardAsset"),
            Tuple.Create("227", "BillboardRenderer"),
            Tuple.Create("228", "SpeedTreeWindAsset"),
            Tuple.Create("229", "AnchoredJoint2D"),
            Tuple.Create("230", "Joint2D"),
            Tuple.Create("231", "SpringJoint2D"),
            Tuple.Create("232", "DistanceJoint2D"),
            Tuple.Create("233", "HingeJoint2D"),
            Tuple.Create("234", "SliderJoint2D"),
            Tuple.Create("235", "WheelJoint2D"),
            Tuple.Create("236", "ClusterInputManager"),
            Tuple.Create("237", "BaseVideoTexture"),
            Tuple.Create("238", "NavMeshData"),
            Tuple.Create("240", "AudioMixer"),
            Tuple.Create("241", "AudioMixerController"),
            Tuple.Create("243", "AudioMixerGroupController"),
            Tuple.Create("244", "AudioMixerEffectController"),
            Tuple.Create("245", "AudioMixerSnapshotController"),
            Tuple.Create("246", "PhysicsUpdateBehaviour2D"),
            Tuple.Create("247", "ConstantForce2D"),
            Tuple.Create("248", "Effector2D"),
            Tuple.Create("249", "AreaEffector2D"),
            Tuple.Create("250", "PointEffector2D"),
            Tuple.Create("251", "PlatformEffector2D"),
            Tuple.Create("252", "SurfaceEffector2D"),
            Tuple.Create("253", "BuoyancyEffector2D"),
            Tuple.Create("254", "RelativeJoint2D"),
            Tuple.Create("255", "FixedJoint2D"),
            Tuple.Create("256", "FrictionJoint2D"),
            Tuple.Create("257", "TargetJoint2D"),
            Tuple.Create("258", "LightProbes"),
            Tuple.Create("259", "LightProbeProxyVolume"),
            Tuple.Create("271", "SampleClip"),
            Tuple.Create("272", "AudioMixerSnapshot"),
            Tuple.Create("273", "AudioMixerGroup"),
            Tuple.Create("290", "AssetBundleManifest"),
            Tuple.Create("300", "RuntimeInitializeOnLoadManager"),
            Tuple.Create("310", "UnityConnectSettings"),
            Tuple.Create("319", "AvatarMask"),
            Tuple.Create("320", "PlayableDirector"),
            Tuple.Create("328", "VideoPlayer"),
            Tuple.Create("329", "VideoClip"),
            Tuple.Create("330", "ParticleSystemForceField"),
            Tuple.Create("331", "SpriteMask"),
            Tuple.Create("362", "WorldAnchor"),
            Tuple.Create("363", "OcclusionCullingData"),
            Tuple.Create("1001", "PrefabInstance"),
            Tuple.Create("1002", "EditorExtensionImpl"),
            Tuple.Create("1003", "AssetImporter"),
            Tuple.Create("1004", "AssetDatabaseV1"),
            Tuple.Create("1005", "Mesh3DSImporter"),
            Tuple.Create("1006", "TextureImporter"),
            Tuple.Create("1007", "ShaderImporter"),
            Tuple.Create("1008", "ComputeShaderImporter"),
            Tuple.Create("1020", "AudioImporter"),
            Tuple.Create("1026", "HierarchyState"),
            Tuple.Create("1028", "AssetMetaData"),
            Tuple.Create("1029", "DefaultAsset"),
            Tuple.Create("1030", "DefaultImporter"),
            Tuple.Create("1031", "TextScriptImporter"),
            Tuple.Create("1032", "SceneAsset"),
            Tuple.Create("1034", "NativeFormatImporter"),
            Tuple.Create("1035", "MonoImporter"),
            Tuple.Create("1038", "LibraryAssetImporter"),
            Tuple.Create("1040", "ModelImporter"),
            Tuple.Create("1041", "FBXImporter"),
            Tuple.Create("1042", "TrueTypeFontImporter"),
            Tuple.Create("1045", "EditorBuildSettings"),
            Tuple.Create("1048", "InspectorExpandedState"),
            Tuple.Create("1049", "AnnotationManager"),
            Tuple.Create("1050", "PluginImporter"),
            Tuple.Create("1051", "EditorUserBuildSettings"),
            Tuple.Create("1055", "IHVImageFormatImporter"),
            Tuple.Create("1101", "AnimatorStateTransition"),
            Tuple.Create("1102", "AnimatorState"),
            Tuple.Create("1105", "HumanTemplate"),
            Tuple.Create("1107", "AnimatorStateMachine"),
            Tuple.Create("1108", "PreviewAnimationClip"),
            Tuple.Create("1109", "AnimatorTransition"),
            Tuple.Create("1110", "SpeedTreeImporter"),
            Tuple.Create("1111", "AnimatorTransitionBase"),
            Tuple.Create("1112", "SubstanceImporter"),
            Tuple.Create("1113", "LightmapParameters"),
            Tuple.Create("1120", "LightingDataAsset"),
            Tuple.Create("1124", "SketchUpImporter"),
            Tuple.Create("1125", "BuildReport"),
            Tuple.Create("1126", "PackedAssets"),
            Tuple.Create("1127", "VideoClipImporter"),
            Tuple.Create("100000", "int"),
            Tuple.Create("100001", "bool"),
            Tuple.Create("100002", "float"),
            Tuple.Create("100003", "MonoObject"),
            Tuple.Create("100004", "Collision"),
            Tuple.Create("100005", "Vector3f"),
            Tuple.Create("100006", "RootMotionData"),
            Tuple.Create("100007", "Collision2D"),
            Tuple.Create("100008", "AudioMixerLiveUpdateFloat"),
            Tuple.Create("100009", "AudioMixerLiveUpdateBool"),
            Tuple.Create("100010", "Polygon2D"),
            Tuple.Create("100011", "void"),
            Tuple.Create("19719996", "TilemapCollider2D"),
            Tuple.Create("41386430", "AssetImporterLog"),
            Tuple.Create("73398921", "VFXRenderer"),
            Tuple.Create("76251197", "SerializableManagedRefTestClass"),
            Tuple.Create("156049354", "Grid"),
            Tuple.Create("156483287", "ScenesUsingAssets"),
            Tuple.Create("171741748", "ArticulationBody"),
            Tuple.Create("181963792", "Preset"),
            Tuple.Create("277625683", "EmptyObject"),
            Tuple.Create("285090594", "IConstraint"),
            Tuple.Create("293259124", "TestObjectWithSpecialLayoutOne"),
            Tuple.Create("294290339", "AssemblyDefinitionReferenceImporter"),
            Tuple.Create("334799969", "SiblingDerived"),
            Tuple.Create("342846651", "TestObjectWithSerializedMapStringNonAlignedStruct"),
            Tuple.Create("367388927", "SubDerived"),
            Tuple.Create("369655926", "AssetImportInProgressProxy"),
            Tuple.Create("382020655", "PluginBuildInfo"),
            Tuple.Create("426301858", "EditorProjectAccess"),
            Tuple.Create("468431735", "PrefabImporter"),
            Tuple.Create("478637458", "TestObjectWithSerializedArray"),
            Tuple.Create("478637459", "TestObjectWithSerializedAnimationCurve"),
            Tuple.Create("483693784", "TilemapRenderer"),
            Tuple.Create("488575907", "ScriptableCamera"),
            Tuple.Create("612988286", "SpriteAtlasAsset"),
            Tuple.Create("638013454", "SpriteAtlasDatabase"),
            Tuple.Create("641289076", "AudioBuildInfo"),
            Tuple.Create("644342135", "CachedSpriteAtlasRuntimeData"),
            Tuple.Create("646504946", "RendererFake"),
            Tuple.Create("662584278", "AssemblyDefinitionReferenceAsset"),
            Tuple.Create("668709126", "BuiltAssetBundleInfoSet"),
            Tuple.Create("687078895", "SpriteAtlas"),
            Tuple.Create("747330370", "RayTracingShaderImporter"),
            Tuple.Create("825902497", "RayTracingShader"),
            Tuple.Create("850595691", "LightingSettings"),
            Tuple.Create("877146078", "PlatformModuleSetup"),
            Tuple.Create("890905787", "VersionControlSettings"),
            Tuple.Create("895512359", "AimConstraint"),
            Tuple.Create("937362698", "VFXManager"),
            Tuple.Create("994735392", "VisualEffectSubgraph"),
            Tuple.Create("994735403", "VisualEffectSubgraphOperator"),
            Tuple.Create("994735404", "VisualEffectSubgraphBlock"),
            Tuple.Create("1001480554", "Prefab"),
            Tuple.Create("1027052791", "LocalizationImporter"),
            Tuple.Create("1091556383", "Derived"),
            Tuple.Create("1111377672", "PropertyModificationsTargetTestObject"),
            Tuple.Create("1114811875", "ReferencesArtifactGenerator"),
            Tuple.Create("1152215463", "AssemblyDefinitionAsset"),
            Tuple.Create("1154873562", "SceneVisibilityState"),
            Tuple.Create("1183024399", "LookAtConstraint"),
            Tuple.Create("1210832254", "SpriteAtlasImporter"),
            Tuple.Create("1223240404", "MultiArtifactTestImporter"),
            Tuple.Create("1268269756", "GameObjectRecorder"),
            Tuple.Create("1325145578", "LightingDataAssetParent"),
            Tuple.Create("1386491679", "PresetManager"),
            Tuple.Create("1392443030", "TestObjectWithSpecialLayoutTwo"),
            Tuple.Create("1403656975", "StreamingManager"),
            Tuple.Create("1480428607", "LowerResBlitTexture"),
            Tuple.Create("1542919678", "StreamingController"),
            Tuple.Create("1628831178", "TestObjectVectorPairStringBool"),
            Tuple.Create("1742807556", "GridLayout"),
            Tuple.Create("1766753193", "AssemblyDefinitionImporter"),
            Tuple.Create("1773428102", "ParentConstraint"),
            Tuple.Create("1803986026", "FakeComponent"),
            Tuple.Create("1818360608", "PositionConstraint"),
            Tuple.Create("1818360609", "RotationConstraint"),
            Tuple.Create("1818360610", "ScaleConstraint"),
            Tuple.Create("1839735485", "Tilemap"),
            Tuple.Create("1896753125", "PackageManifest"),
            Tuple.Create("1896753126", "PackageManifestImporter"),
            Tuple.Create("1953259897", "TerrainLayer"),
            Tuple.Create("1971053207", "SpriteShapeRenderer"),
            Tuple.Create("1977754360", "NativeObjectType"),
            Tuple.Create("1981279845", "TestObjectWithSerializedMapStringBool"),
            Tuple.Create("1995898324", "SerializableManagedHost"),
            Tuple.Create("2058629509", "VisualEffectAsset"),
            Tuple.Create("2058629510", "VisualEffectImporter"),
            Tuple.Create("2058629511", "VisualEffectResource"),
            Tuple.Create("2059678085", "VisualEffectObject"),
            Tuple.Create("2083052967", "VisualEffect"),
            Tuple.Create("2083778819", "LocalizationAsset"),
            Tuple.Create("2089858483", "ScriptedImporter")
        };
    }
}
