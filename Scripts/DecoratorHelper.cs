using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
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
            // Custom models like Handpainted Models have insanley different scales (< 0.0 to 200+) Set all models as a child to a parent, so
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


            //Expanding collider a little gives better hit detection.
            float buffer = 0.02f;

            // Some custom models have a box collider and are made of multiple smaller models. Get the parent collider size.
            if (childCollider = childGo.GetComponent<BoxCollider>())
            {
                parentCollider.size = new Vector3((childCollider.size.x * childGo.transform.localScale.x) + buffer,
                                                    (childCollider.size.y * childGo.transform.localScale.y) + buffer,
                                                    (childCollider.size.z * childGo.transform.localScale.z) + buffer);

                parentCollider.center = new Vector3(childCollider.center.x * childGo.transform.localScale.x,
                                                    childCollider.center.y * childGo.transform.localScale.y,
                                                    childCollider.center.z * childGo.transform.localScale.z);

                // Child colliders screw with EditMode.
                GameObject.Destroy(childCollider);
            }
            else
            {
                Bounds childBounds = childGo.GetComponent<MeshFilter>().sharedMesh.bounds;

                parentCollider.size = new Vector3((childBounds.size.x * childGo.transform.localScale.x) + buffer,
                                                    (childBounds.size.y * childGo.transform.localScale.y) + buffer,
                                                    (childBounds.size.z * childGo.transform.localScale.z) + buffer);

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

                if (data.lootData != null)
                    lootContainer.RestoreSaveData(data.lootData);

                loot.LoadID = 0;
                loot.ContainerType = LootContainerTypes.HouseContainers;
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
