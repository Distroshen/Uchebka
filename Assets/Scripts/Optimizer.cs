using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssemblyReferenceAnalyzer : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        AnalyzeAssemblyReferences();
#endif
    }

    void AnalyzeAssemblyReferences()
    {
        // Этот код работает только в Editor
#if UNITY_EDITOR
        // Получаем все сборки в проекте
        var allAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        var assemblyNames = allAssemblies.Select(a => a.GetName().Name).ToList();

        Debug.Log("Loaded Assemblies: " + assemblyNames.Count);

        // Анализ использования сборок
        HashSet<string> usedAssemblies = new HashSet<string>();
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            if (behaviour != null)
            {
                usedAssemblies.Add(behaviour.GetType().Assembly.GetName().Name);
            }
        }

        // Поиск потенциально неиспользуемых сборок
        List<string> potentiallyUnused = assemblyNames.Except(usedAssemblies).ToList();

        Debug.Log("Potentially unused assemblies: " + string.Join(", ", potentiallyUnused));
#endif
    }
}