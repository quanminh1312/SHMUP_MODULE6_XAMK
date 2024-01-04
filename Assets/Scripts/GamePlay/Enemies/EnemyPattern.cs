using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EnemyPattern : MonoBehaviour
{
    public List<EnemyStep> steps = new List<EnemyStep>();
    public Enemy enmyPrefab = null;
    private int UID;
    private Enemy spawnedEnemy = null;
    public bool stayOnLast = true;
    private int currentStateIndex = 0;
    private int previousStateIndex = -1;
     
    public bool startActive = false;

    public bool spawnOnEasy = true;
    public bool spawnOnNormal = true;
    public bool spawnOnHard = false;
    public bool spawnOnInsane = false;
#if UNITY_EDITOR
    [MenuItem("GameObject/SHMUP/EnemyPattern", false, 10)]
    static void CreateEnemyPatternObject(MenuCommand menuCommand)
    {
        Helper helper = Resources.Load<Helper>("Helper");
        if (helper)
        {
            GameObject go = new GameObject("EnemyPattern" + helper.nextFreePatternID);
            EnemyPattern pattern = go.AddComponent<EnemyPattern>();
            pattern.UID = helper.nextFreePatternID;
            helper.nextFreePatternID++;

            //register creation with undo system
            Undo.RegisterCompleteObjectUndo(go,"Create" + go.name);
            Selection.activeObject = go;
        }
        else Debug.LogError("Helper not found");
    }
#endif
    public void Calculate(Transform enemyTransform, float progressTimer)
    {
        Vector3 pos = CalculatePosition(progressTimer);
        Quaternion rot = CalculateRotation(progressTimer);

        enemyTransform.position = pos;
        enemyTransform.rotation = rot;
        if (currentStateIndex != previousStateIndex)
        {
            if (previousStateIndex>=0)
            {
                //call deactivate
                EnemyStep previousStep = steps[previousStateIndex];
                previousStep.FireDeActivateStates(spawnedEnemy);
            }
            if (currentStateIndex>=0)
            {
                //call activate
                EnemyStep currentStep = steps[currentStateIndex];
                currentStep.FireActivateStates(spawnedEnemy);
            }
            previousStateIndex = currentStateIndex;
        }
    }
    private void Start()
    {
        if (startActive)
            Spawn();
    }
    public void Spawn()
    {
        if (spawnedEnemy == null)
        {
            spawnedEnemy = Instantiate(enmyPrefab, transform.position, transform.rotation).GetComponent<Enemy>();
            spawnedEnemy.SetPattern(this);
        }
    }
    public Vector2 CalculatePosition(float progressTimer)
    {
        currentStateIndex = WhichStep(progressTimer);
        if (currentStateIndex < 0) return spawnedEnemy.transform.position;
        EnemyStep step = steps[currentStateIndex];  
        float stepTime = progressTimer - StartTime(currentStateIndex);
        Vector3 startPos = EndPosition(currentStateIndex - 1);

        return step.CalculatePostion(startPos, stepTime);
    }
    public Quaternion CalculateRotation(float ProgressTimer)
    {
        return Quaternion.identity;
    }

    int WhichStep(float timer)
    {
        float timeToCheck = timer;
        for (int s = 0; s < steps.Count; s++)
        {
            if (timeToCheck < steps[s].TimeToCpmplete())
                return s;
            timeToCheck -= steps[s].TimeToCpmplete();
        }
        if (stayOnLast)
            return steps.Count - 1;
        else
            return -1;
    }
    public float StartTime(int step)
    {
        if (step<=0) return 0;
        
        float result = 0;
        for (int s=0;s<step; s++)
        {
            result += steps[s].TimeToCpmplete();
        }
        return result;
    }
    public Vector3 EndPosition(int stepIndex)
    {
        Vector3 result = transform.position;
        if (stepIndex >=0)
        {
            for (int s = 0; s <= stepIndex; s++)
            {
                result = steps[s].EndPosition(result);
            }
        }
        return result;
    }
}
