﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using UniRx;

public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager>
{
    void Start()
    {
        GameWindowFactory.Create(new GameWindowRequest()).Subscribe();
    }
}
