using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{
    public List<GameObject> weapons; // ������ ��������� ������
    private int currentWeaponIndex = -1; // ������ �������� ������ (-1 �������� ���������� ���������)
    public Animator m_Animator;

    private void Start()
    {
        EquipWeapon(currentWeaponIndex); // �������� � ����������� ���������
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ������� ������
        if (Input.GetKeyDown(KeyCode.E) && currentWeaponIndex == -1)
        {
            SwitchToWeapon(0); // ������� ������ ������
        }
        // ����� ������ � ������� ������ 1 � 2
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
        // ������ ������
        else if (Input.GetKeyDown(KeyCode.Q) && currentWeaponIndex != -1)
        {
            EquipWeapon(-1); // ������������ � ���������� ���������
        }
    }

    private void EquipWeapon(int index)
    {
        // ������������ ��� ������
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // ���������� ������� ������ ��� ������������ � ���������� ���������
        if (index >= 0 && index < weapons.Count)
        {
            weapons[index].SetActive(true);
            currentWeaponIndex = index;
            m_Animator.SetInteger("NWeapon", 1);
        }
        else
        {
            currentWeaponIndex = -1; // ���������� ���������
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
