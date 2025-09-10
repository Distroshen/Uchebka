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
        // ���� ��� �������� ������ � Editor
#if UNITY_EDITOR
        // �������� ��� ������ � �������
        var allAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        var assemblyNames = allAssemblies.Select(a => a.GetName().Name).ToList();

        Debug.Log("Loaded Assemblies: " + assemblyNames.Count);

        // ������ ������������� ������
        HashSet<string> usedAssemblies = new HashSet<string>();
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            if (behaviour != null)
            {
                usedAssemblies.Add(behaviour.GetType().Assembly.GetName().Name);
            }
        }

        // ����� ������������ �������������� ������
        List<string> potentiallyUnused = assemblyNames.Except(usedAssemblies).ToList();

        Debug.Log("Potentially unused assemblies: " + string.Join(", ", potentiallyUnused));
#endif
    }
}