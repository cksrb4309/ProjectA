using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public SelectPanelGroup selectPanelGroup;

    public GameObject descriptionLabel;

    public List<ActiveEffect> activeEffects;
    public List<StackEffect> stackEffects;
    public List<BattleEndEffect> battleEndEffects;

    public StatusData baseData;

    public static StatusData CurrentData;

    StatusData currentData;

    private void Awake()
    {
        instance = this;

        currentData = ScriptableObject.CreateInstance<StatusData>();

        currentData.Init();

        CurrentData = currentData;
    }

    public GameObject inventory;

    public ItemSlotUI[] itemSlots;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            descriptionLabel.SetActive(false);

            inventory.SetActive(!inventory.activeSelf);
        }
    }
    public void GetItem(Item item)
    {
        // ����ִ� ���Կ� �������� �ִ´�
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].SetItemUI(item))
            {
                // �������ͽ� ��ȭ�� ���⿡
                if (!item.statusData.Equals(baseData))
                {
                    currentData.Apply(item.statusData);

                    Debug.Log("��ȭ ����");
                    string str = "";
                    str += "�÷��̾� ü�� : " + currentData.playerHp.ToString() + '\n';
                    str += "�÷��̾� ���ݷ� : " + currentData.playerDamage.ToString() + '\n';
                    str += "�÷��̾� ȸ�� : " + currentData.playerAvoidChance.ToString() + '\n';
                    str += "�÷��̾� ũȮ : " + currentData.playerCriticalChance.ToString() + '\n';
                    str += "�÷��̾� ���¹̳� ���� : " + currentData.playerStaminaRegen.ToString() + '\n';
                    Debug.Log(str);
                    PlayerController.instance.StatusUpdate();
                }

                // �߰��� Ȱ��ȭ �� ������Ʈ�� �ִٸ�
                if (item.activeEffectName != string.Empty)
                {
                    for (int j = 0; j < activeEffects.Count; j++) 
                    {
                        if (activeEffects[j].activeEffectName == item.activeEffectName)
                        {
                            activeEffects[j].Enable(); // ȿ�� Ȱ��ȭ

                            if (activeEffects[j] is StackEffect stackEffect) 
                            {
                                stackEffects.Add(stackEffect);
                            }
                            else if (activeEffects[j] is BattleEndEffect battleEndEffect)
                            {
                                battleEndEffects.Add(battleEndEffect);
                            }
                        }
                    }
                }
                break;
            }
            else
            {
                Debug.Log("������ ����");
            }
        }
    }
    public static void AddStack()
    {
        for (int i = 0; i < instance.stackEffects.Count; i++)
        {
            instance.stackEffects[i].FillCount();
        }
    }

    public static void BattleEnd()
    {
        for (int i = 0; i < instance.battleEndEffects.Count; i++)
        {
            instance.battleEndEffects[i].Play();
        }
    }
}