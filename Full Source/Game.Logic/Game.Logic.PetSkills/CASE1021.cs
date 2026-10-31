using Bussiness;
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
    public class CASE1021 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public CASE1021(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.CASE1021, elementID)
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
            CASE1021 effect = living.PetEffectList.GetOfType(ePetEffectType.CASE1021) as CASE1021;
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
                player.AddPetEffect(new CASE1021EQ(1, m_probability, m_type, m_currentId, m_delay, base.Info.ID.ToString()), 0);
            }
        }
    }
}
namespace Game.Logic.PetEffects
{
    public class CASE1021EQ : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_currentId;
        private int m_added = 0;

        public CASE1021EQ(int count, int probability, int type, int skillId, int delay, string elementID)
            : base(ePetEffectType.CASE1021EQ, elementID)
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
            CASE1021EQ effect = living.PetEffectList.GetOfType(ePetEffectType.CASE1021EQ) as CASE1021EQ;
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
            player.IsNoHole = true;
            player.Game.SendPlayerPicture(player, (int)BuffType.NoHole, true);
            player.PlayerClearBuffSkillPet += Player_PlayerClearBuffSkillPet;
        }

        private void Player_PlayerClearBuffSkillPet(Player player)
        {
            Stop();
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginSelfTurn -= Player_BeginSelfTurn;
            player.IsNoHole = false;
            player.Game.SendPlayerPicture(player, (int)BuffType.NoHole, false);
        }

        private void Player_BeginSelfTurn(Living living)
        {
            m_count--;
            if (m_count < 0)
            {
                Stop();
            }
        }
    }
}
