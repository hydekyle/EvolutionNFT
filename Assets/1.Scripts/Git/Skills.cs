using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Skills : MonoBehaviour{

    public static Skills Instance;

    public List<Skill> skill_list_assassin;
    public List<Skill> skill_list_alpha;
    public List<Skill> skill_list_charming;
    public List<Skill> skill_list_pacifist;

    List<Skill> skill_list = new List<Skill>();

    void Awake()
    {
        Instance = this;
        Inicialize();
    }

    private void Inicialize()
    {
        foreach (Skill s in skill_list_assassin) skill_list.Add(s);
        foreach (Skill s in skill_list_alpha)    skill_list.Add(s);
        foreach (Skill s in skill_list_charming) skill_list.Add(s);
        foreach (Skill s in skill_list_pacifist) skill_list.Add(s);
    }

    public Skill GetSkillByID(string id)
    {
        Skill skillBuscada = new Skill();
        int lista = int.Parse(id.Substring(0, 1));
        int n = int.Parse(id.Substring(1, 2));
        switch (lista)
        {
            case 1: skillBuscada = skill_list_assassin[n - 1]; break;
            case 2: skillBuscada = skill_list_alpha[n - 1]; break;
            case 3: skillBuscada = skill_list_charming[n - 1]; break;
            case 4: skillBuscada = skill_list_pacifist[n - 1]; break;
        }
        return skillBuscada;
    }

    public Skill_Result SkillResolve(int ID_skill, Player mySelf, Player myEnemy, int chance, int fails)
    {
        Skill_Result result = new Skill_Result();
        switch (ID_skill)
        {
            case 101: result = Skill_101(mySelf.status, myEnemy.status, chance); break;
            case 102: result = Skill_102(mySelf.status, myEnemy.status, chance); break;
            case 103: result = Skill_103(mySelf.status, myEnemy.status, chance); break;
            case 104: result = Skill_104(mySelf.status, myEnemy.status, chance); break;
            case 105: result = Skill_105(mySelf.status, myEnemy.status, chance, fails); break;
            case 106: result = Skill_106(mySelf.status, myEnemy.status, chance); break;
            case 107: result = Skill_107(mySelf.status, myEnemy.status, chance); break;
            case 108: result = Skill_108(mySelf.status, myEnemy.status, chance); break;
            case 109: result = Skill_109(mySelf.status, myEnemy.status, chance); break;
            case 110: result = Skill_110(mySelf.status, myEnemy.status, chance); break;
			case 111: result = Skill_111(mySelf.status, myEnemy.status, chance, fails); break;

			case 201: result = Skill_201(mySelf.status, myEnemy.status, chance); break;
            case 202: result = Skill_202(mySelf.status, myEnemy.status, chance); break;
            case 203: result = Skill_203(mySelf.status, myEnemy.status, chance, fails); break;
            case 204: result = Skill_204(mySelf.status, myEnemy.status, chance); break;
            case 205: result = Skill_205(mySelf.status, myEnemy.status, chance); break;
            case 206: result = Skill_206(mySelf.status, myEnemy.status, chance, fails); break;
            case 207: result = Skill_207(mySelf.status, myEnemy.status, chance); break;
            case 208: result = Skill_208(mySelf.status, myEnemy.status, chance); break;
            case 209: result = Skill_209(mySelf.status, myEnemy.status, chance); break;
            case 210: result = Skill_210(mySelf.status, myEnemy.status, chance); break;
			case 211: result = Skill_211(mySelf.status, myEnemy.status, chance); break;

            case 301: result = Skill_301(mySelf.status, myEnemy.status, chance); break;
            case 302: result = Skill_302(mySelf.status, myEnemy.status, chance); break;
            case 303: result = Skill_303(mySelf.status, myEnemy.status, chance); break;
            case 304: result = Skill_304(mySelf.status, myEnemy.status, chance); break;
            case 305: result = Skill_305(mySelf.status, myEnemy.status, chance); break;
            case 306: result = Skill_306(mySelf.status, myEnemy.status, chance); break;
            case 307: result = Skill_307(mySelf.status, myEnemy.status, chance); break;
            case 308: result = Skill_308(mySelf.status, myEnemy.status, chance); break;
            case 309: result = Skill_309(mySelf.status, myEnemy.status, chance); break;
            case 310: result = Skill_310(mySelf.status, myEnemy.status, chance); break;

            case 401: result = Skill_401(mySelf.status, myEnemy.status, chance); break;
            case 402: result = Skill_402(mySelf.status, myEnemy.status, chance); break;
            case 403: result = Skill_403(mySelf.status, myEnemy.status, chance); break;
            case 404: result = Skill_404(mySelf.status, myEnemy.status, chance, fails); break;
            case 406: result = Skill_406(mySelf.status, myEnemy.status, chance, fails); break;
            case 407: result = Skill_407(mySelf.status, myEnemy.status, chance, fails); break;
            case 408: result = Skill_408(mySelf.status, myEnemy.status, chance, fails); break;
            case 409: result = Skill_409(mySelf.status, myEnemy.status, chance, fails); break;
            case 410: result = Skill_410(mySelf.status, myEnemy.status, chance, fails); break;
            case 405: result = Skill_405(mySelf.status, myEnemy.status, chance, fails); break;

        }
        bool fixerDmg = result.damage > 0 ? true : false;
        result.s_type = SkillByID(ID_skill).s_type;
        result.damage -= (int)(0.15f * result.damage) * fails; //Procesar fails
        print("Fails: "+fails);
        if (result.s_type == Skill_Type.Attack && mySelf.status.buff_attack > 0) result.damage += (int)(0.1f * result.damage) * mySelf.status.buff_attack; //Buff Ataque
        else if (result.s_type == Skill_Type.Spell && mySelf.status.buff_skill > 0) result.damage += (int)(0.1f * result.damage) * mySelf.status.buff_skill; //Buff Skill

        if (result.s_type == Skill_Type.Attack && myEnemy.status.buff_shield > 0) result.damage -= (int)(0.1f * result.damage) * myEnemy.status.buff_shield; //Enemy Shield
        else if (result.s_type == Skill_Type.Spell && myEnemy.status.buff_barrier > 0) result.damage -= (int)(0.1f * result.damage) * myEnemy.status.buff_barrier; //Enemy Barrier

        if (result.recoverHP > 0 && mySelf.status.poison > 0) result.recoverHP = result.recoverHP - (int)((result.recoverHP * 0.1f) * mySelf.status.poison);

        if (fixerDmg) result.damage = Mathf.Clamp(result.damage, 1, 150);

        return result;
    }

    Skill_Result Skill_101(Stats stats, Stats enemyStats, int chance) //Acuchillar
    {
        bool lucky = chance > 80 ? true : false;
        int newBleed = lucky ? 3 : 2;
        int dmg = 20 + stats.attack_base;
        if (lucky) dmg += 20;
        return new Skill_Result()
        {
            isLucky = lucky,
            bleed = newBleed,
            damage = dmg
        };
    }

    Skill_Result Skill_102(Stats stats, Stats enemyStats, int chance) //Chupasangre
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 20 + stats.attack_base;
        if (lucky) dmg += 15;
        int vidaRobada = (dmg / 2) + enemyStats.bleed * 10;
        
        return new Skill_Result()
        {
            isLucky = lucky,
            recoverHP = Mathf.Clamp(vidaRobada, 10, 100),
            damage = dmg
        };
    }

    Skill_Result Skill_103(Stats stats, Stats enemyStats, int chance) //Hemorragia
    {
        bool lucky = chance > 80 ? true : false;
        int newBleed = 3 + (int)(stats.skill_base * 0.05f); //Uno de bleed por cada 20 puntos de habilidad.
        if (lucky) newBleed += 2;
        return new Skill_Result()
        {
            isLucky = lucky,
            bleed = Mathf.Clamp(newBleed, 1, 4)
        };
    }

    Skill_Result Skill_104(Stats stats, Stats enemyStats, int chance) //Apuñalar
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 24 + stats.attack_base;
        int newBleed = 3 + (int)(stats.skill_base * 0.05f);
        if (lucky) { dmg += 20; newBleed++; }
        return new Skill_Result()
        {
            isLucky = lucky,
            bleed = newBleed,
            damage = dmg
        };
    }

    Skill_Result Skill_105(Stats stats, Stats enemyStats, int chance, int fails) // Desangrar
    {
        bool lucky = chance > 80 ? true : false;
        int newBleed = 3 - fails;
        newBleed = Mathf.Clamp(newBleed, 0, 4);
        int dmg = 15 + (10 * (enemyStats.bleed + newBleed));
        if (lucky) dmg += 20;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            bleed = newBleed
        };
    }

    Skill_Result Skill_106(Stats stats, Stats enemyStats, int chance) //Rematar
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = stats.attack_base + (8 * enemyStats.bleed + 8 * enemyStats.poison);
        if (lucky) dmg += stats.attack_base;

        return new Skill_Result()
		{
            isLucky = lucky,
            damage = dmg
        };
    }

    Skill_Result Skill_107(Stats stats, Stats enemyStats, int chance) //Venganza
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = (int)((20 + stats.attack_base) * (1f - (enemyStats.health_now / enemyStats.health_base)));
        if (lucky) dmg += dmg / 5;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg
        };
    }

    Skill_Result Skill_108(Stats stats, Stats enemyStats, int chance) //Doble filo
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 48 + stats.attack_base;
        int autoBleed = 2;
        int newBleed = 2;
        if (lucky) { dmg += 20; newBleed++; autoBleed++; }
        return new Skill_Result()
        {
            isLucky = lucky,
            myself_bleed = autoBleed,
            damage = dmg,
            bleed = newBleed
        };
    }

    Skill_Result Skill_109(Stats stats, Stats enemyStats, int chance) //Envenenar
    {
        bool lucky = chance > 80 ? true : false;
        int veneno = 4 + (int)(stats.skill_base * 0.05f);
        if (lucky) veneno += 2;
        return new Skill_Result()
        {
            isLucky = lucky,
            poison = veneno
        };
    }

    Skill_Result Skill_110(Stats stats, Stats enemyStats, int chance) //Intoxicar
    {
        bool lucky = chance > 80 ? true : false;
        int veneno = 2;
		int dmg = stats.skill_base + (enemyStats.poison + veneno) * 8;
        if (lucky) dmg += 20;
        return new Skill_Result()
        {
            isLucky = lucky,
            poison = veneno,
            damage = dmg
        };
    }

	Skill_Result Skill_111(Stats stats, Stats enemyStats, int chance, int fails) //Golpe preciso
	{
        bool lucky = fails == 0 ? true : false;
        int dmg = 23 + stats.attack_base;
        if (lucky) dmg *= 2;
		return new Skill_Result()
		{
            isLucky = lucky,
            damage = dmg,
		};
	}

    Skill_Result Skill_201(Stats stats, Stats enemyStats, int chance) //Tortazo
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 15 + stats.attack_base;
        int mareo = lucky ? 3 : 2;
        return new Skill_Result()
        {
            isLucky = lucky,
            dizziness = mareo,
            damage = dmg
        };
    }

    Skill_Result Skill_202(Stats stats, Stats enemyStats, int chance) //Puñetazo
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 30 + stats.attack_base;
        if (lucky) dmg += 20;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg
        };
    }

    Skill_Result Skill_203(Stats stats, Stats enemyStats, int chance, int fails) //Cabezazo
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 40 + stats.attack_base;
        int myselfDizziness = 3;
        if (lucky) { myselfDizziness++; dmg += 20; }
        return new Skill_Result()
        {
            isLucky = lucky,
            myself_dizziness = myselfDizziness,
            damage = dmg
        };
    }

    Skill_Result Skill_204(Stats stats, Stats enemyStats, int chance) //Apalizar
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 25 + stats.attack_base;
        int v = lucky ? 4 : 3;
        if (lucky) dmg += 15;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            dizziness = v,
            confusion = v
        };
    }

    Skill_Result Skill_205(Stats stats, Stats enemyStats, int chance) //Sacudir
    {
        bool lucky = chance > 80 ? true : false;
        int newDizziness = lucky ? 2 : 1;
        int dmg = 15 + (enemyStats.dizziness + newDizziness) * 8;
        if (lucky) dmg += 15;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            dizziness = newDizziness
        };
    }

    Skill_Result Skill_206(Stats stats, Stats enemyStats, int chance, int fails) //Marear
    {
        bool lucky = chance > 80 ? true : false;
        int mareo = 5 + (int)(stats.attack_base * 0.05f) - fails;
        int dmg = stats.skill_base + 15;
        if (lucky) { dmg += 15; mareo++; }
        mareo = Mathf.Clamp(mareo, 0, 7);

        return new Skill_Result()
        {
            isLucky = lucky,
            dizziness = mareo,
            damage = dmg
        };
    }

    Skill_Result Skill_207(Stats stats, Stats enemyStats, int chance) //Férreo
    {
        bool lucky = chance > 80 ? true : false;
        int newDef = 2;
        int dmg = 10 + stats.attack_base;
        if (lucky)
        {
            newDef ++;
            dmg += 15;
        }

        return new Skill_Result()
        {
            isLucky = lucky,
            buff_shield = newDef,
            damage = dmg
        };
    }

    Skill_Result Skill_208(Stats stats, Stats enemyStats, int chance) //Desorientar
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 35 + stats.attack_base;
        int confusion = 2 + (int)(stats.skill_base * 0.05f);
        if (lucky) { confusion ++; dmg += 15; }

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            confusion = confusion
        };
    }
   
    Skill_Result Skill_209(Stats stats, Stats enemyStats, int chance) //Devastar
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 20 + stats.skill_base;
        if (enemyStats.buff_shield > 0) dmg += enemyStats.buff_shield * 15;
        if (lucky) dmg += 20;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg
        };
    }

    Skill_Result Skill_210(Stats stats, Stats enemyStats, int chance) //Golpe Escudo
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 20 + stats.attack_base;
        int escudo = 2 + (int)(stats.skill_base * 0.05f);
        if (lucky) dmg += 20;
        return new Skill_Result()
        {
            isLucky = lucky,
            buff_shield = escudo,
            damage = dmg
        };
    }

	Skill_Result Skill_211(Stats stats, Stats enemyStats, int chance) //Sobrecarga
	{
        bool lucky = chance > 80 ? true : false;
        int buffAttack = 3 + (int)(stats.skill_base * 0.05f);
        int selfConfusion = 4;
        if (lucky) buffAttack ++;
		return new Skill_Result()
		{
            isLucky = lucky,
            buff_attack = buffAttack,
			myself_confusion = selfConfusion
		};
	}

    Skill_Result Skill_301(Stats stats, Stats enemyStats, int chance) //Descarga
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 35 + stats.skill_base;
        int newConfusion = 3 + (int)(stats.skill_base * 0.05f);
        if (lucky) { newConfusion++; dmg += 18; }

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            confusion = newConfusion
        };
    }

    Skill_Result Skill_302(Stats stats, Stats enemyStats, int chance) //Aturdir
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 20 + stats.skill_base;
        int newConfusion = 2 + (int)(stats.skill_base * 0.05f);
        int newDizziness = 2 + (int)(stats.skill_base * 0.05f);
        if (lucky) { newConfusion ++; newDizziness ++; dmg += 15; }

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            confusion = newConfusion,
            dizziness = newDizziness
        };
    }

    Skill_Result Skill_303(Stats stats, Stats enemyStats, int chance) //Ionizador
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 20 + stats.skill_base;
        int newBarrier = 2;
        if (lucky) { newBarrier++; dmg += 20; }

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            buff_barrier = newBarrier
        };
    }

    Skill_Result Skill_304(Stats stats, Stats enemyStats, int chance) //Campo electro
    {
        bool lucky = chance > 80 ? true : false;
        int newConfusion = 4;
        int newBarrier = 3 + (int)(stats.skill_base * 0.05f);
        if (lucky) { newConfusion += 2; newBarrier ++; }

        return new Skill_Result()
        {
            isLucky = lucky,
            confusion = newConfusion,
            buff_barrier = newBarrier
        };
    }

    Skill_Result Skill_305(Stats stats, Stats enemyStats, int chance) //Sónico
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 25 + stats.attack_base;
        int buffAttack = 2 + (int)(stats.skill_base * 0.05f);
        if (lucky) { buffAttack++; dmg += 15; }

        return new Skill_Result()
        {
            isLucky = lucky,
            buff_attack = buffAttack,
            damage = dmg
		};
    }

    Skill_Result Skill_306(Stats stats, Stats enemyStats, int chance) //Rencor
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 10 + stats.attack_base + 15 * enemyStats.buff_barrier;
        if (lucky) dmg += 20;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg
        };
    }

    Skill_Result Skill_307(Stats stats, Stats enemyStats, int chance) //Traición
    {
        bool lucky = chance > 80 ? true : false;
        int newConfusion = lucky ? 3 : 2;
        int dmg = stats.attack_base + 12 * (enemyStats.confusion + newConfusion);
        

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            confusion = newConfusion
        };
    }

    Skill_Result Skill_308(Stats stats, Stats enemyStats, int chance) //Absorber
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 15 + stats.skill_base;
        int myHeal = lucky ? dmg + stats.skill_base : dmg;
        if (lucky) dmg += 15;
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            recoverHP = myHeal
        };
    }

    Skill_Result Skill_309(Stats stats, Stats enemyStats, int chance) //Malicia
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 20 + stats.skill_base;
        int newSkill = 2;
        if (lucky) { newSkill++; dmg += 15; }
        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            buff_skill = newSkill
        };
    }

    Skill_Result Skill_310(Stats stats, Stats enemyStats, int chance) //Paralizante
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 25 + stats.skill_base;
        int newShield = 2;
        if (lucky) newShield ++;

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            buff_shield = newShield
        };
    }

    Skill_Result Skill_401(Stats stats, Stats enemyStats, int chance) //Curación
    {
        bool lucky = chance > 80 ? true : false;
        int recover = 50 + stats.skill_base;
        if (lucky) recover += recover / 2;
        return new Skill_Result()
        {
            isLucky = lucky,
            recoverHP = recover
		};
    }

    Skill_Result Skill_402(Stats stats, Stats enemyStats, int chance) //Vudú
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 10 + 10 * (enemyStats.buff_skill + enemyStats.buff_attack + enemyStats.buff_barrier + enemyStats.buff_shield);
        if (lucky) dmg += 20;

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            buff_attack = enemyStats.buff_attack,
            buff_barrier = enemyStats.buff_barrier,
            buff_shield = enemyStats.buff_shield,
            buff_skill = enemyStats.buff_skill
        };
    }

    Skill_Result Skill_403(Stats stats, Stats enemyStats, int chance) //Epidemia
    {
        bool lucky = chance > 80 ? true : false;
        int dmg = 15 + 8 * (stats.bleed + stats.poison + stats.dizziness + stats.confusion);
        if (lucky) dmg += 15;

        return new Skill_Result()
        {
            isLucky = lucky,
            damage = dmg,
            dizziness = stats.dizziness,
            confusion = stats.confusion,
            bleed = stats.bleed,
            poison = stats.poison
        };
    }

    Skill_Result Skill_404(Stats stats, Stats enemyStats, int chance, int fails) //Protección
    {
        bool lucky = chance > 80 ? true : false;
        int newShield = 4 + (int)(stats.skill_base * 0.05f) - fails;
        int newBarrier = 4 + (int)(stats.skill_base * 0.05f) - fails;
        if (lucky) { newShield ++; newBarrier ++; }

        return new Skill_Result()
        {
            isLucky = lucky,
            buff_shield = Mathf.Clamp(newShield, 0, 7),
            buff_barrier = Mathf.Clamp(newBarrier, 0, 7)
        };
    }

    Skill_Result Skill_405(Stats stats, Stats enemyStats, int chance, int fails) //Ascender
    {
        bool lucky = chance > 80 ? true : false;
        int newSkill = 4 - fails;
        int newAttack = 4 - fails;
        if (lucky) { newSkill ++; newAttack ++; }

        return new Skill_Result()
        {
            isLucky = lucky,
            buff_attack = Mathf.Clamp(newAttack, 0, 6),
            buff_skill = Mathf.Clamp(newSkill, 0, 6)
        };
    }

    Skill_Result Skill_406(Stats stats, Stats enemyStats, int chance, int fails) //Maldición
    {
        bool lucky = chance > 80 ? true : false;
        int newPoison = 0, newBleed = 0, newDizziness = 0, newConfusion = 0;
        int nEfectos = 6 + (int)(stats.skill_base * 0.05f) - fails;
        if (lucky) nEfectos += 3;
        nEfectos = Mathf.Clamp(nEfectos, 2, 12);
        for(var x = 0; x < nEfectos; x++)
        {
            switch(Random.Range(0, 4))
            {
                case 0: newPoison++; break;
                case 1: newBleed++; break;
                case 2: newDizziness++; break;
                case 3: newConfusion++; break;
            }
        }
        return new Skill_Result()
        {
            isLucky = lucky,
            poison = newPoison,
            bleed = newBleed,
            dizziness = newDizziness,
            confusion = newConfusion
        };
    }

    Skill_Result Skill_407 (Stats stats, Stats enemyStats, int chance, int fails) //Ritual
    {
        bool lucky = chance > 80 ? true : false;
        int n = lucky ? 1 : 0;
        int buffDmg = 4 - fails;
        if (fails < 1) n++;

        return new Skill_Result()
        {
            isLucky = lucky,
            bleed = enemyStats.bleed + n,
            poison = enemyStats.poison + n,
            confusion = enemyStats.confusion + n,
            dizziness = enemyStats.dizziness + n,
            buff_attack = stats.buff_attack,
            buff_skill = stats.buff_skill
        };
    }

    Skill_Result Skill_408(Stats stats, Stats enemyStats, int chance, int fails) //Manipular
    {
        bool lucky = chance > 80 ? true : false;
        int newDizziness = 4 + (int)(stats.skill_base * 0.05f) - fails;
        int newConfusion = 4 + (int)(stats.skill_base * 0.05f) - fails;
        int dmgBuff = 4 - fails;
        if (lucky) { newDizziness++; newConfusion++; dmgBuff++; }

        return new Skill_Result()
        {
            isLucky = lucky,
            dizziness = Mathf.Clamp(newDizziness, 0, 7),
            confusion = Mathf.Clamp(newConfusion, 0, 7),
            buff_attack = dmgBuff,
            buff_skill = dmgBuff
        };
    }

    Skill_Result Skill_409(Stats stats, Stats enemyStats, int chance, int fails) //Confusion
    {
        bool lucky = chance > 80 ? true : false;
        int newConfusion = 5 - fails;
        int newShield = 4 - fails;
        if (lucky) { newConfusion ++; newShield++; }

        return new Skill_Result()
        {
            isLucky = lucky,
            confusion = Mathf.Clamp(newConfusion, 0, 7),
            buff_shield = newShield
        };
    }

    Skill_Result Skill_410(Stats stats, Stats enemyStats, int chance, int fails) //Peste
    {
        bool lucky = chance > 80 ? true : false;
        int newPoison = 5 - fails;
        int newBarrera = 3 - fails;
        if (lucky) { newPoison ++; newBarrera++; }
        return new Skill_Result()
        {
            isLucky = lucky,
            poison = Mathf.Clamp(newPoison, 0, 7),
            buff_barrier = newBarrera
        };
    }

    public Skill SkillByID(int ID)
    {
        return skill_list.Find(sk => sk.ID == ID);
    }

    public Skill_Class SkillClassByID(int ID_skill)
    {
        return SkillByID(ID_skill).s_class;
    }

}

