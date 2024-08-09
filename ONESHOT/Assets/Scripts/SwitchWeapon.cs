using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{
    public List<GameObject> weapons; // Список доступных оружий
    private int currentWeaponIndex = -1; // Индекс текущего оружия (-1 означает безоружное состояние)
    public Animator m_Animator;

    private void Start()
    {
        EquipWeapon(currentWeaponIndex); // Начинаем с безоружного состояния
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Достать оружие
        if (Input.GetKeyDown(KeyCode.E) && currentWeaponIndex == -1)
        {
            SwitchToWeapon(0); // Достаем первое оружие
        }
        // Смена оружия с помощью клавиш 1 и 2
        else if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeaponIndex != -1)
        {
            SwitchToWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeaponIndex != -1)
        {
            SwitchToWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && currentWeaponIndex != -1)
        {
            SwitchToWeapon(2);
        }
        // Убрать оружие
        else if (Input.GetKeyDown(KeyCode.Q) && currentWeaponIndex != -1)
        {
            EquipWeapon(-1); // Возвращаемся в безоружное состояние
        }
    }

    private void EquipWeapon(int index)
    {
        // Деактивируем все оружия
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // Активируем текущее оружие или возвращаемся в безоружное состояние
        if (index >= 0 && index < weapons.Count)
        {
            weapons[index].SetActive(true);
            currentWeaponIndex = index;
            m_Animator.SetInteger("NWeapon", 1);
        }
        else
        {
            currentWeaponIndex = -1; // Безоружное состояние
            m_Animator.SetInteger("NWeapon", 0);
        }
    }

    private void SwitchToWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            EquipWeapon(index);
        }
    }
}
