using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class PlayerCalls : MonoBehaviour{

    Animator powerAnimator;

    private void Awake()
    {
        powerAnimator = transform.GetChild(0).Find("Power").GetChild(0).GetComponent<Animator>();
    }

    private void PowerAnimSpell(Skill_Class sType)
    {
        switch (sType)
        {
            case Skill_Class.Alpha: powerAnimator.Play("Spell_Alpha"); break;
            case Skill_Class.Assassin: powerAnimator.Play("Spell_Assassin"); break;
            case Skill_Class.Charming: powerAnimator.Play("Spell_Charming"); break;
            case Skill_Class.Pacifist: powerAnimator.Play("Spell_Pacifist"); break;
        }
    }

    public void ShowPhysicalStrike()
    {
        Skill usedSkill = Skills.Instance.SkillByID(BattleSystem.Instance.lastSkill_ID);
        switch (usedSkill.s_class)
        {
            case Skill_Class.Alpha: powerAnimator.Play("Attack_Alpha"); break;
            case Skill_Class.Assassin: powerAnimator.Play("Attack_Assassin"); break;
            case Skill_Class.Charming: powerAnimator.Play("Attack_Charming"); break;
            case Skill_Class.Pacifist: powerAnimator.Play("Attack_Pacifist"); break;
        }
    }

    public void ShowExplosion()
    {
        Skill usedSkill = Skills.Instance.SkillByID(BattleSystem.Instance.lastSkill_ID);
        switch (usedSkill.s_class)
        {
            case Skill_Class.Alpha: powerAnimator.Play("Explosion_Alpha"); break;
            case Skill_Class.Assassin: powerAnimator.Play("Explosion_Assassin"); break;
            case Skill_Class.Charming: powerAnimator.Play("Explosion_Charming"); break;
            case Skill_Class.Pacifist: powerAnimator.Play("Explosion_Pacifist"); break;
        }
    }

    public void HitEnemy()
    {
        BattleSystem.Instance.HitEnemyAnim();
    }

    public void PutCamera_Mid()
    {
        BattleSystem.Instance.StopCameraMove();
        StartCoroutine(CanvasBase.Instance.C_PutOnMid());
    }

    public void OnDeadAnimEnded()
    {
        BattleSystem.Instance.DeadAnimEnded();
    }

}
