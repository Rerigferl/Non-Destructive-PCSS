using System.Collections.Immutable;
using System.Threading.Tasks;
using nadena.dev.ndmf.preview;
using nadena.dev.ndmf.runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Numeira;

internal static partial class Ndpcss
{
    public sealed class Preview : IRenderFilter
    {
        public static Animator? ActiveAvatar { get; private set; }

        static Preview()
        {
            EditorApplication.delayCall += () =>
            {
                Selection.selectionChanged += () =>
                {
                    var active = Selection.activeGameObject;
                    if (active == null || !active.activeInHierarchy || RuntimeUtil.FindAvatarInParents(active.transform) is not { } tr || !tr.gameObject.activeInHierarchy || tr == null || tr.GetComponent<Animator>() is not { } animator)
                        return;

                    ActiveAvatar = animator;
                };
            };
        }

        public ImmutableList<RenderGroup> GetTargetGroups(ComputeContext context)
        {
            return Iterate(context)
                .Select(x => RenderGroup.For(x.Renderers).WithData(x))
                .ToImmutableList();

            static IEnumerable<(GameObject Avatar, NonDestructivePCSS Component, Renderer[] Renderers)> Iterate(ComputeContext context)
            {
                foreach(var x in context.GetComponentsByType<NonDestructivePCSS>().Select(x => (Component: x, Avatar: RuntimeUtil.FindAvatarInParents(x.transform)!)).Where(x => x.Avatar != null).GroupBy(x => x.Avatar, x => x.Component))
                {
                    var avatar = x.Key;
                    var component = x.FirstOrDefault();
                    if (component == null)
                        continue;

                    if (!context.Observe(component.gameObject, go => go.activeInHierarchy))
                        continue;

                    context.Observe(component, x => x.ShaderSetting with { }, (x, y) => x == y);

                    foreach (var sub in context.GetComponentsInChildren<PCSSMaterialSetting>(component.gameObject, true))
                    {
                        context.Observe(sub.gameObject, x => x.activeInHierarchy);
                        context.Observe(sub, x => x.Ignore);
                        context.Observe(sub, x => x.Materials.GetArrayHashCode().ToHashCode());
                        context.Observe(sub, x => x.Setting with { }, (x, y) => x == y);
                        context.Observe(sub, x => x.isActiveAndEnabled);
                    }

                    var renderers = avatar.GetComponentsInChildren<Renderer>();
                    if (renderers == null || renderers.Length == 0)
                        continue;


                    yield return (avatar.gameObject, component, renderers);
                }
            }
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            return Task.FromResult<IRenderFilterNode>(new PreviewNode(group, proxyPairs, context));
        }

        private sealed class PreviewNode : IRenderFilterNode
        {
            public RenderAspects WhatChanged => RenderAspects.Material;

            private readonly Dictionary<Material, Material> cache = new();
            private readonly List<GameObject> gameObjList = new();
            private readonly Animator? avatar;
            private readonly IEnumerable<(Renderer, Renderer)> proxyPairs;
            private readonly SessionContext context;

            public PreviewNode(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
            {
                var (avatar, component, _) = group.GetData<(GameObject, NonDestructivePCSS, Renderer[])>();
                this.avatar = avatar.GetComponent<Animator>();
                this.proxyPairs = proxyPairs;
                this.context = new SessionContext(component);
            }

            void IRenderFilterNode.OnFrame(Renderer original, Renderer proxy)
            {
            }

            void IRenderFilterNode.OnFrameGroup()
            {
                if (ActiveAvatar == null)
                    ActiveAvatar = this.avatar;

                if (ActiveAvatar != this.avatar || context.ComponentRoot == null)
                    return;

                foreach (var (original, proxy) in proxyPairs)
                {
                    var cache = this.cache;

                    var materials = original.sharedMaterials;
                    foreach (ref var material in materials.AsSpan())
                    {
                        material = ToPCSSMaterial(material, this.context);
                    }
                    proxy.sharedMaterials = materials;
                }

                context.ApplyPCSSSettings();

                var scene = NDMFPreviewSceneManager.GetPreviewScene();
                scene.GetRootGameObjects(gameObjList);

                var lightObj = gameObjList.FirstOrDefault(x => x.name == $"SelfLight");

                if (lightObj == null)
                {
                    var prefab = CreateControlPrefab();
                    SceneManager.MoveGameObjectToScene(prefab, scene);
                    foreach (var renderer in prefab.GetComponentsInChildren<Renderer>())
                    {
                        renderer.gameObject.SetActive(false);
                    }

                    lightObj = prefab;
                }

                var avatar = ActiveAvatar ?? this.avatar;

                lightObj.transform.position = avatar?.transform.position ?? Vector3.zero;
                var head = lightObj.transform.Find("Armature/Head");
                head.position = avatar?.GetBoneTransform(HumanBodyBones.Head)?.position ?? Vector3.up;
                head.localEulerAngles = context.ComponentRoot.transform.localEulerAngles;
                lightObj.transform.Find("Armature/Chest").position = avatar?.GetBoneTransform(HumanBodyBones.Chest)?.position ?? new(0, 0.5f, 0);
            }

            public void Dispose()
            {
                foreach (var x in cache.Values)
                {
                    if (x != null)
                        Object.DestroyImmediate(x);
                }
            }
        }
    }
}
