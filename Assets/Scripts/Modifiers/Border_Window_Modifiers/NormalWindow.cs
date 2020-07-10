﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Стандартное окно без каких либо модификаций, через него спокойно пролетают шайбы и подсчитываются баллы
public class NormalWindow : MonoBehaviour
{
    GameObject game;

    private void Start()
    {
        game = gameObject.GetComponent<Window>().game;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Checker check = collision.gameObject.GetComponent<Checker>();
        if (collision.gameObject.transform.position.y > 0 && check.field == Checker.Field.Down)
        {
            check.field = Checker.Field.Up;
            game.GetComponent<Mode>().changeCount(collision.gameObject);
        }
        if (collision.gameObject.transform.position.y < 0 && check.field == Checker.Field.Up)
        {
            check.field = Checker.Field.Down;
            game.GetComponent<Mode>().changeCount(collision.gameObject);
        }
    }
}