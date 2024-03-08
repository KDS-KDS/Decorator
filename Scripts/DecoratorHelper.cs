using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using System.Collections.Generic;
using UnityEngine;

namespace Decorator
{
    public static class DecoratorHelper
    {
        #region Public Methods

        public static PlacedObjectData_v2 Parse(string key, Dictionary<string, string> dictionary)
        {
            PlacedObjectData_v2 data = new PlacedObjectData_v2();

            if (!dictionary.TryGetValue(key, out data.name))
            {
                Debug.LogWarning("Could not find value of key in Parse");
                return null;
            }
            else if (key == "-1")
                return data;

            int index = key.IndexOf(".");

            if (index == -1)
                data.modelID = (uint)int.Parse(key);
            else
            {
                data.modelID = 0;

                string[] archiveRecord = key.Split('.');

                data.archive = int.Parse(archiveRecord[0]);
                data.record = int.Parse(archiveRecord[1]);
            }

            return data;
        }

        public static GameObject CreatePlacedObject(PlacedObjectData_v2 data, Transform parent, bool previewGo = false)
        {
            // Set all models as a child to a parent so Edit Mode can scale properly.
            GameObject parentGo = new GameObject();
            GameObject childGo;

            parentGo.transform.parent = parent;

            if (data.modelID == 0)
            {
                childGo = MeshReplacement.ImportCustomFlatGameobject(data.archive, data.record, Vector3.zero, parentGo.transform);

                if (childGo == null)
                    childGo = GameObjectHelper.CreateDaggerfallBillboardGameObject(data.archive, data.record, parentGo.transform);
            }
            else
            {
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

                childGo = MeshReplacement.ImportCustomGameobject(data.modelID, parentGo.transform, matrix);

                if (childGo == null)
                    childGo = GameObjectHelper.CreateDaggerfallMeshGameObject(data.modelID, parentGo.transform);
            }

            parentGo.transform.eulerAngles = Vector3.zero;
            childGo.transform.eulerAngles = Vector3.zero;

            if (previewGo)
                data.isLight = true;

            BoxCollider parentCollider = parentGo.AddComponent<BoxCollider>();
            BoxCollider childCollider;
            Bounds childBounds = new Bounds();

            //Expanding collider a little gives better hit detection.
            float buffer = 0.02f;

            // Some custom models have a box collider and are made of multiple smaller models. Get the parent collider size.
            if (childCollider = childGo.GetComponent<BoxCollider>())
            {
                childBounds.size = childCollider.size;
                childBounds.center = childCollider.center;

                // Child colliders screw with EditMode.
                GameObject.Destroy(childCollider);
            }
            else
            {
                MeshFilter meshFilter;

                if (meshFilter = childGo.GetComponent<MeshFilter>())
                    childBounds = meshFilter.sharedMesh.bounds;
                else
                {
                    SkinnedMeshRenderer skinnedMeshRenderer;

                    if (skinnedMeshRenderer = parentGo.GetComponentInChildren<SkinnedMeshRenderer>())
                        childBounds = skinnedMeshRenderer.bounds;
                }
            }

            parentCollider.size = GetColliderSizeAndCenter(childBounds.size, childGo, buffer);
            parentCollider.center = GetColliderSizeAndCenter(childBounds.center, childGo);

            parentCollider.isTrigger = true;

            parentGo.AddComponent<PlacedObject>();
            SetPlacedObject(data, parentGo);

            return parentGo;
        }

        private static Vector3 GetColliderSizeAndCenter(Vector3 bounds, GameObject gameObject, float buffer = 0.0f)
        {
            Vector3 size = new Vector3((bounds.x * gameObject.transform.localScale.x) + buffer,
                                       (bounds.y * gameObject.transform.localScale.y) + buffer,
                                       (bounds.z * gameObject.transform.localScale.z) + buffer);

            return size;
        }

        public static void SetPlacedObject(PlacedObjectData_v2 data, GameObject placedObject)
        {
            placedObject.GetComponent<PlacedObject>().SetData(data);
            SetLight(placedObject, data);
            SetContainer(placedObject, data);
            SetLayer(placedObject);
        }

        #endregion Public Methods

        #region Private Methods

        private static void SetLight(GameObject placedObject, PlacedObjectData_v2 data = null)
        {
            if (data == null)
                data = placedObject.GetComponent<PlacedObject>().GetData();

            if (data.isLight)
            {
                Light light;
                DaggerfallLight dfLight;
                GameObject lightGo;

                if (placedObject.transform.childCount > 1)
                {
                    if (!(light = placedObject.transform.GetComponentInChildren<Light>()))
                    {
                        lightGo = new GameObject(data.name + " Light");
                        lightGo.transform.parent = placedObject.transform;

                        light = lightGo.AddComponent<Light>();
                        dfLight = lightGo.AddComponent<DaggerfallLight>();
                    }
                    else
                    {
                        lightGo = light.gameObject;

                        if (!(dfLight = lightGo.transform.GetComponent<DaggerfallLight>()))
                            dfLight = lightGo.AddComponent<DaggerfallLight>();
                    }
                }
                else
                {
                    lightGo = new GameObject(data.name + " Light");
                    lightGo.transform.parent = placedObject.transform;

                    light = lightGo.AddComponent<Light>();
                    dfLight = lightGo.AddComponent<DaggerfallLight>();
                }

                dfLight.Animate = true;
                dfLight.InteriorLight = true;
                light.color = data.lightColor;
                light.intensity = data.lightIntensity;
                light.type = data.lightType;
                light.spotAngle = data.lightSpotAngle;

                if (!DaggerfallUnity.Settings.InteriorLightShadows)
                    light.shadows = LightShadows.None;
                else
                    light.shadows = LightShadows.Soft;

                light.enabled = true;
                dfLight.enabled = true;

                lightGo.transform.localPosition = Vector3.zero;
                lightGo.transform.localEulerAngles = new Vector3(data.lightVerticalRotation, data.lightHorizontalRotation, 0.0f);
            }
            else
            {
                if (placedObject.transform.childCount > 1)
                {
                    Light light = placedObject.GetComponentInChildren<Light>();
                    Object.Destroy(light.gameObject);
                }
            }
        }

        private static void SetContainer(GameObject placedObject, PlacedObjectData_v2 data = null)
        {
            if (data == null)
                data = placedObject.GetComponent<PlacedObject>().GetData();

            DaggerfallLoot loot;
            SerializableLootContainer lootContainer;

            if (data.isContainer)
            {
                if (!(loot = placedObject.GetComponent<DaggerfallLoot>()))
                    loot = placedObject.AddComponent<DaggerfallLoot>();

                if (!(lootContainer = placedObject.GetComponent<SerializableLootContainer>()))
                    lootContainer = placedObject.AddComponent<SerializableLootContainer>();

                if (data.lootData != null)
                    lootContainer.RestoreSaveData(data.lootData);

                loot.LoadID = 0;
                loot.ContainerType = LootContainerTypes.Nothing;
                loot.ContainerImage = InventoryContainerImages.Shelves;
            }
            else
            {
                Object.Destroy(placedObject.GetComponent<DaggerfallLoot>());
                Object.Destroy(placedObject.GetComponent<SerializableLootContainer>());

                data.lootData = null;
            }
        }

        private static void SetLayer(GameObject placedObject)
        {
            placedObject.layer = 0;

            if (placedObject.transform.childCount > 0)
                foreach (Transform child in placedObject.transform)
                    SetLayer(child.gameObject);
        }

        #endregion Private Methods
    }
}