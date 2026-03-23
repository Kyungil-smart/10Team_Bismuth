using System.Collections.Generic;
using UnityEngine;

public static class YieldContainer
{
    private static readonly Dictionary<float, WaitForSeconds> _wait 
        = new Dictionary<float, WaitForSeconds>();

    // WaitForSeconds 생성 메서드
    public static WaitForSeconds Wait(float seconds)
    {
        // 'seconds'가 없으면 생성
        if (!_wait.ContainsKey(seconds))
        {
            _wait.Add(seconds, new WaitForSeconds(seconds));
        }
        
        return _wait[seconds];
    }
}
