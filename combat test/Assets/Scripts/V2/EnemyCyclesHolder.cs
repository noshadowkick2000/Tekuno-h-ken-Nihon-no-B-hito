using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCyclesHolder : MonoBehaviour
{
    [SerializeField] public int nextCycleHealthTreshold;
    [SerializeField] public TimingState[] statesInCycle;
}
