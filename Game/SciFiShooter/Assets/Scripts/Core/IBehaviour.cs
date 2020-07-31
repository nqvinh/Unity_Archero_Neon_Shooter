using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviour
{
    void OnEnter(GameObject actor,params object[] @params);

    void Update(GameObject actor);
    void FixedUpdate(GameObject actor);
    void LateUpdate(GameObject actor);

}