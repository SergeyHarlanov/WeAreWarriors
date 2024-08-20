using Cysharp.Threading.Tasks;
using GAME.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.Skills {
    public class SkillButton : MonoBehaviour {
        [SerializeField] private Button _button;

        [Inject] private ResourcesService _resourcesService;
        
        public Button Button => _button;
        
        public void Init(Skill skill) {
            if (skill == null) return;
            LoadSkillSprite(skill).Forget();
        }

        public void Active() {
           gameObject.SetActive(true);
          // _button.gameObject.SetActive(true);
        }
        public void Disactive() {
            gameObject.SetActive(false);
          //  _button.gameObject.SetActive(false);
        }
        private async UniTaskVoid LoadSkillSprite(Skill skill) {
            Sprite skillSprite = await _resourcesService.LoadSprite(skill.uiSpriteLink);
            _button.image.sprite = skillSprite;
        }
    }
}
