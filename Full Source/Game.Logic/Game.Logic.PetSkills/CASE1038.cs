using Game.Logic.PetEffects;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;
using Game.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game.logic.Game.Logic.PetSkills
{
    public class CASE1038 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public CASE1038(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.CASE1038, elementID)
        {
            m_count = count;
            m_coldDown = count;
            m_probability = probability == -1 ? 10000 : probability;
            m_type = type;
            m_delay = delay;
            m_currentId = skillId;
        }

        public override bool Start(Living living)
        {
            CASE1038 effect = living.PetEffectList.GetOfType(ePetEffectType.CASE1038) as CASE1038;
            if (effect != null)
            {
                effect.m_probability = m_probability > effect.m_probability ? m_probability : effect.m_probability;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBuffSkillPet += new PlayerEventHandle(player_AfterBuffSkillPetByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(player_AfterBuffSkillPetByLiving);
        }

        void player_AfterBuffSkillPetByLiving(Player player)
        {
            if (player.PetEffects.CurrentUseSkill == m_currentId)
            {
                if (m_currentId >= 25 && m_currentId <= 27 && player.Game is PVEGame)
                    return;
                player.AddPetEffect(new CASE1038EQ(0, m_probability, m_type, m_currentId, m_delay, base.Info.ID.ToString()), 0);
            }
        }
    }
}
namespace Game.Logic.PetEffects
{
    public class CASE1038EQ : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public CASE1038EQ(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.CASE1038EQ, elementID)
        {
            m_count = count;
            m_coldDown = count;
            m_probability = probability == -1 ? 10000 : probability;
            m_type = type;
            m_delay = delay;
            m_currentId = skillId;
        }

        public override bool Start(Living living)
        {
            CASE1038EQ effect = living.PetEffectList.GetOfType(ePetEffectType.CASE1038EQ) as CASE1038EQ;
            if (effect != null)
            {
                effect.m_probability = m_probability > effect.m_probability ? m_probability : effect.m_probability;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginSelfTurn += Player_BeginSelfTurn;
            player.ControlBall = true;
            player.PlayerClearBuffSkillPet += Player_PlayerClearBuffSkillPet;
        }

        private void Player_PlayerClearBuffSkillPet(Player player)
        {
            Stop();
        }

        private void Player_BeginSelfTurn(Living living)
        {
            living.ControlBall = true;
            m_count--;
            if (m_count < 0)
            {
                Stop();
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.ControlBall = false;
            player.BeginSelfTurn -= Player_BeginSelfTurn;
        }
    }
}
