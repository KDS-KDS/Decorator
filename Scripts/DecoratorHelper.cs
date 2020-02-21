using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
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

                string archiveString = string.Empty;
                string recordString = string.Empty;

                for (int i = 0; i < index; i++)
                    archiveString += key[i];

                for (int i = index + 1; i < key.Length; i++)
                    recordString += key[i];

                int archive = int.Parse(archiveString);
                int record = int.Parse(recordString);

                data.archive = archive;
                data.record = record;
            }

            return data;
        }

        public static GameObject CreatePlacedObject(PlacedObjectData_v2 data, Transform parent, bool previewGo = false)
        {
            // Custom models like Handpainted Models have insanley different scales (0.0... to 200+) Set all models as a child to a parent, so
            // EditMode can uniformly scale properly.

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
                childGo = MeshReplacement.ImportCustomGameobject(data.modelID, parentGo.transform, new Matrix4x4());

                if (childGo == null)
                    childGo = GameObjectHelper.CreateDaggerfallMeshGameObject(data.modelID, parentGo.transform);
            }

            parentGo.transform.eulerAngles = Vector3.zero;
            childGo.transform.eulerAngles = Vector3.zero;

            if (previewGo)
                data.isLight = true;

            BoxCollider parentCollider = parentGo.AddComponent<BoxCollider>();
            BoxCollider childCollider;

            // Some custom models have a box collider and are made of multiple smaller models. Get the parent collider size.
            if (childCollider = childGo.GetComponent<BoxCollider>())
            {
                parentCollider.size   = new Vector3(childCollider.size.x * childGo.transform.localScale.x,
                                                    childCollider.size.y * childGo.transform.localScale.y,
                                                    childCollider.size.z * childGo.transform.localScale.z);

                parentCollider.center = new Vector3(childCollider.center.x * childGo.transform.localScale.x,
                                                    childCollider.center.y * childGo.transform.localScale.y,
                                                    childCollider.center.z * childGo.transform.localScale.z);

                // Child colliders screw with EditMode.
                GameObject.Destroy(childCollider);
            }
            else
            {
                Bounds childBounds = childGo.GetComponent<MeshFilter>().sharedMesh.bounds;

                parentCollider.size   = new Vector3(childBounds.size.x * childGo.transform.localScale.x,
                                                    childBounds.size.y * childGo.transform.localScale.y,
                                                    childBounds.size.z * childGo.transform.localScale.z);

                parentCollider.center = new Vector3(childBounds.center.x * childGo.transform.localScale.x,
                                                    childBounds.center.y * childGo.transform.localScale.y,
                                                    childBounds.center.z * childGo.transform.localScale.z);
            }
                
            parentCollider.isTrigger = true;

            parentGo.AddComponent<PlacedObject>();
            SetPlacedObject(data, parentGo);

            return parentGo;
        }

        public static void SetPlacedObject(PlacedObjectData_v2 data, GameObject placedObject)
        {
            placedObject.GetComponent<PlacedObject>().SetData(data);
            SetLight(placedObject, data);
            SetContainer(placedObject, data);
            SetLayer(placedObject);
        }

        #endregion

        #region Private Methods

        static void SetLight(GameObject placedObject, PlacedObjectData_v2 data = null)
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

        static void SetContainer(GameObject placedObject, PlacedObjectData_v2 data = null)
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

                loot.ContainerType = LootContainerTypes.Nothing;
                loot.LoadID = 0;

                if (data.lootData != null)
                    lootContainer.RestoreSaveData(data.lootData);
            }
            else
            {
                Object.Destroy(placedObject.GetComponent<DaggerfallLoot>());
                Object.Destroy(placedObject.GetComponent<SerializableLootContainer>());

                data.lootData = null;
            }
        }

        static void SetLayer(GameObject placedObject)
        {
            placedObject.layer = 0;

            if (placedObject.transform.childCount > 0)
                foreach (Transform child in placedObject.transform)
                    SetLayer(child.gameObject);
        }

        #endregion
    }
}
