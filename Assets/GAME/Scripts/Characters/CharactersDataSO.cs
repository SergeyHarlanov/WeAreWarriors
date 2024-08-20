using System.Collections.Generic;
using UnityEngine;

namespace GAME.Scripts.Characters {
    [CreateAssetMenu(fileName = "CharactersDataSO", menuName = "GAME/CharactersDataSO")]
    public class CharactersDataSO : ScriptableObject {
        [SerializeField] private List<Character> _charactersData;


        public List<Character> CharactersData => _charactersData;
    }
}
