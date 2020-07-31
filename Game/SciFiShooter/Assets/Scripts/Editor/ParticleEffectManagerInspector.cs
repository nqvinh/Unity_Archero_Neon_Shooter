using UnityEngine;
using UnityEditor;
using System.Collections;

// Cartoon FX  - (c) 2013,2014 Jean Moreno

// CFX Spawn System Editor interface

[CustomEditor(typeof(ParticleEffectManager))]
public class ParticleEffectManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        (this.target as ParticleEffectManager).hideObjectsInHierarchy = GUILayout.Toggle((this.target as ParticleEffectManager).hideObjectsInHierarchy, "Hide Preloaded Objects in Hierarchy");

        GUI.SetNextControlName("DragDropBox");
        EditorGUILayout.HelpBox("Drag GameObjects you want to preload here!\n\nTIP:\nUse the Inspector Lock at the top right to be able to drag multiple objects at once!", MessageType.None);

        for (int i = 0; i < (this.target as ParticleEffectManager).particlesToPreload.Length; i++)
        {
            GUILayout.BeginHorizontal();

            (this.target as ParticleEffectManager).particlesToPreload[i] = (ParticleSystem)EditorGUILayout.ObjectField((this.target as ParticleEffectManager).particlesToPreload[i], typeof(ParticleSystem), true);
            EditorGUILayout.LabelField(new GUIContent("Pool Count", "Number of times to copy the effect\nin the pool, i.e. the max number of\ntimes the object will be used\nsimultaneously"), GUILayout.Width(40));
            int nb = EditorGUILayout.IntField("", (this.target as ParticleEffectManager).particlesPoolCount[i], GUILayout.Width(50));
            if (nb < 1)
                nb = 1;
            (this.target as ParticleEffectManager).particlesPoolCount[i] = nb;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(24)))
            {
                Object preloadedObject = (this.target as ParticleEffectManager).particlesToPreload[i];
                string objectName = (preloadedObject == null) ? "" : preloadedObject.name;
                Undo.RegisterCompleteObjectUndo(target, string.Format("Remove {0} from Spawn System", objectName));

                ArrayUtility.RemoveAt<ParticleSystem>(ref (this.target as ParticleEffectManager).particlesToPreload, i);
                ArrayUtility.RemoveAt<int>(ref (this.target as ParticleEffectManager).particlesPoolCount, i);

                EditorUtility.SetDirty(target);
            }

            GUILayout.EndHorizontal();
        }

        if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (Event.current.type == EventType.DragPerform)
            {
                foreach (Object o in DragAndDrop.objectReferences)
                {
                    if (o is GameObject)
                    {
                        ParticleSystem oParticle = ((GameObject)o).GetComponent<ParticleSystem>();
                        bool already = false;
                        foreach (ParticleSystem otherObj in (this.target as ParticleEffectManager).particlesToPreload)
                        {
                            if (oParticle == otherObj)
                            {
                                already = true;
                                Debug.LogWarning("Effect has already been added: " + o.name);
                                break;
                            }
                        }

                        if (!already)
                        {
                            Undo.RegisterUndo(target, string.Format("Add {0} to Spawn System", o.name));

                            ArrayUtility.Add<ParticleSystem>(ref (this.target as ParticleEffectManager).particlesToPreload, oParticle);
                            ArrayUtility.Add<int>(ref (this.target as ParticleEffectManager).particlesPoolCount, 1);

                            EditorUtility.SetDirty(target);
                        }
                    }
                }
            }
        }
    }
}
