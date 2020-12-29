using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : PlayerBase
{
    // Start is called before the first frame update
    override protected void Start()
    {
        Type = PlayerType.Archer;

        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
    }
}
